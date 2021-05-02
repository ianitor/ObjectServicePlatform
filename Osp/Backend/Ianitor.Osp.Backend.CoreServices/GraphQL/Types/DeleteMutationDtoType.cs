using GraphQL.Types;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class DeleteMutationDtoType : InputObjectGraphType<MutationDto<object>>
    {
        public DeleteMutationDtoType(IGraphType itemType)
        {
            Name = $"{CommonConstants.GraphQlDeletePrefix}{itemType.Name}";
            Field(x => x.RtId, type: typeof(OspObjectIdType));
        }
    }
}