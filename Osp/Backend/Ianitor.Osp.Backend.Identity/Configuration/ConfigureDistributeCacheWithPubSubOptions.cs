using Ianitor.Osp.Backend.DistributedCache;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Identity.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureDistributeCacheWithPubSubOptions : IConfigureNamedOptions<DistributeCacheWithPubSubOptions>
    {
        private readonly IOptions<OspIdentityOptions> _ospIdentityOptions;

        public ConfigureDistributeCacheWithPubSubOptions(IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            _ospIdentityOptions = ospIdentityOptions;
        }


        public void Configure(DistributeCacheWithPubSubOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, DistributeCacheWithPubSubOptions options)
        {
            options.Host = _ospIdentityOptions.Value.RedisCacheHost;
            options.Password = _ospIdentityOptions.Value.RedisCachePassword;
        }
    }
}