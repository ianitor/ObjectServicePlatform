using GraphQL.Types;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// GraphQL type representing the RtEntityId type (struct with CkId and rtId)
    /// </summary>
    public class RtEntityIdType : InputObjectGraphType<RtEntityId>
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        public RtEntityIdType()
        {
            Name = "RtEntityId";
            Description = "Id information consists of CkId and RtId";

            Field(x => x.RtId, type: typeof(NonNullGraphType<OspObjectIdType>)).Description("Unique id of the object.");
            Field(x => x.CkId, type: typeof(NonNullGraphType<StringGraphType>)).Description("Construction kit id of the object.");
        }
    }
}