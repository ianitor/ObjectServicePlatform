using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public class RtModelRoot
    {
        public RtModelRoot()
        {
            RtEntities = new List<RtEntity>();
        }

        [JsonProperty("entities")]
        public List<RtEntity> RtEntities { get; }
    }
}
