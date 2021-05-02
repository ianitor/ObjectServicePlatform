using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class FieldFilterOperatorDtoType: EnumerationGraphType<FieldFilterOperatorDto>
    {
        public FieldFilterOperatorDtoType()
        {
            Name = "FieldFilterOperators";
            Description = "Defines the operator of field compare";
        }
    }
}