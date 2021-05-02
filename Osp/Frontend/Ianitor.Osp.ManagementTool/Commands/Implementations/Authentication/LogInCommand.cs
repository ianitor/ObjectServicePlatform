using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Common.Configuration;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Authentication;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Authentication
{
    internal class LogInCommand : OspCommand
    {
        private readonly IOptions<OspToolAuthenticationOptions> _authenticationOptions;
        private readonly IAuthenticatorClient _authenticatorClient;
        private readonly IAuthenticationService _authenticationService;
        private IArgument _interactiveArg;

        public LogInCommand(IOptions<OspToolOptions> options, IOptions<OspToolAuthenticationOptions> authenticationOptions, IAuthenticatorClient authenticatorClient,
            IAuthenticationService authenticationService)
            : base("LogIn", "LogIn to the configured identity services.", options)
        {
            _authenticationOptions = authenticationOptions;
            _authenticatorClient = authenticatorClient;
            _authenticationService = authenticationService;
        }

        protected override void AddArguments()
        {
            _interactiveArg = CommandArgumentValue.AddArgument("i", "interactive",
                new[] { "Interactive by opening a browser for device log-In" }, false);
        }

        public override async Task Execute()
        {
            bool isInteractive = CommandArgumentValue.IsArgumentUsed(_interactiveArg);

            Logger.Info($"Device log-in at '{Options.Value.IdentityServiceUrl}' in progress...");

            var response = await _authenticatorClient.RequestDeviceAuthorizationAsync(
                CommonConstants.ApiScopes.IdentityApiFullAccess | CommonConstants.ApiScopes.SystemApiFullAccess |
                CommonConstants.ApiScopes.JobApiFullAccess);

            Logger.Info($"Device Code: {response.DeviceCode}");
            Logger.Info($"Estimated code expiration at: {response.ExpiresAt}");
            Logger.Info("");
            Logger.Info("");
            Logger.Info($"Using a browser, visit:");
            Logger.Info($"{response.VerificationUri}");
            Logger.Info($"Enter the code:");
            Logger.Info($"{response.UserCode}");

            if (isInteractive)
            {
                Logger.Info($"Opening default browser...");
                Process.Start(new ProcessStartInfo(response.VerificationUriComplete) { UseShellExecute = true });
            }


            while (true)
            {
                Logger.Info("Waiting for device authentication...");
                Thread.Sleep(response.PollingInterval * 1000);

                var authenticationData = await _authenticatorClient.RequestDeviceTokenAsync(response.DeviceCode);
                if (authenticationData.IsAuthenticationPending)
                {
                    Thread.Sleep(response.PollingInterval * 1000);
                    continue;
                }

                _authenticationService.SaveAuthenticationData(authenticationData);
                
                Logger.Info($"Device log-in successful. Token expires at '{authenticationData.ExpiresAt}'.");
                break;
            }
        }
    }
}