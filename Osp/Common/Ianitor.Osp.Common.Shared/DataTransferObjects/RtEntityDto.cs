using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class RtEntityDto : GraphQLDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(NewtonOspObjectIdConverter))]
        public OspObjectId? RtId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CkId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WellKnownName { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, object> Properties{ get; set; }
    }
}