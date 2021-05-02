using System;
using Newtonsoft.Json;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class NotificationMessageInputDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubjectText { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BodyText { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RecipientAddress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NotificationTypesDto? NotificationType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SentDateTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SendStatusDto? SendStatus { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastTryDateTime { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorText { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RtAssociationInputDto[] RelatesTo{ get; set; }
    }
}