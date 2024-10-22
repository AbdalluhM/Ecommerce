using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class TicketDto : RetrieveTicketBaseDto
    {
        public string Serial { get; set; }

        public string ProductNumber { get; set; }

        public string Product { get; set; }

        public int TopicId { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("priorityId")]
        public int PriorityId { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("attachments")]
        public List<MessageAttachmentDto> Attachments { get; set; } = new List<MessageAttachmentDto>();

        [JsonProperty("owners")]
        public List<OwnerDto> Owners { get; set; } = new List<OwnerDto>();
    }
}
