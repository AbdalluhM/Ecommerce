namespace Ecommerce.DTO.Customers.Invoices.Outputs
{
    public class TicketInvoiceLookupDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Serial { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }
    }
}
