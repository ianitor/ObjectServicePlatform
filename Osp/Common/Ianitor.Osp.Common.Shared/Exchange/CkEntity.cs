﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.Exchange
{
    public class CkEntity
    {
        public CkEntity()
        {
            Attributes = new List<CkEntityAttribute>();
            Associations = new List<CkEntityAssociation>();
            Indexes = new List<CkEntityIndexDto>();
        }

        [JsonProperty("ckId")]
        public string CkId { get; set; }

        [JsonProperty("ckDerivedId")]
        public string CkDerivedId { get; set; }

        [JsonProperty("isFinal")]
        public bool IsFinal { get; set; }

        [JsonProperty("isAbstract")]
        public bool IsAbstract { get; set; }


        [JsonProperty("attributes")]
        public List<CkEntityAttribute> Attributes { get; }
        
        [JsonProperty("indexes")]
        public List<CkEntityIndexDto> Indexes { get; }

        [JsonProperty("associations")]
        public List<CkEntityAssociation> Associations { get; }
    }
}
