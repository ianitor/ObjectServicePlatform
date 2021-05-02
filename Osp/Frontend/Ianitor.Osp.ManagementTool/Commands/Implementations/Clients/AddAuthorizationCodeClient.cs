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
    internal class AddAuthorizationCodeClient : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _clientUri;
        private IArgument _clientId;
        private IArgument _clientName;
        private IArgument _redirectUri;

        public AddAuthorizationCodeClient(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("AddAuthorizationCodeClient", "Adds a new client using grant type 'AuthorizationCode'.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _clientUri = CommandArgumentValue.AddArgument("u", "clientUri", new[] {"URI of client"},
                true, 1);
            _redirectUri = CommandArgumentValue.AddArgument("ru", "redirectUri", new[] {"Redirect URI for login procedure"}, false, 1);
            _clientId = CommandArgumentValue.AddArgument("id", "clientId", new[] {"ServiceClient ID, must be unique"}, true,
                1);
            _clientName = CommandArgumentValue.AddArgument("n", "name", new[] {"Display name of client used for grants"}, true,
                1);
        }

        public override async Task Execute()
        {
            var clientUriArgData = CommandArgumentValue.GetArgumentValue(_clientUri);
            var clientUri = clientUriArgData.GetValue<string>();

            var clientIdArgData = CommandArgumentValue.GetArgumentValue(_clientId);
            var clientId = clientIdArgData.GetValue<string>();

            var clientNameArgData = CommandArgumentValue.GetArgumentValue(_clientName);
            var clientName = clientNameArgData.GetValue<string>();

            Logger.Info($"Creating client '{clientId}' (URI '{clientUri}') at '{ServiceClient.ServiceUri}'");

            var clientDto = new ClientDto
            {
                IsEnabled = true,
                ClientId = clientId,
                ClientName = clientName,
                ClientUri = clientUri,
                AllowedCorsOrigins = new[] {clientUri.TrimEnd('/')},
                AllowedGrantTypes = new[] {OidcConstants.GrantTypes.AuthorizationCode},
                AllowedScopes = new[] {CommonConstants.SystemApiFullAccess},
                PostLogoutRedirectUris = new[] {clientUri.EnsureEndsWith("/")},
                RedirectUris = new[] {clientUri.EnsureEndsWith("/")},
                IsOfflineAccessEnabled = true
            };
            
            if (CommandArgumentValue.IsArgumentUsed(_redirectUri))
            {
                var redirectUriArgData = CommandArgumentValue.GetArgumentValue(_redirectUri);
                var redirectUri = redirectUriArgData.GetValue<string>();
                
                clientDto.RedirectUris = new[] { redirectUri };
            }

            await ServiceClient.CreateClient(clientDto);

            Logger.Info($"ServiceClient '{clientId}' (URI '{clientUri}') at '{ServiceClient.ServiceUri}' created.");
        }
    }
}