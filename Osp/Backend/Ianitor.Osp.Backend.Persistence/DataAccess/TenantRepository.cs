using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.CkModelEntities;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.Commands;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal class TenantRepository : ITenantRepositoryInternal
    {
        private ICkCache CkCache { get; }

        private readonly IDatabaseContext _databaseContext;

        public TenantRepository(ICkCache ckCache, IDatabaseContext databaseContext)
        {
            CkCache = ckCache;
            _databaseContext = databaseContext;
        }

        #region Transaction Handling

        public async Task<IOspSession> StartSessionAsync()
        {
            return await _databaseContext.StartSessionAsync();
        }

        #endregion Transaction Handling

        #region Data manipulation

        public async Task ApplyChanges(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList,
            IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList)
        {
            CkEntityRuleEngine ckEntityRuleEngine = new CkEntityRuleEngine(CkCache, this);
            var entityValidatorResult = await ckEntityRuleEngine.ValidateAsync(entityUpdateInfoList);

            CkGraphRuleEngine ckGraphRuleEngine = new CkGraphRuleEngine(CkCache, this);
            var graphValidationResult =
                await ckGraphRuleEngine.ValidateAsync(session, entityUpdateInfoList, associationUpdateInfoList);

            await ApplyRtEntityChangesAsync(session, entityValidatorResult);
            await ApplyRtAssociationChangesAsync(session, graphValidationResult);
        }

        public async Task ApplyChanges(IOspSession session,
            IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList)
        {
            await ApplyChanges(session, new List<EntityUpdateInfo>(), associationUpdateInfoList);
        }

        public async Task ApplyChanges(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList)
        {
            await ApplyChanges(session, entityUpdateInfoList, new List<AssociationUpdateInfo>());
        }

        private async Task ApplyRtAssociationChangesAsync(IOspSession session,
            GraphRuleEngineResult graphRuleEngineResult)
        {
            if (graphRuleEngineResult.RtAssociationsToCreate.Any())
            {
                await InsertRtAssociationsAsync(session, graphRuleEngineResult.RtAssociationsToCreate);
            }

            if (graphRuleEngineResult.RtAssociationsToDelete.Any())
            {
                await DeleteRtAssociationsAsync(session, graphRuleEngineResult.RtAssociationsToDelete);
            }
        }

        private async Task ApplyRtEntityChangesAsync(IOspSession session,
            CkEntityRuleEngineResult ckEntityRuleEngineResult)
        {
            if (ckEntityRuleEngineResult.RtEntitiesToDelete.Any())
            {
                await DeleteRtEntityAsync(session, ckEntityRuleEngineResult.RtEntitiesToDelete);
            }

            if (ckEntityRuleEngineResult.RtEntitiesToUpdate.Any())
            {
                await UpdateRtEntities(session, ckEntityRuleEngineResult.RtEntitiesToUpdate);
            }

            if (ckEntityRuleEngineResult.RtEntitiesToCreate.Any())
            {
                await InsertRtEntitiesAsync(session, ckEntityRuleEngineResult.RtEntitiesToCreate);
            }
        }

        public async Task InsertRtEntitiesAsync(IOspSession session, IEnumerable<RtEntity> rtEntityList,
            bool disableAutoIncrement = false)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(rtEntityList), rtEntityList);

            var rtEntities = rtEntityList.ToList();

            if (!disableAutoIncrement)
            {
                foreach (var rtEntitiesByType in rtEntities.GroupBy(x => x.CkId))
                {
                    await RunAutoIncrementAsync(session, rtEntitiesByType.Key, rtEntitiesByType);
                }
            }

            foreach (var groupedEntities in rtEntities.GroupBy(x => x.CkId))
            {
                await _databaseContext.GetRtCollection<RtEntity>(groupedEntities.Key)
                    .InsertMultipleAsync(session, groupedEntities);
            }
        }

        public async Task<AggregatedBulkImportResult> BulkInsertRtEntitiesAsync(IOspSession session,
            IEnumerable<RtEntity> rtEntityList)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(rtEntityList), rtEntityList);

            var results = new List<BulkImportResult>();
            foreach (var groupedEntities in rtEntityList.GroupBy(x => x.CkId))
            {
                results.Add(await _databaseContext.GetRtCollection<RtEntity>(groupedEntities.Key)
                    .BulkImportAsync(session, groupedEntities));
            }

            return new AggregatedBulkImportResult(results);
        }

        private async Task DeleteRtEntityAsync(IOspSession session, IReadOnlyList<RtEntity> rtEntities)
        {
            foreach (var rtEntity in rtEntities.AsParallel())
            {
                await _databaseContext.GetRtCollection<RtEntity>(rtEntity.CkId).DeleteOneAsync(session, rtEntity.RtId);
            }
        }

        private async Task UpdateRtEntities(IOspSession session, IReadOnlyList<RtEntity> rtEntities)
        {
            foreach (var rtEntityGrouping in rtEntities.GroupBy(x => x.CkId))
            {
                var collection = _databaseContext.GetRtCollection<RtEntity>(rtEntityGrouping.Key);

                foreach (var document in rtEntityGrouping.AsParallel())
                {
                    List<UpdateDefinition<RtEntity>> updateDefList = new List<UpdateDefinition<RtEntity>>();
                    foreach (var keyValuePair in document.Attributes)
                    {
                        updateDefList.Add(Builders<RtEntity>.Update.Set(
                            $"{Constants.AttributesName}.{keyValuePair.Key.ToCamelCase()}", keyValuePair.Value));
                    }

                    var updateDefinition = Builders<RtEntity>.Update.Combine(updateDefList);
                    await collection.UpdateOneAsync(session, document.RtId, updateDefinition);
                }
            }
        }

        public async Task InsertRtAssociationsAsync(IOspSession session, IEnumerable<RtAssociation> rtAssociations)
        {
            await _databaseContext.RtAssociations.InsertMultipleAsync(session, rtAssociations);
        }

        public async Task<BulkImportResult> BulkRtAssociationsAsync(IOspSession session,
            IEnumerable<RtAssociation> rtAssociations)
        {
            ArgumentValidation.Validate(nameof(rtAssociations), rtAssociations);

            return await _databaseContext.RtAssociations.BulkImportAsync(session, rtAssociations);
        }

        private async Task DeleteRtAssociationsAsync(IOspSession session, IEnumerable<RtAssociation> rtAssociations)
        {
            await _databaseContext.RtAssociations.DeleteManyAsync(session, rtAssociations.Select(x => x.AssociationId));
        }

        #endregion Data manipulation

        #region Data query

        public async Task<ResultSet<CkAttribute>> GetCkAttributesAsync(IOspSession session,
            IReadOnlyList<string> attributeIds,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null)
        {
            var resultSet = new List<CkAttribute>();
            long totalCount = 0;

            var statementCreator = new CkAttributeStatementCreator(_databaseContext);
            statementCreator.AddFieldFilters(dataQueryOperation.FieldFilters);
            statementCreator.AddIdFilter(attributeIds);
            statementCreator.AddTextSearchFilter(dataQueryOperation.TextSearchFilter);
            statementCreator.AddAttributeSearchFilter(dataQueryOperation.AttributeSearchFilter);
            statementCreator.AddSort(dataQueryOperation.SortOrders);

            var tempResultSet = await statementCreator.ExecuteQuery(session, skip, take);
            resultSet.AddRange(tempResultSet.Result);
            totalCount += tempResultSet.TotalCount;

            return new ResultSet<CkAttribute>(resultSet, totalCount);
        }

        public async Task<ResultSet<CkEntity>> GetCkEntityAsync(IOspSession session, IReadOnlyList<string> ckIds,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null)
        {
            var resultSet = new List<CkEntity>();
            long totalCount = 0;

            var statementCreator = new CkEntityStatementCreator(_databaseContext);
            statementCreator.AddFieldFilters(dataQueryOperation.FieldFilters);
            statementCreator.AddIdFilter(ckIds);
            statementCreator.AddTextSearchFilter(dataQueryOperation.TextSearchFilter);
            statementCreator.AddAttributeSearchFilter(dataQueryOperation.AttributeSearchFilter);
            statementCreator.AddSort(dataQueryOperation.SortOrders);

            var tempResultSet = await statementCreator.ExecuteQuery(session, skip, take);
            resultSet.AddRange(tempResultSet.Result);
            totalCount += tempResultSet.TotalCount;

            return new ResultSet<CkEntity>(resultSet, totalCount);
        }

        public async Task<RtEntity> GetRtEntityAsync(IOspSession session, RtEntityId rtEntityId)
        {
            return await _databaseContext.GetRtCollection<RtEntity>(rtEntityId.CkId)
                .DocumentAsync(session, rtEntityId.RtId.ToObjectId());
        }

        public async Task<TEntity> GetRtEntityAsync<TEntity>(IOspSession session, RtEntityId rtEntityId)
            where TEntity : RtEntity, new()
        {
            ArgumentValidation.Validate(nameof(rtEntityId), rtEntityId);
            ArgumentValidation.ValidateString(nameof(rtEntityId.CkId), rtEntityId.CkId);
            ArgumentValidation.Validate(nameof(rtEntityId.RtId), rtEntityId.RtId);

            return await _databaseContext.GetRtCollection<TEntity>(rtEntityId.CkId)
                .DocumentAsync(session, rtEntityId.RtId.ToObjectId());
        }

        public async Task<ResultSet<RtEntity>> GetRtEntitiesByIdAsync(IOspSession session, string ckId,
            IReadOnlyList<ObjectId> rtIds,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null)
        {
            ArgumentValidation.ValidateString(nameof(ckId), ckId);
            ArgumentValidation.Validate(nameof(rtIds), rtIds);
            ArgumentValidation.Validate(nameof(dataQueryOperation), dataQueryOperation);

            if (!rtIds.Any())
            {
                return new ResultSet<RtEntity>(new List<RtEntity>(), 0);
            }

            var resultSet = new List<RtEntity>();
            long totalCount = 0;
            var entityCacheItem = GetEntityCacheItem(ckId);

            var statementCreator =
                new RtStatementCreator(entityCacheItem, _databaseContext, dataQueryOperation.Language);
            statementCreator.AddFieldFilters(dataQueryOperation.FieldFilters);
            statementCreator.AddIdFilter(rtIds);
            statementCreator.AddTextSearchFilter(dataQueryOperation.TextSearchFilter);
            statementCreator.AddAttributeSearchFilter(dataQueryOperation.AttributeSearchFilter);
            statementCreator.AddSort(dataQueryOperation.SortOrders);

            var tempResultSet = await statementCreator.ExecuteQuery(session, skip, take);
            resultSet.AddRange(tempResultSet.Result);
            totalCount += tempResultSet.TotalCount;

            return new ResultSet<RtEntity>(resultSet, totalCount);
        }

        public async Task<CurrentMultiplicity> GetCurrentRtAssociationMultiplicityAsync(IOspSession session,
            RtEntityId rtEntityId, string roleId,
            GraphDirections graphDirections)
        {
            long counter = 0;
            if (graphDirections == GraphDirections.Inbound || graphDirections == GraphDirections.Any)
            {
                var filterDefinition = Builders<RtAssociation>.Filter.And(
                    Builders<RtAssociation>.Filter.Eq(x => x.TargetRtId, rtEntityId.RtId.ToObjectId()),
                    Builders<RtAssociation>.Filter.Eq(x => x.TargetCkId, rtEntityId.CkId),
                    Builders<RtAssociation>.Filter.Eq(x => x.AssociationRoleId, roleId)
                );

                var r = await _databaseContext.RtAssociations.GetTotalCountAsync(session, filterDefinition);
                counter = Math.Max(r, counter);
            }

            if (graphDirections == GraphDirections.Outbound || graphDirections == GraphDirections.Any)
            {
                var filterDefinition = Builders<RtAssociation>.Filter.And(
                    Builders<RtAssociation>.Filter.Eq(x => x.OriginRtId, rtEntityId.RtId.ToObjectId()),
                    Builders<RtAssociation>.Filter.Eq(x => x.TargetCkId, rtEntityId.CkId),
                    Builders<RtAssociation>.Filter.Eq(x => x.AssociationRoleId, roleId)
                );

                var r = await _databaseContext.RtAssociations.GetTotalCountAsync(session, filterDefinition);
                counter = Math.Max(r, counter);
            }

            if (counter >= 2)
            {
                return CurrentMultiplicity.Many;
            }

            if (counter == 1)
            {
                return CurrentMultiplicity.One;
            }

            return CurrentMultiplicity.Zero;
        }

        public async Task<ResultSet<RtEntity>> GetRtAssociationTargetsAsync(IOspSession session, ObjectId rtId,
            string roleId,
            string targetCkId,
            GraphDirections graphDirection, DataQueryOperation dataQueryOperation, int? skip = null, int? take = null)
        {
            ArgumentValidation.Validate(nameof(rtId), rtId);
            ArgumentValidation.ValidateString(nameof(roleId), roleId);
            ArgumentValidation.ValidateString(nameof(targetCkId), targetCkId);
            ArgumentValidation.Validate(nameof(dataQueryOperation), dataQueryOperation);
            
            var entityCacheItem = GetEntityCacheItem(targetCkId);

            var hierarchicalRtStatementCreator =
                new HierarchicalRtStatementCreator(entityCacheItem, _databaseContext, dataQueryOperation.Language, rtId, graphDirection, targetCkId);

            hierarchicalRtStatementCreator.AddFieldFilters(dataQueryOperation.FieldFilters);
            hierarchicalRtStatementCreator.AddTextSearchFilter(dataQueryOperation.TextSearchFilter);
            hierarchicalRtStatementCreator.AddAttributeSearchFilter(dataQueryOperation.AttributeSearchFilter);
            hierarchicalRtStatementCreator.AddSort(dataQueryOperation.SortOrders);

            return await hierarchicalRtStatementCreator.ExecuteQuery(session, skip, take);
        }

        public async Task<ResultSet<RtEntity>> GetRtEntitiesByTypeAsync(IOspSession session, string ckId,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null)
        {
            return await GetRtEntitiesByTypeAsync<RtEntity>(session, ckId, dataQueryOperation, skip, take);
        }

        public async Task<ResultSet<TEntity>> GetRtEntitiesByTypeAsync<TEntity>(IOspSession session,
            DataQueryOperation dataQueryOperation,
            int? skip = null, int? take = null) where TEntity : RtEntity, new()
        {
            var ckId = GetCkId<TEntity>();
            return await GetRtEntitiesByTypeAsync<TEntity>(session, ckId, dataQueryOperation, skip, take);
        }

        private async Task<ResultSet<TEntity>> GetRtEntitiesByTypeAsync<TEntity>(IOspSession session, string ckId,
            DataQueryOperation dataQueryOperation, int? skip = null, int? take = null) where TEntity : RtEntity, new()
        {
            ArgumentValidation.Validate(nameof(dataQueryOperation), dataQueryOperation);

            var entityCacheItem = GetEntityCacheItem(ckId);
            var statementCreator =
                new RtStatementCreator<TEntity>(entityCacheItem, _databaseContext, dataQueryOperation.Language);
            statementCreator.AddFieldFilters(dataQueryOperation.FieldFilters);
            statementCreator.AddTextSearchFilter(dataQueryOperation.TextSearchFilter);
            statementCreator.AddAttributeSearchFilter(dataQueryOperation.AttributeSearchFilter);
            statementCreator.AddSort(dataQueryOperation.SortOrders);

            return await statementCreator.ExecuteQuery(session, skip, take);
        }

        public async Task<RtAssociation> GetRtAssociationAsync(IOspSession session, RtEntityId rtEntityIdOrigin,
            RtEntityId rtEntityIdTarget,
            string roleId)
        {
            return await _databaseContext.RtAssociations.FindSingleOrDefaultAsync(session, x =>
                x.OriginRtId == rtEntityIdOrigin.RtId.ToObjectId()
                && x.OriginCkId == rtEntityIdOrigin.CkId
                && x.TargetRtId == rtEntityIdTarget.RtId.ToObjectId()
                && x.TargetCkId == rtEntityIdTarget.CkId
                && x.AssociationRoleId == roleId);
        }

        public async Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, string rtId,
            GraphDirections graphDirections, string roleId)
        {
            return await GetRtAssociationsAsync(session, ObjectId.Parse(rtId), graphDirections, roleId);
        }

        public async Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, ObjectId rtId,
            GraphDirections graphDirections, string roleId)
        {
            List<RtAssociation> associations = new List<RtAssociation>();

            if (graphDirections == GraphDirections.Any || graphDirections == GraphDirections.Inbound)
            {
                associations.AddRange(await _databaseContext.RtAssociations.FindManyAsync(session, x =>
                    x.TargetRtId == rtId && x.AssociationRoleId == roleId));
            }

            if (graphDirections == GraphDirections.Any || graphDirections == GraphDirections.Outbound)
            {
                associations.AddRange(await _databaseContext.RtAssociations.FindManyAsync(session, x =>
                    x.OriginRtId == rtId && x.AssociationRoleId == roleId));
            }

            return associations;
        }

        public async Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, string rtId,
            GraphDirections graphDirections)
        {
            return await GetRtAssociationsAsync(session, ObjectId.Parse(rtId), graphDirections);
        }

        public async Task<IReadOnlyList<RtAssociation>> GetRtAssociationsAsync(IOspSession session, ObjectId rtId,
            GraphDirections graphDirections)
        {
            List<RtAssociation> associations = new List<RtAssociation>();

            if (graphDirections == GraphDirections.Any || graphDirections == GraphDirections.Inbound)
            {
                associations.AddRange(await _databaseContext.RtAssociations.FindManyAsync(session, x =>
                    x.TargetRtId == rtId));
            }

            if (graphDirections == GraphDirections.Any || graphDirections == GraphDirections.Outbound)
            {
                associations.AddRange(await _databaseContext.RtAssociations.FindManyAsync(session, x =>
                    x.OriginRtId == rtId));
            }

            return associations;
        }

        #endregion Data query

        #region Transient data

        private static string GetCkId<TEntity>() where TEntity : RtEntity, new()
        {
            CkIdAttribute ckIdAttribute =
                (CkIdAttribute) Attribute.GetCustomAttribute(typeof(TEntity), typeof(CkIdAttribute));

            return ckIdAttribute.CkId;
        }

        public RtEntity CreateTransientRtEntity(string ckId)
        {
            ArgumentValidation.ValidateString(nameof(ckId), ckId);

            var entityCacheItem = CkCache.GetEntityCacheItem(ckId);
            return CreateTransientRtEntity<RtEntity>(entityCacheItem);
        }

        public RtEntity CreateTransientRtEntity(EntityCacheItem entityCacheItem)
        {
            return CreateTransientRtEntity<RtEntity>(entityCacheItem);
        }

        public TEntity CreateTransientRtEntity<TEntity>() where TEntity : RtEntity, new()
        {
            var ckId = GetCkId<TEntity>();
            if (string.IsNullOrWhiteSpace(ckId))
            {
                throw new InvalidCkIdException($"No Construction Kit Id for type '{typeof(TEntity).FullName}'" +
                                               $" is defined. Is attribute '{typeof(CkIdAttribute).FullName}' missing?");
            }

            var entityCacheItem = CkCache.GetEntityCacheItem(ckId);
            if (entityCacheItem == null)
            {
                throw new InvalidCkIdException($"Construction Kit Id '{ckId}' was not found in model cache." +
                                               $" Wrong CkId used?");
            }

            return CreateTransientRtEntity<TEntity>(entityCacheItem);
        }

        private TEntity CreateTransientRtEntity<TEntity>(EntityCacheItem entityCacheItem)
            where TEntity : RtEntity, new()
        {
            ArgumentValidation.Validate(nameof(entityCacheItem), entityCacheItem);

            var rtEntity = new TEntity
            {
                RtId = ObjectId.GenerateNewId(),
                CkId = entityCacheItem.CkId
            };
            foreach (var attributeCacheItem in entityCacheItem.Attributes.Values)
            {
                object value = attributeCacheItem.DefaultValue ?? attributeCacheItem.DefaultValues?.ToList();
                rtEntity.SetAttributeValue(attributeCacheItem.AttributeName, attributeCacheItem.AttributeValueType,
                    value);
            }

            return rtEntity;
        }

        #endregion Transient data

        #region Advanced functionality

        public IUpdateStream<RtEntity> SubscribeToRtEntities(string ckId, UpdateStreamFilter updateStreamFilter,
            CancellationToken cancellationToken = default)
        {
            return _databaseContext.GetRtCollection<RtEntity>(ckId).Subscribe(updateStreamFilter, cancellationToken);
        }

        public async Task<IEnumerable<AutoCompleteText>> ExtractAutoCompleteValuesAsync(IOspSession session,
            string ckId,
            string attributeName, string regexFilterValue, int takeCount)
        {
            ArgumentValidation.ValidateString(nameof(ckId), ckId);
            ArgumentValidation.ValidateString(nameof(attributeName), attributeName);
            ArgumentValidation.ValidateString(nameof(regexFilterValue), regexFilterValue);

            var entityCacheItem = GetEntityCacheItem(ckId);
            if (entityCacheItem == null)
            {
                throw new InvalidCkIdException($"Construction Kit Id '{ckId}' is invalid.");
            }

            if (!entityCacheItem.Attributes.Keys.Contains(attributeName))
            {
                throw new InvalidAttributeException(
                    $"Attribute '{attributeName}' does not exist at type '{ckId}'");
            }

            var match = new BsonDocument
            {
                {
                    "$match", new BsonDocument
                    {
                        {
                            $"attributes.{attributeName.ToCamelCase()}", new BsonDocument
                            {
                                {
                                    "$regex", regexFilterValue
                                }
                            }
                        }
                    }
                }
            };

            var sortByCount = new BsonDocument
            {
                {
                    "$sortByCount", $"$attributes.{attributeName.ToCamelCase()}"
                }
            };

            var limit = new BsonDocument
            {
                {
                    "$limit", takeCount
                }
            };

            var collection = _databaseContext.GetRtCollection<RtEntity>(entityCacheItem.CkId);
            var result = collection.Aggregate(session,
                PipelineDefinition<RtEntity, AutoCompleteText>.Create(match, sortByCount, limit));
            return await result.ToListAsync();
        }


        private async Task RunAutoIncrementAsync(IOspSession session, string ckId, IEnumerable<RtEntity> rtEntities)
        {
            var entityCacheItem = CkCache.GetEntityCacheItem(ckId);
            if (entityCacheItem == null)
            {
                throw new InvalidCkIdException($"Construction Kit Id '{ckId}' is invalid.");
            }

            var autoIncrementReferences = entityCacheItem.Attributes.Values
                .Where(a => !string.IsNullOrEmpty(a.AutoIncrementReference)).ToList();


            var dataQueryOperation = new DataQueryOperation
            {
                FieldFilters = new[]
                {
                    new FieldFilter(nameof(RtEntity.WellKnownName), FieldFilterOperator.In,
                        autoIncrementReferences.Select(x => x.AutoIncrementReference))
                }
            };

            var autoIncrementerSet = await GetRtEntitiesByTypeAsync<RtSystemAutoIncrement>(session, dataQueryOperation);

            foreach (var rtEntity in rtEntities)
            {
                foreach (var autoIncrementReference in autoIncrementReferences)
                {
                    var attributeCacheItem = entityCacheItem.Attributes[autoIncrementReference.AttributeName];
                    if (attributeCacheItem == null)
                    {
                        throw new InvalidAttributeException(
                            $"Attribute with name '{autoIncrementReference.AttributeName}' does not exist at Ck-Id {ckId}");
                    }

                    var autoIncrement = autoIncrementerSet.Result.FirstOrDefault(x =>
                        x.WellKnownName == autoIncrementReference.AutoIncrementReference);

                    rtEntity.SetAttributeValue(autoIncrementReference.AttributeName,
                        attributeCacheItem.AttributeValueType,
                        await ExecuteAutoIncrementAsync(session, autoIncrement));
                }
            }
        }

        private async Task<long> ExecuteAutoIncrementAsync(IOspSession session, RtSystemAutoIncrement autoIncrement)
        {
            var end = autoIncrement.End;
            if (!autoIncrement.CurrentValue.HasValue)
            {
                throw new AutoIncrementFailedException(
                    $"'{autoIncrement.RtId}' cannot be incremented because current value was null.");
            }

            var currentValue = autoIncrement.CurrentValue.Value;

            currentValue++;

            if (currentValue > end)
            {
                throw new AutoIncrementFailedException(
                    $"'{autoIncrement.RtId}' cannot be incremented because end value is reached.");
            }

            autoIncrement.CurrentValue = currentValue;
            await _databaseContext.GetRtCollection<RtEntity>(autoIncrement.CkId)
                .ReplaceByIdAsync(session, autoIncrement.RtId, autoIncrement);

            return currentValue;
        }

        public async Task UpdateAutoCompleteTexts(IOspSession session, string ckId, string attributeName,
            IEnumerable<string> autoCompleteTexts)
        {
            ArgumentValidation.ValidateString(nameof(ckId), ckId);
            ArgumentValidation.ValidateString(nameof(attributeName), attributeName);
            ArgumentValidation.Validate(nameof(autoCompleteTexts), autoCompleteTexts);

            var ckEntity =
                await _databaseContext.CkEntities.FindSingleOrDefaultAsync(session, x => x.CkId == ckId);
            if (ckEntity == null)
            {
                throw new EntityNotFoundException($"'{ckId}' does not exist in database.");
            }

            var attribute = ckEntity.Attributes.FirstOrDefault(x => x.AttributeName == attributeName);
            if (attribute == null)
            {
                throw new InvalidAttributeException(
                    $"Attribute with name '{attributeName}' does not exist on type '{ckId}'");
            }

            attribute.AutoCompleteTexts = autoCompleteTexts.ToList();

            try
            {
                await _databaseContext.CkEntities.ReplaceByIdAsync(session, ckEntity.CkId, ckEntity);
            }
            catch (Exception e)
            {
                throw new OperationFailedException("An error occurred during import: " + e.Message, e);
            }
        }

        #endregion Advanced functionality


        #region Helper

        private EntityCacheItem GetEntityCacheItem(string ckId)
        {
            var entityCacheItem = CkCache.GetEntityCacheItem(ckId);
            if (entityCacheItem == null)
            {
                throw new OperationFailedException($"Type '{ckId}' does not exist.");
            }

            return entityCacheItem;
        }

        #endregion Helper
    }
}