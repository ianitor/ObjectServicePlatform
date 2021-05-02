using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Tenants
{
    internal class AttachTenant : ServiceClientOspCommand<ICoreServicesClient>
    {
        private IArgument _tenantIdArg;
        private IArgument _databaseArg;

        public AttachTenant(IOptions<OspToolOptions> options, ICoreServicesClient coreServicesClient, IAuthenticationService authenticationService)
            : base("Attach", "Attach an existing database to a tenant.", options, coreServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _tenantIdArg = CommandArgumentValue.AddArgument("tid", "tenantId", new[] { "Id of tenant" },
                true, 1);
            _databaseArg = CommandArgumentValue.AddArgument("db", "database", new[] { "Name of database" }, true,
                1);
        }

        public override async Task Execute()
        {
            var tenantIdArgData = CommandArgumentValue.GetArgumentValue(_tenantIdArg);
            var tenantId = tenantIdArgData.GetValue<string>().ToLower();

            var databaseArgData = CommandArgumentValue.GetArgumentValue(_databaseArg);
            var databaseName = databaseArgData.GetValue<string>().ToLower();

            Logger.Info($"Attach tenant '{tenantId}' (database '{databaseName}') at '{ServiceClient.ServiceUri}'");

            await ServiceClient.AttachTenant(tenantId, databaseName);

            Logger.Info($"Tenant '{tenantId}' (database '{databaseName}') at '{ServiceClient.ServiceUri}' added.");
        }

    }
}
