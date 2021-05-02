using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal interface ITenantRepositoryInternal : ITenantRepository
    {
        Task<CurrentMultiplicity> GetCurrentRtAssociationMultiplicityAsync(IOspSession session, RtEntityId rtEntityId, string roleId, GraphDirections graphDirections);

        Task InsertRtEntitiesAsync(IOspSession session, IEnumerable<RtEntity> rtEntityList, bool disableAutoIncrement = false);
        Task<AggregatedBulkImportResult> BulkInsertRtEntitiesAsync(IOspSession session, IEnumerable<RtEntity> rtEntityList);

        Task InsertRtAssociationsAsync(IOspSession session, IEnumerable<RtAssociation> rtAssociations);
        Task<BulkImportResult> BulkRtAssociationsAsync(IOspSession session, IEnumerable<RtAssociation> rtAssociations);

    }
}