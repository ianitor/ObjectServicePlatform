using System;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class ImportRuntimeModel : JobWithWaitOspCommand
    {
        private readonly ICoreServicesClient _coreServicesClient;
        private IArgument _fileArg;

        public ImportRuntimeModel(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IJobServicesClient jobServicesClient, IAuthenticationService authenticationService)
            : base("ImportRt", 
                "Schedules an import job for runtime files. File is specified using -f argument. To wait for job, use -w argument.", options, jobServicesClient, authenticationService)
        {
            _coreServicesClient = coreServicesClient;
        }

        protected override void AddArguments()
        {
            base.AddArguments();

            _fileArg = CommandArgumentValue.AddArgument("f", "file", new[] { "File to import" }, true, 1);
        }
        
        public override async Task PreValidate()
        {
            await base.PreValidate();

            _coreServicesClient.AccessToken.AccessToken = ServicesClient.AccessToken.AccessToken;
        }

        public override async Task Execute()
        {
            var fileArgData = CommandArgumentValue.GetArgumentValue(_fileArg);
            var rtModelFilePath = fileArgData.GetValue<string>().ToLower();

            var tenantId = Options.Value.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ServiceConfigurationMissingException("Tenant is missing.");
            }

            Logger.Info("Importing runtime model '{0}'", rtModelFilePath);

            var id = await _coreServicesClient.ImportRtModel(tenantId, rtModelFilePath);
            Logger.Info($"Runtime model import id '{id}' has been started.");
            await WaitForJob(id);
        }

     
    }
}
