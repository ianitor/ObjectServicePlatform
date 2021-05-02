using Ianitor.Common.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Swagger.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds Osp to the pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder UseOspApiVersioningAndDocumentation(
            this IApplicationBuilder app)
        {
            ArgumentValidation.Validate(nameof(app), app);
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            var ospOptions = app.ApplicationServices.GetRequiredService<IOptions<OspSwaggerOptions>>();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }

                    options.InjectStylesheet("/css/swagger.css");

                    options.OAuthClientId(ospOptions.Value.ClientId);
                    options.OAuthAppName(ospOptions.Value.AppName);
                    options.OAuthUsePkce();
                });

            return app;
        }
    }
}