using GraphQL.Types;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class UpdateMutationDtoType<TItemType> : InputObjectGraphType<MutationDto<TItemType>>
    {
        public UpdateMutationDtoType(IGraphType itemType)
        {
            Name = $"{CommonConstants.GraphQlUpdatePrefix}{itemType.Name}";
            Field(x => x.RtId, type: typeof(OspObjectIdType));
            this.Field("item",
                "Item to update",
                new NonNullGraphType(itemType));
        }
    }
}