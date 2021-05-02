using GraphQL.Language.AST;
using GraphQL.Types;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class SimpleScalarType : ScalarGraphType
    {
        public override object Serialize(object value)
        {
            return value;
        }

        public override object ParseValue(object value)
        {
            return value;
        }

        public override object ParseLiteral(IValue value)
        {
            return value.Value;
        }
    }
}