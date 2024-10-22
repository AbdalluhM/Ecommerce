using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TicketType:EntityBase
    {
        public TicketType()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
