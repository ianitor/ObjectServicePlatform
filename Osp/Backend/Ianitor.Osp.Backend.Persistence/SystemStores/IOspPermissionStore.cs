using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemEntities;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public interface IOspPermissionStore
    {
        Task StorePermissionAsync(OspPermission ospPermission);
        Task<OspPermission> GetPermissionById(string permissionId);

        Task EnsurePermission(string permissionId);
    }
}