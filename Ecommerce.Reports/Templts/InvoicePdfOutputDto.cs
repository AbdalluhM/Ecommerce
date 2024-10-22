using DevExpress.DataAccess.ObjectBinding;
using Ecommerce.DTO.Pdfs.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace Ecommerce.Reports.Templts
{
    [HighlightedClass]
    public class InvoicePdfOutputDto : InvoicePdfDto
    {
        //public InvoiceInfoPdfDto InvoiceInfo { get; set; } //= new();

        //public CustomerInfoPdfDto CustomerInfo { get; set; } //= new();

        //public IEnumerable<InvoiceItemPdfDto> InvoiceItems { get; set; } = new List<InvoiceItemPdfDto>();

        //public InvoiceSummaryPdfDto InvoiceSummary { get; set; } //= new();

        [JsonIgnore]
        public int StatusId { get; set; }
        //[JsonIgnore]
        //public Uri LogoPath { get; set; }
        [JsonIgnore]
        public string LogoPath { get; set; }
        [JsonIgnore]
        public string QrImagePath { get; set; }
        [JsonIgnore]
        public byte [] QrCode { get; set; }

    }
}
