using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Users
{
    internal class GetUsers: ServiceClientOspCommand<IIdentityServicesClient>
    {

        public GetUsers(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("GetUsers", "Gets users.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
        }

        public override async Task Execute()
        {
            Logger.Info($"Getting users from '{ServiceClient.ServiceUri}'");

            var result = await ServiceClient.GetUsers();

            var users = result.ToArray();
            if (!users.Any())
            {
                Logger.Info("No users has been returned.");
                return;
            }
            
            Logger.Info(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}