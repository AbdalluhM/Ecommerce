using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Pdfs.Invoices;
using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Compression;
using Microsoft.Extensions.Options;
using System.Linq;
using Dexef.QRGenerator;
using System.Drawing.Imaging;

namespace Ecommerce.BLL.Pdfs
{
    public class IronPdfGeneratorBLL : BaseBLL//, IPdfGeneratorBLL
    {
        private readonly IMapper _mapper;
        private readonly CompanyInfoPdf _companyInfoPdf;
        private const string Base64MimeType = "data:application/octet-stream;base64,";

        public IronPdfGeneratorBLL(IMapper mapper, IOptions<CompanyInfoPdf> companyInfoPdfOptions)
            : base(mapper)
        {
            _mapper = mapper;
            _companyInfoPdf = companyInfoPdfOptions.Value;
        }

        //    public async Task<IResponse<DownloadInvoiceFileDto>> GenerateInvoiceFileAsync(IEnumerable<InvoicePdfDto> invoicesDto)
        //    {
        //        var response = new Response<DownloadInvoiceFileDto>();

        //        try
        //        {
        //            var downloadInvoiceDto = new DownloadInvoiceFileDto();

        //            var renderer = new ChromePdfRenderer();

        //            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A3;

        //            License.LicenseKey = "IRONPDF.MUHAMMADSWILAM.8994-D5D6B0B50D-CGO7Z6SBYHBHO-NJBBXF6RUJDN-IZCJVKRJTDR3-EVDRK2QNDYKA-YWL6OJ67ZRC7-5JT4VU-T4GD7GG3OOSHEA-DEPLOYMENT.TRIAL-7M72SR.TRIAL.EXPIRES.01.SEP.2022";

        //            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //            var assetsPath = $@"{basePath}\Files\PDFs\Invoices\Assets";

        //            var uri = new Uri(assetsPath);

        //            var tempPath = $@"{basePath}\Files\PDFs\Invoices\Temp";

        //            var templatesDto = GenerateInvoicesPdfTemplates(invoicesDto);

        //            // generate invoice pdf.
        //            if (invoicesDto.Count() == 1)
        //            {
        //                using var pdf = await renderer.RenderHtmlAsPdfAsync(templatesDto.FirstOrDefault().Template, uri);

        //                var pdfBase64 = Convert.ToBase64String(pdf.BinaryData);

        //                downloadInvoiceDto.FileBase64 = $"{Base64MimeType} {pdfBase64}";
        //                downloadInvoiceDto.FileName = $"{invoicesDto.FirstOrDefault().InvoiceInfo.Serial}.pdf";
        //            }
        //            else
        //            {
        //                // generate invoices zip file.
        //                if (!Directory.Exists(tempPath))
        //                    Directory.CreateDirectory(tempPath);

        //                var tempGuid = Guid.NewGuid();
        //                var tempInvoicesDir = Directory.CreateDirectory($@"{tempPath}\TempInvoices{tempGuid}");

        //                foreach (var templateDto in templatesDto)
        //                {
        //                    using var pdf = await renderer.RenderHtmlAsPdfAsync(templateDto.Template, uri);

        //                    var invoicePdfPath = $@"{tempInvoicesDir.FullName}\{templateDto.Serial}.pdf";

        //                    pdf.SaveAs(invoicePdfPath);
        //                }

        //                // compress files.
        //                var extension = ".zip";
        //                var compressedFileGeneratedNameWithExtension = $"Invoices-{Guid.NewGuid()}{extension}";
        //                var compressedFilesPath = Path.Combine(tempPath, compressedFileGeneratedNameWithExtension);
        //                ZipFile.CreateFromDirectory(tempInvoicesDir.FullName, compressedFilesPath);

        //                // delete temp invoices directory.
        //                Directory.Delete(tempInvoicesDir.FullName, recursive: true);

        //                var compressedFileBase64 = Convert.ToBase64String(File.ReadAllBytes(compressedFilesPath));

        //                File.Delete(compressedFilesPath);

        //                downloadInvoiceDto.FileBase64 = $"{Base64MimeType} {compressedFileBase64}";
        //                downloadInvoiceDto.FileName = $"Invoices{extension}";
        //            }

        //            return response.CreateResponse(downloadInvoiceDto);
        //        }
        //        catch (Exception ex)
        //        {
        //            return response.CreateResponse(ex);
        //        }
        //    }

        //    #region Convert to Pdf using IRon PDf
        //    /// <summary>
        //    /// Generate invoice/s file, one invoice = pdf base64, more than one invoice = zip file base64.
        //    /// </summary>
        //    /// <param name="invoicesDto"></param>
        //    /// <returns></returns>
        //    //public async Task<IResponse<DownloadInvoiceFileDto>> GenerateInvoiceFileAsync(IEnumerable<InvoicePdfDto> invoicesDto)
        //    //{
        //    //    var response = new Response<DownloadInvoiceFileDto>();

        //    //    try
        //    //    {
        //    //        var downloadInvoiceDto = new DownloadInvoiceFileDto();

        //    //        var renderer = new ChromePdfRenderer();

        //    //        renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A3;

        //    //        License.LicenseKey = "IRONPDF.MUHAMMADSWILAM.8994-D5D6B0B50D-CGO7Z6SBYHBHO-NJBBXF6RUJDN-IZCJVKRJTDR3-EVDRK2QNDYKA-YWL6OJ67ZRC7-5JT4VU-T4GD7GG3OOSHEA-DEPLOYMENT.TRIAL-7M72SR.TRIAL.EXPIRES.01.SEP.2022";

        //    //        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //    //        var assetsPath = $@"{basePath}\Files\PDFs\Invoices\Assets";

        //    //        var uri = new Uri(assetsPath);

        //    //        var tempPath = $@"{basePath}\Files\PDFs\Invoices\Temp";

        //    //        var templatesDto = GenerateInvoicesPdfTemplates(invoicesDto);

        //    //        generate invoice pdf.
        //    //        if (invoicesDto.Count() == 1)
        //    //        {
        //    //            using var pdf = await renderer.RenderHtmlAsPdfAsync(templatesDto.FirstOrDefault().Template, uri);

        //    //            var pdfBase64 = Convert.ToBase64String(pdf.BinaryData);

        //    //            downloadInvoiceDto.FileBase64 = $"{Base64MimeType} {pdfBase64}";
        //    //            downloadInvoiceDto.FileName = $"{invoicesDto.FirstOrDefault().InvoiceInfo.Serial}.pdf";
        //    //        }
        //    //        else
        //    //        {
        //    //            generate invoices zip file.
        //    //            if (!Directory.Exists(tempPath))
        //    //                Directory.CreateDirectory(tempPath);

        //    //            var tempGuid = Guid.NewGuid();
        //    //            var tempInvoicesDir = Directory.CreateDirectory($@"{tempPath}\TempInvoices{tempGuid}");

        //    //            foreach (var templateDto in templatesDto)
        //    //            {
        //    //                using var pdf = await renderer.RenderHtmlAsPdfAsync(templateDto.Template, uri);

        //    //                var invoicePdfPath = $@"{tempInvoicesDir.FullName}\{templateDto.Serial}.pdf";

        //    //                pdf.SaveAs(invoicePdfPath);
        //    //            }

        //    //            compress files.
        //    //            var extension = ".zip";
        //    //            var compressedFileGeneratedNameWithExtension = $"Invoices-{Guid.NewGuid()}{extension}";
        //    //            var compressedFilesPath = Path.Combine(tempPath, compressedFileGeneratedNameWithExtension);
        //    //            ZipFile.CreateFromDirectory(tempInvoicesDir.FullName, compressedFilesPath);

        //    //            delete temp invoices directory.
        //    //            Directory.Delete(tempInvoicesDir.FullName, recursive: true);

        //    //            var compressedFileBase64 = Convert.ToBase64String(File.ReadAllBytes(compressedFilesPath));

        //    //            File.Delete(compressedFilesPath);

        //    //            downloadInvoiceDto.FileBase64 = $"{Base64MimeType} {compressedFileBase64}";
        //    //            downloadInvoiceDto.FileName = $"Invoices{extension}";
        //    //        }

        //    //        return response.CreateResponse(downloadInvoiceDto);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        return response.CreateResponse(ex);
        //    //    }
        //    //}
        //    #endregion
        //    public async Task<IResponse<string>> GenerateInvoicePdfAsync(InvoicePdfDto invoiceDto, bool savePdf = true)
        //    {
        //        var response = new Response<string>();

        //        try
        //        {
        //            var renderer = new ChromePdfRenderer();

        //            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A3;

        //            License.LicenseKey = "IRONPDF.MUHAMMADSWILAM.8994-D5D6B0B50D-CGO7Z6SBYHBHO-NJBBXF6RUJDN-IZCJVKRJTDR3-EVDRK2QNDYKA-YWL6OJ67ZRC7-5JT4VU-T4GD7GG3OOSHEA-DEPLOYMENT.TRIAL-7M72SR.TRIAL.EXPIRES.01.SEP.2022";

        //            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //            var assetsPath = $@"{basePath}\Files\PDFs\Invoices\Assets";

        //            var uri = new Uri(assetsPath);

        //            var invoiceTemplate = GenerateInvoicePdfTemplate(invoiceDto);

        //            using var pdf = await renderer.RenderHtmlAsPdfAsync(invoiceTemplate.Template, uri);


        //            if (savePdf)
        //            {
        //                var tempPath = $@"{basePath}\Files\PDFs\Invoices\Temp";

        //                if (!Directory.Exists(tempPath))
        //                    Directory.CreateDirectory(tempPath);

        //                var invoicePdfPath = $@"{tempPath}\{Guid.NewGuid()}.pdf";

        //                using var savedPdf = pdf.SaveAs(invoicePdfPath);

        //                return response.CreateResponse(invoicePdfPath);
        //            }
        //            else
        //                return response.CreateResponse(Convert.ToBase64String(pdf.BinaryData));
        //        }
        //        catch (Exception ex)
        //        {
        //            return response.CreateResponse(ex.ToString());
        //        }
        //    }

        //    public string GenerateInvoiceQrCode(InvoicePdfDto invoicePdfDto)
        //    {
        //        var tags = new List<Tag>
        //        {
        //            new Tag
        //            {
        //                Number = 1,
        //                Value = _companyInfoPdf.Name
        //            },
        //            new Tag
        //            {
        //                Number = 2,
        //                Value = _companyInfoPdf.TaxReg
        //            },
        //            new Tag
        //            {
        //                Number = 3,
        //                Value = invoicePdfDto.InvoiceInfo.CreateDate.ToString()
        //            },
        //            new Tag
        //            {
        //                Number = 4,
        //                Value = invoicePdfDto.InvoiceSummary.Total.ToString()
        //            },
        //            new Tag
        //            {
        //                Number = 5,
        //                Value = invoicePdfDto.InvoiceSummary.VatValue.ToString()
        //            }
        //        };

        //        var qrEngine = new QREngine();

        //        var qrCode = qrEngine.GetQRCode(tags);

        //        var memoryStream = new MemoryStream();
        //        qrCode.Save(memoryStream, ImageFormat.Png);
        //        var qrByteArr = memoryStream.GetBuffer();

        //        return $"data:image/png;base64,{Convert.ToBase64String(qrByteArr)}";
        //    }

        //    #region Helpers.
        //    private IEnumerable<InvoiceTemplateDto> GenerateInvoicesPdfTemplates(IEnumerable<InvoicePdfDto> invoicesDto)
        //    {
        //        var templates = new List<InvoiceTemplateDto>();

        //        foreach (var invoice in invoicesDto)
        //        {
        //            templates.Add(GenerateInvoicePdfTemplate(invoice));
        //        }

        //        return templates;
        //    }

        //    private InvoiceTemplateDto GenerateInvoicePdfTemplate(InvoicePdfDto invoiceDto)
        //    {
        //        var template = new InvoiceTemplateDto();

        //        var sb = new StringBuilder();

        //        var qrBase64 = GenerateInvoiceQrCode(invoiceDto);

        //        sb.AppendFormat(@"<!DOCTYPE html>
        //                                <html lang='en'>

        //                                <head>
        //                                    <meta charset='UTF-8'>
        //                                    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
        //                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        //                                    <title>Invoice</title>
        //                                    <link rel='stylesheet' href='style.css' />
        //                                </head>

        //                                <body>
        //                                    <div class='pdf'>
        //                                        <div class='pdf-header d-flex justify-content-between'>
        //                                            <div class='mt-4'>
        //                                                <img src='img\dexef_logo.png' alt='' />
        //                                                <p>{0}</p>
        //                                                <p>TAX Reg. No. {1}</p>
        //                                            </div>
        //                                            <img class='invoice-status' src='img\{2}.png' alt='invoice-status' />
        //                                        </div>
        //                                        <div class='content'>
        //                                            <div class='qr-pdf'>
        //                                                <div>
        //                                                    <h1 class='title'>INVOICE #{3}</h1>
        //                                                    <p>Invoice Date: <span>{4}</span> </p>
        //                                                    <p> Period: <span>{5}-{6} </span></p>
        //                                                </div>
        //                                                <div>
        //                                                    <img class='img-qr' src='{7}' alt='qr' />
        //                                                </div>
        //                                            </div>
        //                                            <div class='invoiceTo'>
        //                                                <h2>INVOICE TO</h2>
        //                                                <p>Account: {8} </p>
        //                                                <p>Name: {9} </p>
        //                                                <p>Tax Reg: {10} </p>
        //                                                <p>Address: {11} </p>
        //                                            </div>", _companyInfoPdf.Address,
        //                                                     _companyInfoPdf.TaxReg,
        //                                                     invoiceDto.InvoiceInfo.Status,
        //                                                     invoiceDto.InvoiceInfo.Serial,
        //                                                     //invoiceDto.InvoiceInfo.CreateDate.ToString("dd/MM/yyyy"),
        //                                                     //invoiceDto.InvoiceInfo.StartDate.ToString("dd/MM/yyyy"),
        //                                                     //invoiceDto.InvoiceInfo.EndDate.ToString("dd/MM/yyyy"),
        //                                                     qrBase64,
        //                                                     invoiceDto.CustomerInfo.ContractSerial,
        //                                                     invoiceDto.CustomerInfo.Name,
        //                                                     invoiceDto.CustomerInfo.TaxReg,
        //                                                     invoiceDto.CustomerInfo.Address);

        //        foreach (var item in invoiceDto.InvoiceItems)
        //        {
        //            sb.AppendFormat(@"<div class='product-table'>
        //                                <table>
        //                                    <thead>
        //                                        <tr>
        //                                            <th>Product</th>
        //                                            <th>Price Level</th>
        //                                            <th>Subscription Type</th>
        //                                            <th>Discount</th>
        //                                            <th>Net Price</th>
        //                                        </tr>
        //                                    </thead>
        //                                    <tbody>
        //                                        <tr>
        //                                            <td>
        //                                                <span class='bold'>{0} Expire on {1}</span>
        //                                            </td>
        //                                            <td>{2}</td>
        //                                            <td>{3}</td>
        //                                            <td class='green'>-{4} {5}</td>
        //                                            <td>{6} {7}</td>
        //                                        </tr>
        //                                    </tbody>
        //                                </table>
        //                            </div>", item.ProductName,
        //                                 //invoiceDto.InvoiceInfo.EndDate.ToString("dd/MM/yyyy"),
        //                                 item.PriceLevel,
        //                                 item.SubscriptionType,
        //                                 item.Discount,
        //                                 item.CurrencyName,
        //                                 item.NetPrice,
        //                                 item.CurrencyName
        //                        );
        //        }

        //        sb.AppendFormat(@"<div class='total'>
        //                                <div class='d-flex justify-content-between'>
        //                                    <h1>Subtotal</h1>
        //                                    <p>{0} {1}</p>
        //                                </div>
        //                                <div class='d-flex justify-content-between'>
        //                                    <h2>Applicable VAT</h2>
        //                                    <span> {2}%</span>
        //                                </div>
        //                                <div class='d-flex justify-content-between'>
        //                                    <h2>VAT Value</h2>
        //                                    <span>{3} {4}</span>
        //                                </div>
        //                                <hr class='hr' />
        //                                <div class='d-flex justify-content-between'>
        //                                    <h1 class='h1-total'>Total</h1>
        //                                    <p class='p-total'>{5} {6}</p>
        //                                </div>
        //                            </div>
        //                        </div>
        //                    </div>
        //                </body>
        //            </html>", invoiceDto.InvoiceSummary.Subtotal,
        //                  invoiceDto.InvoiceSummary.CurrencyName,
        //                  invoiceDto.InvoiceSummary.VatPercentage,
        //                  invoiceDto.InvoiceSummary.VatValue,
        //                  invoiceDto.InvoiceSummary.CurrencyName,
        //                  invoiceDto.InvoiceSummary.Total,
        //                  invoiceDto.InvoiceSummary.CurrencyName
        //        );

        //        template.Template = sb.ToString();
        //        template.Serial = invoiceDto.InvoiceInfo.Serial;

        //        return template;
        //    }


        //    #endregion
        //}
    }
}
