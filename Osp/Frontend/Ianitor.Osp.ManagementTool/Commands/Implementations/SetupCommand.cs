using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Common.Configuration;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.System;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class SetupCommand : OspCommand
    {
        private readonly IIdentityServicesSetupClient _identityServicesSetupClient;
        private IArgument _eMailArg;
        private IArgument _passwordArg;

        public SetupCommand(IOptions<OspToolOptions> options, IIdentityServicesSetupClient identityServicesSetupClient)
            : base("Setup", "Sets identity services up", options)
        {
            _identityServicesSetupClient = identityServicesSetupClient;
        }

        protected override void AddArguments()
        {
            _eMailArg = CommandArgumentValue.AddArgument("e", "email",
                new[] {"E-Mail of admin"}, 1);
            _passwordArg = CommandArgumentValue.AddArgument("p", "password",
                new[] {"Password of admin"}, 1);
        }

        public override async Task Execute()
        {
            Logger.Info("Adding admin");

            var eMail = CommandArgumentValue.GetArgumentScalarValue<string>(_eMailArg);
            var password = CommandArgumentValue.GetArgumentScalarValue<string>(_passwordArg);

            await _identityServicesSetupClient.AddAdminUser(new AdminUserDto
            {
                EMail = eMail,
                Password = password
            });
        }
    }
}