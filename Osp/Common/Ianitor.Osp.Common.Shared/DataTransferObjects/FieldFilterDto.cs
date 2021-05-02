using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class FieldFilterDto
    {
        public string AttributeName { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), typeof(ConstantCaseNamingStrategy))]
        public FieldFilterOperatorDto Operator { get; set; }
        public object ComparisonValue { get; set; }
    }
}