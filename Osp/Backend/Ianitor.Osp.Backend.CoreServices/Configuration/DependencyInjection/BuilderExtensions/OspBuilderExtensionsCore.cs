using GraphQL.Types.Relay;
using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.Options;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.Services;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.BuilderExtensions
{
    /// <summary>
    /// Builder extension methods for registering core services
    /// </summary>
    public static class OspBuilderExtensionsCore
    {
        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IOspBuilder AddRequiredPlatformServices(this IOspBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.AddSingleton(
                resolver => resolver.GetRequiredService<IOptions<OspCoreServicesOptions>>().Value);
            builder.Services.AddSingleton(
                resolver => resolver.GetRequiredService<IOptions<OspSystemConfiguration>>().Value);
            
            builder.Services.ConfigureOptions<ConfigureOspSwaggerOptions>();

            
            // Add GraphQL types (GraphQL.Relay)
            builder.Services.AddTransient(typeof(ConnectionType<>));
            builder.Services.AddTransient(typeof(EdgeType<>));
            builder.Services.AddTransient<PageInfoType>();
            
            // GraphQL custom services
            builder.Services.AddSingleton<ISchemaContext, SchemaContext>();

            // Add the basic services of OSP
            builder.Services.AddSingleton<ISystemContext, SystemContext>();
            builder.Services.AddSingleton<IOspService, OspService>();
            builder.Services.AddTransient<IUserSchemaService, UserSchemaService>();
            builder.Services.AddTransient<IOspClientStore, ClientStore>();
            builder.Services.AddTransient<IOspResourceStore, ResourceStore>();
            builder.Services.AddTransient<IOspPersistentGrantStore, PersistentGrantStore>();
            
            return builder;
        }
    }
}