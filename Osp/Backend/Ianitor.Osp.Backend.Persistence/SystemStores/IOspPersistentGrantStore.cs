using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public interface IOspPersistentGrantStore : IPersistedGrantStore
    {
        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public Task RemoveExpiredGrantsAsync();
    }
}
