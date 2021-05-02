using Ianitor.Osp.Backend.CoreServices.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using GraphQL.Server.Ui.Playground;
using Ianitor.Osp.Backend.CoreServices;
using Ianitor.Osp.Backend.CoreServices.GraphQL;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware;
using Ianitor.Osp.Backend.Persistence.Configuration;
using Ianitor.Osp.Backend.Swagger.Configuration;
using Microsoft.AspNetCore.HttpOverrides;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Pipeline extension methods for adding Osp
    /// </summary>
    public static class OspApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds Osp to the pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder UseOsp(
            this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            ConfigureOsp(app);
            return app;
        }

        private static void ConfigureOsp(IApplicationBuilder app)
        {
            app.UseOspPersistence();
            app.UseOspApiVersioningAndDocumentation();

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var userSchemaService = scope.ServiceProvider.GetService<IUserSchemaService>();
                userSchemaService.SetupAsync().GetAwaiter().GetResult();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // this is required for websockets support
            app.UseWebSockets();

            // Because we are behind a load balancer using HTTP it is needed to use XForwardProto to ensure
            // that requests are send by HTTPS (e. g. Authentication to Identity Server)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                
                endpoints.MapGraphQlTenantPlayground(new PlaygroundOptions
                     {
                         RequestCredentials = RequestCredentials.Include,
                         GraphQLEndPoint = "/tenants/{tenantId}/graphQl"
                     }, "tenants/{tenantId:tenantId}/graphQl/playground").RequireAuthorization(CoreServiceConstants.AuthenticatedUserPolicy);
                endpoints.MapGraphQL<OspSchema>("tenants/{tenantId:tenantId}/graphQl").RequireAuthorization(CoreServiceConstants.TenantApiReadWritePolicy);
                endpoints.MapGraphQlTenantWebSockets("tenants/{tenantId:tenantId}/graphQlws");
            });
        }
    }
}