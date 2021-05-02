using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.Types;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class RtEntityAssociationType : ObjectGraphType
    {
        public RtEntityAssociationType(string name, string description, IGraphTypesCache entityDtoCache,
            IEnumerable<RtEntityDtoType> rtEntityDtoTypes, string roleId, GraphDirections graphDirection)
        {
            ArgumentValidation.ValidateString(nameof(name), name);
            ArgumentValidation.ValidateString(nameof(description), description);

            Name = name;
            Description = description;

            foreach (var rtEntityDtoType in rtEntityDtoTypes)
            {
                this.Connection<object, IGraphType, RtEntityDto>(entityDtoCache, rtEntityDtoType, rtEntityDtoType.Name)
                    .AddMetadata(Statics.CkId, rtEntityDtoType.CkId)
                    .AddMetadata(Statics.RoleId, roleId)
                    .AddMetadata(Statics.GraphDirection, graphDirection)
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

        private async Task<object> ResolveRtEntitiesQuery(IResolveConnectionContext<RtEntityDto> ctx)
        {
            var graphQlContext = (GraphQLUserContext) ctx.UserContext;
            using var session = await graphQlContext.TenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var ckId = (string) ctx.FieldDefinition.Metadata[Statics.CkId];
            var roleId = (string) ctx.FieldDefinition.Metadata[Statics.RoleId];
            var graphDirections = (GraphDirections) ctx.FieldDefinition.Metadata[Statics.GraphDirection];

            int? offset = ctx.GetOffset();
            DataQueryOperation dataQueryOperation = ctx.GetDataQueryOperation();

            if (ctx.Source.RtId != null)
            {
                var resultSet = await graphQlContext.TenantContext.Repository.GetRtAssociationTargetsAsync(session,
                    ctx.Source.RtId.Value.ToObjectId(), roleId, ckId,
                    graphDirections, dataQueryOperation, offset, ctx.First);

                return ConnectionUtils.ToConnection(resultSet.Result.Select(RtEntityDtoType.CreateRtEntityDto), ctx,
                    resultSet.TotalCount > 0 ? offset.GetValueOrDefault(0) : 0, (int) resultSet.TotalCount);
            }

            await session.CommitTransactionAsync();
            
            return ConnectionUtils.ToConnection(new RtEntityDto[] { }, ctx, 0, 0);
        }
    }
}