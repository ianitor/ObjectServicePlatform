using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.Persistence.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds Osp to the pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder UseOspPersistence(
            this IApplicationBuilder app)
        {
            ArgumentValidation.Validate(nameof(app), app);

            ConfigureOsp(app).GetAwaiter().GetResult();
            return app;
        }

        private static async Task ConfigureOsp(IApplicationBuilder app)
        {
            var systemContext = app.ApplicationServices.GetRequiredService<ISystemContext>();

            if (!await systemContext.IsSystemDatabaseExistingAsync())
            {
                await systemContext.CreateSystemDatabaseAsync();
            }
        }
    }
}