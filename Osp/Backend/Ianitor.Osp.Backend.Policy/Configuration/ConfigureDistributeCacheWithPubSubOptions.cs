using Ianitor.Osp.Backend.DistributedCache;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Policy.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureDistributeCacheWithPubSubOptions : IConfigureNamedOptions<DistributeCacheWithPubSubOptions>
    {
        private readonly IOptions<OspPolicyOptions> _ospPolicyServiceOptions;

        public ConfigureDistributeCacheWithPubSubOptions(IOptions<OspPolicyOptions> ospPolicyServiceOptions)
        {
            _ospPolicyServiceOptions = ospPolicyServiceOptions;
        }


        public void Configure(DistributeCacheWithPubSubOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, DistributeCacheWithPubSubOptions options)
        {
            options.Host = _ospPolicyServiceOptions.Value.RedisCacheHost;
            options.Password = _ospPolicyServiceOptions.Value.RedisCachePassword;
        }
    }
}