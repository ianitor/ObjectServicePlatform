using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Users
{
    internal class UpdateUser : ServiceClientOspCommand<IIdentityServicesClient>
    {
        private IArgument _eMailArg;
        private IArgument _nameArg;
        private IArgument _newNameArg;
        private IArgument _roleArg;

        public UpdateUser(IOptions<OspToolOptions> options, IIdentityServicesClient identityServicesClient, IAuthenticationService authenticationService)
            : base("UpdateUser", "Updates an user", options, identityServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _eMailArg = CommandArgumentValue.AddArgument("e", "eMail", new[] {"E-Mail of user"},
                false, 1);
            _nameArg = CommandArgumentValue.AddArgument("un", "userName", new[] {"User name"}, true,
                1);
            _newNameArg = CommandArgumentValue.AddArgument("nun", "newUserName", new[] {"New user name, if the user name has to be changed"}, false,
                1);
            _roleArg = CommandArgumentValue.AddArgument("r", "role", new[] {"Role of user"}, false,
                1);
        }

        public override async Task Execute()
        {
            var nameArgData = CommandArgumentValue.GetArgumentValue(_nameArg);
            var name = nameArgData.GetValue<string>().ToLower();

            string newUserName = null;
            if (CommandArgumentValue.IsArgumentUsed(_newNameArg))
            {
                var newUserNameArgData = CommandArgumentValue.GetArgumentValue(_newNameArg);
                newUserName = newUserNameArgData.GetValue<string>().ToLower();
            }

            string roleName = null;
            if (CommandArgumentValue.IsArgumentUsed(_roleArg))
            {
                var roleArgData = CommandArgumentValue.GetArgumentValue(_roleArg);
                roleName = roleArgData.GetValue<string>().ToLower();
            }

            string eMail = null;
            if (CommandArgumentValue.IsArgumentUsed(_eMailArg))
            {
                var eMailArgData = CommandArgumentValue.GetArgumentValue(_eMailArg);
                eMail = eMailArgData.GetValue<string>().ToLower();
            }

            Logger.Info($"Updating user '{name}' at '{ServiceClient.ServiceUri}'");

            var userDto = new UserDto
            {
                Email = eMail,
                Name = newUserName,
                Roles = new List<RoleDto>()
            };

            if (!string.IsNullOrWhiteSpace(roleName))
            {
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
                
                userDto.Roles = new[] {roleDto};
            }

            await ServiceClient.UpdateUser(name, userDto);

            Logger.Info($"User '{name}' updated.");
        }
    }
}