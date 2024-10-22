using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using MyDexef.BLL.Pdfs.PdfSettings;
using MyDexef.BLL.Responses;
using MyDexef.DTO.Pdfs.Invoices;
using MyDexef.DTO.Settings.Pdfs;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MyDexef.BLL.Pdfs
{
    public class DinkToPdfGeneratorBLL : BaseBLL
    {
        private readonly IMapper _mapper;
        private readonly IPdfConfigBLL _pdfConfigBLL;
        private readonly IConverter _converter;

        public DinkToPdfGeneratorBLL(IMapper mapper,
                               IPdfConfigBLL pdfConfigBLL,
                               IConverter converter)
            : base(mapper)
        {
            _mapper = mapper;
            _pdfConfigBLL = pdfConfigBLL;
            _converter = converter;
        }

        public IResponse<string> GenerateInvoicePdf(IPdfSetting pdfSetting, InvoicePdfDto invoicePdf)
        {
            var response = new Response<string>();

            try
            {
                var globalSettings = _pdfConfigBLL.GetPdfGlobalSettings(pdfSetting);
                var objectSettings = _pdfConfigBLL.GetPdfObjectSettings(pdfSetting, GenerateInvoicePdfTemplate(invoicePdf, pdfSetting));

                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                var generatedPdf = _converter.Convert(pdf);
                
                return response.CreateResponse(Convert.ToBase64String(generatedPdf));
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex.ToString());
            }
        }

        private string GenerateInvoicePdfTemplate(InvoicePdfDto invoicePdfDto, IPdfSetting pdfSetting)
        {
            var sb = new StringBuilder();

            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var statusFullPath = Path.Combine(basePath,
                                              pdfSetting.ObjectSetting.StyleSheetPath,
                                              "img",
                                              $"{invoicePdfDto.InvoiceInfo.Status}.png");

            var logoFullPath = Path.Combine(basePath,
                                              pdfSetting.ObjectSetting.StyleSheetPath,
                                              "img",
                                              "dexef_logo.png");

            sb.AppendFormat(@"<!DOCTYPE html>
                                <html lang='en'>

                                <head>
                                    <meta charset='UTF-8'>
                                    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                    <title>Invoice</title>
                                    <link rel='stylesheet' href='style.css' />
                                </head>

                                <body>
                                    <div class='pdf'>
                                        <div class='pdf-header d-flex justify-content-between'>
                                            <div class='mt-4'>
                                                <img src='data:image/png;base64, {0}' alt='' />
                                                <p>{1}</p>
                                                <p>TAX Reg. No. {2}</p>
                                            </div>
                                            <img class='invoice-status' src='data:image/png;base64, {3}' alt='invoice-status' />
                                        </div>
                                        <div class='content'>
                                            <div class='qr-pdf'>
                                                <div>
                                                    <h1 class='title'>INVOICE #{4}</h1>
                                                    <p>Invoice Date: <span>{5}</span> </p>
                                                    <p> Period: <span>{6}-{7} </span></p>
                                                </div>
                                                <div>
                                                    <img class='img-qr' src='' alt='qr' />
                                                </div>
                                            </div>
                                            <div class='invoiceTo'>
                                                <h2>INVOICE TO</h2>
                                                <p>Account: {8} </p>
                                                <p>Name: {9} </p>
                                                <p>Tax Reg: {10} </p>
                                                <p>Address: {11} </p>
                                            </div>", Convert.ToBase64String(File.ReadAllBytes(logoFullPath)),
                                                     invoicePdfDto.CompanyInfo.Address,
                                                     invoicePdfDto.CompanyInfo.TaxReg,
                                                     Convert.ToBase64String(File.ReadAllBytes(statusFullPath)),
                                                     invoicePdfDto.InvoiceInfo.Serial,
                                                     invoicePdfDto.InvoiceInfo.CreateDate,
                                                     invoicePdfDto.InvoiceInfo.StartDate,
                                                     invoicePdfDto.InvoiceInfo.EndDate,
                                                     invoicePdfDto.CustomerInfo.ContractSerial,
                                                     invoicePdfDto.CustomerInfo.Name,
                                                     invoicePdfDto.CustomerInfo.TaxReg,
                                                     invoicePdfDto.CustomerInfo.Address
                                                     );

            foreach (var item in invoicePdfDto.InvoiceItems)
            {
                sb.AppendFormat(@"<div class='product-table'>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>Product</th>
                                            <th>Price Level</th>
                                            <th>Subscription Type</th>
                                            <th>Discount</th>
                                            <th>Net Price</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <span class='bold'>{0}</span>
                                            </td>
                                            <td>{1}</td>
                                            <td>{2}</td>
                                            <td class='green'>-{3} {4}</td>
                                            <td>{5} {6}</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>", item.ProductName,
                                     item.PriceLevel,
                                     item.SubscriptionType,
                                     item.Discount,
                                     item.Currency,
                                     item.NetPrice,
                                     item.Currency
                            );
            }

            sb.AppendFormat(@"<div class='total'>
                                <div class='d-flex justify-content-between'>
                                    <h1>Subtotal</h1>
                                    <p>{0} {1}</p>
                                </div>
                                <div class='d-flex justify-content-between'>
                                    <h2>Applicable VAT</h2>
                                    <span> {2}%</span>
                                </div>
                                <div class='d-flex justify-content-between'>
                                    <h2>VAT Value</h2>
                                    <span>{3} {4}</span>
                                </div>
                                <hr class='hr' />
                                <div class='d-flex justify-content-between'>
                                    <h1 class='h1-total'>Total</h1>
                                    <p class='p-total'>{5} {6}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </body>
            </html>", invoicePdfDto.InvoiceSummary.Subtotal,
                      invoicePdfDto.InvoiceSummary.Currency,
                      invoicePdfDto.InvoiceSummary.VatPercentage,
                      invoicePdfDto.InvoiceSummary.VatValue,
                      invoicePdfDto.InvoiceSummary.Currency,
                      invoicePdfDto.InvoiceSummary.Total,
                      invoicePdfDto.InvoiceSummary.Currency
            );

            return sb.ToString();
        }
    }
}
