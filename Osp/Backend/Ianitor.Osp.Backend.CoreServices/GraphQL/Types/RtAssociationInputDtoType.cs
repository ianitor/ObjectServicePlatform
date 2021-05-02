using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class RtAssociationInputDtoType : InputObjectGraphType<RtAssociationInputDto>
    {
        public RtAssociationInputDtoType()
        {
            Name = "RtAssociationInput";
            Description = "Input field for associations";
            
            Field(x => x.Target, type: typeof(NonNullGraphType<RtEntityIdType>)).Description("Runtime ID of the target entity");
            Field(x => x.ModOption, type: typeof(AssociationModOptionsDtoType)).Description("Type of modification.");

        }
        
    }
}