using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public static class RtSerializer
    {
        public static void Serialize(StreamWriter streamWriter, RtModelRoot model)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (JsonWriter writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, model);
            }
        }

        public static RtModelRoot Deserialize(string s)
        {
            return Deserialize(new StringReader(s));
        }

        public static RtModelRoot Deserialize(TextReader textReader)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (JsonReader reader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<RtModelRoot>(reader);
            }
        }

        public static async Task DeserializeAsync(TextReader textReader, Func<RtEntity, Task> entityDeserializedAction, CancellationToken? cancellationToken = null)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
            };

            var startQueue = new Queue<Tuple<JsonToken, object>>();
            startQueue.Enqueue(new Tuple<JsonToken, object>(JsonToken.StartObject, null));
            startQueue.Enqueue(new Tuple<JsonToken, object>(JsonToken.PropertyName, "entities"));
            startQueue.Enqueue(new Tuple<JsonToken, object>(JsonToken.StartArray, null));

            JsonTextReader reader = new JsonTextReader(textReader);
            while (reader.Read())
            {
                reader.SupportMultipleContent = true;

                if (startQueue.Count > 0)
                {
                    var data = startQueue.Dequeue();
                    if (reader.TokenType == data.Item1 && Equals(reader.Value, data.Item2))
                    {
                        continue;
                    }

                    throw new ModelSerializerException("Missing structure of JSON file format. Ensure that file begins with { \"entities\" : [ {");
                }

                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    return;
                }

                if (reader.TokenType == JsonToken.StartObject)
                {
                    RtEntity c = serializer.Deserialize<RtEntity>(reader);
                    await entityDeserializedAction(c);
                }
            }
        }
    }
}