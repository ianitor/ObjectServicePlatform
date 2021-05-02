using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public class RtAttribute
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
