using Ianitor.Osp.Backend.Swagger;
using Ianitor.Osp.Common.Shared;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.JobServices.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ConfigureOspSwaggerOptions : IConfigureNamedOptions<OspSwaggerOptions>
    {
        private readonly IOptions<OspJobServicesOptions> _ospJobOptions;

        public ConfigureOspSwaggerOptions(IOptions<OspJobServicesOptions> ospJobOptions)
        {
            _ospJobOptions = ospJobOptions;
        }
        
        public void Configure(OspSwaggerOptions options) => Configure(Options.DefaultName, options);

        public void Configure(string name, OspSwaggerOptions options)
        {
            options.AuthorityUrl = _ospJobOptions.Value.AuthorityUrl.EnsureEndsWith("/");
        }
    }
}