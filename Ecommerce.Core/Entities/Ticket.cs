using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Ticket:EntityBase
    {
        public Ticket()
        {
            ChatMessages = new HashSet<ChatMessage>();
        }

        public int Id { get; set; }
        public int Topic { get; set; }
        public string ProductNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Program { get; set; }
        public int? CustomerId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual TicketStatu Status { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
