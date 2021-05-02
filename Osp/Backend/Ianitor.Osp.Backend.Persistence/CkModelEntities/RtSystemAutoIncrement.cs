using System;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Newtonsoft.Json;

namespace Ianitor.Osp.Backend.Persistence.CkModelEntities
{
    [CkId(Constants.SystemAutoIncrementCkId)]

    public class RtSystemAutoIncrement: RtEntity
    {
        [JsonIgnore]
        public long? Start
        {
            get => GetAttributeValueOrDefault<long>(nameof(Start));
            set => SetAttributeValue(nameof(Start), AttributeValueTypes.Int, value);
        }
        
        [JsonIgnore]
        public long? End
        {
            get => GetAttributeValueOrDefault<long>(nameof(End), Int64.MaxValue);
            set => SetAttributeValue(nameof(End), AttributeValueTypes.Int, value);
        }    
        
        [JsonIgnore]
        public long? CurrentValue
        {
            get => GetAttributeValueOrDefault(nameof(CurrentValue), Start);
            set => SetAttributeValue(nameof(CurrentValue), AttributeValueTypes.Int, value);
        } 
        
    }
}