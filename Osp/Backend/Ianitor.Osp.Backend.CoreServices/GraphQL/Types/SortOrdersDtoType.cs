using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class SortOrdersDtoType: EnumerationGraphType<SortOrdersDto>
    {
        public SortOrdersDtoType()
        {
            Name = "SortOrders";
            Description = "Defines the sort order";
        }
    }
}