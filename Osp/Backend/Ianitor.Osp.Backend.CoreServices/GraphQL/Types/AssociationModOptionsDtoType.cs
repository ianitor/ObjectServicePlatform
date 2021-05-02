using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class AssociationModOptionsDtoType: EnumerationGraphType<AssociationModOptionsDto>
    {
        public AssociationModOptionsDtoType()
        {
            Name = "AssociationModOptions";
            Description = "Defines the type of modification during write operations";
        }
    }
}