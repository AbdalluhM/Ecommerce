using Ecommerce.Core.Entities;
using System.Collections.Generic;

namespace Ecommerce.DTO.Customers.Invoices
{
    public class CustomerSubInvoiceDto
    {
        public int SubId { get; set; }

        public Invoice Invoice { get; set; }
    }
    
    public class CustomerSubInvoiceTestDto
    {
        public int SubId { get; set; }

        public List<Invoice> Invoices { get; set; } = new();
    }
}
