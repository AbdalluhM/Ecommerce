using Newtonsoft.Json;
using System;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class RetrieveTicketBaseDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("StatusId")]
        public int StatusId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("totalMessages")]
        public int TotalMessages { get; set; }

        [JsonProperty("totalAttachments")]
        public int TotalAttachments { get; set; }

        [JsonProperty("agent")]
        public TicketAgnetDto Agent { get; set; } = new TicketAgnetDto();
    }
}
