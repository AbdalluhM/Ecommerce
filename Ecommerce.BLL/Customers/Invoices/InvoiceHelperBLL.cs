using AutoMapper;

using Dexef.Notification.Enums;
using Dexef.Notification.Models.Mailing;
using Dexef.Payment.FawryAPI;
using Dexef.Payment.PAYFORTAPI;
using Microsoft.Extensions.Options;

using Ecommerce.BLL.Addons;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Customers.Auth;
using Ecommerce.BLL.Customers.Crm;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Payments;
using Ecommerce.BLL.Pdfs;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Taxes;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Crm;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.DTO.Settings.Notifications.Mails;
using Ecommerce.DTO.Settings.StaticNumbers;
using Ecommerce.DTO.Taxes;
using Ecommerce.Reports.Templts;
using Ecommerce.Repositroy.Base;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;

using Contract = Ecommerce.Core.Entities.Contract;
using IO = System.IO;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Customers.Invoices
{
    public class InvoiceHelperBLL : BaseBLL, IInvoiceHelperBLL
    {
        #region Fields
        private readonly Numbers _numbers;
        private readonly IMapper _mapper;
        private readonly IRepository<PriceLevel> _priceLevelRepository;
        private readonly ITaxBLL _taxBLL;

        private readonly IRepository<CustomerSubscription> _customerSubscriptionRepository;
        private readonly IRepository<VersionSubscription> _versionSubscriptionRepository;
        private readonly IRepository<AddonSubscription> _addonSubscriptionRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepository;

        private readonly IRepository<VersionPrice> _versionPriceRepository;
        private readonly IRepository<AddOnPrice> _addOnPriceRepository;
        private readonly IRepository<VersionRelease> _versionReleaseRepository;

        private readonly IRepository<Contract> _customerContract;

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Core.Entities.PaymentMethod> _paymentMethodRepository;


        private readonly ICountryBLL _countryBLL;
        //private readonly ICustomerBLL _customerBLL;
        private readonly IPaymentSetupBLL _paymentSetupBLL;

        private readonly IVersionBLL _versionBLL;
        private readonly IAddOnBLL _addOnBLL;

        private readonly ICrmBLL _crmBLL;
        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationBLL _notificationBLL;
        private readonly InvoiceSetting _invoiceSetting;
        private readonly PaymentGateWaySettings _paymentGateWaySettings;
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IAuthBLL _authBLL;

        private readonly EmailTemplateSetting _emailTemplateSetting;
        #endregion
        #region Constructor
        public InvoiceHelperBLL(IMapper mapper,
                          IOptions<Numbers> numbers,
                          IRepository<PriceLevel> priceLevelRepository,
                          ITaxBLL taxBLL,
                          IRepository<CustomerSubscription> customerSubscriptionRepository,
                          IRepository<VersionSubscription> versionSubscriptionRepository,
                          IRepository<AddonSubscription> addonSubscriptionRepository,
                          IRepository<Invoice> invoiceRepository,
                          IRepository<InvoiceDetail> invoiceDetailRepository,
                          IRepository<VersionPrice> versionPriceRepository,
                          IRepository<AddOnPrice> addOnPriceRepository,
                          IRepository<VersionRelease> versionReleaseRepository,
                          IRepository<Contract> customerContract,
                          IRepository<Customer> customerRepository,
                          ICountryBLL countryBLL,
                          //ICustomerBLL customerBLL,
                          IPaymentSetupBLL paymentSetupBLL,

                          IVersionBLL versionBLL,
                          IAddOnBLL addOnBLL,
                          ICrmBLL crmBLL,
                          IUnitOfWork unitOfWork,
                          INotificationBLL notificationBLL,
                          IOptions<InvoiceSetting> invoiceOptions,
                          IOptions<PaymentGateWaySettings> paymentGateWaySettings,
                          IPdfGeneratorBLL pdfGeneratorBLL,
                          INotificationDataBLL notificationDataBLL,
                          IAuthBLL authBLL,
        IOptions<EmailTemplateSetting> emailTemplateSetting,
        IRepository<Core.Entities.PaymentMethod> paymentMethodRepository)
            : base(mapper)
        {
            _mapper = mapper;
            _priceLevelRepository = priceLevelRepository;
            _taxBLL = taxBLL;

            _customerSubscriptionRepository = customerSubscriptionRepository;
            _versionSubscriptionRepository = versionSubscriptionRepository;
            _addonSubscriptionRepository = addonSubscriptionRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _versionPriceRepository = versionPriceRepository;
            _addOnPriceRepository = addOnPriceRepository;
            _versionReleaseRepository = versionReleaseRepository;


            _customerContract = customerContract;
            _customerRepository = customerRepository;
            _versionBLL = versionBLL;
            _addOnBLL = addOnBLL;
            _countryBLL = countryBLL;
            //_customerBLL = customerBLL;
            _paymentSetupBLL = paymentSetupBLL;
            _crmBLL = crmBLL;
            _unitOfWork = unitOfWork;
            _notificationBLL = notificationBLL;
            _invoiceSetting = invoiceOptions.Value;
            _paymentGateWaySettings = paymentGateWaySettings.Value;
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _notificationDataBLL = notificationDataBLL;
            _emailTemplateSetting = emailTemplateSetting.Value;
            _authBLL = authBLL;
            _numbers = numbers.Value;
            _paymentMethodRepository = paymentMethodRepository;
        }

        #endregion
        #region Invoice Price Calculations
        public InvoiceTotals CalculateInvoice(decimal itemPrice, decimal discount, Tax tax, bool firstPurchase, string title)
        {
            var output = new InvoiceTotals { Details = new List<CreateInvoiceDetailInputDto>() };
            var detail = new CreateInvoiceDetailInputDto();

            decimal xPrice = 0M;
            decimal xTotal = 0M;
            decimal xNetPrice = 0M;
            var taxPercentage = (tax?.Percentage ?? 0) / 100;
            var taxPriceIncludeTax = tax?.PriceIncludeTax ?? false;

            decimal discountPercentage = discount / 100;
            decimal originalPrice = itemPrice;

            //will apply discount when first purchase
            discountPercentage = (firstPurchase ? discountPercentage : 0);
            //equation xTotal = xPrice * (1 + taxPercentage) *(1- discountPercentage)

            if (taxPriceIncludeTax)
            {
                //comment this to get original price without vat
                originalPrice = itemPrice / (1 + taxPercentage);
                //given xTotal
                xTotal = itemPrice;
                xPrice = xTotal / (((1 + taxPercentage)) > 0 ? ((1 + taxPercentage)) : 1);
                xNetPrice = xPrice * (1 - discountPercentage);
            }
            else
            {
                //given xPrice
                xPrice = itemPrice;
                xTotal = xPrice * (1 + taxPercentage) * (1 - discountPercentage);
                xNetPrice = xPrice * (1 - discountPercentage);
            }



            detail.Discount = discountPercentage * originalPrice;
            detail.DiscountPercentage = discountPercentage * 100;
            detail.NetPrice = xNetPrice;
            detail.Price = originalPrice;
            detail.VatAmount = xNetPrice * taxPercentage;
            detail.VersionName = title;
            output.Details.Add(detail);


            output.TotalVatAmount = Math.Round(xNetPrice * taxPercentage, 2);// output.Details.Sum(x => x.VatAmount);
            output.Price = Math.Round(output.Details.Sum(x => x.Price), 2);
            output.TotalDiscountAmount = Math.Round(output.Details.Sum(x => x.Discount), 2);
            output.VatPercentage = Math.Round(taxPercentage, 2) * 100;
            output.PriceAfterDiscount = Math.Round(output.Details.Sum(x => x.NetPrice), 2);
            output.SubTotal = Math.Round(output.Details.Sum(x => x.NetPrice), 2);
            output.Total = Math.Round(output.SubTotal + output.TotalVatAmount, 2);
            return output;
        }
        public InvoiceTotals CalculateInvoice(VersionPrice vp, Tax tax, int discriminator, bool firstPurchase)
        {
            return discriminator switch
            {
                (int)DiscriminatorsEnum.Forever => CalculateInvoice(vp.ForeverPrice, vp.ForeverPrecentageDiscount, tax, firstPurchase, vp.Version.Title),
                (int)DiscriminatorsEnum.Yearly => CalculateInvoice(vp.YearlyPrice, vp.YearlyPrecentageDiscount, tax, firstPurchase, vp.Version.Title),
                (int)DiscriminatorsEnum.Monthly => CalculateInvoice(vp.MonthlyPrice, vp.MonthlyPrecentageDiscount, tax, firstPurchase, vp.Version.Title),
                _ => new InvoiceTotals { Details = new List<CreateInvoiceDetailInputDto>() }
            };

        }
        public InvoiceTotals CalculateInvoice(AddOnPrice ap, Tax tax, int discriminator, bool firstPurchase)
        {
            return discriminator switch
            {
                (int)DiscriminatorsEnum.Forever => CalculateInvoice(ap.ForeverPrice, ap.ForeverPrecentageDiscount, tax, firstPurchase, ap.AddOn.Title),
                (int)DiscriminatorsEnum.Yearly => CalculateInvoice(ap.YearlyPrice, ap.YearlyPrecentageDiscount, tax, firstPurchase, ap.AddOn.Title),
                (int)DiscriminatorsEnum.Monthly => CalculateInvoice(ap.MonthlyPrice, ap.MonthlyPrecentageDiscount, tax, firstPurchase, ap.AddOn.Title),
                _ => new InvoiceTotals { Details = new List<CreateInvoiceDetailInputDto>() }
            };


        }
        public GetSupportInvoicePriceOutputDto GetInvoicePrice(decimal Price, GetTaxOutputDto tax)
        {
            var output = new GetSupportInvoicePriceOutputDto();
            output.SupportPrice = (decimal)_numbers.SupportInvoiceNumbers.SupportPresentage * Price;
            if (tax != null)
            {
                if (!tax.PriceIncludeTax)
                {
                    output.VatPrice = output.SupportPrice * (tax.Percentage / (decimal)_numbers.HundredNum);
                    output.NetPrice = output.SupportPrice;
                    output.TotalPrice = output.VatPrice + output.NetPrice;
                }
                else
                {
                    output.NetPrice = output.SupportPrice /
                         ((decimal)_numbers.OneNum + (tax.Percentage / (decimal)_numbers.HundredNum));
                    output.VatPrice = output.SupportPrice - output.NetPrice;
                    output.TotalPrice = output.SupportPrice;
                }
            }
            else
            {
                output.VatPrice = 0;
                output.NetPrice = output.SupportPrice;
                output.TotalPrice = output.SupportPrice;
            }

            return output;
        }

        public InvoiceTotals GetInvoiceSummary(Invoice invoice)
        {
            var isAddOn = invoice.CustomerSubscription.IsAddOn;
            var output = new InvoiceTotals { Details = new List<CreateInvoiceDetailInputDto>() };
            var detail = new CreateInvoiceDetailInputDto();
            detail.VersionName = !isAddOn
                                ? invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName
                                : invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName;
            //TODO:validate against first and may add in mapping


            detail.Discount = invoice.InvoiceDetails.FirstOrDefault()?.Discount ?? 0;
            detail.DiscountPercentage = invoice.InvoiceDetails.FirstOrDefault()?.DiscountPercentage ?? 0;
            detail.NetPrice = invoice.InvoiceDetails.FirstOrDefault()?.NetPrice ?? 0;
            detail.Price = invoice.InvoiceDetails.FirstOrDefault()?.Price ?? 0;
            detail.VatAmount = invoice.InvoiceDetails.FirstOrDefault()?.VatAmount ?? 0;

            output.Details.Add(detail);

            output.Price = invoice.SubTotal + invoice.TotalDiscountAmount;
            output.PriceAfterDiscount = invoice.SubTotal;
            output.TotalDiscountAmount = invoice.TotalDiscountAmount;
            output.TotalVatAmount = invoice.TotalVatAmount;
            output.VatPercentage = invoice.VatPercentage;
            output.SubTotal = invoice.SubTotal;
            output.Total = invoice.Total;

            return output;
        }
        public Invoice CalculateInvoiceFromPreviousInvoice(Invoice previousInvoice)
        {
            bool isAddOn = previousInvoice.CustomerSubscription.IsAddOn;
            AddonSubscription addOnSubscription = isAddOn ? previousInvoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault() : null;
            VersionSubscription versionSubscription
                = isAddOn
                ? addOnSubscription.VersionSubscription
                : previousInvoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault();
            var customer = isAddOn ? addOnSubscription.CustomerSubscription.Customer : versionSubscription.CustomerSubscription.Customer;
            var renewEvery = isAddOn ? addOnSubscription.CustomerSubscription.RenewEvery : versionSubscription.CustomerSubscription.RenewEvery;
            var contract = _customerContract.GetById(customer.Id);
            var tax = _taxBLL.GetDefaultTaxForCountry(previousInvoice.CustomerSubscription.Customer.CountryId);

            var invoiceDetails = _mapper.Map<List<InvoiceDetail>>(CalculateInvoiceDetailsFromPreviousInvoice(previousInvoice, tax).ToList());
            //TODO:we need to add PriceIncludeTax n Subscription
            decimal totalDiscountAmount = invoiceDetails.Sum(x => x.Discount);
            decimal totalVatAmount = invoiceDetails.Sum(x => x.VatAmount.GetValueOrDefault(0));
            decimal vatPercentage = tax?.Percentage ?? 0; // or from old invoice
            decimal subTotal = invoiceDetails.Sum(x => x.Price);//- (tax is not null ? (!tax.PriceIncludeTax ? totalVatAmount : 0) : 0);
            return new Invoice
            {
                PaymentMethodId = previousInvoice.PaymentMethodId,
                CustomerSubscriptionId = isAddOn ? addOnSubscription.CustomerSubscriptionId : versionSubscription.CustomerSubscriptionId,
                InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid,
                Serial = GenerateInvoiceSerial(0, contract.Serial),// "INV-" + Guid.NewGuid().ToString(),
                TaxReg = customer.TaxRegistrationNumber,
                Address = customer.FullAddress,
                InvoiceTypeId = GetInvoiceTypeFromPreviousInvoice(previousInvoice.InvoiceTypeId),
                CreateDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(renewEvery),
                InvoiceDetails = invoiceDetails,
                TotalDiscountAmount = totalDiscountAmount,
                TotalVatAmount = totalVatAmount,
                VatPercentage = tax?.Percentage ?? 0, // or from old invoice
                SubTotal = subTotal,
                Total = subTotal + totalVatAmount,
                CurrencyId = isAddOn ? addOnSubscription?.AddonPrice?.CountryCurrency?.CurrencyId :
                         versionSubscription?.VersionPrice?.CountryCurrency?.CurrencyId
            };
        }

        public List<CreateInvoiceDetailInputDto> CalculateInvoiceDetailsFromPreviousInvoice(Invoice previousInvoice, Tax tax)
        {
            var output = new List<CreateInvoiceDetailInputDto>();
            List<InvoiceDetail> previousInvoiceDetails = previousInvoice.InvoiceDetails.ToList();
            foreach (var item in previousInvoiceDetails)
            {

                //apply discount only on first invoice and don't apply on renewal or support invoices
                //for support invoice  make 15% of firt invoice value
                decimal newItemPrice = default(decimal);
                if (previousInvoice.InvoiceTypeId == (int)InvoiceTypes.ForeverSubscription)
                {
                    newItemPrice = ((decimal)_numbers.FifteenNum * item.Price) / (decimal)_numbers.HundredNum;
                    output.Add(new CreateInvoiceDetailInputDto
                    {
                        //TODO:put this in setting

                        Price = newItemPrice,
                        Discount = (decimal)_numbers.ZeroNum,
                        DiscountPercentage = (decimal)_numbers.ZeroNum,
                        NetPrice = (newItemPrice),
                        VersionName = item.VersionName, //"Support on " +  //TODO:check if will changed to support
                        VatAmount = ((tax?.Percentage ?? 0) * newItemPrice) / (decimal)_numbers.HundredNum // from current tax configuration
                    })
                    ;
                    ;
                }
                //apply same previous price 
                else if (previousInvoice.InvoiceTypeId == (int)InvoiceTypes.Support)
                {
                    newItemPrice = item.Price;
                    output.Add(new CreateInvoiceDetailInputDto
                    {
                        Price = item.Price,
                        Discount = (decimal)_numbers.ZeroNum,
                        DiscountPercentage = (decimal)_numbers.ZeroNum,
                        NetPrice = item.Price,
                        VersionName = item.VersionName,
                        VatAmount = ((tax?.Percentage ?? 0) * newItemPrice) / (decimal)_numbers.HundredNum  // from current tax configuration 
                    });
                }
                // //TODO: check apply current version prices and taxes
                else //if(previousInvoiceInvoiceTypeId ==  (int)InvoiceTypes.Renewal)
                {
                    #region get Current Item Price
                    int discriminator = GetInvoiceDiscriminator(previousInvoice);
                    bool isAddOn = previousInvoice.CustomerSubscription.IsAddOn;
                    if (isAddOn)
                    {
                        var invoiceTotals = CalculateInvoice(previousInvoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonPrice, tax, discriminator, false);

                        newItemPrice = invoiceTotals.Price;
                    }
                    else
                    {
                        var invoiceTotals = CalculateInvoice(previousInvoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionPrice, tax, discriminator, false);

                        newItemPrice = invoiceTotals.Price;

                    }
                    #endregion
                    output.Add(new CreateInvoiceDetailInputDto
                    {
                        Price = newItemPrice,
                        Discount = (decimal)_numbers.ZeroNum,
                        DiscountPercentage = (decimal)_numbers.ZeroNum,
                        NetPrice = newItemPrice,
                        VersionName = item.VersionName,
                        VatAmount = ((tax?.Percentage ?? 0) * newItemPrice) / (decimal)_numbers.HundredNum  // from current tax configuration
                    });
                }


            }



            return output;
        }
        public async Task<Invoice> GetInvoiceById(int invoiceId)
        {
            var invoice = invoiceId > 0 ? await _invoiceRepository.GetByIdAsync(invoiceId) : null;
            return invoice;
        }
        public async Task<RetrieveInvoiceDto> GetMappedInvoiceById(int invoiceId)
        {
            var invoice = await GetInvoiceById(invoiceId);
            return invoice != null ? _mapper.Map<RetrieveInvoiceDto>(invoice) : null;
        }
        #endregion
        #region Calculate number of invoices to create and periods
        public int CalculateNumberOfInvoicesToCreate(DateTime lastInvoiceDate, int renewEvery)
        {
            if ((DateTime.UtcNow - lastInvoiceDate).TotalDays < 0)
                return 0;
            return Convert.ToInt32(Math.Ceiling((DateTime.UtcNow - lastInvoiceDate).TotalDays / renewEvery));
        }
        public List<InvoicePeriodDto> GetInvoicePeriods(DateTime lastInvoiceDate, int renewEvery)
        {
            var nubmerOfInvoices = CalculateNumberOfInvoicesToCreate(lastInvoiceDate, renewEvery);
            var periods = new List<InvoicePeriodDto>();
            var newStartDate = lastInvoiceDate;
            ////uncomment this for leap year
            //var newEndDate =  renewEvery == (int)_numbers.YearDays ? lastInvoiceDate.AddYears(1) :  lastInvoiceDate.AddDays(renewEvery);
            var newEndDate = lastInvoiceDate.AddDays(renewEvery);
            for (int i = 0; i < nubmerOfInvoices; i++)
            {
                periods.Add(new InvoicePeriodDto
                {
                    StartDate = newStartDate,
                    EndDate = newEndDate
                });

                newStartDate = newStartDate.AddDays(renewEvery);
                newEndDate = newEndDate.AddDays(renewEvery);

            }
            return periods;
        }
        #endregion
        #region Check For First Subscription
        public bool IsFirstCustomerSubscription(int customerId, int id, int priceLevelId, bool isAddon)
        {


            if (!isAddon)
            {

                return !(_versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customerId && x.VersionReleaseId == id && x.VersionPrice.PriceLevelId == priceLevelId).Any(x => x.CustomerSubscription.Invoices.Any(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded)));
            }
            else
            {
                return !(_addonSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customerId && x.AddonPrice.AddOnId == id && x.AddonPrice.PriceLevelId == priceLevelId).Any(x => x.CustomerSubscription.Invoices.Any(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded)));
            }

        }
        public bool IsFirstCustomerSubscription(int versionSubscriptionId, int? addOnSubscriptionId, bool isAddOn, bool isProductService)
        {
            if (isProductService)
                return false;

            else if (!isAddOn)
            {

                var versionSubscription = _versionSubscriptionRepository.GetById(versionSubscriptionId);
                if (versionSubscription == null)
                    return true;
                return !versionSubscription?.CustomerSubscription?.Invoices?.Any(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded) ?? true;

            }
            else if (isAddOn)
            {
                var addOnSubscription = _addonSubscriptionRepository.GetById(addOnSubscriptionId ?? 0);
                if (addOnSubscription == null)
                    return true;
                return !addOnSubscription?.CustomerSubscription?.Invoices?.Any(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded) ?? true;
            }

            else
                return true;

        }
        public bool IsFirstCustomerSubscriptionByInvoice(int invoiceId)
        {
            var invoice = _invoiceRepository.GetById(invoiceId);
            if (invoice is null)
                return true;
            if (invoice.CustomerSubscription?.Invoices?.Count(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded) <= 1)
                return true;
            return false;
        }
        public bool IsFirstCustomerSubscriptionBySubscription(int customerSubscriptionId)
        {
            var subscription = _customerSubscriptionRepository.GetById(customerSubscriptionId);
            if (subscription is null)
                return true;
            if (subscription?.Invoices?.Count(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded) <= 1)
                return true;
            return false;
        }

        #endregion
        #region Invoice Discriminator
        public int GetInvoiceDiscriminator(RetrieveInvoiceDto invoice)
        {
            if (invoice == null)
                return 0;
            return GetInvoiceDiscriminator(invoice.InvoiceTypeId, invoice.CustomerSubscription.RenewEvery);
        }
        public int GetInvoiceDiscriminator(Invoice invoice)
        {
            if (invoice == null)
                return 0;
            return GetInvoiceDiscriminator(invoice.InvoiceTypeId, invoice.CustomerSubscription.RenewEvery);
        }

        public int GetInvoiceDiscriminator(int invoiceTypeId, int renewEvery)
        {
            if (invoiceTypeId != (int)InvoiceTypes.Renewal) //this is for for support and forever subscription
                return (int)DiscriminatorsEnum.Forever;
            else if (invoiceTypeId == (int)InvoiceTypes.Renewal && renewEvery == (int)_numbers.MonthDays)
                return (int)DiscriminatorsEnum.Monthly;
            else if (invoiceTypeId == (int)InvoiceTypes.Renewal && renewEvery == (int)_numbers.YearDays)
                return (int)DiscriminatorsEnum.Yearly;
            return 0;

        }

        #endregion
        #region Invoice Type
        public int GetInvoiceTypeFromPreviousInvoice(int previousInvoiceTypeId)
        {   //previousInvoiceType => newImvoiceType
            //support => support
            //renewal => renewal
            //ForeverSubscription => support

            return previousInvoiceTypeId == (int)InvoiceTypes.ForeverSubscription ? (int)InvoiceTypes.Support : previousInvoiceTypeId;
        }
        public int GetInvoiceType(int discriminator, bool firstSubscription)
        {
            int invoiceTypeId = 0;
            if (discriminator == (int)DiscriminatorsEnum.Forever && firstSubscription)
            {
                invoiceTypeId = (int)InvoiceTypes.ForeverSubscription;

            }
            else if (discriminator == (int)DiscriminatorsEnum.Forever && !firstSubscription)
            {
                invoiceTypeId = (int)InvoiceTypes.Support;

            }
            else if (discriminator == (int)DiscriminatorsEnum.Yearly || discriminator == (int)DiscriminatorsEnum.Monthly)
            {
                invoiceTypeId = (int)InvoiceTypes.Renewal;

            }
            else
            {
                invoiceTypeId = (int)InvoiceTypes.Renewal;

            }
            return invoiceTypeId;
        }
        #endregion
        #region Invoice Serial
        public string GenerateInvoiceSerial(int invoiceId = 0, string contractSerial = null)
        {
            var serialParts = !string.IsNullOrWhiteSpace(contractSerial) ? contractSerial.Split('-') : Guid.NewGuid().ToString().Split('-');
            return invoiceId > 0 ? "INV-" + serialParts.LastOrDefault() + "-" + invoiceId : "INV-" + Guid.NewGuid().ToString();

        }
        private string GenerateInvoiceSerial(Invoice invoice, Contract contract)
        {
            return GenerateInvoiceSerial(invoice.Id, contract?.Serial);

        }
        public async Task<bool> UpdateInvoiceSerialAsync(Invoice invoice, Contract contract = null, bool commit = false)
        {
            try
            {
                contract = contract == null ? await CreateOrGetCustomerContractAsync(invoice.CustomerSubscription.Customer.Id, false) : contract;

                if (invoice != null)
                {
                    //var contract = invoice.CustomerSubscription.Customer.Contract;
                    invoice.Serial = GenerateInvoiceSerial(invoice, contract);
                    if (commit)
                        _unitOfWork.Commit();
                    return true;
                }

            }
            catch (Exception e)
            {

                return false;
            }
            return false;
            #region Old
            //try
            //{
            //    if (invoice != null)
            //    {
            //        var serialParts = contract != null ? contract.Serial.Split('-') : Guid.NewGuid().ToString().Split('-');
            //        //TODO:Add Contract and Invoice Formats to appsettings
            //        invoice.Serial = "INV-" + serialParts.LastOrDefault() + "-" + invoice.Id;
            //        if (commit)
            //            _unitOfWork.Commit();
            //        return true;
            //    }

            //}
            //catch (Exception e)
            //{

            //    return false;
            //}
            //return false;
            #endregion

        }

        #endregion
        #region Currency
        public string GetCurrencyCode(string objCode, LangEnum lang = LangEnum.Default)
        {
            try
            {
                var desiralizedObject = objCode.Deserialize<JsonLanguageModel>();
                return lang == LangEnum.Default ? desiralizedObject?.Default : desiralizedObject?.Ar;
            }
            catch (Exception)
            {
                return !string.IsNullOrWhiteSpace(objCode) ? objCode : string.Empty;
            }

        }
        #endregion
        #region Subscriptions
        private Invoice MapedAdminInvoice(APIPayAndSubscribeInputDto inputDto, bool isFirstSubscription, string productName, Invoice invoice)
        {
            var customerCountry = _customerRepository.GetById(inputDto.CustomerId);
            var tax = _taxBLL.GetDefaultTaxForCountry(customerCountry.CountryId);
            var invoiceTotals = CalculateInvoice(
                inputDto.Price,
                0M,
                tax,
                isFirstSubscription,
                productName
                );
            invoice = _mapper.Map(invoiceTotals, invoice);
            invoice.StartDate = inputDto.StartDate;
            invoice.EndDate = inputDto.EndDate;
            return invoice;
        }

        private Invoice MapedInvoiceData(APIPayAndSubscribeInputDto inputDto, Invoice invoice, int currencyId)
        {
            invoice.Serial = GenerateInvoiceSerial();
            invoice.InvoiceTitle = inputDto.InvoiceTitle != null ? inputDto.InvoiceTitle : null;
            invoice.Notes = inputDto.Notes ?? string.Empty;
            invoice.TaxReg = inputDto.Customer.TaxRegistrationNumber;
            invoice.Address = inputDto.Customer.FullAddress;
            invoice.Address = inputDto.Customer.FullAddress ?? string.Empty;
            invoice.InvoiceStatusId = inputDto.InvoiceStatusId == 0 ? (int)InvoiceStatusEnum.Unpaid : inputDto.InvoiceStatusId;
            invoice.CurrencyId = currencyId;
            invoice.PaymentMethodId = inputDto.PaymentMethodId;
            return invoice;
        }

        public int GetSubscriptionId(VersionSubscription versionSubscription, AddonSubscription addonSubscription = null, bool isAddOn = false)
        {
            return isAddOn && addonSubscription != null ? addonSubscription.CustomerSubscriptionId :
              versionSubscription.CustomerSubscriptionId;
        }
        public int GetSubscriptionCurrency(VersionSubscription versionSubscription, AddonSubscription addonSubscription = null, bool isAddOn = false)
        {
            return (isAddOn ? addonSubscription?.AddonPrice?.CountryCurrency?.CurrencyId :
                versionSubscription?.VersionPrice?.CountryCurrency?.CurrencyId) ??
                (isAddOn ? addonSubscription.CustomerSubscription.Customer.Country.CountryCurrency.CurrencyId : versionSubscription.CustomerSubscription.Customer.Country.CountryCurrency.CurrencyId);
        }
        public CustomerSubscription CreateOrGetCustomerSubscription(APIPayAndSubscribeInputDto inputDto, bool commit = false)
        {
            //create CustomerSubscription if not exists
            CustomerSubscription customerSubscription = null;
            if (inputDto.CustomerSubscriptionId == 0)
            {
                //Todo Check
                if (inputDto.IsAdminInvoice)
                {
                    if (inputDto.InvoiceTypeId == (int)InvoiceTypes.ForeverSubscription)
                        inputDto.SubscriptionTypeId = (int)SubscriptionTypeEnum.Forever;
                    else
                        inputDto.SubscriptionTypeId = (int)SubscriptionTypeEnum.Others;
                }
                customerSubscription = _customerSubscriptionRepository.Add(_mapper.Map<CustomerSubscription>(inputDto));
                if (commit)
                    _unitOfWork.Commit();
            }
            else
            {
                customerSubscription = _customerSubscriptionRepository.GetById(inputDto.CustomerSubscriptionId);

            }
            //assign customersubscriptionId to 
            inputDto.CustomerSubscriptionId = customerSubscription.Id;
            return customerSubscription;
        }
        public IResponse<VersionSubscription> CreateOrGetVersionSubscription(APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, bool commit = false)
        {
            var output = new Response<VersionSubscription>();
            bool isFirstInvoice = inputDto.CustomerSubscriptionId == 0 /*&& inputDto.VersionSubscriptionId == 0 && inputDto.AddOnSubscriptionId == 0*/;
            //validate version Price for Admin
            VersionRelease versionRelease = null;
            VersionPrice versionPrice = null;
            //TODO:Refactor this to get current versionSubscription Id not deducted one because default versionreleaseid may be change

            if (inputDto.IsAddOn && inputDto.VersionSubscriptionId == 0)
            {
                ////TODO:use new Created InputDto VersionReleaseId
                // versionRelease = _versionBLL.GetVersionReleaseById(inputDto.VersionReleaseId);
                versionRelease = _versionBLL.GetCurrentVersionReleaseOrNull(inputDto.VersionId.GetValueOrDefault(0), isFirstInvoice);

                versionPrice = isFirstInvoice
                                ? versionRelease.Version.VersionPrices.FirstOrDefault(x => x.PriceLevelId == inputDto.PriceLevelId)
                                : _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.PriceLevelId);
                if (versionPrice == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(versionPrice));


            }
            //add version subscription
            VersionSubscription versionSubscription = null;
            if (inputDto.VersionSubscriptionId == 0)
            {
                if (inputDto.IsAddOn)
                {
                    versionSubscription = _versionSubscriptionRepository.Where(x =>
                    x.VersionReleaseId == versionRelease.Id &&
                   x.CustomerSubscription.CustomerId == inputDto.CustomerId &&
                   !x.CustomerSubscription.IsAddOn &&
                   x.VersionPriceId == versionPrice.Id)
                        .FirstOrDefault();
                    if (versionSubscription == null)
                    {
                        return output.CreateResponse(MessageCodes.MustPurchaseVersionFirst, nameof(Version));
                    }
                }
                else
                {
                    versionSubscription = _versionSubscriptionRepository.Add(_mapper.Map<VersionSubscription>(inputDto));

                }
            }
            else
            {
                versionSubscription = _versionSubscriptionRepository.GetById(inputDto.VersionSubscriptionId.GetValueOrDefault(0));

            }
            //assign version subscriptionId to 
            inputDto.VersionSubscriptionId = versionSubscription.Id;

            // Todo check by abdalluh
            if (inputDto.VersionSubscriptionId == 0)
                versionSubscription.CustomerSubscription = customerSubscription;

            if (commit)
                _unitOfWork.Commit();


            return output.CreateResponse(versionSubscription);
        }
        public AddonSubscription CreateOrGetAddOnSubscription(APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, VersionSubscription versionSubscription, bool commit = false)
        {
            AddonSubscription addonSubscription = null;

            if (inputDto.IsAddOn)
            {

                //get related version Subscription
                //add version subscription
                if (inputDto.AddOnSubscriptionId == 0 || inputDto.AddOnSubscriptionId == null)
                {
                    addonSubscription = _addonSubscriptionRepository.Add(_mapper.Map<AddonSubscription>(inputDto));
                }
                else
                {
                    addonSubscription = _addonSubscriptionRepository.GetById(inputDto.AddOnSubscriptionId.GetValueOrDefault(0));

                }
                //assign addon subscriptionId to 
                inputDto.AddOnSubscriptionId = addonSubscription.Id;
                addonSubscription.CustomerSubscription = customerSubscription;
                addonSubscription.VersionSubscription = versionSubscription;
                if (commit)
                    _unitOfWork.Commit();


            }

            return addonSubscription;
        }

        public async Task<Invoice> CreateOrGetInvoiceAsync(APIPayAndSubscribeInputDto inputDto, Contract contract, CustomerSubscription customerSubscription, VersionSubscription versionSubscription, AddonSubscription addonSubscription, bool commit = false)
        {
            bool isFirstSubscription = IsFirstCustomerSubscription(inputDto.CustomerId,
                  inputDto.IsAddOn ? inputDto.AddOnId.GetValueOrDefault(0) : inputDto.VersionReleaseId, inputDto.PriceLevelId, inputDto.IsAddOn);
            Invoice invoice = null;
            InvoiceDto invoiceDto = null;
            if (inputDto.InvoiceId == 0)
            {

                invoiceDto = _mapper.Map<InvoiceDto>(inputDto);
                invoice = /*_invoiceRepository.Add(*/_mapper.Map<Invoice>(invoiceDto)/*)*/;
                invoice.CustomerSubscription = customerSubscription;
                if (inputDto.IsAdminInvoice)
                {
                    var productName = !inputDto.IsAddOn ? versionSubscription.VersionName : addonSubscription.AddonName;
                    invoice = MapedAdminInvoice(inputDto, isFirstSubscription, productName, invoice);

                };

                //int currencyId = (inputDto.IsAddOn ? addonSubscription?.AddonPrice?.CountryCurrency?.CurrencyId :
                //versionSubscription?.VersionPrice?.CountryCurrency?.CurrencyId) ?? customerSubscription.Customer.Country.CountryCurrency.CurrencyId;
                int currencyId = GetSubscriptionCurrency(versionSubscription, addonSubscription, inputDto.IsAddOn);
                invoice = MapedInvoiceData(inputDto, invoice, currencyId);


                invoice.CustomerSubscription = customerSubscription;
                _invoiceRepository.Add(invoice);

                if (inputDto.IsAdminInvoice && !string.IsNullOrEmpty(inputDto.InvoiceSerial))

                    invoice.Serial = inputDto.InvoiceSerial;
                else
                    await UpdateInvoiceSerialAsync(invoice, commit: false);


                if (commit)
                    _unitOfWork.Commit();



                //assign invoice to 
                inputDto.InvoiceId = invoice.Id;

            }
            else
            {
                invoice = _invoiceRepository.GetById(inputDto.InvoiceId);
            }
            return invoice;
        }

        public bool IsSubscriptionHasAnyPaidInvoice(int versionSupscriptionId)
        {
            var versionSubscriptions = _versionSubscriptionRepository.GetById(versionSupscriptionId);
            return versionSubscriptions.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.RefundRequest.RefundRequestStatusId != (int)RefundRequestStatusEnum.Accepted);
        }
        public bool IsSubscriptionHasAnyPaidInvoice(CustomerSubscription customerSubscription)
        {
            return _invoiceRepository
                       .Any(x => x.CustomerSubscriptionId == customerSubscription.Id && x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.RefundRequest.RefundRequestStatusId != (int)RefundRequestStatusEnum.Accepted);
        }
        #endregion
        #region RollBack
        public bool RollBackSubscription(int customerSubscriptionId)
        {
            var customerSubscription = _customerSubscriptionRepository.GetById(customerSubscriptionId);
            if (customerSubscription != null)
            {
                var versiionSubscriptions = customerSubscription.VersionSubscriptions.ToList();
                var addonSubscriptions = customerSubscription.AddonSubscriptions.ToList();
                if (customerSubscription.Invoices.Count(x => x.InvoiceStatusId != (int)InvoiceStatusEnum.Paid) == 1)
                {

                    var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscriptionId).FirstOrDefault();
                    if (versiionSubscriptions.Any())
                    {

                        _versionSubscriptionRepository.DeleteRange(versiionSubscriptions);
                        //foreach (var versionSubscription in versiionSubscriptions)
                        //{

                        //    _versionSubscriptionRepository.Delete(versionSubscription);
                        //}
                    }
                    if (addonSubscriptions.Any())
                    {
                        _addonSubscriptionRepository.DeleteRange(addonSubscriptions);
                        //foreach (var addonSubscription in addonSubscriptions)
                        //{

                        //    _addonSubscriptionRepository.Delete(addonSubscription);
                        //}
                    }
                    if (invoice.InvoiceDetails.Any())
                    {
                        _invoiceDetailRepository.DeleteRange(invoice.InvoiceDetails);
                        //foreach (var item in invoice.InvoiceDetails)
                        //{
                        //    _invoiceDetailRepository.Delete(item);
                        //}
                        _invoiceRepository.Delete(invoice);
                    }
                    //delte notification related subscription
                    // _notificationDataBLL.DeleteRelatedNotificationAsync(new DeleteNotificationDto { CustomerSubscriptionId = customerSubscription.Id });
                    if (!customerSubscription.Invoices.Any(x => x.Id != invoice.Id && x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid))
                    {
                        _customerSubscriptionRepository.Delete(customerSubscription);
                    }
                    _unitOfWork.Commit();
                    return true;
                }
            }
            return false;

        }

        public bool RollBackSubscriptionByInvoiceId(int invoiceId)
        {
            int customerSubscriptionId = _invoiceRepository.GetById(invoiceId)?.CustomerSubscriptionId ?? 0;
            return RollBackSubscription(customerSubscriptionId);

        }
        #endregion
        #region Other


        //TODO:Add in Appsetting
        public int GetInvoiceRenewEvery(int discriminator)
        {
            int renewEvery = 0;
            if (discriminator == (int)DiscriminatorsEnum.Forever || discriminator == (int)DiscriminatorsEnum.Yearly)
            {
                renewEvery = (int)_numbers.YearDays;
            }
            else if (discriminator == (int)DiscriminatorsEnum.Monthly)
            {
                renewEvery = (int)_numbers.MonthDays;

            }
            else
            {
                renewEvery = (int)_numbers.ZeroNum;
            }
            return renewEvery;
        }

        //id can hold versionReleaseId or AddOnId


        public async Task<InvoiceDto> UpdateInvoiceTotalsWithConvertedCurrencyAsync(InvoiceDto invoice, string fromCurrency, string toCurrency)
        {
            //TODO:Add CurrencyCode, ActualCurrencyCode Columns to Invoice
            //CurrencyCode=>paymentData.CurrencyCode
            //ActualCurrencyCode => paypalDefaultCurrency
            var conversionPaymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Bank);

            PayFortClient client = new PayFortClient(conversionPaymentMethod.Credential.ApiKey, conversionPaymentMethod.Credential.SercretKey, _paymentGateWaySettings.Language, _paymentGateWaySettings.Bank.SHARequestPhrase, _paymentGateWaySettings.Bank.SHAResponsePhrase, _paymentGateWaySettings.Bank.SHAType, _paymentGateWaySettings.IsSandBox);

            var conversionResult = await client.ConvertCurrency(fromCurrency, toCurrency, invoice.Total.ToString());
            //Update Invoice Totals with new USD Conversion Rate
            if (conversionResult.IsSuccess)
            {
                var convertionRate = decimal.Round(conversionResult.Rate, 2, MidpointRounding.AwayFromZero);

                invoice.Total *= convertionRate;
                ;
                invoice.SubTotal *= convertionRate;
                ;
                invoice.TotalDiscountAmount *= convertionRate;
                invoice.TotalVatAmount *= convertionRate;
                invoice.Details.ForEach(x =>
                {
                    x.Discount *= convertionRate;
                    x.Price *= convertionRate;
                    x.NetPrice *= convertionRate;
                    x.VatAmount *= convertionRate;
                });
            }
            //else
            //{
            //    // invoice.InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid;
            //}
            return invoice;
        }



        public string GetConcatenatedJsonString(string separator, params string[] jsonNames)
        {

            var concatenatedJsonString = new JsonLanguageModel
            {
                Default
                = jsonNames.Aggregate((current, next)
               => JsonConvert.DeserializeObject<JsonLanguageModel>(current).Default
               + separator
               + JsonConvert.DeserializeObject<JsonLanguageModel>(next).Default)
                ,
                Ar = jsonNames.Aggregate((current, next)
                  => JsonConvert.DeserializeObject<JsonLanguageModel>(current).Ar
                  + separator
                  + JsonConvert.DeserializeObject<JsonLanguageModel>(next).Ar)

            };

            return JsonConvert.SerializeObject(concatenatedJsonString).ToLower();
        }

        public String CreatePaymentResultForSearch(int PaymentTypeId, InvoicePaymentInfoJson paymentInfo)
        {
            String resultForsarch = "";
            switch (PaymentTypeId)
            {
                case (int)PaymentTypesEnum.PayPal:
                    resultForsarch = "Paypal - " + paymentInfo.Paypal.Account;
                    break;
                case (int)PaymentTypesEnum.Fawry:
                    resultForsarch = "Fawry - " + (paymentInfo.Fawry.IsFawryCard ? paymentInfo.Fawry.CardNumber : paymentInfo.Fawry.ReferenceNumber);
                    break;
                case (int)PaymentTypesEnum.Bank:
                    resultForsarch = "Visa - " + paymentInfo.Bank.CardNumber;
                    break;
                default:
                    break;
            }
            return resultForsarch;
        }
        public DateTime GetRenewalDate(int customerSubscriptionId)
        {
            DateTime date = DateTime.UtcNow;
            var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscriptionId
            && x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid).OrderByDescending(i => i.Id).FirstOrDefault()
            ;

            if (invoice == null)
            {
                var customerSubscription = _customerSubscriptionRepository.Where(x => x.Id == customerSubscriptionId)
                    .FirstOrDefault();
                if (customerSubscription == null)
                {
                    date = DateTime.UtcNow;
                }
                date = customerSubscription.CreateDate.AddDays(customerSubscription.RenewEvery);
            }
            else
            {
                date = invoice.EndDate;
            }

            return date;

        }

        #endregion
        #region Update Invoice PaymentData and Status

        public bool UpdateInvoicePaymentInfoForSearch(Invoice invoice, InvoicePaymentInfoJson paymentInfo, bool commit = false)
        {
            try
            {
                if (invoice != null)
                {
                    invoice.PaymentInfoSearch = CreatePaymentResultForSearch(invoice.PaymentMethod.PaymentTypeId, paymentInfo);
                    invoice.PaymentInfo = paymentInfo.Serialize();
                    if (commit)
                        _unitOfWork.Commit();
                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }
            return false;

        }
        public async Task<bool> UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(int invoiceId, PaymentTypesEnum paymentTypeId, int status = (int)InvoiceStatusEnum.Paid, string paymentResponse = null, bool commit = true)
        {
            try
            {
                var invoice = _invoiceRepository.GetById(invoiceId);
                if (invoice != null)
                {

                    if (invoice.InvoiceStatusId != (int)InvoiceStatusEnum.Paid && status == (int)InvoiceStatusEnum.Paid)
                    {
                        invoice.InvoiceStatusId = (int)status;
                        invoice.PaymentResponse = paymentResponse;
                        var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(paymentTypeId);
                        invoice.PaymentDate = DateTime.UtcNow;
                        invoice.PaymentMethodId = paymentMethod.Id;
                        invoice.PaymentResponse = paymentResponse != null ? paymentResponse : string.Empty;
                        var paymentInfo = JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo);
                        //TODO:Refactor in  UpdateFawryCardReferenceCodeAndCardData
                        if (invoice.PaymentMethod.PaymentTypeId == (int)PaymentTypesEnum.Fawry && paymentInfo != null && paymentInfo.Fawry != null && paymentInfo.Fawry.IsFawryCard)
                        {
                            int customerId = invoice.CustomerSubscription.CustomerId;
                            var customerCardNumber = _customerRepository.GetById(customerId).CustomerCards?.Where(x => x.PaymentMethodId == invoice.PaymentMethodId && x.CardToken == paymentInfo.Fawry.Token).FirstOrDefault()?.CardNumber;
                            {
                                var response = JsonConvert.DeserializeObject<InvoicePaymentResponseJson>(paymentResponse);

                                paymentInfo.Fawry.CardNumber = customerCardNumber;
                                paymentInfo.Fawry.ReferenceNumber = response.Fawry.referenceNumber;
                            }
                        }
                        //Update PaymentInfo and PaymentInfoSearch
                        invoice.PaymentInfo = JsonConvert.SerializeObject(paymentInfo);
                        invoice.PaymentInfoSearch = CreatePaymentResultForSearch(invoice.PaymentMethod.PaymentTypeId, paymentInfo);
                        if (commit)
                            _unitOfWork.Commit();
                        //Qualify Customer Must be after commit invoice with status Paid
                        var isFirstSubscription = IsFirstCustomerSubscriptionByInvoice(invoiceId);
                        await QualifyCustomerAsync(invoice.CustomerSubscription.CustomerId, isFirstSubscription, true);



                        // TODO: Move crm qualify calling from PayAndSubscribe to here. 

                        //push notification for Success Payment                     
                        var _notificationItem = new GetNotificationForCreateDto();
                        //chek if new subscribtion
                        _notificationItem.IsAdminSide = true;
                        _notificationItem.IsCreatedBySystem = false;
                        _notificationItem.InvoiceId = invoiceId;
                        if (isFirstSubscription)
                        {
                            _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.SuccessPayment;
                        }
                        else
                        {
                            // cases for tecnical support and renew
                            switch (invoice.InvoiceTypeId)
                            {
                                case (int)InvoiceTypes.Renewal:
                                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.SubscribtionRenewal;
                                    break;
                                case (int)InvoiceTypes.Support:
                                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.TechnicalSupport;
                                    break;
                                default:
                                    break;
                            }
                            // invoiceDto.InvoiceTypeId
                        }
                        _notificationItem.CustomerId = invoice.CustomerSubscription.CustomerId;
                        await _notificationDataBLL.CreateAsync(_notificationItem);



                        //send email.
                        //TODO :check send mail in Refund Status ?!
                        await SendInvoicesEmails(new List<Invoice> { invoice }, true);
                        //|TODO: Check if Update renew date & license status in License Table ??
                        // invoice.CustomerSubscription.Licenses

                    }
                    else if (status == (int)InvoiceStatusEnum.Refunded)
                    {
                        invoice.InvoiceStatusId = (int)status;
                        if (status == (int)InvoiceStatusEnum.Refunded)
                        {
                            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(paymentTypeId);
                            invoice.PaymentDate = DateTime.UtcNow;
                            invoice.PaymentMethodId = paymentMethod.Id;
                            invoice.PaymentResponse = paymentResponse != null ? paymentResponse : string.Empty;
                            if (commit)
                                _unitOfWork.Commit();
                        }
                    }
                    else
                    {
                        //   RollBackSubscriptionByInvoiceId(invoiceId);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }
            return false;

        }


        #endregion
        #region Customer
        public async Task<Contract> CreateOrGetCustomerContractAsync(int customerId, bool commit = false)
        {
            #region Contract
            //add contract if not exist
            var contract = await _customerContract.GetByIdAsync(customerId);
            try
            {

                if (contract == null)
                {
                    contract = _customerContract.Add(new Contract
                    {
                        Id = customerId,
                        Serial = "CUST-" + Guid.NewGuid().ToString() + "-" + customerId,
                        CreateDate = DateTime.UtcNow
                    });

                    if (commit)
                        _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                //log exception here

            }
            return contract;

            #endregion
        }

        public async Task<Contract> QualifyCustomerAsync(int customerId, bool isFirstSubscription, bool commit)
        {
            #region Contract
            //add contract if not exist
            var contract = await CreateOrGetCustomerContractAsync(customerId, commit);
            try
            {

                var customer = _customerRepository.GetById(customerId);
                //qualify 
                if (isFirstSubscription || string.IsNullOrWhiteSpace(customer.CrmleadId) || string.IsNullOrWhiteSpace(customer.CustomerCrmid))
                {
                    CrmRetrieveBaseDto crmAccount = null;
                    if (string.IsNullOrWhiteSpace(customer.CrmleadId))
                    {
                        //create lead
                        customer.CrmleadId = await _authBLL.CreateCrmLeadAsync(customerId);
                    }
                    if (!string.IsNullOrWhiteSpace(customer.CrmleadId))
                    {
                        crmAccount = !string.IsNullOrWhiteSpace(customer.CrmleadId) ? await _crmBLL.GetAccountByLeadIdAsync(Guid.Parse(customer.CrmleadId)) : null;
                        if (crmAccount is null && !string.IsNullOrWhiteSpace(customer.CrmleadId))
                        {
                            var crmId = await _crmBLL.QualifyLeadAsync(new QualifyLeadDto
                            {
                                Id = Guid.Parse(customer.CrmleadId)
                            });
                            if (!string.IsNullOrWhiteSpace(crmId))
                            {
                                crmAccount = new CrmRetrieveBaseDto
                                {
                                    Id = Guid.Parse(crmId),
                                    Name = ""
                                };
                                customer.CustomerCrmid = crmAccount.Id.ToString();
                                if (commit)
                                    _unitOfWork.Commit();

                            }
                        }
                        else if (crmAccount is not null)
                        {
                            customer.CustomerCrmid = crmAccount.Id.ToString();
                            if (commit)
                                _unitOfWork.Commit();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //log exception here

            }
            return contract;

            #endregion

        }
        #endregion
        #region Email Templates
        public async Task SendInvoicesEmails(List<Invoice> invoices, bool hasAttachment = false)
        {
            foreach (var invoice in invoices)
            {
                var filesPaths = new List<string>();

                if (hasAttachment)
                {

                    var invoicePdfDto = _mapper.Map<InvoicePdfOutputDto>(invoice);
                    var lang = Thread.CurrentThread.CurrentCulture.Name.Contains("ar") ? "Ar" : "En";
                    var pdfPathResult = await _pdfGeneratorBLL.GenerateInvoicePdfAsync(invoicePdfDto, true, lang);

                    if (!pdfPathResult.IsSuccess)
                        continue;

                    filesPaths.Add(pdfPathResult.Data);
                }

                try
                {
                    var emailResult = await _notificationBLL.SendEmailAsync(EmailProvider.Smtp,
                                                                            new MailContent
                                                                            {
                                                                                FromEmail = _invoiceSetting.Email.FromEmail,
                                                                                ToEmails = new List<string> { invoice.CustomerSubscription.Customer.CustomerEmails.FirstOrDefault(e => e.IsPrimary).Email },
                                                                                Subject = _invoiceSetting.Email.Subject,
                                                                                DisplayName = _invoiceSetting.Email.DisplayName,
                                                                                Body = await GenerateInvoiceEmailTemplate(invoice),
                                                                                FilesPath = filesPaths
                                                                            });

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<string> GenerateInvoiceEmailTemplate(Invoice invoice)
        {
            try
            {
                var assemblyFolder = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


                var file = await IO.File.ReadAllTextAsync(IO.Path.Combine(assemblyFolder,
                                                                          _emailTemplateSetting.BaseFilesPath,
                                                                          _emailTemplateSetting.Folder.Invoice.Name,
                                                                          _emailTemplateSetting.Folder.Invoice.HtmlFile));

                var invoiceDto = _mapper.Map<InvoicePdfOutputDto>(invoice);

                // Commented as base64 causes issue of exceeding limitation length.
                // So Omar decided to remove qr from mail.

                //var qrCodeBase64 = _pdfGeneratorBLL.GenerateInoviceQRBase64(invoiceDto);

                var replacedFile = file.Replace("%customerName%", invoice.CustomerSubscription.Customer.Name)
                                       .Replace("%totalAmount%", invoice.Total.ToString())
                                       .Replace("%currency%", invoice.CustomerSubscription.CurrencyName)
                                       .Replace("%invoiceSerial%", invoice.Serial)
                                       //.Replace("%qrCode%", qrCodeBase64)
                                       .Replace("%contactUsLink%",
                                            IO.Path.Combine(_emailTemplateSetting.CLientBaseUrl,
                                                            _emailTemplateSetting.ClientMethod.ContactUs))
                                       .Replace("%dexefLogoImg%",
                                            IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                            _emailTemplateSetting.BaseFilesPath,
                                                            _emailTemplateSetting.Folder.Image.Name,
                                                            _emailTemplateSetting.Folder.Image.Logo));

                return replacedFile;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
