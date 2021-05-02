using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Frontend.Client.Authentication
{
    public interface IAuthenticatorClient
    {
        AuthenticatorOptions Options { get; }
        
        Task<EnsureAuthenticatedData> EnsureAuthenticatedAsync(string refreshToken, string accessToken);
        
        Task<DeviceAuthenticationRequestData> RequestDeviceAuthorizationAsync(CommonConstants.ApiScopes apiScopes);

        Task<DeviceAuthenticationData> RequestDeviceTokenAsync(string deviceCode);
        
        Task<AuthenticationData> RequestClientCredentialsTokenAsync(CommonConstants.ApiScopes apiScopes, CommonConstants.DefaultScopes defaultScopes);
        Task<AuthenticationData> RequestPasswordTokenAsync(string username, string password, CommonConstants.ApiScopes apiScopes);
        Task<bool> IntrospectApiResource(string accessToken, string apiName, string apiSecret);

        Task<AuthenticationData> RefreshTokenAsync(string refreshToken);
        
        Task<UserInfoData> GetUserInfoAsync(string accessToken);
    }
}