using GraphQL;
using GraphQL.Types;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// Implements an update item for RtEntities
    /// </summary>
    public class RtEntityUpdateItemDtoType: ObjectGraphType<RtEntityUpdateItemDto>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rtEntityDtoType">GraphQL type the corresponding RtEntity type</param>
        public RtEntityUpdateItemDtoType(RtEntityDtoType rtEntityDtoType)
        {
            Name = $"{rtEntityDtoType.Name}{CommonConstants.GraphQlUpdateSuffix}";
            this.Field("Item", "The corresponding item", graphType: rtEntityDtoType, resolve: ResolveItem);
            Field(o => o.UpdateState, type: typeof(UpdateTypesDtoType));
        }

        private object ResolveItem(IResolveFieldContext<RtEntityUpdateItemDto> arg)
        {
            var rtEntity = (RtEntity) arg.Source.UserContext;

            return RtEntityDtoType.CreateRtEntityDto(rtEntity);
        }
    }
}