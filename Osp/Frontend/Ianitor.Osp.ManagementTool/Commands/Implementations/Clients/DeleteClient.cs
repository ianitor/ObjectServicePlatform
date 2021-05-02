using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Clients
{
    internal class DeleteClient : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _clientId;

        public DeleteClient(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("DeleteClient", "Deletes a client.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _clientId = CommandArgumentValue.AddArgument("id", "clientId", new[] {"ServiceClient ID, must be unique"}, true,
                1);
        }

        public override async Task Execute()
        {
            var clientIdArgData = CommandArgumentValue.GetArgumentValue(_clientId);
            var clientId = clientIdArgData.GetValue<string>();


            Logger.Info($"Deleting client '{clientId}' from '{ServiceClient.ServiceUri}'");

            await ServiceClient.DeleteClient(clientId);

            Logger.Info($"ServiceClient '{clientId}' deleted.");
        }
    }
}