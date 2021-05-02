using Ianitor.Osp.Common.Shared;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Identity.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureIdentityServerOptions : IConfigureNamedOptions<IdentityServerOptions>
    {
        private readonly IOptions<OspIdentityOptions> _ospIdentityOptions;

        public ConfigureIdentityServerOptions(IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            _ospIdentityOptions = ospIdentityOptions;
        }
        
        public void Configure(IdentityServerOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, IdentityServerOptions options)
        {
            options.IssuerUri = _ospIdentityOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}