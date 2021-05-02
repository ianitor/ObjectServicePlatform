using Ianitor.Osp.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Identity.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IOptions<OspIdentityOptions> _ospIdentityOptions;

        public ConfigureJwtBearerOptions(IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            _ospIdentityOptions = ospIdentityOptions;
        }


        public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, JwtBearerOptions options)
        {
            options.Authority = _ospIdentityOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}