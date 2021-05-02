using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public class PermissionStore : IOspPermissionStore
    {
        private readonly ICachedCollection<OspPermission> _permissionCollection;
        private readonly IRepository _repository;

        public PermissionStore(ISystemContext systemContext)
        {
            _repository = systemContext.OspSystemDatabase;

            _permissionCollection = _repository.GetCollection<OspPermission>();
        }

        public async Task StorePermissionAsync(OspPermission ospPermission)
        {
            ArgumentValidation.Validate(nameof(ospPermission), ospPermission);

            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var persistentPermission = await GetPermissionById(ospPermission.PermissionId);
            if (persistentPermission == null)
            {
                await _permissionCollection.InsertAsync(session, ospPermission);
            }
            else
            {
                await _permissionCollection.ReplaceByIdAsync(session, persistentPermission.Id,
                    ospPermission);
            }

            await session.CommitTransactionAsync();
        }

        public async Task<OspPermission> GetPermissionById(string permissionId)
        {
            ArgumentValidation.ValidateString(nameof(permissionId), permissionId);

            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var result = await _permissionCollection.FindSingleOrDefaultAsync(session, x => x.PermissionId == permissionId);
            
            await session.CommitTransactionAsync();
            return result;
        }

        public async Task EnsurePermission(string permissionId)
        {
            var permission = await GetPermissionById(permissionId);
            if (permission == null)
            {
                permission = new OspPermission
                {
                    PermissionId = permissionId
                };
                await StorePermissionAsync(permission);
            }
        }
    }
}