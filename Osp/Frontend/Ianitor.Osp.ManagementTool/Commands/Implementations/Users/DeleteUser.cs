using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Users
{
    internal class DeleteUser : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _nameArg;

        public DeleteUser(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("DeleteUser", "Deletes an user", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _nameArg = CommandArgumentValue.AddArgument("un", "userName", new[] {"User name"}, true,
                1);
        }

        public override async Task Execute()
        {
            var nameArgData = CommandArgumentValue.GetArgumentValue(_nameArg);
            var name = nameArgData.GetValue<string>().ToLower();

            Logger.Info($"Deleting user '{name}' at '{ServiceClient.ServiceUri}'");

            await ServiceClient.DeleteUser(name);

            Logger.Info($"User '{name}' deleted.");
        }
    }
}