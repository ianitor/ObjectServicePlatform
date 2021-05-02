using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using IdentityModel;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Clients
{
    internal class AddClientCredentialsClient : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _clientId;
        private IArgument _clientName;
        private IArgument _clientSecret;

        public AddClientCredentialsClient(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("AddClientCredentialsClient", "Adds a new client using grant type 'ClientCredentials'.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _clientId = CommandArgumentValue.AddArgument("id", "clientId", new[] {"ServiceClient ID, must be unique"}, true,
                1);
            _clientName = CommandArgumentValue.AddArgument("n", "name", new[] {"Display name of client used for grants"}, true,
                1);
            _clientSecret = CommandArgumentValue.AddArgument("s", "secret", new[] {"Secret that is used for client credential authentication"}, true,
                1);
        }

        public override async Task Execute()
        {
            var clientIdArgData = CommandArgumentValue.GetArgumentValue(_clientId);
            var clientId = clientIdArgData.GetValue<string>();

            var clientNameArgData = CommandArgumentValue.GetArgumentValue(_clientName);
            var clientName = clientNameArgData.GetValue<string>();
            
            var clientSecretArgData = CommandArgumentValue.GetArgumentValue(_clientSecret);
            var clientSecret = clientSecretArgData.GetValue<string>();

            Logger.Info($"Creating client '{clientId}' at '{ServiceClient.ServiceUri}'");

            var clientDto = new ClientDto
            {
                IsEnabled = true,
                ClientId = clientId,
                ClientName = clientName,
                ClientSecret = clientSecret,
                AllowedGrantTypes = new[] {OidcConstants.GrantTypes.ClientCredentials},
                AllowedScopes = new[] {CommonConstants.SystemApiFullAccess},
                IsOfflineAccessEnabled = true
            };
            
            await ServiceClient.CreateClient(clientDto);

            Logger.Info($"ServiceClient '{clientId}' at '{ServiceClient.ServiceUri}' created.");
        }
    }
}