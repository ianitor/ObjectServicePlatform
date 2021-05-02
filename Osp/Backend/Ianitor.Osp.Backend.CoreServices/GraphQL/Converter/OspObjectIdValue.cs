using GraphQL.Language.AST;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Converter
{
    internal class OspObjectIdValue : ValueNode<OspObjectId>
    {
        public OspObjectIdValue(OspObjectId ospObjectId)
        {
            Value = ospObjectId;
        }
    }
}