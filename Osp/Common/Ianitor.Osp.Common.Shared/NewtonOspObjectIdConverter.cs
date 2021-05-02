using System;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared
{
    public class NewtonOspObjectIdConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        { 
            if (value is OspObjectId)
            {
                var objectId = (OspObjectId)value;
                writer.WriteValue(objectId != OspObjectId.Empty ? objectId.ToString() : string.Empty);
            }
            else
            {
                throw new Exception("Expected ObjectId value.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new Exception($"Unexpected token parsing ObjectId. Expected String, got {reader.TokenType}.");
            }

            var value = (string)reader.Value;
            return string.IsNullOrEmpty(value) ? OspObjectId.Empty : new OspObjectId(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(OspObjectId).IsAssignableFrom(objectType);
        }


    }
}