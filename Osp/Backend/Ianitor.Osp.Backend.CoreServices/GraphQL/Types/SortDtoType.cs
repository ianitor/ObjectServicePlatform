using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class SortDtoType: InputObjectGraphType<SortDto>
    {
        public SortDtoType()
        {
            Name = "Sort";
            Field(x => x.SortOrder, type: typeof(SortOrdersDtoType));
            Field(x => x.AttributeName);
        }
    }
}