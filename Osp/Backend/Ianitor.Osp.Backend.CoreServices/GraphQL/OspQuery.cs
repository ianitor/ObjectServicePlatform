using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using MongoDB.Bson;
using NLog;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    /// <summary>
    /// Implements an OSP query, based on a given data source
    /// </summary>
    public class OspQuery : ObjectGraphType
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor
        /// </summary >
        /// <param name="entityDtoCache">The cache of a RtEntityTypes that is build </param>
        /// <param name="rtEntityDtoTypes">List of RT entities accessible as query</param>
        public OspQuery(IGraphTypesCache entityDtoCache, IEnumerable<RtEntityDtoType> rtEntityDtoTypes)
        {
            Name = "OspQuery";

            Connection<CkEntityDtoType>()
                .Argument<StringGraphType>(Statics.CkIdArg, "Returns the construction kit type with the given id.")
                .Argument<ListGraphType<StringGraphType>>(Statics.CkIdsArg, "Returns the construction kit types with the given ids.")
                .Argument<SearchFilterDtoType>(Statics.SearchFilterArg, "Filters items based on text search")
                .Argument<ListGraphType<SortDtoType>>(Statics.SortOrderArg, "Sort order for items")
                .Argument<ListGraphType<FieldFilterDtoType>>(Statics.FieldFilterArg,  "Filters items based on field compare")                
                .Name("ConstructionKitTypes")
                .Resolve(ResolveCkEntitiesQuery);
            
            Connection<CkAttributeDtoType>()
                .Argument<StringGraphType>(Statics.AttributeIdArg, "Returns the entity with the given attribute id.")
                .Argument<ListGraphType<StringGraphType>>(Statics.AttributeIdsArg,
                    "Returns entities with the given attribute ids.")
                .Argument<SearchFilterDtoType>(Statics.SearchFilterArg, "Filters items based on text search")
                .Argument<ListGraphType<SortDtoType>>(Statics.SortOrderArg, "Sort order for items")
                .Argument<ListGraphType<FieldFilterDtoType>>(Statics.FieldFilterArg,  "Filters items based on field compare")
                .Name("ConstructionKitAttributes")
                .Resolve(ResolveCkAttributesQuery);
            
            Connection<RtEntityGenericDtoType>()
                .Argument<StringGraphType>(Statics.CkIdArg, "The construction kit type with the given id.")
                .Argument<OspObjectIdType>(Statics.RtIdArg, "Returns the entity with the given rtId.")
                .Argument<ListGraphType<OspObjectIdType>>(Statics.RtIdsArg,
                    "Returns entities with the given rtIds.")
                .Argument<SearchFilterDtoType>(Statics.SearchFilterArg, "Filters items based on text search")
                .Argument<ListGraphType<SortDtoType>>(Statics.SortOrderArg, "Sort order for items")
                .Argument<ListGraphType<FieldFilterDtoType>>(Statics.FieldFilterArg,
                    "Filters items based on field compare")
                .Name("RuntimeEntities")
                .ResolveAsync(ResolveGenericRtEntitiesQuery);
            
            foreach (var rtEntityDtoType in rtEntityDtoTypes)
            {
                this.Connection<object, IGraphType, RtEntityDto>(entityDtoCache, rtEntityDtoType, rtEntityDtoType.Name)
                    .AddMetadata(Statics.CkId, rtEntityDtoType.CkId)
                    .Argument<OspObjectIdType>(Statics.RtIdArg, "Returns the entity with the given rtId.")
                    .Argument<ListGraphType<OspObjectIdType>>(Statics.RtIdsArg,
                        "Returns entities with the given rtIds.")
                    .Argument<SearchFilterDtoType>(Statics.SearchFilterArg, "Filters items based on text search")
                    .Argument<ListGraphType<SortDtoType>>(Statics.SortOrderArg, "Sort order for items")
                    .Argument<ListGraphType<FieldFilterDtoType>>(Statics.FieldFilterArg,
                        "Filters items based on field compare")
                    .ResolveAsync(ResolveRtEntitiesQuery);
            }
        }
        
        private async Task<object> ResolveCkAttributesQuery(IResolveConnectionContext<object> arg)
        {
            Logger.Debug("GraphQL query handling of contruction kit attributes started");

            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;

            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            int? offset = arg.GetOffset();
            DataQueryOperation dataQueryOperation = arg.GetDataQueryOperation();

            arg.TryGetArgument(Statics.AttributeIdArg, out string key);
            arg.TryGetArgument(Statics.AttributeIdsArg, new List<string>(), out IEnumerable<string> keys);

            var keysList = keys.ToList();
            if (!keysList.Any() && key != null)
            {
                keysList.Add(key);
            }

            var resultSet =
                await graphQlUserContext.TenantContext.Repository.GetCkAttributesAsync(session, keysList, dataQueryOperation, offset, arg.First);

            await session.CommitTransactionAsync();
            
            Logger.Debug("GraphQL query handling returning data for contruction kit attributes");
            return ConnectionUtils.ToConnection(resultSet.Result.Select(CkAttributeDtoType.CreateCkAttributeDto), arg,
                resultSet.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSet.TotalCount);
        }

        private async Task<object> ResolveCkEntitiesQuery(IResolveConnectionContext<object> arg)
        {
            Logger.Debug("GraphQL query handling of contruction kit attributes started");

            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;
            
            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();

            int? offset = arg.GetOffset();
            DataQueryOperation dataQueryOperation = arg.GetDataQueryOperation();
            
            arg.TryGetArgument(Statics.CkIdArg, out string key);
            arg.TryGetArgument(Statics.CkIdsArg, new List<string>(), out IEnumerable<string> keys);

            var keysList = keys.ToList();
            if (!keysList.Any() && key != null)
            {
                keysList.Add(key);
            }

            var resultSet =
                await graphQlUserContext.TenantContext.Repository.GetCkEntityAsync(session, keysList, dataQueryOperation, offset, arg.First);

            await session.CommitTransactionAsync();
            
            Logger.Debug("GraphQL query handling returning data for contruction kit attributes");
            return ConnectionUtils.ToConnection(resultSet.Result.Select(CkEntityDtoType.CreateCkEntityDto), arg,
                resultSet.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSet.TotalCount);
        }

        private async Task<object> ResolveGenericRtEntitiesQuery(IResolveConnectionContext<object> arg)
        {
            Logger.Debug("GraphQL query handling for generic runtime entity started");
            
            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;
            var ckId = arg.GetArgument<string>(Statics.CkIdArg);
            
            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            int? offset = arg.GetOffset();
            DataQueryOperation dataQueryOperation = arg.GetDataQueryOperation();

            arg.TryGetArgument(Statics.RtIdArg, out OspObjectId? key);
            arg.TryGetArgument(Statics.RtIdsArg, new List<ObjectId>(), out IEnumerable<ObjectId> keys);

            var keysList = keys.ToList();
            
            if (keysList.Any())
            {
                var resultSetIds =
                    await graphQlUserContext.TenantContext.Repository.GetRtEntitiesByIdAsync(session, ckId, keysList, dataQueryOperation,
                        offset, arg.First);

                Logger.Debug("GraphQL query handling returning data by keys");
                return ConnectionUtils.ToConnection(resultSetIds.Result.Select(RtEntityDtoType.CreateRtEntityDto), arg,
                    resultSetIds.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSetIds.TotalCount);
            }
            
            if (key.HasValue)
            {
                var result = await graphQlUserContext.TenantContext.Repository.GetRtEntityAsync(session, new RtEntityId(ckId, key.Value));

                var resultList = new List<RtEntityDto>();
                if (result != null)
                {
                    resultList.Add(RtEntityDtoType.CreateRtEntityDto(result));
                }

                Logger.Debug("GraphQL query handling returning data by key");
                return ConnectionUtils.ToConnection(resultList, arg);
            }

            var resultSet =
                await graphQlUserContext.TenantContext.Repository.GetRtEntitiesByTypeAsync(session, ckId, dataQueryOperation, offset,
                    arg.First);

            await session.CommitTransactionAsync();
            
            Logger.Debug("GraphQL query handling returning data");
            return ConnectionUtils.ToConnection(resultSet.Result.Select(RtEntityDtoType.CreateRtEntityDto), arg,
                resultSet.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSet.TotalCount);
        }
        
        private async Task<object> ResolveRtEntitiesQuery(IResolveConnectionContext<RtEntityDto> arg)
        {
            Logger.Debug("GraphQL query handling for specific runtime entity type started");

            var graphQlUserContext = (GraphQLUserContext) arg.UserContext;
            var ckId = (string) arg.FieldDefinition.Metadata[Statics.CkId];

            using var session = await graphQlUserContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();

            int? offset = arg.GetOffset();
            DataQueryOperation dataQueryOperation = arg.GetDataQueryOperation();

            arg.TryGetArgument(Statics.RtIdArg, out OspObjectId? key);
            arg.TryGetArgument(Statics.RtIdsArg, new List<OspObjectId>(), out IEnumerable<OspObjectId> keys);
            var keysList = keys.Select(x=> x.ToObjectId()).ToList();
            if (keysList.Any())
            {
                var resultSetIds =
                    await graphQlUserContext.TenantContext.Repository.GetRtEntitiesByIdAsync(session, ckId, keysList, dataQueryOperation,
                        offset, arg.First);

                Logger.Debug("GraphQL query handling returning data by keys");
                return ConnectionUtils.ToConnection(resultSetIds.Result.Select(RtEntityDtoType.CreateRtEntityDto), arg,
                    resultSetIds.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSetIds.TotalCount);
            }

            if (key.HasValue)
            {
                var result = await graphQlUserContext.TenantContext.Repository.GetRtEntityAsync(session, new RtEntityId(ckId, key.Value));

                var resultList = new List<RtEntityDto>();
                if (result != null)
                {
                    resultList.Add(RtEntityDtoType.CreateRtEntityDto(result));
                }

                Logger.Debug("GraphQL query handling returning data by key");
                return ConnectionUtils.ToConnection(resultList, arg);
            }

            var resultSet =
                await graphQlUserContext.TenantContext.Repository.GetRtEntitiesByTypeAsync(session, ckId, dataQueryOperation, offset,
                    arg.First);

            await session.CommitTransactionAsync();
            
            Logger.Debug("GraphQL query handling returning data");
            return ConnectionUtils.ToConnection(resultSet.Result.Select(RtEntityDtoType.CreateRtEntityDto), arg,
                resultSet.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSet.TotalCount);
        }
    }
}