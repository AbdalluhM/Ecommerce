using Ecommerce.Core.Enums.Invoices;
using System;

namespace Ecommerce.DTO.Pdfs.Invoices
{
    public class InvoiceInfoPdfDto
    {
        public string Serial { get; set; }

        public string CreateDate { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Status { get; set; }
    }
}
