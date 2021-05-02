using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.Options;
using Ianitor.Osp.Backend.Swagger;
using Ianitor.Osp.Common.Shared;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.CoreServices.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureOspSwaggerOptions : IConfigureNamedOptions<OspSwaggerOptions>
    {
        private readonly IOptions<OspCoreServicesOptions> _ospCoreOptions;

        public ConfigureOspSwaggerOptions(IOptions<OspCoreServicesOptions> ospCoreOptions)
        {
            _ospCoreOptions = ospCoreOptions;
        }
        
        public void Configure(OspSwaggerOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, OspSwaggerOptions options)
        {
            options.AuthorityUrl = _ospCoreOptions.Value.Authority.EnsureEndsWith("/");
        }
    }
}