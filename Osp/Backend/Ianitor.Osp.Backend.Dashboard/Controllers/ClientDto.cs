using System.Text.Json.Serialization;
using Ianitor.Osp.Common.Shared;


namespace Ianitor.Osp.Backend.Dashboard.Controllers
{
    public class ClientDto
    {
        public ClientDto(string clientId, OspDashboardOptions ospDashboardOptions)
        {
            ClientId = clientId;
            Authority = ospDashboardOptions.AuthorityUrl.EnsureEndsWith("/");
            CoreServices = ospDashboardOptions.CoreServiceUrl.EnsureEndsWith("/");
            JobServices = ospDashboardOptions.JobServiceUrl.EnsureEndsWith("/");
            RedirectUri = ospDashboardOptions.PublicUrl.EnsureEndsWith("/");
            PostLogoutRedirectUri = ospDashboardOptions.PublicUrl.EnsureEndsWith("/");
            Scope = CommonConstants.GetScopes(CommonConstants.ApiScopes.IdentityApiFullAccess | CommonConstants.ApiScopes.SystemApiFullAccess | CommonConstants.ApiScopes.JobApiFullAccess, 
                CommonConstants.DefaultScopes.UserDefault | CommonConstants.DefaultScopes.OfflineAccess);
        }

        [JsonPropertyName("coreServices")] public string CoreServices { get; set; }

        [JsonPropertyName("jobServices")] public string JobServices { get; set; }

        [JsonPropertyName("issuer")] public string Authority { get; set; }

        [JsonPropertyName("clientId")] public string ClientId { get; set; }

        [JsonPropertyName("redirectUri")] public string RedirectUri { get; set; }

        [JsonPropertyName("postLogoutRedirectUri")]
        public string PostLogoutRedirectUri { get; set; }

        [JsonPropertyName("scope")] public string Scope { get; set; }
    }
}