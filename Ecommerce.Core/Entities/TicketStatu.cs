using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TicketStatu:EntityBase
    {
        public TicketStatu()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
