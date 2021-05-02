using Ianitor.Osp.Backend.Persistence.DataAccess;

namespace Ianitor.Osp.Backend.Persistence
{
    internal interface ITenantContextInternal : ITenantContext
    {
        ITenantRepositoryInternal InternalRepository { get; }
    }
}