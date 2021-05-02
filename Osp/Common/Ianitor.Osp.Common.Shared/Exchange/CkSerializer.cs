using System.IO;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public static class CkSerializer
    {
        public static void Serialize(StreamWriter streamWriter, CkModelRoot model)
        {
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };

            using (JsonWriter writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, model);
            }
        }
        
        public static CkModelRoot Deserialize(string s)
        {
            return Deserialize(new StringReader(s));
        }

        public static CkModelRoot Deserialize(TextReader textReader)
        {
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };

            using (JsonReader reader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<CkModelRoot>(reader);
            }
        }
    }
}
