using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using IdentityServer4.Services;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IOspClientStore _clientStore;

        public CorsPolicyService(IOspClientStore clientStore)
        {
            _clientStore = clientStore;
        }


        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var clients = await _clientStore.GetClients();
            var result = clients.Any(x => x.AllowedCorsOrigins?.Contains(origin) ?? false);
            return result;
        }
    }
}
