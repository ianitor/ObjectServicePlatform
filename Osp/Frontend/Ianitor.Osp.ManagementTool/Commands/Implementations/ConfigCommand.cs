using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Common.Configuration;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class ConfigOspCommand : OspCommand
    {
        private readonly IConfigWriter _configWriter;
        private IArgument _identityServicesUriArg;
        private IArgument _coreServicesUriArg;
        private IArgument _jobServicesUriArg;
        private IArgument _tenantIdArg;

        public ConfigOspCommand(IOptions<OspToolOptions> options, IConfigWriter configWriter)
        : base("Config", "Configures the tool.", options)
        {
            _configWriter = configWriter;
        }

        protected override void AddArguments()
        {
            _coreServicesUriArg = CommandArgumentValue.AddArgument("csu", "coreServicesUri",
                new[] { "URI of core services (e. g. 'https://localhost:5001/')" }, 1);
            _jobServicesUriArg = CommandArgumentValue.AddArgument("jsu", "jobServicesUri",
                new[] { "URI of job services (e. g. 'https://localhost:5009/')" }, 1);            
            _identityServicesUriArg = CommandArgumentValue.AddArgument("isu", "identityServicesUri",
                new[] { "URI of identity services (e. g. 'https://localhost:5003/')" }, 1);
            _tenantIdArg = CommandArgumentValue.AddArgument("tid", "tenantId",
                new[] { "Id of tenant (e. g. 'myService')" }, 1);
        }

        public override Task Execute()
        {
            Logger.Info("Configuring the tool");

            if (CommandArgumentValue.IsArgumentUsed(_tenantIdArg))
            {
                var tenantIdArgData = CommandArgumentValue.GetArgumentValue(_tenantIdArg);
                Options.Value.TenantId = tenantIdArgData.GetValue<string>().ToLower();
            }

            if (CommandArgumentValue.IsArgumentUsed(_coreServicesUriArg))
            {
                var coreServicesUriData = CommandArgumentValue.GetArgumentValue(_coreServicesUriArg);
                Options.Value.CoreServiceUrl = coreServicesUriData.GetValue<string>().ToLower();
            }
            
            if (CommandArgumentValue.IsArgumentUsed(_jobServicesUriArg))
            {
                var jobServicesUriData = CommandArgumentValue.GetArgumentValue(_jobServicesUriArg);
                Options.Value.JobServiceUrl = jobServicesUriData.GetValue<string>().ToLower();
            }

            if (CommandArgumentValue.IsArgumentUsed(_identityServicesUriArg))
            {
                var identityServicesUriData = CommandArgumentValue.GetArgumentValue(_identityServicesUriArg);
                Options.Value.IdentityServiceUrl = identityServicesUriData.GetValue<string>().ToLower();
            }

            _configWriter.WriteSettings(Constants.OspToolUserFolderName);

            return Task.CompletedTask;
        }
    }
}
