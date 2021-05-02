using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Tenants
{
    internal class ClearTenantCache : ServiceClientOspCommand<ICoreServicesClient>
    {
        private IArgument _tenantIdArg;

        public ClearTenantCache(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IAuthenticationService authenticationService)
            : base("ClearCache", "Clears the cache of a tenant", options, coreServicesClient, authenticationService)
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
            
            Logger.Info($"Clearing cache tenant '{tenantId}' on at '{ServiceClient.ServiceUri}'");

            await ServiceClient.ClearTenantCache(tenantId);

            Logger.Info($"Tenant cache '{tenantId}' on at '{ServiceClient.ServiceUri}' cleared.");
        }
    }
}