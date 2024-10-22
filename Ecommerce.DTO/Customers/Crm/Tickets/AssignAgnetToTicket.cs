using System;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class AssignAgnetToTicket : TicketBaseDto
    {
        public Guid AgentId { get; set; }
    }
}
