using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.IdentityProviders
{
    internal class AddIdentityProvider : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _alias;
        private IArgument _enabled;
        private IArgument _clientId;
        private IArgument _clientSecret;
        private IArgument _type;

        public AddIdentityProvider(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("AddIdentityProvider", "Adds a new identity provider.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _alias = CommandArgumentValue.AddArgument("a", "alias", new[] {"Alias of identity provider, must be unique"}, true,
                1);
            _enabled = CommandArgumentValue.AddArgument("e", "enabled", new[] {"True if identity provider should be enabled, otherwise false"}, true,
                1);         
            _clientId = CommandArgumentValue.AddArgument("cid", "clientId", new[] { "ServiceClient ID, provided by provider" }, true, 1);
            _clientSecret = CommandArgumentValue.AddArgument("cs", "clientSecret", new[] { "ServiceClient secret, provided by provider" }, true, 1);
            _type = CommandArgumentValue.AddArgument("t", "type", new[] { "Type of provider, available is 'google' or 'microsoft'" }, true, 1);
        }

        public override async Task Execute()
        {
            var alias = CommandArgumentValue.GetArgumentScalarValue<string>(_alias);
            var type = CommandArgumentValue.GetArgumentScalarValue<IdentityProviderTypesDto>(_type);
            
            Logger.Info($"Creating identity provider '{@alias}' at '{ServiceClient.ServiceUri}'");

            if (type == IdentityProviderTypesDto.Google)
            {
                var identityProviderDto = new GoogleIdentityProviderDto
                {
                    IsEnabled = CommandArgumentValue.GetArgumentScalarValue<bool>(_enabled),
                    ClientId = CommandArgumentValue.GetArgumentScalarValue<string>(_clientId),
                    ClientSecret = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_clientSecret),
                    Alias = alias
                };
                await ServiceClient.CreateIdentityProvider(identityProviderDto);

            }
            else if (type == IdentityProviderTypesDto.Microsoft)
            {
                var identityProviderDto = new MicrosoftIdentityProviderDto
                {
                    IsEnabled = CommandArgumentValue.GetArgumentScalarValue<bool>(_enabled),
                    ClientId = CommandArgumentValue.GetArgumentScalarValue<string>(_clientId),
                    ClientSecret = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_clientSecret),
                    Alias = alias
                };
                await ServiceClient.CreateIdentityProvider(identityProviderDto);
            }
          
            

            Logger.Info($"ServiceClient '{alias}' at '{ServiceClient.ServiceUri}' created.");
        }
    }
}