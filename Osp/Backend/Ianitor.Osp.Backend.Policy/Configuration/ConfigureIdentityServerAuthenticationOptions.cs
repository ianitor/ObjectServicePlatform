using Ianitor.Osp.Common.Shared;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Policy.Configuration
{
    internal class ConfigureIdentityServerAuthenticationOptions: IConfigureNamedOptions<IdentityServerAuthenticationOptions>
    {
        private readonly IOptions<OspPolicyOptions> _policyServicesOptions;

        public ConfigureIdentityServerAuthenticationOptions(IOptions<OspPolicyOptions> policyServicesOptions)
        {
            _policyServicesOptions = policyServicesOptions;
        }
        
        public void Configure(IdentityServerAuthenticationOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, IdentityServerAuthenticationOptions options)
        {
            options.Authority = _policyServicesOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}