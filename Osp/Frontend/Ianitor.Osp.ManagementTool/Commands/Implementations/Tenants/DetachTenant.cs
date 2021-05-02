using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Tenants
{
    internal class DetachTenant : ServiceClientOspCommand<ICoreServicesClient>
    {
        private IArgument _tenantIdArg;

        public DetachTenant(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IAuthenticationService authenticationService)
            : base("Detach", "Detach tenant.", options, coreServicesClient, authenticationService)
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

            Logger.Info($"Detach tenant '{tenantId}' at '{ServiceClient.ServiceUri}'");

            await ServiceClient.DetachTenant(tenantId);

            Logger.Info($"Tenant '{tenantId}' at '{ServiceClient.ServiceUri}' detached.");
        }

    }
}
