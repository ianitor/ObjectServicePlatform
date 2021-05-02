using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class GraphQLDto 
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object UserContext { get; set; }
    }
}