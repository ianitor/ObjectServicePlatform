using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Clients
{
    internal class UpdateClient : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _clientUri;
        private IArgument _clientId;
        private IArgument _clientName;
        private IArgument _redirectUri;

        public UpdateClient(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("UpdateClient", "Updates a new client.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _clientUri = CommandArgumentValue.AddArgument("u", "clientUri", new[] { "URI of client" }, false, 1);
            _redirectUri = CommandArgumentValue.AddArgument("ru", "redirectUri", new[] { "Redirect URI for login procedure" }, false, 1);
            _clientId = CommandArgumentValue.AddArgument("id", "clientId", new[] { "ServiceClient ID, must be unique" }, false, 1);
            _clientName = CommandArgumentValue.AddArgument("n", "name", new[] { "Display name of client used for grants" }, false, 1);
        }

        public override async Task Execute()
        {
            var clientIdArgData = CommandArgumentValue.GetArgumentValue(_clientId);
            var clientId = clientIdArgData.GetValue<string>();

            Logger.Info($"Updating client '{clientId}' at '{ServiceClient.ServiceUri}'");

            var clientDto = await ServiceClient.GetClient(clientId);

            if (CommandArgumentValue.IsArgumentUsed(_clientUri))
            {
                var clientUriArgData = CommandArgumentValue.GetArgumentValue(_clientUri);
                var clientUri = clientUriArgData.GetValue<string>();

                clientDto.ClientUri = clientUri;
                clientDto.AllowedCorsOrigins = new[] { clientUri.TrimEnd('/') };
                clientDto.PostLogoutRedirectUris = new[] { clientUri.EnsureEndsWith("/") };
                clientDto.RedirectUris = new[] { clientUri.EnsureEndsWith("/") };
            }

            if (CommandArgumentValue.IsArgumentUsed(_redirectUri))
            {
                var redirectUriArgData = CommandArgumentValue.GetArgumentValue(_redirectUri);
                var redirectUri = redirectUriArgData.GetValue<string>();
                
                clientDto.RedirectUris = new[] { redirectUri };
            }

            if (CommandArgumentValue.IsArgumentUsed(_clientName))
            {
                var clientNameArgData = CommandArgumentValue.GetArgumentValue(_clientName);
                var clientName = clientNameArgData.GetValue<string>();
                clientDto.ClientName = clientName;
            }

            await ServiceClient.UpdateClient(clientId, clientDto);

            Logger.Info($"ServiceClient '{clientId}' at '{ServiceClient.ServiceUri}' updated.");
        }
    }
}
