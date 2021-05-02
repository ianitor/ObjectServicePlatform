using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ianitor.Osp.Common.Shared
{
    public class OspObjectIdArrayConverter: JsonConverter<OspObjectId[]>
    {
        public override OspObjectId[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new Exception(
                    $"Unexpected token parsing ObjectId. Expected start array, got {(object) reader.TokenType}.");
            }

            reader.Read();

            var list = new List<OspObjectId>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var str = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new Exception(
                    $"Unexpected token parsing ObjectId. Expected String, got {(object) reader.TokenType}.");
                list.Add(string.IsNullOrEmpty(str) ? OspObjectId.Empty : new OspObjectId(str));
                reader.Read();
            }

            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, OspObjectId[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var ospObjectId in value)
            {
                writer.WriteStringValue(ospObjectId != OspObjectId.Empty ? value.ToString() : string.Empty);

            }
            writer.WriteEndArray();
        } 
    }
}