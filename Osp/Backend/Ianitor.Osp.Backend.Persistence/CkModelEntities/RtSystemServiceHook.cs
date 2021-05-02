using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Newtonsoft.Json;

namespace Ianitor.Osp.Backend.Persistence.CkModelEntities
{
    [CkId(Constants.SystemServiceHookCkId)]

    public class RtSystemServiceHook: RtEntity
    {
        [JsonIgnore]
        public bool? Enabled
        {
            get => GetAttributeValueOrDefault<bool>(nameof(Enabled));
            set => SetAttributeValue(nameof(Enabled), AttributeValueTypes.Boolean, value);
        }
        
        [JsonIgnore]
        public string QueryCkId
        {
            get => GetAttributeStringValueOrDefault(nameof(QueryCkId));
            set => SetAttributeValue(nameof(QueryCkId), AttributeValueTypes.String, value);
        }    
        
        [JsonIgnore]
        public string FieldFilter
        {
            get => GetAttributeStringValueOrDefault(nameof(FieldFilter));
            set => SetAttributeValue(nameof(FieldFilter), AttributeValueTypes.String, value);
        } 
        
        [JsonIgnore]
        public string Name
        {
            get => GetAttributeStringValueOrDefault(nameof(Name));
            set => SetAttributeValue(nameof(Name), AttributeValueTypes.String, value);
        } 
        
        [JsonIgnore]
        public string ServiceHookBaseUri
        {
            get => GetAttributeStringValueOrDefault(nameof(ServiceHookBaseUri));
            set => SetAttributeValue(nameof(ServiceHookBaseUri), AttributeValueTypes.String, value);
        } 
        
        [JsonIgnore]
        public string ServiceHookAction
        {
            get => GetAttributeStringValueOrDefault(nameof(ServiceHookAction));
            set => SetAttributeValue(nameof(ServiceHookAction), AttributeValueTypes.String, value);
        } 
    }
}