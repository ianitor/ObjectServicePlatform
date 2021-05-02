using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Common.Shared;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Frontend.Client.Authentication
{
    public class AuthenticatorClient : IAuthenticatorClient
    {
        public AuthenticatorOptions Options { get; private set; }
        private IDiscoveryCache _cache;

        private IDiscoveryCache Cache
        {
            get
            {
                if (_cache == null)
                {
                    if (string.IsNullOrWhiteSpace(Options.IssuerUri))
                    {
                        throw new ServiceConfigurationMissingException($"Issuer URI is not configured.");
                    }
                    
                    var url = new Uri(Options.IssuerUri);
                    _cache = new DiscoveryCache(url.AbsoluteUri.TrimEnd('/'));
                }

                return _cache;
            }
        }

        public AuthenticatorClient(IOptionsMonitor<AuthenticatorOptions> options)
        {
            options.OnChange(CreateCache);
            CreateCache(options.CurrentValue);
        }

        private void CreateCache(AuthenticatorOptions authenticatorOptions)
        {
            ArgumentValidation.Validate(nameof(authenticatorOptions), authenticatorOptions);

            Options = authenticatorOptions;
        }

        public async Task<EnsureAuthenticatedData> EnsureAuthenticatedAsync(string refreshToken, string accessToken)
        {
            ArgumentValidation.ValidateString(nameof(refreshToken), refreshToken);
            ArgumentValidation.ValidateString(nameof(accessToken), accessToken);

            var userInfoData = await GetUserInfoAsync(accessToken);
            if (!userInfoData.IsAuthenticated)
            {
                var authenticationData = await RefreshTokenAsync(refreshToken);
                return new EnsureAuthenticatedData
                    {IsRefreshDone = true, RefreshedAuthenticationData = authenticationData};
            }

            return new EnsureAuthenticatedData {IsRefreshDone = false};
        }

        public async Task<AuthenticationData> RequestClientCredentialsTokenAsync(CommonConstants.ApiScopes apiScopes,
            CommonConstants.DefaultScopes defaultScopes)
        {
            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                GrantType = OidcConstants.GrantTypes.ClientCredentials,

                Address = disco.TokenEndpoint,

                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,

                Scope = CommonConstants.GetScopes(apiScopes, defaultScopes)
            });

            ValidateResponse(response);

            return new AuthenticationData
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
            };
        }

        public async Task<DeviceAuthenticationRequestData> RequestDeviceAuthorizationAsync(
            CommonConstants.ApiScopes apiScopes)
        {
            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,

                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,

                Scope = CommonConstants.GetScopes(apiScopes,
                    CommonConstants.DefaultScopes.UserDefault | CommonConstants.DefaultScopes.OfflineAccess)
            });

            ValidateResponse(response);

            return new DeviceAuthenticationRequestData
            {
                UserCode = response.UserCode,
                DeviceCode = response.DeviceCode,
                VerificationUri = response.VerificationUri,
                VerificationUriComplete = response.VerificationUriComplete,
                PollingInterval = response.Interval,
                ExpiresAt = response.ExpiresIn.HasValue
                    ? DateTime.Now.AddSeconds(response.ExpiresIn.Value)
                    : default(DateTime?)
            };
        }

        public async Task<DeviceAuthenticationData> RequestDeviceTokenAsync(string deviceCode)
        {
            ArgumentValidation.ValidateString(nameof(deviceCode), deviceCode);

            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,

                DeviceCode = deviceCode
            });

            if (response.IsError && (response.Error == OidcConstants.TokenErrors.AuthorizationPending ||
                                     response.Error == OidcConstants.TokenErrors.SlowDown))
            {
                return new DeviceAuthenticationData {IsAuthenticationPending = true};
            }

            ValidateResponse(response);

            return new DeviceAuthenticationData
            {
                IsAuthenticationPending = false,
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
            };
        }

        public async Task<AuthenticationData> RequestPasswordTokenAsync(string username, string password,
            CommonConstants.ApiScopes apiScopes)
        {
            ArgumentValidation.ValidateString(nameof(username), username);
            ArgumentValidation.ValidateString(nameof(password), password);

            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,

                UserName = username,
                Password = password,

                Scope = CommonConstants.GetScopes(apiScopes,
                    CommonConstants.DefaultScopes.UserDefault | CommonConstants.DefaultScopes.OfflineAccess)
            });

            ValidateResponse(response);

            return new AuthenticationData
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
            };
        }

        private static void ValidateResponse(ProtocolResponse response)
        {
            if (response.IsError)
            {
                throw new AuthenticationFailedException(response.Error, response.Error, response.Exception);
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryResponse()
        {
            var disco = await Cache.GetAsync();
            ValidateResponse(disco);

            return disco;
        }

        public async Task<AuthenticationData> RefreshTokenAsync(string refreshToken)
        {
            ArgumentValidation.ValidateString(nameof(refreshToken), refreshToken);

            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,

                RefreshToken = refreshToken
            });
            ValidateResponse(response);

            return new AuthenticationData
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
            };
        }

        public async Task<UserInfoData> GetUserInfoAsync(string accessToken)
        {
            ArgumentValidation.ValidateString(nameof(accessToken), accessToken);

            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
            {
                return new UserInfoData(false, null);
            }

            return new UserInfoData(true, response.Claims);
        }

        public async Task<bool> IntrospectApiResource(string accessToken, string apiName, string apiSecret)
        {
            ArgumentValidation.ValidateString(nameof(accessToken), accessToken);
            ArgumentValidation.ValidateString(nameof(apiName), apiName);
            ArgumentValidation.ValidateString(nameof(apiSecret), apiSecret);

            var disco = await GetDiscoveryResponse();

            var client = new HttpClient();
            var result = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = disco.IntrospectionEndpoint,

                ClientId = apiName,
                ClientSecret = apiSecret,

                Token = accessToken
            });

            if (result.IsError || !result.IsActive)
            {
                return false;
            }

            return true;
        }
    }
}