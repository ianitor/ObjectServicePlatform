using System;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class ExportRuntimeModel : JobOspCommand
    {
        private readonly ICoreServicesClient _coreServicesClient;
        private IArgument _fileArg;
        private IArgument _queryIdArg;

        public ExportRuntimeModel(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IJobServicesClient jobServicesClient, IAuthenticationService authenticationService)
            : base("ExportRt", 
                "Schedules an export job for runtime files. File is specified using -f argument. The file is downloaded in ZIP-format after job is finished.", options, jobServicesClient, authenticationService)
        {
            _coreServicesClient = coreServicesClient;
        }

        protected override void AddArguments()
        {
            _fileArg = CommandArgumentValue.AddArgument("f", "file", new[] { "File to export" }, true, 1);
            _queryIdArg = CommandArgumentValue.AddArgument("q", "queryId", new[] { "Query ID that is used for export" }, true, 1);
        }
        
        public override async Task PreValidate()
        {
            await base.PreValidate();

            _coreServicesClient.AccessToken.AccessToken = ServicesClient.AccessToken.AccessToken;
        }

        public override async Task Execute()
        {
            var rtModelFilePath = CommandArgumentValue.GetArgumentScalarValue<string>(_fileArg);
            var queryId = CommandArgumentValue.GetArgumentScalarValue<OspObjectId>(_queryIdArg);

            var tenantId = Options.Value.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ServiceConfigurationMissingException("Tenant is missing.");
            }

            Logger.Info("Exporting runtime data of query '{0}' to '{1}'",  queryId,rtModelFilePath);

            var id = await _coreServicesClient.ExportRtModel(tenantId, queryId);
            Logger.Info($"Runtime model export id '{id}' has been started.");
            await WaitForJob(id);

            await DownloadJobResultAsync(id, rtModelFilePath);
        }

     
    }
}
