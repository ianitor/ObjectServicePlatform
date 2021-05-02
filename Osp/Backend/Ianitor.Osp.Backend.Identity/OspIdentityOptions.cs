#pragma warning disable 1591
namespace Ianitor.Osp.Backend.Identity
{
    public class OspIdentityOptions
    {
        public OspIdentityOptions()
        {
            AuthorityUrl = "https://localhost:5003";
            RedisCacheHost = "localhost";
            EnableTokenCleanup = true;
            TokenCleanupInterval = 60 * 60; // default: once an hour
        }
        
        /// <summary>
        /// Gets or sets the redis cache host name
        /// </summary>
        public string RedisCacheHost { get; set; }
        
        /// <summary>
        /// Gets or sets the redis cache password
        /// </summary>
        public string RedisCachePassword { get; set; }

        public string AuthorityUrl { get; set; }
        
        public string KeyFilePath { get; set; }
        public string KeyFilePassword { get; set; }

        /// <summary>
        /// If true, a host service is started to check periodically for expired tokens in grant store
        /// </summary>
        public bool EnableTokenCleanup { get; set; }
        
        /// <summary>
        /// The interval in seconds, expired tokens in grant store are checked to be deleted.
        /// </summary>
        public int TokenCleanupInterval { get; set; }
    }
}
