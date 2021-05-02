using System.Collections.Generic;
using AutoMapper;
using Ianitor.Osp.Backend.Authentication.DynamicAuth;
using Ianitor.Osp.Backend.Common.Authorization;
using Ianitor.Osp.Backend.Common.Cors;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Identity.Configuration;
using Ianitor.Osp.Backend.Identity.Services;
using Ianitor.Osp.Backend.Infrastructure.CredentialGenerator;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.Configuration;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Backend.Swagger.Configuration;
using Ianitor.Osp.Common.Internationalization;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

namespace Ianitor.Osp.Backend.Identity
{
    /// <summary>
    /// The startup class
    /// </summary>
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        /// <param name="configuration"></param>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<OemOptions>(options => Configuration.GetSection("ApplicationName").Bind(options));
            services.Configure<OspIdentityOptions>(options => Configuration.GetSection("OspIdentity").Bind(options));
            services.Configure<OspSystemConfiguration>(options => Configuration.GetSection("OspSystem").Bind(options));

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.ConfigureOptions<ConfigureDistributeCacheWithPubSubOptions>();
            services.AddDistributedPubSubCache();

            services.AddScoped<INotificationRepository, EntityNotificationRepository>();
            services.AddTransient<IUserSchemaService, UserSchemaService>();
            services.AddScoped<ICorsPolicyService, CorsPolicyService>();
            services.AddTransient<ICredentialGenerator, CredentialGenerator>();

            services.AddDynamicAuthentication()
                .AddGoogle()
                .AddMicrosoft()
                .AddOpenIdConnect();

            services.AddInitializationService<UserSchemaInitializer>();
            services.AddTransient<IUserSchemaService, UserSchemaService>();

            services.AddSingleton<ICorsPolicyProvider, CorsPolicyProvider>();
            services.AddCors();
            
            services.AddOspPersistence();

            // Add IdentityServer 4 for authentication using OpenID
            var identityServerBuilder = services.AddIdentityServer(serverOptions =>
                {
                    serverOptions.Events.RaiseErrorEvents = true;
                    serverOptions.Events.RaiseInformationEvents = true;
                    serverOptions.Events.RaiseFailureEvents = true;
                    serverOptions.Events.RaiseSuccessEvents = true;
                })
                .AddClientStore<IOspClientStore>()
                .AddResourceStore<ResourceStore>()
                .AddPersistedGrantStore<PersistentGrantStore>()
                .AddAspNetIdentity<OspUser>()
                .AddCorsPolicyService<CorsPolicyService>()
                .AddAppAuthRedirectUriValidator()
                .AddJwtBearerClientAuthentication();

            // Service that periodically cleans up tokens in grant database
            services.AddSingleton<IHostedService, TokenCleanupHostService>();

            // Add the extra configuration;
            services.ConfigureOptions<ConfigureIdentityServerOptions>();
            services.ConfigureOptions<ConfigureOspSwaggerOptions>();

            if (_webHostEnvironment.IsDevelopment())
            {
                identityServerBuilder
                    .AddDeveloperSigningCredential();
            }
            else
            {
                identityServerBuilder.AddOspSigningCredential();
            }

            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
            services.ConfigureOptions<ConfigureJwtBearerOptions>();

            services.AddAuthentication()
                .AddJwtBearer(jwt => { jwt.Audience = CommonConstants.IdentityApi; });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityServiceConstants.IdentityApiReadOnlyPolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess or SystemApiReadOnly
                    authorizationPolicyBuilder.RequireScope(CommonConstants.IdentityApiFullAccess,
                        CommonConstants.IdentityApiReadOnly);
                });

                options.AddPolicy(IdentityServiceConstants.IdentityApiReadWritePolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess
                    authorizationPolicyBuilder.RequireScope(CommonConstants.IdentityApiFullAccess);
                });
            });

            services.AddOspApiVersioningAndDocumentation(options =>
            {
                options.AddXmlDocAssembly<Startup>();
                options.AddXmlDocAssembly<ClientDto>();
                options.Scopes = new Dictionary<string, string>
                {
                    {
                        CommonConstants.IdentityApiFullAccess,
                        Texts.Backend_IdentityServices_Api_FullAccess
                    },
                    {
                        CommonConstants.IdentityApiReadOnly,
                        Texts.Backend_IdentityServices_Api_ReadOnlyAccess
                    }
                };

                options.ApiTitle = "Identity Services API";
                options.ApiDescription = "Object Service Platform (OSP) Identity Services.";

                options.ClientId = CommonConstants.IdentityServicesSwaggerClientId;
                options.AppName = Texts.Backend_IdentityServices_UserSchema_Swagger_DisplayName;
            });
           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddAutoMapper(typeof(Startup));
            
            // Add the Kendo UI services to the services container.
            services.AddKendo();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            // Because we are behind an ingress the tls connection is terminated at the ingress itself,
            // the cluster is itself secure and so we reduce complexity by running http, but the discovery
            // documents should always show https.
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            app.UseCors();

            // Conversion of request query jwt token to cookie for switch from dashboard to hangfire ui dashboard
            app.UseMiddleware<CookieBasedAuthorizationMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            app.UseOspPersistence();

            app.UseOspApiVersioningAndDocumentation();
            
            app.UseRouting();

            // The sequence of the add statements in the configure function is of importance.
            // app.UseAuthentication() 
            // !!!UseIdentityServer calls already UseAuthentication; comes before app.UseMvc();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}