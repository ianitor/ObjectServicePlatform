using Ianitor.Osp.Common.Shared;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.JobServices
{
    internal class ConfigureIdentityServerAuthenticationOptions: IConfigureNamedOptions<IdentityServerAuthenticationOptions>
    {
        private readonly IOptions<OspJobServicesOptions> _ospJobServicesOptions;

        public ConfigureIdentityServerAuthenticationOptions(IOptions<OspJobServicesOptions> ospJobServicesOptions)
        {
            _ospJobServicesOptions = ospJobServicesOptions;
        }
        
        public void Configure(IdentityServerAuthenticationOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, IdentityServerAuthenticationOptions options)
        {
            options.Authority = _ospJobServicesOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}