using System.Text.Json.Serialization;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class ExportModelRequestDto
    {
        [JsonConverter(typeof(OspObjectIdConverter))]
        public OspObjectId QueryId { get; set; }
    }
}