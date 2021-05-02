using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
  public class RtEntity
  {
    public RtEntity()
    {
      Attributes = new List<RtAttribute>();
      Associations = new List<RtAssociation>();
    }

    [JsonProperty("id")]
    [JsonConverter(typeof(NewtonOspObjectIdConverter))]
    public OspObjectId Id { get; set; }

    [JsonProperty("ckId")]
    public string CkId { get; set; }

    [JsonProperty("wellKnownName")]
    public string WellKnownName { get; set; }

    [JsonProperty("attributes")]
    public List<RtAttribute> Attributes { get; }

    [JsonProperty("associations")]
    public List<RtAssociation> Associations { get; }
  }
}
