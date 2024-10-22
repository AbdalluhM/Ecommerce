using Ecommerce.Core.Entities;

namespace Ecommerce.DTO.Customers.Invoices
{
    public class InvoiceEmailDto
    {
        public string Serial { get; set; }

        public string Status { get; set; }

        public decimal TotalPrice { get; set; }

        public string Email { get; set; }

        public bool HasAttachment { get; set; } = false;

        public Invoice Invoice { get; set; }
    }
}
