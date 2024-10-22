namespace Ecommerce.DTO.Pdfs.Invoices
{
    public class InvoiceItemPdfDto
    {
        public string ProductName { get; set; }

        public int PriceLevel { get; set; }

        public string SubscriptionType { get; set; }

        public decimal Discount { get; set; }

        public decimal NetPrice { get; set; }

        public string CurrencyName { get; set; }
    }
}
