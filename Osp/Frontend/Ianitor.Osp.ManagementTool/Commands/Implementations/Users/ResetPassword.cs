using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Users
{
    internal class ResetPassword : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _nameArg;
        private IArgument _passwordArg;

        public ResetPassword(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("ResetPassword", "Resets the password of a user", options, identityServicesClient, authenticationService)
        {
        }
        
        protected override void AddArguments()
        {
            _nameArg = CommandArgumentValue.AddArgument("un", "userName", new[] {"User name"}, true,
                1);
            _passwordArg = CommandArgumentValue.AddArgument("p", "password", new[] {"New password of user"}, true,
                1);
        }

        public override async Task Execute()
        {
            var nameArgData = CommandArgumentValue.GetArgumentValue(_nameArg);
            var name = nameArgData.GetValue<string>().ToLower();

            var passwordArgData = CommandArgumentValue.GetArgumentValue(_passwordArg);
            var password = passwordArgData.GetValue<string>();

            Logger.Info($"Resetting password for user '{name}' at '{ServiceClient.ServiceUri}'");

            await ServiceClient.ResetPassword(name, password);
            
            Logger.Info($"Resetting password for user '{name}' done.");
        }
    }
}