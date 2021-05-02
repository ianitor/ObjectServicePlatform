using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands
{
    internal abstract class JobWithWaitOspCommand : JobOspCommand
    {
        private IArgument _waitForJobArg;

        protected JobWithWaitOspCommand(string commandValue, string commandDescription, IOptions<OspToolOptions> options,
            IJobServicesClient jobServicesClient, IAuthenticationService authenticationService)
            : base(commandValue, commandDescription, options, jobServicesClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            base.AddArguments();

            _waitForJobArg = CommandArgumentValue.AddArgument("w", "wait",
                new[] {"Wait for a import job to complete"}, false, 0);
        }

        protected override async Task WaitForJob(string id)
        {
            if (CommandArgumentValue.IsArgumentUsed(_waitForJobArg))
            {
                await base.WaitForJob(id);
            }
        }
    }
}