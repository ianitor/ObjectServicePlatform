using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Clients
{
    internal class GetClients : ServiceClientOspCommand<IIdentityServicesClient>
    {

        public GetClients(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("GetClients", "Gets all clients.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
        
        }

        public override async Task Execute()
        {
            Logger.Info($"Getting clients from '{ServiceClient.ServiceUri}'");

            var result = await ServiceClient.GetClients();
            if (!result.Any())
            {
                Logger.Info("No clients has been returned.");
                return;
            }
            
            Logger.Info(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}