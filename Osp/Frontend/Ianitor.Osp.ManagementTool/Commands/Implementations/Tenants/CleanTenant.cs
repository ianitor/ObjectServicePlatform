using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Tenants
{
    internal class CleanTenant : ServiceClientOspCommand<ICoreServicesClient>
    {
        private IArgument _tenantIdArg;

        public CleanTenant(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IAuthenticationService authenticationServices)
            : base("Clean", "Resets a tenant to factory defaults by deleting the construction kit and runtime model.", options, coreServicesClient, authenticationServices)
        {
        }

        protected override void AddArguments()
        {
            _tenantIdArg = CommandArgumentValue.AddArgument("tid", "tenantId", new[] { "Id of tenant" },
                true, 1);
        }

        public override async Task Execute()
        {
            var tenantIdArgData = CommandArgumentValue.GetArgumentValue(_tenantIdArg);
            var tenantId = tenantIdArgData.GetValue<string>().ToLower();

            Logger.Info($"Cleaning tenant '{tenantId}' on at '{ServiceClient.ServiceUri}'");

            await ServiceClient.CleanTenant(tenantId);

            Logger.Info($"Tenant '{tenantId}' on at '{ServiceClient.ServiceUri}' cleaned.");
        }
    }
}