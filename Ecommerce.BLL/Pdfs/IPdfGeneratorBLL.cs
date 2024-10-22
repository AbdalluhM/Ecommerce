using DevExpress.XtraReports.UI;
using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Pdfs;
using Ecommerce.Reports.Templts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Pdfs
{
    public interface IPdfGeneratorBLL
    {
        Task<IResponse<string>> GenerateInvoicePdfAsync(InvoicePdfOutputDto invoiceDto ,bool savePdf = true, string lang = DXConstants.SupportedLanguage.EN);

        //IResponse<string> GenerateInvoicePdf(IPdfSetting pdfSetting, InvoicePdfDto invoiceDto);

        /// <summary>
        /// Generate encrypted QR.
        /// </summary>
        /// <param name="invoicePdfDto"></param>
        /// <returns></returns>
        string GenerateInvoiceQrCode(InvoicePdfDto invoicePdfDto);

        string GenrateQrImagePath(InvoicePdfDto invoicePdfDto);

        /// <summary>
        /// Generate Public QR.
        /// </summary>
        /// <param name="invoicePdfDto"></param>
        /// <returns></returns>
        string GenerateInoviceQRBase64(InvoicePdfDto invoicePdfDto);


        IResponse<byte[]> InvoiceByteArr(string lang ,InvoicePdfOutputDto invoiceDto);
       // IResponse<XtraReport> GetInvoiceXtraReport(int id);
    }
}
