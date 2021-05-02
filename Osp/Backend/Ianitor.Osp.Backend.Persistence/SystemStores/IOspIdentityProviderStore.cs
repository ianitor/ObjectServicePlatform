using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemEntities;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public interface IOspIdentityProviderStore
    {
        Task<OspIdentityProvider> GetAsync(string id);

        Task<IEnumerable<OspIdentityProvider>> GetAllAsync();

        Task StoreAsync(OspIdentityProvider identityProvider);
        Task RemoveAsync(string id);
    }
}