using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class ImportConstructionKitModel : JobWithWaitOspCommand
    {
        private readonly ICoreServicesClient _coreServicesClient;
        private IArgument _fileArg;
        private IArgument _scopeArg;

        public ImportConstructionKitModel(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IJobServicesClient jobServicesClient, IAuthenticationService authenticationService)
            : base("ImportCk", 
                "Schedules an import job for construction kit files. File is specified using -f argument. To wait for job, use -w argument.", options, jobServicesClient, authenticationService)
        {
            _coreServicesClient = coreServicesClient;
        }

        protected override void AddArguments()
        {
            base.AddArguments();

            _fileArg = CommandArgumentValue.AddArgument("f", "file", new[] { "File to import" }, true, 1);
            _scopeArg = CommandArgumentValue.AddArgument("s", "scope", new[] { "Scope to import, available is 'Application', 'Layer2', 'Layer3' or 'Layer4'" }, true, 1);
        }

        public override async Task PreValidate()
        {
            await base.PreValidate();

            _coreServicesClient.AccessToken.AccessToken = ServicesClient.AccessToken.AccessToken;
        }

        public override async Task Execute()
        {
            var fileArgData = CommandArgumentValue.GetArgumentValue(_fileArg);
            var scopeArgData = CommandArgumentValue.GetArgumentValue(_scopeArg);
            var ckModelFilePath = fileArgData.GetValue<string>().ToLower();
            var scopeId = scopeArgData.GetValue<ScopeIdsDto>();

            var tenantId = Options.Value.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ServiceConfigurationMissingException("Tenant is missing.");
            }

            Logger.Info("Importing construction kit model '{0}'", ckModelFilePath);

            var id = await _coreServicesClient.ImportCkModel(tenantId, scopeId, ckModelFilePath);         
            Logger.Info($"Construction kit model import id '{id}' has been started.");
            await WaitForJob(id);
        }
    }
}
