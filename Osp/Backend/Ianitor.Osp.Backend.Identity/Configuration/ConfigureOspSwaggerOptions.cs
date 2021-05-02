using Ianitor.Osp.Backend.Swagger;
using Ianitor.Osp.Common.Shared;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Identity.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureOspSwaggerOptions : IConfigureNamedOptions<OspSwaggerOptions>
    {
        private readonly IOptions<OspIdentityOptions> _ospIdentityOptions;

        public ConfigureOspSwaggerOptions(IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            _ospIdentityOptions = ospIdentityOptions;
        }
        
        public void Configure(OspSwaggerOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, OspSwaggerOptions options)
        {
            options.AuthorityUrl = _ospIdentityOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}