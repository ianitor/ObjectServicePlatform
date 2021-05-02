using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public interface IOspResourceStore : IResourceStore
    {
        Task CreateApiResourceAsync(OspApiResource apiResource);
        Task CreateIdentityResourceAsync(OspIdentityResource identityResource);
        Task CreateApiScopeAsync(OspApiScope apiScope);
        Task<OspIdentityResource> TryCreateIdentityResourceAsync(IdentityResource identityResource);
        Task<OspApiScope> TryCreateApiScopeAsync(ApiScope apiScope);
        Task<OspApiResource> TryCreateApiResourceAsync(ApiResource apiResource);
        Task DeleteApiResourceAsync(string resourceId);
        Task DeleteIdentityResourceAsync(string resourceId);
        Task DeleteApiScopeAsync(string resourceId);

        Task<OspApiResource> GetApiResourceByNameAsync(string apiResourceName);
        Task<OspIdentityResource> GetIdentityResourceByNameAsync(string identityResourceName);
    }
}
