using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public interface ITenantRepository
    {
        #region Transaction Handling

        Task<IOspSession> StartSessionAsync();
        
        #endregion Transaction Handling

        #region Data manipulation

        Task ApplyChanges(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList,
            IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList);

        // ReSharper disable once UnusedMember.Global
        Task ApplyChanges(IOspSession session, IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList);
        Task ApplyChanges(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList);

        #endregion Data manipulation

        #region Data query

        Task<ResultSet<CkAttribute>> GetCkAttributesAsync(IOspSession session, IReadOnlyList<string> attributeIds,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null);

        Task<ResultSet<CkEntity>> GetCkEntityAsync(IOspSession session, IReadOnlyList<string> ckIds, DataQueryOperation dataQueryOperation,
            int? skip = null, int? take = null);

        Task<RtEntity> GetRtEntityAsync(IOspSession session, RtEntityId rtEntityId);
        Task<TEntity> GetRtEntityAsync<TEntity>(IOspSession session, RtEntityId rtEntityId) where TEntity : RtEntity, new();
        
        Task<ResultSet<RtEntity>> GetRtEntitiesByIdAsync(IOspSession session, string ckId, IReadOnlyList<ObjectId> rtIds,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null);

        Task<ResultSet<RtEntity>> GetRtAssociationTargetsAsync(IOspSession session, ObjectId rtId, string roleId, string targetCkId,
            GraphDirections graphDirection, DataQueryOperation dataQueryOperation, int? skip = null, int? take = null);

        Task<RtAssociation> GetRtAssociationAsync(IOspSession session, RtEntityId rtEntityIdOrigin, RtEntityId rtEntityIdTarget,
            string roleId);

        Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, ObjectId rtId,
            GraphDirections graphDirections, string roleId);

        Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, ObjectId rtId,
            GraphDirections graphDirections);
        
        Task<ResultSet<RtEntity>> GetRtEntitiesByTypeAsync(IOspSession session, string ckId, DataQueryOperation dataQueryOperation,
            int? skip = null, int? take = null);

        Task<ResultSet<TEntity>> GetRtEntitiesByTypeAsync<TEntity>(IOspSession session, DataQueryOperation dataQueryOperation,
            int? skip = null, int? take = null) where TEntity : RtEntity, new();

        // ReSharper disable once UnusedMember.Global
        Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, string rtId, GraphDirections graphDirections,
            string roleId);

        Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, string rtId, GraphDirections graphDirections);

        #endregion Data query

        #region Transient data

        RtEntity CreateTransientRtEntity(string ckId);

        // ReSharper disable once UnusedMemberInSuper.Global
        RtEntity CreateTransientRtEntity(EntityCacheItem entityCacheItem);
        TEntity CreateTransientRtEntity<TEntity>() where TEntity : RtEntity, new();

        #endregion Transient data

        #region Advanced functionality

        IUpdateStream<RtEntity> SubscribeToRtEntities(string ckId, UpdateStreamFilter updateStreamFilter,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<AutoCompleteText>> ExtractAutoCompleteValuesAsync(IOspSession session, string ckId, string attributeName,
            string regexFilterValue, int takeCount);

        Task UpdateAutoCompleteTexts(IOspSession session, string rtId, string attributeName, IEnumerable<string> autoCompleteTexts);

        #endregion Advanced functionality
    }
}