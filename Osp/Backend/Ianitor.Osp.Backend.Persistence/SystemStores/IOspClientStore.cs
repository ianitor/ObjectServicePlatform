using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using IdentityServer4.Stores;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public interface IOspClientStore : IClientStore
    {
        Task<IEnumerable<OspClient>> GetClients();

        Task CreateAsync(OspClient client);

        Task UpdateAsync(string clientId, OspClient client);

        Task DeleteAsync(string clientId);
    }
}
