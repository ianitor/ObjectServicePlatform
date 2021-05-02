using System;
using System.Text.Json;

namespace Ianitor.Osp.Common.Shared
{
    public class OspObjectIdConverter : System.Text.Json.Serialization.JsonConverter<OspObjectId>
    {
        public override OspObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string str = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new Exception(
                $"Unexpected token parsing ObjectId. Expected String, got {(object) reader.TokenType}.");
            return string.IsNullOrEmpty(str) ? OspObjectId.Empty : new OspObjectId(str);
        }

        public override void Write(Utf8JsonWriter writer, OspObjectId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value != OspObjectId.Empty ? value.ToString() : string.Empty);
        }
    }
}