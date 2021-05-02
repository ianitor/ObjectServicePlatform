using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public class RtAssociation
  {
        [JsonProperty("roleId")]
        public string RoleId { get; set; }

        [JsonProperty("targetRtId")]
        [JsonConverter(typeof(NewtonOspObjectIdConverter))]
        public OspObjectId TargetRtId { get; set; }
        
        [JsonProperty("targetCkId")]
        public string TargetCkId { get; set; }
    }
}
