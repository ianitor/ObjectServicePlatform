using System;
using System.Threading.Tasks;
using Ianitor.Common.Configuration;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Authentication;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.Authentication
{
    internal class AuthStatusCommand : OspCommand
    {
        private readonly IOptions<OspToolAuthenticationOptions> _authenticationOptions;
        private readonly IAuthenticatorClient _authenticatorClient;
        private readonly IAuthenticationService _authenticationService;

        public AuthStatusCommand(IOptions<OspToolOptions> options, IOptions<OspToolAuthenticationOptions> authenticationOptions, IAuthenticatorClient authenticatorClient, IAuthenticationService authenticationService)
        : base("AuthStatus", "Gets authentication status to the configured identity services.", options)
        {
            _authenticationOptions = authenticationOptions;
            _authenticatorClient = authenticatorClient;
            _authenticationService = authenticationService;
        }

        protected override void AddArguments()
        {
        }

        public override async Task Execute()
        {
            Logger.Info($"Check of authentication status at '{Options.Value.IdentityServiceUrl}' in progress...");

            bool result = await TestAuthenticationStatus();
            if (!result)
            {
                Logger.Info($"Refreshing token.");
                var authenticationData = await _authenticatorClient.RefreshTokenAsync(_authenticationOptions.Value.RefreshToken);
                
                _authenticationService.SaveAuthenticationData(authenticationData);
                
                Logger.Info($"Refresh successful. Token expires at '{authenticationData.ExpiresAt}'.");
                await TestAuthenticationStatus();
            }
        }

        private async Task<bool> TestAuthenticationStatus()
        {
            var userInfoData = await _authenticatorClient.GetUserInfoAsync(_authenticationOptions.Value.AccessToken);

            if (userInfoData.IsAuthenticated)
            {
                Logger.Info($"Access token is valid.");
                foreach (var claim in userInfoData.Claims)
                {
                    Logger.Info($"\t{claim.Type}: {claim.Value}");
                }
                Logger.Info($"\tAccess Token: {_authenticationOptions.Value.AccessToken}");

                return true;
            }

            Logger.Info($"Access token is INVALID.");

            return false;
        }
    }
}