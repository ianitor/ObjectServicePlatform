using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Converter;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// GraphQL type for Object Ids
    /// </summary>
    public class OspObjectIdType : ScalarGraphType
    {
        /// <inheritdoc />
        public override object ParseLiteral(IValue value)
        {
            if (value is OspObjectIdValue ospObjectIdValue)
            {
                return ParseValue(ospObjectIdValue.Value);
            }

            return value is StringValue stringValue ? new OspObjectId(stringValue.Value) : new OspObjectId?();
        }

        /// <inheritdoc />
        public override object ParseValue(object value)
        {
            return ValueConverter.ConvertTo(value, typeof(OspObjectId));
        }

        /// <inheritdoc />
        public override object Serialize(object value)
        {
            var ospObjectId = (OspObjectId) value;
            return ospObjectId.ToString();
        }
    }
}