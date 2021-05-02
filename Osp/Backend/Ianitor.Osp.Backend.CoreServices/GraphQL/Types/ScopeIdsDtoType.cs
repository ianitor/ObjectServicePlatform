using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class ScopeIdsDtoType : EnumerationGraphType<ScopeIdsDto>
    {
        public ScopeIdsDtoType()
        {
            Name = "Scopes";
            Description = "The scope of the construction kit model";
        }
    }
}