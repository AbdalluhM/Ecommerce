using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ChatMessage:EntityBase
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime? ReadTime { get; set; }
        public int SenderId { get; set; }
        public int TicketId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? HasAttachment { get; set; }
        public bool IsCustomer { get; set; }
        public DateTime? ReceivedTime { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}
