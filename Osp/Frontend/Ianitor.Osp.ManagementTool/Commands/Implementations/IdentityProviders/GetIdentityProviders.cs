using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.IdentityProviders
{
    internal class GetIdentityProviders : ServiceClientOspCommand<IIdentityServicesClient>
    {

        public GetIdentityProviders(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("GetIdentityProviders", "Gets all identity providers.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
        
        }

        public override async Task Execute()
        {
            Logger.Info($"Getting identity providers from '{ServiceClient.ServiceUri}'");

            var result = await ServiceClient.GetIdentityProviders();
            if (!result.Any())
            {
                Logger.Info("No identity providers has been returned.");
                return;
            }
            
            Logger.Info(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}