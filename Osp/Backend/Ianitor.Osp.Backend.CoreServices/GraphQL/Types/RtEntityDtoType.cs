using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// Implements the GraphQL Runtime Entity Type
    /// </summary>
    public class RtEntityDtoType : ObjectGraphType<RtEntityDto>
    {
        /// <summary>
        /// Returns the Construction Kid Id of the object type
        /// </summary>
        public string CkId { get; }

        /// <inheritdoc />
        public RtEntityDtoType(string ckId)
        {
            CkId = ckId;

            Name = ckId.GetGraphQLName();
            Description = $"Runtime entities of construction kit type '{ckId}'";
            IsTypeOf = o =>
            {
                if (o is RtEntityDto rtEntityDto)
                {
                    return rtEntityDto.CkId == ckId;
                }

                return false;
            };

            Field(d => d.RtId, type: typeof(OspObjectIdType));
            Field(x => x.WellKnownName, true);
        }

        /// <summary>
        /// Populates the type with ck related attributes and associations
        /// </summary>
        /// <param name="entityDtoCache">The entity type cache</param>
        /// <param name="entityCacheItem">The cache item</param>
        public void Populate(IGraphTypesCache entityDtoCache, EntityCacheItem entityCacheItem)
        {
            AddConstructionKit(entityCacheItem);

            foreach (var attribute in entityCacheItem.Attributes.Values)
            {
                AddAttribute(attribute);
            }

            foreach (var cacheItems in entityCacheItem.OutboundAssociations)
            {
                var allowedTypes = cacheItems.Value.SelectMany(x => x.AllowedTypes).ToList();
                string name = cacheItems.Key;
                if (!allowedTypes.Any())
                {
                    continue; // All Ck entities are abstract for that associations
                }

                this.AssociationField(entityDtoCache, name, allowedTypes.Select(x => x.CkId).Distinct().ToList(), cacheItems.Value.First().RoleId, GraphDirections.Outbound);
            }

            foreach (var cacheItems in entityCacheItem.InboundAssociations)
            {
                var allowedTypes = cacheItems.Value.SelectMany(x => x.AllowedTypes).ToList();
                string name = cacheItems.Key;
                if (!allowedTypes.Any())
                {
                    continue; // All Ck entities are abstract for that associations
                }

                this.AssociationField(entityDtoCache, name, allowedTypes.Select(x => x.CkId).Distinct().ToList(), cacheItems.Value.First().RoleId, GraphDirections.Inbound);
            }
        }

        private void AddAttribute(AttributeCacheItem attributeCacheItem)
        {
            Expression<Func<RtEntityDto, object>> scalarValueExpression = dto => ((RtEntity) dto.UserContext).GetAttributeValueOrDefault(attributeCacheItem.AttributeName, null);

            Expression<Func<RtEntityDto, ICollection<object>>> compoundValueExpression = dto =>
                (ICollection<object>) ((RtEntity) dto.UserContext).GetAttributeValueOrDefault(attributeCacheItem.AttributeName, null);

            var attributeName = attributeCacheItem.AttributeName;
            switch (attributeCacheItem.AttributeValueType)
            {
                case AttributeValueTypes.String:
                    Field(attributeName, type: typeof(StringGraphType), expression: scalarValueExpression);
                    break;
                case AttributeValueTypes.StringArray:
                    Field(attributeName, type: typeof(ListGraphType<StringGraphType>),
                        expression: compoundValueExpression);
                    break;
                case AttributeValueTypes.Int:
                    Field(attributeName, type: typeof(IntGraphType), expression: scalarValueExpression);
                    break;
                case AttributeValueTypes.IntArray:
                    Field(attributeName, type: typeof(ListGraphType<IntGraphType>),
                        expression: compoundValueExpression);
                    break;
                case AttributeValueTypes.Boolean:
                    Field(attributeName, type: typeof(BooleanGraphType), expression: scalarValueExpression);
                    break;
                case AttributeValueTypes.Double:
                    Field(attributeName, type: typeof(DecimalGraphType), expression: scalarValueExpression);
                    break;
                case AttributeValueTypes.DateTime:
                    Field(attributeName, type: typeof(DateTimeGraphType), expression: scalarValueExpression);
                    break;
                default:
                    throw new NotImplementedException("Type is not supported for RT Entity GraphQL implementation");
            }
        }

        private void AddConstructionKit(EntityCacheItem entityCacheItem)
        {
            Field(typeof(CkEntityDtoType), "ConstructionKitType", resolve: ResolveCkEntity)
                .AddMetadata(Statics.EntityCacheItem, entityCacheItem);
        }

        private object ResolveCkEntity(IResolveFieldContext<RtEntityDto> arg)
        {
            var entityCacheItem = (EntityCacheItem) arg.FieldDefinition.Metadata[Statics.EntityCacheItem];
            return CkEntityDtoType.CreateCkEntityDto(entityCacheItem);
        }

        internal static RtEntityDto CreateRtEntityDto(RtEntity rtEntity)
        {
            var rtEntityDto = new RtEntityDto
            {
                RtId = rtEntity.RtId.ToOspObjectId(),
                CkId = rtEntity.CkId,
                WellKnownName = rtEntity.WellKnownName,
                UserContext = rtEntity
            };
            return rtEntityDto;
        }
    }
}