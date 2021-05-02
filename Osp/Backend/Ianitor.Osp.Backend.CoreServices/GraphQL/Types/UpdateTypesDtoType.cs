using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class UpdateTypesDtoType : EnumerationGraphType<UpdateTypesDto>
    {
        public UpdateTypesDtoType()
        {
            Name = "UpdateType";
            Description = "Enum of valid update types";
        }
    }
}