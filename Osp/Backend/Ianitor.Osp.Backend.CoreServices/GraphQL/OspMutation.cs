using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    /// <summary>
    /// Implements mutations of OSP
    /// </summary>
    public class OspMutation : ObjectGraphType
    {
        private readonly ICkCache _ckCache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityCacheItems">List of CkEntities for RT generation</param>
        /// <param name="graphTypesCache">The cache</param>
        /// <param name="ckCache"></param>
        public OspMutation(IEnumerable<EntityCacheItem> entityCacheItems, IGraphTypesCache graphTypesCache, ICkCache ckCache)
        {
            _ckCache = ckCache;
            foreach (var cacheItem in entityCacheItems)
            {
                var inputType = graphTypesCache.GetOrCreateInput(cacheItem.CkId);
                var outputType = graphTypesCache.GetOrCreate(cacheItem.CkId);

                var createArgument = new QueryArgument(new NonNullGraphType(new ListGraphType(inputType))) {Name = Statics.EntitiesArg};
                var updateArgument = new QueryArgument(new NonNullGraphType(new ListGraphType(new UpdateMutationDtoType<RtEntityDto>(inputType)))) {Name = Statics.EntitiesArg};
                var deleteArgument = new QueryArgument(new NonNullGraphType(new ListGraphType(new DeleteMutationDtoType(inputType)))) {Name = Statics.EntitiesArg};

                this.FieldAsync($"create{outputType.Name}s", $"Creates new entities of type '{outputType.Name}'.", new ListGraphType(outputType),
                        new QueryArguments(createArgument), resolve: ResolveCreate)
                    .AddMetadata(Statics.CkId, cacheItem.CkId);

                this.FieldAsync($"update{outputType.Name}s", $"Updates existing entity of type '{outputType.Name}'.", new ListGraphType(outputType),
                        new QueryArguments(updateArgument), resolve: ResolveUpdate)
                    .AddMetadata(Statics.CkId, cacheItem.CkId);

                this.FieldAsync($"delete{outputType.Name}s", $"Deletes an entity of type '{outputType.Name}'.", new BooleanGraphType(),
                        new QueryArguments(deleteArgument), resolve: ResolveDelete)
                    .AddMetadata(Statics.CkId, cacheItem.CkId);
            }
        }

        private async Task<object> ResolveDelete(IResolveFieldContext<object> arg)
        {
            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;

            var ckId = arg.FieldDefinition.GetMetadata<string>(Statics.CkId);

            var inputObjects = arg.GetArgument<List<MutationDto<object>>>(Statics.EntitiesArg);

            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            try
            {
                var entityUpdateInfos = new List<EntityUpdateInfo>();
                foreach (var mutationDto in inputObjects)
                {
                    var document = new RtEntity
                    {
                        RtId = mutationDto.RtId.ToObjectId(),
                        CkId = ckId
                    };
                    
                    entityUpdateInfos.Add(new EntityUpdateInfo(document, EntityModOptions.Delete));
                }

                await graphQlUserContext.TenantContext.Repository.ApplyChanges(session, entityUpdateInfos);
                await session.CommitTransactionAsync();
                return true;
            }
            catch (OperationFailedException e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError(e.Message, e) {Code = CommonConstants.GraphQLErrorDataStore});
                return false;
            }
            catch (Exception e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError("A general error occurred", e) {Code = CommonConstants.GraphQLErrorCommon});
                return false;
            }
        }

        private async Task<object> ResolveUpdate(IResolveFieldContext<object> arg)
        {
            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;

            var ckId = arg.FieldDefinition.GetMetadata<string>(Statics.CkId);

            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var inputObjects = arg.GetArgument<List<MutationDto<RtEntityDto>>>(Statics.EntitiesArg);

            try
            {
                var entityUpdateInfos = new List<EntityUpdateInfo>();
                var associationUpdateInfoList = new List<AssociationUpdateInfo>();
                foreach (var mutationDto in inputObjects)
                {
                    var document = new RtEntity
                    {
                        RtId = mutationDto.RtId.ToObjectId(),
                        CkId = ckId
                    };
 
                    RtEntityFromInputObject(document, mutationDto.Item, associationUpdateInfoList);
                    entityUpdateInfos.Add(new EntityUpdateInfo(document, EntityModOptions.Update));
                }
                
                await graphQlUserContext.TenantContext.Repository.ApplyChanges(session, entityUpdateInfos, associationUpdateInfoList);

                await session.CommitTransactionAsync();

                return await GetResultSet(graphQlUserContext.TenantContext.Repository, ckId, entityUpdateInfos);
            }
            catch (CkModelViolationException e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError(e.Message, e) {Code = CommonConstants.GraphQLCkModelViolation});
                return null;
            }
            catch (OperationFailedException e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError(e.Message, e) {Code = CommonConstants.GraphQLErrorDataStore});
                return null;
            }
            catch (Exception e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError("A general error occurred", e) {Code = CommonConstants.GraphQLErrorCommon});
                return null;
            }
        }

        private async Task<object> ResolveCreate(IResolveFieldContext<object> arg)
        {
            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;
            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var ckId = arg.FieldDefinition.GetMetadata<string>(Statics.CkId);

            var inputObjects = arg.GetArgument<List<RtEntityDto>>(Statics.EntitiesArg);

            try
            {
                var entityUpdateInfos = new List<EntityUpdateInfo>();
                var associationUpdateInfoList = new List<AssociationUpdateInfo>();
                foreach (RtEntityDto rtEntityDto in inputObjects)
                {
                    var rtEntity = graphQlUserContext.TenantContext.Repository.CreateTransientRtEntity(ckId);
                    RtEntityFromInputObject(rtEntity, rtEntityDto, associationUpdateInfoList);
                    entityUpdateInfos.Add(new EntityUpdateInfo(rtEntity, EntityModOptions.Create));
                }

                var deleteAssociations = associationUpdateInfoList.Where(x => x.ModOption == AssociationModOptionsDto.Delete);
                if (deleteAssociations.Any())
                {
                    arg.Errors.Add(new ExecutionError("Delete operations during creation are supported") {Code = CommonConstants.GraphQLDeleteOperationsNotSupported});
                    return null;
                }
                
                await graphQlUserContext.TenantContext.Repository.ApplyChanges(session, entityUpdateInfos, associationUpdateInfoList);
                await session.CommitTransactionAsync();

                return await GetResultSet(graphQlUserContext.TenantContext.Repository, ckId, entityUpdateInfos);
            }
            catch (CkModelViolationException e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError(e.Message, e) {Code = CommonConstants.GraphQLCkModelViolation});
                return null;
            }
            catch (OperationFailedException e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError(e.Message, e) {Code = CommonConstants.GraphQLErrorDataStore});
                return null;
            }
            catch (Exception e)
            {
                await session.AbortTransactionAsync();

                arg.Errors.Add(new ExecutionError("A general error occurred", e) {Code = CommonConstants.GraphQLErrorCommon});
                return null;
            }
        }

        private async Task<IEnumerable<RtEntityDto>> GetResultSet(ITenantRepository repository, string ckId, List<EntityUpdateInfo> entityUpdateInfos)
        {
            using var session = await repository.StartSessionAsync();
            session.StartTransaction();

            var resultSet = await repository.GetRtEntitiesByIdAsync(session, ckId,
                entityUpdateInfos.Select(x => x.RtEntity.RtId).ToList(), new DataQueryOperation());
            await session.CommitTransactionAsync();

            return resultSet.Result.Select(RtEntityDtoType.CreateRtEntityDto);
        }
        
        private void RtEntityFromInputObject(RtEntity rtEntity, RtEntityDto rtEntityDto, List<AssociationUpdateInfo> associations)
        {
            var metaEntityCacheItem = _ckCache.GetEntityCacheItem(rtEntity.CkId);

            rtEntity.WellKnownName = rtEntityDto.WellKnownName;

            if (rtEntityDto.Properties != null)
            {
                foreach (var item in rtEntityDto.Properties)
                {
                    if (TryHandleAttribute(rtEntity, metaEntityCacheItem, item))
                        continue;
                    if (TryHandleInboundAssoc(rtEntity, metaEntityCacheItem, item, associations))
                        continue;
                    TryHandleOutboundAssoc(rtEntity, metaEntityCacheItem, item, associations);
                }
            }
        }

        private bool TryHandleAttribute(RtEntity rtEntity, EntityCacheItem entityCacheItem, KeyValuePair<string, object> item)
        {
            var attributeName = item.Key;

            var attributeCacheItem = entityCacheItem.Attributes.Values.FirstOrDefault(a => a.AttributeName == attributeName);
            if (attributeCacheItem == null)
            {
                return false;
            }
            
            rtEntity.SetAttributeValue(attributeCacheItem.AttributeName, attributeCacheItem.AttributeValueType, item.Value);
            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool TryHandleInboundAssoc(RtEntity rtEntity, EntityCacheItem entityCacheItem, KeyValuePair<string, object> item, List<AssociationUpdateInfo> associations)
        {
            var assocName = item.Key;

            var associationCacheItem = entityCacheItem.InboundAssociations.Values.SelectMany(x => x).FirstOrDefault(a => a.Name == assocName);
            if (associationCacheItem == null)
            {
                return false;
            }

            var rtAssociationInputDtos = (IEnumerable<object>) item.Value;
            foreach (RtAssociationInputDto rtAssociationDto in rtAssociationInputDtos)
            {
                var assocInfo = new AssociationUpdateInfo(
                    rtAssociationDto.Target,
                    rtEntity.ToRtEntityId(),
                    associationCacheItem.RoleId,
                    rtAssociationDto.ModOption ?? AssociationModOptionsDto.Create);
                associations.Add(assocInfo);
            }

            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool TryHandleOutboundAssoc(RtEntity rtEntity, EntityCacheItem entityCacheItem, KeyValuePair<string, object> item, List<AssociationUpdateInfo> associations)
        {
            var assocName = item.Key;

            var associationCacheItem = entityCacheItem.OutboundAssociations.Values.SelectMany(x => x).FirstOrDefault(a => a.Name == assocName);
            if (associationCacheItem == null)
            {
                return false;
            }

            var rtAssociationInputDtos = (IEnumerable<object>) item.Value;
            foreach (RtAssociationInputDto rtAssociationDto in rtAssociationInputDtos)
            {
                var assocInfo = new AssociationUpdateInfo(
                    rtEntity.ToRtEntityId(),
                    rtAssociationDto.Target,
                    associationCacheItem.RoleId,
                    rtAssociationDto.ModOption ?? AssociationModOptionsDto.Create);
                associations.Add(assocInfo);
            }

            return true;
        }
    }
}