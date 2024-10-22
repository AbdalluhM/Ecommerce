using AutoMapper;
using Dexef.QRGenerator;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.Reports;
using Ecommerce.Reports.Templts;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Pdfs
{
    public class DevExpressBLL : BaseBLL, IPdfGeneratorBLL
    {
        private readonly IMapper _mapper;
        private readonly CompanyInfoPdf _companyInfoPdf;
        private const string Base64MimeType = "data:application/octet-stream;base64,";
        private readonly IReportManager _reportManger;
        private readonly IRepository<Ecommerce.Core.Entities.Currency> _currencyRepository;

        public DevExpressBLL(IMapper mapper,
                             IOptions<CompanyInfoPdf> companyInfoPdfOptions,
                             IReportManager reportManger,
                             IRepository<Core.Entities.Currency> currencyRepository) : base(mapper)
        {
            _mapper = mapper;
            _companyInfoPdf = companyInfoPdfOptions.Value;
            _reportManger = reportManger;
            _currencyRepository = currencyRepository;
        }

        public async Task<IResponse<string>> GenerateInvoicePdfAsync(InvoicePdfOutputDto invoiceDto, bool savePdf = true, string lang = DXConstants.SupportedLanguage.EN)
        {
            var response = new Response<string>();
            //Get Byte array of Pdf 
            var arrByte = InvoiceByteArr(lang, invoiceDto).Data;

            if (savePdf)
            {
                if (!Directory.Exists(GetPaths().TempPath))
                    Directory.CreateDirectory(GetPaths().TempPath);

                var invoicePdfPath = $@"{GetPaths().TempPath}\{Guid.NewGuid()}.pdf";

                File.WriteAllBytes(invoicePdfPath, arrByte);

                return response.CreateResponse(invoicePdfPath);
            }

            //Convert to base64
            var pdfBase64 = Convert.ToBase64String(arrByte);

            return response.CreateResponse(pdfBase64);
        }

        public string GenerateInvoiceQrCode(InvoicePdfDto invoicePdfDto)
        {
            try
            {
                var qrByteArr = GetQrCodeByteArr(invoicePdfDto);

                var qruBase64 = Convert.ToBase64String(qrByteArr);
                return qruBase64;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public string GenrateQrImagePath(InvoicePdfDto invoicePdfDto)
        {
            try
            {
                //create directory if not exisit
                if (!Directory.Exists(GetPaths().TempPath))
                    Directory.CreateDirectory(GetPaths().TempPath);

                var tempGuid = Guid.NewGuid();
                
                var tempInvoicesDir = System.IO.Directory.CreateDirectory($@"{GetPaths().TempPath}\TempInvoices{tempGuid}\QrImages");
                
                var invoiceQRImagePath = $@"{tempInvoicesDir.FullName}\{invoicePdfDto.InvoiceInfo.Serial}.png";

                var qrByteArr = GenerateInoviceQRByteArray(invoicePdfDto);
                
                File.WriteAllBytes(invoiceQRImagePath, qrByteArr);

                return invoiceQRImagePath;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public IResponse<byte[]> InvoiceByteArr(string lang, InvoicePdfOutputDto invoiceDto)
        {
            var response = new Response<byte[]>();
            try
            {
                var assetsPath = GetPaths(lang).AssetPath;
                // GET Logo For type of image 
                invoiceDto = GetLogoPathDevExpress(invoiceDto, assetsPath);
                //Get QrCode
                var QrImagePath = GenrateQrImagePath(invoiceDto);
                invoiceDto.QrImagePath = QrImagePath;

                var isDefault = lang.ToLower() == nameof(JsonLangEnum.En).ToLower();
                var currency = GetCurrencyCodeAsJson(invoiceDto.InvoiceSummary.CurrencyName);
                var localizedCurrencyName = isDefault ? GetJsonLanguageModelOrNull(currency)?.Default : GetJsonLanguageModelOrNull(currency)?.Ar;
                invoiceDto.InvoiceSummary.CurrencyName = localizedCurrencyName;

                invoiceDto.InvoiceItems.ToList().ForEach(x =>
                {
                    x.ProductName = isDefault ? GetJsonLanguageModelOrNull(x.ProductName).Default : GetJsonLanguageModelOrNull(x.ProductName).Ar;
                    x.SubscriptionType = isDefault ? GetJsonLanguageModelOrNull(x.SubscriptionType).Default : GetJsonLanguageModelOrNull(x.SubscriptionType).Ar;
                    x.CurrencyName = localizedCurrencyName;
                });
                var arrByte = _reportManger.ExportAs(Reports.Enums.FormatType.pdf, Reports.Enums.ReportNames.InvoiceReport, invoiceDto);
                return response.CreateResponse(arrByte);
            }
            catch (Exception e)
            {

                return response.CreateResponse(e);
            }

        }

        public string GenerateInoviceQRBase64(InvoicePdfDto invoicePdfDto)
        {
            try
            {
                var imageByteArray = GenerateInoviceQRByteArray(invoicePdfDto);

                var imageBase64 = Convert.ToBase64String(imageByteArray);

                return imageBase64;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public byte[] GenerateInoviceQRByteArray(InvoicePdfDto invoicePdfDto)
        {
            try
            {
                var qrGenerator = new QRCodeGenerator();

                var qrContent = GenerateInvoiceQRContent(invoicePdfDto);

                var qrcodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);

                var qrCode = new QRCode(qrcodeData);

                //var client = new WebClient();

                //byte[] logoButeArray = client.DownloadData("https://dexef.net/wp-content/uploads/2020/08/Logowhite1.png");

                //using var ms = new MemoryStream(logoButeArray);

                //var logoBitmap = new Bitmap(ms);

                var qrCodeImage = qrCode.GetGraphic(20, Color.White, Color.Black, true);

                var imageConverter = new ImageConverter();

                var imageByteArray = (byte[])imageConverter.ConvertTo(qrCodeImage, typeof(byte[]));

                return imageByteArray;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Helper
        private string GenerateInvoiceQRContent(InvoicePdfDto invoicePdfDto)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Invoice Info:");

            var invoiceStatus = JsonConvert.DeserializeObject<JsonLanguageModel>(invoicePdfDto.InvoiceInfo.Status);

            sb.AppendLine($"Serial: {invoicePdfDto.InvoiceInfo.Serial}");
            sb.AppendLine($"Inovice Date: {invoicePdfDto.InvoiceInfo.CreateDate}");
            sb.AppendLine($"Period: {invoicePdfDto.InvoiceInfo.StartDate} - {invoicePdfDto.InvoiceInfo.EndDate}");
            sb.AppendLine($"Inoivce Status: {invoiceStatus.Default}");

            sb.AppendLine("------------------------------------------------------");

            sb.AppendLine("Customer Info:");

            sb.AppendLine($"Contract Serail: {invoicePdfDto.CustomerInfo.ContractSerial}");
            sb.AppendLine($"Name: {invoicePdfDto.CustomerInfo.Name}");
            sb.AppendLine($"Tax Reg: {invoicePdfDto.CustomerInfo.TaxReg}");
            sb.AppendLine($"Address: {invoicePdfDto.CustomerInfo.Address}");

            sb.AppendLine("------------------------------------------------------");

            sb.AppendLine("Invoice Items:");

            foreach (var item in invoicePdfDto.InvoiceItems)
            {
                var productName = JsonConvert.DeserializeObject<JsonLanguageModel>(item.ProductName);
                var subscriptionType = JsonConvert.DeserializeObject<JsonLanguageModel>(item.SubscriptionType);

                sb.AppendLine($"Product: {productName.Default}");
                sb.AppendLine($"Expire On: {invoicePdfDto.InvoiceInfo.EndDate}");
                sb.AppendLine($"Price Level: {item.PriceLevel}");
                sb.AppendLine($"Subscription Type: {subscriptionType.Default}");
                sb.AppendLine($"Discount: {item.Discount} {item.CurrencyName}");
                sb.AppendLine($"Net Price: {item.NetPrice} {item.CurrencyName}");

                sb.AppendLine("------------------------------------------------------");
            }

            sb.AppendLine("Invoice Summary:");

            sb.AppendLine($"Sub Total: {invoicePdfDto.InvoiceSummary.Subtotal} {invoicePdfDto.InvoiceSummary.CurrencyName}");
            sb.AppendLine($"VAT: {invoicePdfDto.InvoiceSummary.VatPercentage} %");
            sb.AppendLine($"VAT Value: {invoicePdfDto.InvoiceSummary.VatValue} {invoicePdfDto.InvoiceSummary.CurrencyName}");
            sb.AppendLine($"Total: {invoicePdfDto.InvoiceSummary.Total} {invoicePdfDto.InvoiceSummary.CurrencyName}");

            return sb.ToString();
        }

        private byte[] GetQrCodeByteArr(InvoicePdfDto invoicePdfDto)
        {
            try
            {
                var tags = new List<Tag>
            {
                new Tag
                {
                    Number = 1,
                    Value = _companyInfoPdf.Name
                },
                new Tag
                {
                    Number = 2,
                    Value = _companyInfoPdf.TaxReg
                },
                new Tag
                {
                    Number = 3,
                    Value = invoicePdfDto.InvoiceInfo.CreateDate.ToString()
                },
                new Tag
                {
                    Number = 4,
                    Value = invoicePdfDto.InvoiceSummary.Total.ToString()
                },
                new Tag
                {
                    Number = 5,
                    Value = invoicePdfDto.InvoiceSummary.VatValue.ToString()
                }
            };

                var qrEngine = new QREngine();

                var qrCode = qrEngine.GetQRCode(tags);

                var memoryStream = new MemoryStream();
                qrCode.Save(memoryStream, ImageFormat.Png);
                var qrByteArr = memoryStream.GetBuffer();

                return qrByteArr;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetCurrencyCodeAsJson(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return _currencyRepository
                                 .Where(a => Json.Value(a.Code, "$.default") == name || Json.Value(a.Code, "$.ar") == name)
                                 .FirstOrDefault()?.Code?.Trim() ?? string.Empty;


        }

        private InvoicePdfOutputDto GetLogoPathDevExpress(InvoicePdfOutputDto invoice, string assetsPath)
        {

            invoice.LogoPath = invoice.StatusId switch
            {
                (int)InvoiceStatusEnum.Draft => $"{assetsPath}Draft.png",
                (int)InvoiceStatusEnum.Unpaid => $"{assetsPath}Unpaid.png",
                (int)InvoiceStatusEnum.Paid => $"{assetsPath}Paid.png",
                (int)InvoiceStatusEnum.Cancelled => $"{assetsPath}Cancelled.png",
                (int)InvoiceStatusEnum.Refunded => $"{assetsPath}Refunded.png",
                _ => $"{assetsPath} + Draft.png"
            };


            return invoice;
        }

        private GetPathesOutputDto GetPaths(string lang = DXConstants.SupportedLanguage.EN)
        {
            var output = new GetPathesOutputDto();
            try
            {
                var basePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return new GetPathesOutputDto
                {
                    BasePath = basePath,
                    TempPath = $@"{basePath}\Files\PDFs\Invoices\Temp",
                    AssetPath = $@"{basePath}\Files\PDFs\Invoices\Assets\img\{lang}\",
                };
            }
            catch (Exception e)
            {

                throw;
            }
        }

        #endregion
    }
}

