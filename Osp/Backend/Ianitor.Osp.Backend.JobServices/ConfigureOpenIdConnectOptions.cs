using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.JobServices
{
    internal class ConfigureOpenIdConnectOptions : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IOptions<OspJobServicesOptions> _ospJobServicesOptions;

        public ConfigureOpenIdConnectOptions(IOptions<OspJobServicesOptions> ospJobServicesOptions)
        {
            _ospJobServicesOptions = ospJobServicesOptions;
        }
        
        public void Configure(OpenIdConnectOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, OpenIdConnectOptions options)
        {
            options.Authority = _ospJobServicesOptions.Value.AuthorityUrl;
        }
    }
}