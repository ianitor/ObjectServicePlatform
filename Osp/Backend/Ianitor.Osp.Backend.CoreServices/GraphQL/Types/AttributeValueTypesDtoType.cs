using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class AttributeValueTypesDtoType : EnumerationGraphType<AttributeValueTypesDto>
    {
        public AttributeValueTypesDtoType()
        {
            Name = "AttributeValueType";
            Description = "Enum of valid attribute types";
        }
    }
}