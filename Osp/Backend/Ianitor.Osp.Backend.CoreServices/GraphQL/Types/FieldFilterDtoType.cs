using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class FieldFilterDtoType: InputObjectGraphType<FieldFilterDto>
    {
        public FieldFilterDtoType()
        {
            Name = "FieldFilter";
            Field(x => x.AttributeName);
            Field(x => x.Operator, type: typeof(FieldFilterOperatorDtoType));
            Field(x => x.ComparisonValue, type: typeof(SimpleScalarType));
        }
    }
}