using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache
{
    public interface ICkCache : IDisposable
    {
        string TenantId { get; }
        
        bool IsDisposed { get; }

        IEnumerable<EntityCacheItem> GetCkEntities();

        EntityCacheItem GetEntityCacheItem(string ckId);

        Task Initialize(IDatabaseContext databaseContext);
    }
}