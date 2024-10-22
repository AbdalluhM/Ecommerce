using System;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class TicketFilterDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int StatusId { get; set; }

        public string Search { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
