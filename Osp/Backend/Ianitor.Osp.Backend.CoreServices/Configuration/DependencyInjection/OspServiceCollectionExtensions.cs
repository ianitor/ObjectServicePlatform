using Hangfire;
using Ianitor.Osp.Backend.CoreServices;
using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection;
using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.BuilderExtensions;
using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Ianitor.Osp.Backend.Common;
using Ianitor.Osp.Backend.CoreServices.GraphQL;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Common.Internationalization;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// DI extension methods for adding IdentityServer
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class OspServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        private static IOspBuilder AddOspBuilder(this IServiceCollection services)
        {
            return new OspBuilder(services);
        }

        /// <summary>
        /// Adds Osp.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        private static IOspBuilder AddOsp(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = services.AddOspBuilder();

            builder
                .AddRequiredPlatformServices();

            services.AddMemoryCache();
            services.AddDistributedPubSubCache(options =>
            {
                var ospOptions = services.BuildServiceProvider().GetRequiredService<OspCoreServicesOptions>();

                options.Host = ospOptions.RedisCacheHost;
                options.Password = ospOptions.RedisCachePassword;
            });

            services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = BackendCommon.OidcAuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = CoreServiceConstants.CookieExpireTimeSpan;
                    options.Cookie.Name = CoreServiceConstants.CookieName;
                })
                .AddOpenIdConnect(BackendCommon.OidcAuthenticationScheme, options =>
                {
                    var ospOptions = services.BuildServiceProvider().GetRequiredService<OspCoreServicesOptions>();

                    options.Authority = ospOptions.Authority;
                    //options.RequireHttpsMetadata = false;

                    options.ClientId = CommonConstants.CoreServicesClientId;

                    options.Scope.Clear();
                    options.Scope.Add(CommonConstants.Scopes.OpenId);
                    options.Scope.Add(CommonConstants.Scopes.Profile);
                    options.Scope.Add(CommonConstants.Scopes.Email);
                    options.Scope.Add(CommonConstants.Scopes.Role);

                    options.SaveTokens = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                })
                .AddIdentityServerAuthentication(options =>
                {
                    var ospOptions = services.BuildServiceProvider().GetRequiredService<OspCoreServicesOptions>();
                    // base-address of your identity server
                    options.Authority = ospOptions.Authority;

                    // name of the API resource
                    options.ApiName = CommonConstants.SystemApi;
                    
                    // Added, because otherwise exception is thrown when passing an invalid access token by a client.
                    options.SupportedTokens = SupportedTokens.Jwt;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(CoreServiceConstants.AuthenticatedUserPolicy,
                    policyBuilder => policyBuilder.RequireAuthenticatedUser());

                options.AddPolicy(CoreServiceConstants.SystemApiReadOnlyPolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess or SystemApiReadOnly
                    authorizationPolicyBuilder.RequireScope(CommonConstants.SystemApiFullAccess,
                        CommonConstants.SystemApiReadOnly);
                });

                options.AddPolicy(CoreServiceConstants.SystemApiReadWritePolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess
                    authorizationPolicyBuilder.RequireScope(CommonConstants.SystemApiFullAccess);
                });
                
                options.AddPolicy(CoreServiceConstants.TenantApiReadWritePolicy, authorizationPolicyBuilder =>
                {
                    authorizationPolicyBuilder.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    authorizationPolicyBuilder.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    authorizationPolicyBuilder.RequireAuthenticatedUser();
                });
            });

            services.AddMvcCore().AddAuthorization();

            services.AddOspApiVersioningAndDocumentation(options =>
            {
                options.AddXmlDocAssembly<Startup>();
                options.AddXmlDocAssembly<ClientDto>();
                options.Scopes = new Dictionary<string, string>
                {
                    {
                        CommonConstants.SystemApiFullAccess,
                        Texts.Backend_CoreServices_Api_FullAccess
                    },
                    {
                        CommonConstants.SystemApiReadOnly,
                        Texts.Backend_CoreServices_Api_ReadOnlyAccess
                    }
                };

                options.ApiTitle = "System Services API";
                options.ApiDescription = "Object Service Platform (OSP) System Services.";

                options.ClientId = CommonConstants.CoreServicesSwaggerClientId;
                options.AppName = Texts.Backend_CoreServices_UserSchema_Swagger_DisplayName;
            });


            // Hangfire is used to handle background jobs and scheduled jobs
            services.AddHangfire(config =>
            {
                var ospOptions = services.BuildServiceProvider().GetRequiredService<IOptions<OspCoreServicesOptions>>();
                var systemOptions = services.BuildServiceProvider()
                    .GetRequiredService<IOptions<OspSystemConfiguration>>();

                var storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new DropMongoMigrationStrategy(),
                        BackupStrategy = new NoneMongoBackupStrategy()
                    }
                };
                MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder
                {
                    DatabaseName = ospOptions.Value.JobDatabaseName,
                    Username = systemOptions.Value.AdminUser,
                    Password = systemOptions.Value.AdminUserPassword,
                    AuthenticationSource = systemOptions.Value.AuthenticationDatabaseName
                };

                if (systemOptions.Value.DatabaseHost.Contains(","))
                {
                    mongoUrlBuilder.Servers =
                        systemOptions.Value.DatabaseHost.Split(",").Select(x => new MongoServerAddress(x));
                }
                else
                {
                    mongoUrlBuilder.Server = new MongoServerAddress(systemOptions.Value.DatabaseHost);
                }

                config.UseMongoStorage(mongoUrlBuilder.ToString(), storageOptions);
            });

            // GraphQL
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();
            services.AddScoped<IUserContextBuilder, TenantUserContextBuilder>();
            services.AddScoped<IGraphQLExecuter<OspSchema>, TenantGraphQlExecuter>();

            // Add GraphQL services and configure options
            services.AddSingleton<OspSchema>();
            services.AddSingleton<OspQuery>();

            // Separate web socket factory to handle tenant specific queries correctly
            services.AddTransient<ITenantWebSocketConnectionFactory, TenantWebSocketFactory>();
            services.TryAddSingleton<IDocumentExecuter, SubscriptionDocumentExecuter>();

            services.AddGraphQL(options => { options.EnableMetrics = true; })
                .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
                .AddWebSockets()
                .AddSystemTextJson();

            return builder;
        }

        /// <summary>
        /// Adds Osp.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="systemOptionsSetupAction">Setup action for osp system persistence</param>
        /// <param name="setupAction">The setup action of core services options</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IOspBuilder AddOsp(this IServiceCollection services,
            Action<OspSystemConfiguration> systemOptionsSetupAction, Action<OspCoreServicesOptions> setupAction)
        {
            services.Configure(systemOptionsSetupAction);
            services.Configure(setupAction);
            return services.AddOsp();
        }
    }
}