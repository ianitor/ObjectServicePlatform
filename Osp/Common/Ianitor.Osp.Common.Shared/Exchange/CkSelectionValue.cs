using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public class CkSelectionValue
    {
        [JsonProperty("key")]
        public int Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}