using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.IdentityProviders
{
    internal class UpdateIdentityProvider: ServiceClientOspCommand<IIdentityServicesClient>
    {
        
        private IArgument _id;
        private IArgument _alias;
        private IArgument _enabled;
        private IArgument _clientId;
        private IArgument _clientSecret;

        public UpdateIdentityProvider(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("UpdateIdentityProvider", "Updates an identity provider.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _id = CommandArgumentValue.AddArgument("id", "identifier", new[] {"ID of identity provider, must be unique"}, true,
                1);
            _alias = CommandArgumentValue.AddArgument("a", "alias", new[] {"Alias of identity provider, must be unique"}, true,
                1);
            _enabled = CommandArgumentValue.AddArgument("e", "enabled", new[] {"True if identity provider should be enabled, otherwise false"}, true,
                1);         
            _clientId = CommandArgumentValue.AddArgument("cid", "clientId", new[] { "ServiceClient ID, provided by provider" }, true, 1);
            _clientSecret = CommandArgumentValue.AddArgument("cs", "clientSecret", new[] { "ServiceClient secret, provided by provider" }, true, 1);
        }

        public override async Task Execute()
        {
            var id = CommandArgumentValue.GetArgumentScalarValue<string>(_id);

            Logger.Info($"Updating identity provider '{id}' at '{ServiceClient.ServiceUri}'");

            var identityProviderDto = await ServiceClient.GetIdentityProvider(id);
            if (identityProviderDto == null)
            {
                Logger.Error($"Identity provider '{id}' at '{ServiceClient.ServiceUri}' not found.");
                return;
            }
            
            if (identityProviderDto is GoogleIdentityProviderDto)
            {
                var newIdentityProviderDto = new GoogleIdentityProviderDto
                {
                    Id = identityProviderDto.Id,
                    IsEnabled = CommandArgumentValue.GetArgumentScalarValue<bool>(_enabled),
                    ClientId = CommandArgumentValue.GetArgumentScalarValue<string>(_clientId),
                    ClientSecret = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_clientSecret),
                    Alias = CommandArgumentValue.GetArgumentScalarValue<string>(_alias)
                };
                await ServiceClient.UpdateIdentityProvider(id, identityProviderDto);

            }
            else if (identityProviderDto is MicrosoftIdentityProviderDto)
            {
                var newIdentityProviderDto = new MicrosoftIdentityProviderDto
                {
                    IsEnabled = CommandArgumentValue.GetArgumentScalarValue<bool>(_enabled),
                    ClientId = CommandArgumentValue.GetArgumentScalarValue<string>(_clientId),
                    ClientSecret = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_clientSecret),
                    Alias = CommandArgumentValue.GetArgumentScalarValue<string>(_alias),
                };
                await ServiceClient.UpdateIdentityProvider(id, newIdentityProviderDto);
            }

            Logger.Info($"Identity provider '{id}' at '{ServiceClient.ServiceUri}' updated.");
        }
    }
}
