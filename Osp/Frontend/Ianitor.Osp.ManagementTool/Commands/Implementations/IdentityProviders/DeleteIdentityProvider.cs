using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.IdentityProviders
{
    internal class DeleteIdentityProvider : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _id;

        public DeleteIdentityProvider(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("DeleteIdentityProvider", "Deletes an identity provider.", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _id = CommandArgumentValue.AddArgument("id", "identifier", new[] {"ID of identity provider, must be unique"}, true,
                1);
        }

        public override async Task Execute()
        {
            var id = CommandArgumentValue.GetArgumentScalarValue<string>(_id);


            Logger.Info($"Deleting identity provider '{id}' from '{ServiceClient.ServiceUri}'");

            await ServiceClient.DeleteIdentityProvider(id);

            Logger.Info($"Identity provider '{id}' deleted.");
        }
    }
}