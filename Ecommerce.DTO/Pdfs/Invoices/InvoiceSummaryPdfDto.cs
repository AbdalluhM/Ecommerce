namespace Ecommerce.DTO.Pdfs.Invoices
{
    public class InvoiceSummaryPdfDto
    {
        public decimal Subtotal { get; set; }

        public decimal VatPercentage { get; set; }

        public decimal VatValue { get; set; }

        public decimal Total { get; set; }

        public string CurrencyName { get; set; }
    }
}
