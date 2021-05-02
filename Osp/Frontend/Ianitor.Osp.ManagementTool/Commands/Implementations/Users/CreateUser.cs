using System;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Users
{
    internal class CreateUser : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _eMailArg;
        private IArgument _nameArg;
        private IArgument _roleArg;

        public CreateUser(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("CreateUser", "Create a new user account", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _eMailArg = CommandArgumentValue.AddArgument("e", "eMail", new[] {"E-Mail of user"},
                true, 1);
            _nameArg = CommandArgumentValue.AddArgument("un", "userName", new[] {"User name"}, true,
                1);
            _roleArg = CommandArgumentValue.AddArgument("r", "role", new[] {"Role of user"}, true,
                1);
        }


        public override async Task Execute()
        {
            var eMailArgData = CommandArgumentValue.GetArgumentValue(_eMailArg);
            var eMail = eMailArgData.GetValue<string>().ToLower();

            var nameArgData = CommandArgumentValue.GetArgumentValue(_nameArg);
            var name = nameArgData.GetValue<string>().ToLower();

            var roleArgData = CommandArgumentValue.GetArgumentValue(_roleArg);
            var roleName = roleArgData.GetValue<string>().ToLower();

            Logger.Info($"Creating user '{name}' at '{ServiceClient.ServiceUri}'");

            RoleDto roleDto;
            try
            {
                roleDto = await ServiceClient.GetRoleByName(roleName);
            }
            catch (ServiceClientResultException)
            {
                Logger.Error($"Role '{roleName}' does not exist at '{ServiceClient.ServiceUri}'");
                return;
            }


            var userDto = new UserDto
            {
                Email = eMail,
                Name = name,
                Roles = new[] {roleDto}
            };

            await ServiceClient.CreateUser(userDto);

            Logger.Info($"User '{name}' added.");
        }
    }
}