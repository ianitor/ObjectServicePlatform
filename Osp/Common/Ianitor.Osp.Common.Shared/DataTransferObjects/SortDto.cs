using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class SortDto
    {
        public string AttributeName { get; set; }
        [JsonConverter(typeof(StringEnumConverter), typeof(ConstantCaseNamingStrategy))]
        public SortOrdersDto SortOrder { get; set; }
    }
}