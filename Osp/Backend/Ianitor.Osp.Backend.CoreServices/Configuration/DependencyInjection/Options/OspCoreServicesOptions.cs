// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.Options
{
    /// <summary>
    /// General Object Service options
    /// </summary>
    public class OspCoreServicesOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OspCoreServicesOptions()
        {
            JobDatabaseName = "OspSystemJobs";
            PrepareJobSchemaIfNecessary = true;
            Authority = "https://localhost:5003";
            PublicUrl = "https://localhost:5001";
            PublicDashboardUrl = "https://localhost:5005";
            RedisCacheHost = "localhost";
        }
        
        /// <summary>
        /// Gets or sets the redis cache host name
        /// </summary>
        public string RedisCacheHost { get; set; }
        
        /// <summary>
        /// Gets or sets the redis cache password
        /// </summary>
        public string RedisCachePassword { get; set; }

        /// <summary>
        /// URL of arango db
        /// </summary>
        public string JobDatabaseName { get; set; }

        /// <summary>
        /// When true, the collections of arango db job database are created when they do not exist
        /// </summary>
        public bool PrepareJobSchemaIfNecessary { get; set; }

        /// <summary>
        /// (Public) base address of the CAS (Central Authorization Services)
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// (public) base address of the public URI
        /// </summary>
        public string PublicUrl { get; set; }

        /// <summary>
        /// (public) base address of the dashboard
        /// </summary>
        public string PublicDashboardUrl { get; set; }
    }
}