using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// Implements a generic runtime entities type that can be used for generic access to entities
    /// </summary>
    public class RtEntityGenericDtoType : ObjectGraphType<RtEntityDto>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RtEntityGenericDtoType()
        {
            Name = "RtEntity";
            Description = "A runtime entity type of OSP";
            Field(d => d.RtId, type: typeof(OspObjectIdType));
            Field(x => x.WellKnownName, true);

            Connection<RtEntityAttributeDtoType>()
                .Name("attributes")
                .Argument<ListGraphType<StringGraphType>>(Statics.AttributeNamesFilterArg, "Filter of attribute names")
                .Unidirectional()
                .Resolve(ResolveAttributes);
        }

        private object ResolveAttributes(IResolveConnectionContext<RtEntityDto> ctx)
        {
            var graphQlContext = (GraphQLUserContext) ctx.UserContext;

            var filterAttributeNames = ctx.GetArgument<IEnumerable<string>>(Statics.AttributeNamesFilterArg);

            var entityCacheItem = graphQlContext.TenantContext.CkCache.GetEntityCacheItem(ctx.Source.CkId);

            IEnumerable<AttributeCacheItem> resultList;
            if (filterAttributeNames == null)
            {
                resultList = entityCacheItem.Attributes.Values;
            }
            else
            {
                resultList =
                    entityCacheItem.Attributes.Values.Where(a =>
                        filterAttributeNames.Contains(a.AttributeName.ToCamelCase()));
            }

            return ConnectionUtils.ToConnection(resultList.Select(item => CreateRtEntityAttributeDto((RtEntity)ctx.Source.UserContext, item)),
                ctx);
        }

        private RtEntityAttributeDto CreateRtEntityAttributeDto(RtEntity rtEntity,
            AttributeCacheItem attributeCacheItem)
        {
            var attributeDto = new RtEntityAttributeDto
            {
                AttributeName = attributeCacheItem.AttributeName.ToCamelCase(),
                Value = rtEntity.GetAttributeValueOrDefault(attributeCacheItem.AttributeName),
                UserContext = attributeCacheItem
            };
            return attributeDto;
        }
    }
}