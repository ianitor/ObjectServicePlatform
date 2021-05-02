using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class RtAssociationInputDto
    {
        public RtEntityId Target { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), typeof(ConstantCaseNamingStrategy))]
        public AssociationModOptionsDto? ModOption { get; set; }
    }
}