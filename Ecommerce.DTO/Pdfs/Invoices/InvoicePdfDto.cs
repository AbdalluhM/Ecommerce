using System.Collections.Generic;

namespace Ecommerce.DTO.Pdfs.Invoices
{
    public class InvoicePdfDto
    {
        public InvoiceInfoPdfDto InvoiceInfo { get; set; } = new();

        public CustomerInfoPdfDto CustomerInfo { get; set; } = new();

        public IEnumerable<InvoiceItemPdfDto> InvoiceItems { get; set; } = new List<InvoiceItemPdfDto>();

        public InvoiceSummaryPdfDto InvoiceSummary { get; set; } = new();

    }
    public class GetPathesOutputDto
    {
        public string BasePath { get; set; }
        public string TempPath { get; set; }
        public string AssetPath { get; set; }
    }


}
