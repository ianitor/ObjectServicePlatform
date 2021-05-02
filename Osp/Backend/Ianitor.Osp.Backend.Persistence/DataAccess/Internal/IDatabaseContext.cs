using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.DataAccess.Internal
{
    public interface IDatabaseContext
    {
        Task<IOspSession> StartSessionAsync();
        
        ICachedCollection<CkEntity> CkEntities { get; }
        ICachedCollection<CkAttribute> CkAttributes { get; }
        ICachedCollection<CkEntityAssociation> CkEntityAssociations { get; }
        ICachedCollection<CkEntityInheritance> CkEntityInheritances { get; }
        ICachedCollection<RtAssociation> RtAssociations { get; }

        ICachedCollection<TEntity> GetRtCollection<TEntity>(string ckId) where TEntity : RtEntity, new();

        Task<ICollection<CkTypeInfo>> GetCkTypeInfoAsync(IOspSession session);
        Task<CkTypeInfo> GetCkTypeInfoAsync(IOspSession session, string ckId);
        Task<CkTypeInfo> GetCkTypeInfoAsync(IOspSession session, CkEntity ckId);

        Task UpdateCollectionsAsync(IOspSession session);
        Task UpdateIndexAsync(IOspSession session);
    }
}