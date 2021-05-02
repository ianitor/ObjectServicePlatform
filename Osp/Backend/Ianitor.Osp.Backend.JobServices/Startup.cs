using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Hangfire;
using Ianitor.Osp.Backend.Common;
using Ianitor.Osp.Backend.Common.Authorization;
using Ianitor.Osp.Backend.Common.Cors;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Jobs.Jobs;
using Ianitor.Osp.Backend.Jobs.Services;
using Ianitor.Osp.Backend.JobServices.Hangfire;
using Ianitor.Osp.Backend.JobServices.Jobs;
using Ianitor.Osp.Backend.JobServices.Services;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Ianitor.Osp.Backend.JobServices.Configuration;
using Ianitor.Osp.Backend.Persistence.Configuration;
using Ianitor.Osp.Backend.Swagger.Configuration;
using Ianitor.Osp.Common.Internationalization;
using MongoDB.Driver;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.JobServices
{
    /// <summary>
    /// OWIN startup class implementation
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.Configure<OspJobServicesOptions>(options =>
                Configuration.GetSection("OspJobServices").Bind(options));
            services.Configure<OspSystemConfiguration>(options => Configuration.GetSection("OspSystem").Bind(options));
            services.Configure<EMailOptions>(options => Configuration.GetSection("EMail").Bind(options));

            services.AddTransient<IOspClientStore, ClientStore>();
            services.AddTransient<IOspResourceStore, ResourceStore>();
            services.AddSingleton<ICorsPolicyProvider, CorsPolicyProvider>();
            services.AddSingleton<INotificationRepository, EntityNotificationRepository>();
            services.AddSingleton<IEMailSender, EMailSender>();
            services.AddCors();

            services.AddSingleton<ISystemContext, SystemContext>();

            services.AddTransient<IUserSchemaService, UserSchemaService>();
            services.AddTransient<IServiceHookService, ServiceHookService>();
            services.AddScoped<ImportModelJob>();
            services.AddScoped<ExportModelJob>();
            services.AddScoped<EMailSenderJob>();
            services.AddScoped<ServiceHookJob>();
            services.AddScoped<AttributeValueAggregatorJob>();

            services.AddDistributedPubSubCache(options =>
            {
                var ospOptions = services.BuildServiceProvider().GetRequiredService<IOptions<OspJobServicesOptions>>();

                options.Host = ospOptions.Value.RedisCacheHost;
                options.Password = ospOptions.Value.RedisCachePassword;
            });

            services.AddMemoryCache();

            services.ConfigureOptions<ConfigureIdentityServerAuthenticationOptions>();
            services.ConfigureOptions<ConfigureOpenIdConnectOptions>();
            services.ConfigureOptions<ConfigureOspSwaggerOptions>();

            services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = BackendCommon.OidcAuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = JobServiceConstants.CookieExpireTimeSpan;
                    options.Cookie.Name = JobServiceConstants.CookieName;
                })
                .AddOpenIdConnect(BackendCommon.OidcAuthenticationScheme, options =>
                {
                    options.ClientId = CommonConstants.JobServicesClientId;

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
                        RoleClaimType = JwtClaimTypes.Role
                    };
                }).AddIdentityServerAuthentication(options =>
                {
                    // name of the API resource
                    options.ApiName = CommonConstants.JobApi;
                });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JobServiceConstants.AuthenticatedUserPolicy, policyBuilder => policyBuilder.RequireAuthenticatedUser());

                options.AddPolicy(JobServiceConstants.JobApiReadOnlyPolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess or SystemApiReadOnly
                    authorizationPolicyBuilder.RequireScope(CommonConstants.JobApiFullAccess,
                        CommonConstants.JobApiReadOnly);
                });

                options.AddPolicy(JobServiceConstants.JobApiReadWritePolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess
                    authorizationPolicyBuilder.RequireScope(CommonConstants.JobApiFullAccess);
                });
            });
            
            services.AddMvcCore().AddAuthorization();
            services.AddMvc();

            services.AddOspApiVersioningAndDocumentation(options =>
            {
                options.AddXmlDocAssembly<Startup>();
                options.Scopes = new Dictionary<string, string>
                {
                    {
                        CommonConstants.JobApiFullAccess,
                        Texts.Backend_JobServices_Api_FullAccess
                    },
                    {
                        CommonConstants.JobApiReadOnly,
                        Texts.Backend_JobServices_Api_ReadOnlyAccess
                    }
                };

                options.ApiTitle = "Job Services API";
                options.ApiDescription = "Object Service Platform (OSP) Job Services.";

                options.ClientId = CommonConstants.JobServicesSwaggerClientId;
                options.AppName = Texts.Backend_JobServices_UserSchema_Swagger_DisplayName;
            });

            // Hangfire is used to handle background jobs and scheduled jobs
            services.AddHangfire(config =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var ospOptions = serviceProvider.GetRequiredService<IOptions<OspJobServicesOptions>>();
                var systemOptions = serviceProvider.GetRequiredService<IOptions<OspSystemConfiguration>>();
                
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
                config.UseLogProvider(new NLogProvider());
                config.UseActivator(new OspJobActivator(serviceProvider));
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseCors();

            // Conversion of request query jwt token to cookie for switch from dashboard to hangfire ui dashboard
            app.UseMiddleware<CookieBasedAuthorizationMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();


            var backgroundJobServerOptions = new BackgroundJobServerOptions
            {
                Queues = new[] {"ospSystem", "default"}
            };
            app.UseHangfireServer(backgroundJobServerOptions);

            app.UseOspPersistence();
            app.UseOspApiVersioningAndDocumentation();

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var userSchemaService = scope.ServiceProvider.GetService<IUserSchemaService>();
                userSchemaService.SetupAsync().GetAwaiter().GetResult();

                var serviceHookService = scope.ServiceProvider.GetService<IServiceHookService>();
                serviceHookService.SyncDataSourceAndCreateJobsAsync().GetAwaiter().GetResult();
            }

            // Because we are behind a load balancer using HTTP it is needed to use XForwardProto to ensure
            // that requests are send by HTTPS (e. g. Authentication to Identity Server)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseHttpsRedirection();

            var ospOptions = app.ApplicationServices.GetRequiredService<IOptions<OspJobServicesOptions>>();

            app.Map("/ui", branchedApp =>
            {
                // ReSharper disable once ASP0001
                branchedApp.UseAuthorization(JobServiceConstants.AuthenticatedUserPolicy);
                branchedApp.UseHangfireDashboard("/jobs", new DashboardOptions
                {
                    AppPath = ospOptions.Value.PublicDashboardUrl,
                    Authorization = new[] {new HangfireDashboardAuthorizationFilter()}
                });
            });

            app.UseStaticFiles();
        }
    }
}