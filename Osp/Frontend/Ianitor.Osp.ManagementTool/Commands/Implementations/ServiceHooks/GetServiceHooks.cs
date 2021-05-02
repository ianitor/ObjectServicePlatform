using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.ServiceHooks
{
    internal class GetServiceHooks: ServiceClientOspCommand<ITenantClient>
    {
        private readonly ITenantClient _tenantClient;

        public GetServiceHooks(IOptions<OspToolOptions> options, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("GetServiceHooks", "Gets service hooks.", options, tenantClient, authenticationService)
        {
            _tenantClient = tenantClient;
        }

        protected override void AddArguments()
        {
        }

        public override async Task Execute()
        {
            Logger.Info($"Getting service hooks from '{_tenantClient.ServiceUri}'");

            var getQuery = new GraphQLRequest
            {
                Query = GraphQl.GetServiceHook,
            };

            var getResult = await _tenantClient.SendQueryAsync<RtServiceHookDto>(getQuery);
            if (!getResult.Items.Any())
            {
                Logger.Info($"No service hooks has been returned.");
                return;
            }
            
            Logger.Info(JsonConvert.SerializeObject(getResult.Items, Formatting.Indented));
        }
    }
}