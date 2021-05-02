#pragma warning disable 1591
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.Configuration;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Backend.Policy.Configuration;
using Ianitor.Osp.Backend.Policy.Services;
using Ianitor.Osp.Common.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace Ianitor.Osp.Backend.Policy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OspPolicyOptions>(options => Configuration.GetSection("OspPolicy").Bind(options));
            services.Configure<OspSystemConfiguration>(options => Configuration.GetSection("OspSystem").Bind(options));


            services.ConfigureOptions<ConfigureDistributeCacheWithPubSubOptions>();
            services.ConfigureOptions<ConfigureIdentityServerAuthenticationOptions>();
            
            services.AddTransient<IOspResourceStore, ResourceStore>();
            services.AddTransient<IOspPermissionStore, PermissionStore>();
            services.AddSingleton<ISystemContext, SystemContext>();
            services.AddTransient<IUserSchemaService, UserSchemaService>();

            services.AddDistributedPubSubCache();

            services.AddAuthentication()
                .AddIdentityServerAuthentication(options =>
                {
                    // name of the API resource
                    options.ApiName = CommonConstants.PolicyApi;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyServiceConstants.PolicyApiReadOnlyPolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess or SystemApiReadOnly
                    authorizationPolicyBuilder.RequireScope(CommonConstants.PolicyApiFullAccess,
                        CommonConstants.PolicyApiReadOnly);
                });

                options.AddPolicy(PolicyServiceConstants.PolicyApiReadWritePolicy, authorizationPolicyBuilder =>
                {
                    // require SystemApiFullAccess
                    authorizationPolicyBuilder.RequireScope(CommonConstants.PolicyApiFullAccess);
                });
            });

            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddVersionedApiExplorer();

            //  services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISystemContext systemContext,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider, IUserSchemaService userSchemaService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            
            app.UseOspPersistence();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}