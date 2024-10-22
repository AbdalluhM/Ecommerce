using AutoMapper;

using Dexef.Payment.FawryAPI;
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
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Settings.Notifications.Mails;
using Ecommerce.DTO.Settings.StaticNumbers;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerDto;

using Contract = Ecommerce.Core.Entities.Contract;

namespace Ecommerce.BLL.Customers.Invoices
{
    public class InvoiceBLL : BaseBLL, IInvoiceBLL
    {
        #region Field
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
        private readonly IRepository<CountryCurrency> _countryCurrencyRepository;

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
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IAuthBLL _authBLL;
        private readonly IInvoiceHelperBLL _invoiceHelper;
        private readonly IPaymentBLL _paymentBLL;
        #endregion

        #region Constructor
        public InvoiceBLL(IMapper mapper,
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
                          IPaymentSetupBLL paymentSetupBLL,
                          IVersionBLL versionBLL,
                          IAddOnBLL addOnBLL,
                          ICrmBLL crmBLL,
                          IUnitOfWork unitOfWork,
                          INotificationBLL notificationBLL,
                          IOptions<InvoiceSetting> invoiceOptions,
                          IPdfGeneratorBLL pdfGeneratorBLL,
                          INotificationDataBLL notificationDataBLL,
                          IAuthBLL authBLL,
                           IInvoiceHelperBLL invoiceHelper,
        IRepository<Core.Entities.PaymentMethod> paymentMethodRepository
,
        IPaymentBLL paymentBLL,
        IRepository<CountryCurrency> countryCurrencyRepository)
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
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _notificationDataBLL = notificationDataBLL;

            _invoiceHelper = invoiceHelper;
            _authBLL = authBLL;
            _numbers = numbers.Value;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentBLL = paymentBLL;
            _countryCurrencyRepository = countryCurrencyRepository;
        }
        #endregion

        #region Actions

        public IResponse<PagedResultDto<RetrieveInvoiceDto>> GetInvoices(FilterInvoiceDto pagedDto, int currentUserId)
        {
            var response = new Response<PagedResultDto<RetrieveInvoiceDto>>();
            var search = pagedDto.SearchTerm?.ToLower().Trim() ?? string.Empty;
            // Type parameters
            var orderExpre = GetOrderExpression<Invoice>(pagedDto.SortBy);
            var invoices = GetPagedList<RetrieveInvoiceDto, Invoice, dynamic>(pagedDto: pagedDto,
                repository: _invoiceRepository,
                orderExpression: orderExpre,
                searchExpression: i => ((!pagedDto.InvoiceStatusId.HasValue || pagedDto.InvoiceStatusId == 0) ||
                                        (pagedDto.InvoiceStatusId.HasValue && pagedDto.InvoiceStatusId.Value > 0 &&
                                         i.InvoiceStatusId == pagedDto.InvoiceStatusId.Value)) && (string.IsNullOrWhiteSpace(search) || (!string.IsNullOrWhiteSpace(search)) &&
                                                             (i.Serial.ToLower().Contains(search) ||
                                                             i.InvoiceDetails.FirstOrDefault().VersionName.Contains(search)))

                                       && i.CustomerSubscription.CustomerId == currentUserId &&
                                       i.InvoiceStatusId != (int)InvoiceStatusEnum.WaitingPaymentConfirmation &&
                                       i.InvoiceStatusId != (int)InvoiceStatusEnum.Draft,


                sortDirection: pagedDto.SortingDirection, /*nameof(SortingDirection.DESC)*/
                disableFilter: true);


            //TODO:Add in Resolver
            invoices.Items.ToList().ForEach(x => x.Discriminator = _invoiceHelper.GetInvoiceDiscriminator(x));
            //if (pagedDto.SortBy != null)
            //{
            //    var orderedQuery = GetOrderExpressionQuerabl<RetrieveInvoiceDto>(pagedDto.SortBy, Enum.Parse<SortingDirection>(pagedDto.SortingDirection))(invoices.Items.AsQueryable());
            //    var orderedItems = orderedQuery.ToList(); // or any other materialization method
            //    invoices.Items = orderedItems;
            //}

            return response.CreateResponse(invoices);
        }

        public async Task<IResponse<RetrieveInvoiceDto>> GetInvoiceById(GetInvoiceInputDto inputDto)
        {
            var response = new Response<RetrieveInvoiceDto>();
            var invoice = await _invoiceRepository.GetByIdAsync(inputDto.Id);


            var mappedResult = await _invoiceHelper.GetMappedInvoiceById(inputDto.Id);

            return response.CreateResponse(mappedResult);
        }
        #region Old
        //public async Task<IResponse<GetPayAndSubscribeOutputDto>> PayAndSubscribe( APIPayAndSubscribeInputDto inputDto )
        //{
        //    var output = new Response<GetPayAndSubscribeOutputDto>();
        //    try
        //    {
        //        //Input Validation
        //        //Add Validators
        //        //Business Validation
        //        //TODO:check if there is invoices to roll back subscription and invoice if it failed

        //        bool isFirstInvoice = inputDto.CustomerSubscriptionId == 0 /*&& inputDto.VersionSubscriptionId == 0 && inputDto.AddOnSubscriptionId == 0*/;
        //        //check if there is a subscription already exist to
        //        //-qualify customer
        //        //-don't perform discount
        //        bool isFirstSubscription = _invoiceHelper.IsFirstCustomerSubscription(inputDto.CustomerId,
        //          inputDto.IsAddOn ? inputDto.AddOnId.GetValueOrDefault(0) : inputDto.VersionReleaseId, inputDto.PriceLevelId, inputDto.IsAddOn);
        //        //Business 
        //        #region CustomerSubscription , VersionSubscriptions and AddOnSubscriptions

        //        //validate version Price for Admin
        //        VersionRelease versionRelease = null;
        //        VersionPrice versionPrice = null;
        //        //TODO:Refactor this to get current versionSubscription Id not deducted one because default versionreleaseid may be change

        //        if (inputDto.IsAddOn && inputDto.VersionSubscriptionId == 0)
        //        {
        //            ////TODO:use new Created InputDto VersionReleaseId
        //            // versionRelease = _versionBLL.GetVersionReleaseById(inputDto.VersionReleaseId);
        //            versionRelease = _versionBLL.GetCurrentVersionReleaseOrNull(inputDto.VersionId.GetValueOrDefault(0), isFirstInvoice);

        //            versionPrice = isFirstInvoice
        //                            ? versionRelease.Version.VersionPrices.FirstOrDefault(x => x.PriceLevelId == inputDto.PriceLevelId)
        //                            : _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.PriceLevelId);
        //            if (versionPrice == null)
        //                return output.CreateResponse(MessageCodes.NotFound, nameof(versionPrice));


        //        }
        //        #region CustomerSubscription

        //        //TODO:Refactor
        //        //create CustomerSubscription if not exists
        //        CustomerSubscription customerSubscription = null;
        //        if (inputDto.CustomerSubscriptionId == 0)
        //        {
        //            //Todo Check
        //            if (inputDto.IsAdminInvoice)
        //            {
        //                if (inputDto.InvoiceTypeId == (int)InvoiceTypes.ForeverSubscription)
        //                    inputDto.SubscriptionTypeId = (int)SubscriptionTypeEnum.Forever;
        //                else
        //                    inputDto.SubscriptionTypeId = (int)SubscriptionTypeEnum.Others;
        //            }
        //            customerSubscription = _customerSubscriptionRepository.Add(_mapper.Map<CustomerSubscription>(inputDto));
        //            _unitOfWork.Commit();
        //        }
        //        else
        //        {
        //            customerSubscription = _customerSubscriptionRepository.GetById(inputDto.CustomerSubscriptionId);

        //        }
        //        //assign customersubscriptionId to 
        //        inputDto.CustomerSubscriptionId = customerSubscription.Id;
        //        #endregion
        //        #region VersionSubscription
        //        //create version or addon subscription if not exists

        //        //add version subscription
        //        VersionSubscription versionSubscription = null;
        //        if (inputDto.VersionSubscriptionId == 0)
        //        {
        //            if (inputDto.IsAddOn)
        //            {
        //                versionSubscription = _versionSubscriptionRepository.Where(x =>
        //                x.VersionReleaseId == versionRelease.Id &&
        //               x.CustomerSubscription.CustomerId == inputDto.CustomerId &&
        //               !x.CustomerSubscription.IsAddOn &&
        //               x.VersionPriceId == versionPrice.Id)
        //                    .FirstOrDefault();
        //                if (versionSubscription == null)
        //                {
        //                    return output.CreateResponse(MessageCodes.MustPurchaseVersionFirst, nameof(Version));
        //                }
        //            }
        //            else
        //            {
        //                versionSubscription = _versionSubscriptionRepository.Add(_mapper.Map<VersionSubscription>(inputDto));

        //            }
        //        }
        //        else
        //        {
        //            versionSubscription = _versionSubscriptionRepository.GetById(inputDto.VersionSubscriptionId.GetValueOrDefault(0));

        //        }
        //        _unitOfWork.Commit();

        //        //assign version subscriptionId to 
        //        inputDto.VersionSubscriptionId = versionSubscription.Id;

        //        #endregion
        //        #region AddOnSubscription
        //        AddonSubscription addonSubscription = null;

        //        if (inputDto.IsAddOn)
        //        {

        //            //get related version Subscription
        //            //add version subscription
        //            if (inputDto.AddOnSubscriptionId == 0 || inputDto.AddOnSubscriptionId == null)
        //            {
        //                addonSubscription = _addonSubscriptionRepository.Add(_mapper.Map<AddonSubscription>(inputDto));
        //            }
        //            else
        //            {
        //                addonSubscription = _addonSubscriptionRepository.GetById(inputDto.AddOnSubscriptionId.GetValueOrDefault(0));

        //            }
        //            _unitOfWork.Commit();

        //            //assign addon subscriptionId to 
        //            inputDto.AddOnSubscriptionId = addonSubscription.Id;
        //        }
        //        #endregion


        //        #endregion
        //        #region Contract
        //        var contract = await _invoiceHelper.CreateOrGetCustomerContractAsync(inputDto.CustomerId, isFirstSubscription, false);

        //        #endregion
        //        #region Invoice & InvoiceDetails & Payments
        //        //get customer data
        //        inputDto.Customer = _mapper.Map<GetCustomerOutputDto>(_customerRepository.GetById(inputDto.CustomerId));

        //        //create invoice && invoice details
        //        Invoice invoice = null;
        //        InvoiceDto invoiceDto = null;
        //        if (inputDto.InvoiceId == 0)
        //        {

        //            invoiceDto = _mapper.Map<InvoiceDto>(inputDto);
        //            invoice = /*_invoiceRepository.Add(*/_mapper.Map<Invoice>(invoiceDto)/*)*/;

        //            if (inputDto.IsAdminInvoice)
        //            {
        //                var productName = !inputDto.IsAddOn ? versionSubscription.VersionName : addonSubscription.AddonName;
        //                invoice = MapedAdminInvoice(inputDto, isFirstSubscription, productName, invoice);
        //                #region Old
        //                //var taxPercentage = tax?.Percentage ?? 0;
        //                //var taxPriceIncludeTax = tax?.PriceIncludeTax ?? false;
        //                //invoice.InvoiceDetails.ToList().ForEach(x =>
        //                //{
        //                //    x.VersionName = !inputDto.IsAddOn ? versionSubscription.VersionName : addonSubscription.AddonName;
        //                //    decimal vatAmount = (inputDto.Price * (taxPercentage / 100));
        //                //    //TODO:validate against first and may add in mapping
        //                //    x.Discount = 0M;
        //                //    x.VatAmount = vatAmount;
        //                //    x.NetPrice = inputDto.Price - (taxPriceIncludeTax ? vatAmount : 0);
        //                //    x.Price = inputDto.Price;
        //                //    x.DiscountPercentage = 0M;
        //                //});
        //                //invoice.VatPercentage = taxPercentage;
        //                //invoice.TotalVatAmount = invoice.InvoiceDetails.Sum(x => x.VatAmount.GetValueOrDefault(0));
        //                //invoice.SubTotal = invoice.InvoiceDetails.Sum(x => x.NetPrice);
        //                //invoice.Total = invoice.SubTotal + invoice.TotalVatAmount;
        //                #endregion
        //            };

        //            int currencyId = (inputDto.IsAddOn ? addonSubscription?.AddonPrice?.CountryCurrency?.CurrencyId :
        //            versionSubscription?.VersionPrice?.CountryCurrency?.CurrencyId) ?? customerSubscription.Customer.Country.CountryCurrency.CurrencyId;

        //            MapedInvoiceData(inputDto, invoice, currencyId);

        //            _invoiceRepository.Add(invoice);
        //            _unitOfWork.Commit();

        //            if (inputDto.IsAdminInvoice && !string.IsNullOrEmpty(inputDto.InvoiceSerial))
        //                invoice.Serial = inputDto.InvoiceSerial;
        //            else
        //                _invoiceHelper.UpdateInvoiceSerial(invoice, contract, true);


        //            _unitOfWork.Commit();




        //            //assign invoice to 
        //            inputDto.InvoiceId = invoice.Id;

        //        }
        //        else
        //        {
        //            invoice = _invoiceRepository.GetById(inputDto.InvoiceId);
        //            invoiceDto = _mapper.Map<InvoiceDto>(invoice);

        //        }
        //        //qualify customer
        //        //return data;

        //        //get payment type
        //        inputDto.PaymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);

        //        if (!inputDto.IsUnPaid)
        //        {
        //            var paymentDto = _mapper.Map<ThirdPartyInvoiceInputDto>(inputDto);
        //            //paymentDto.CurrencyCode = customerSubscription.Customer.Country?.CountryCurrency?.Currency?.Code;
        //            paymentDto.CurrencyCode = customerSubscription.CurrencyName;
        //            invoiceDto = _mapper.Map<InvoiceDto>(inputDto);

        //            //pay invoice
        //            var paymentResult = await _paymentBLL.Pay(paymentDto, invoiceDto);
        //            invoice.PaymentMethodId = paymentDto.PaymentMethodId;
        //            //RollBack Subscription if payment failed
        //            if (isFirstInvoice && !paymentResult.IsSuccess)
        //            {
        //                RollBackSubscription(invoice.CustomerSubscriptionId);
        //                return output.CreateResponse(MessageCodes.PaymentFailed);
        //            }
        //            else
        //            {
        //                //Update Invoice Serial & Payment Info
        //                inputDto.PaymentInfo = paymentResult.Serialize();
        //                _invoiceHelper.UpdateInvoicePaymentInfoForSearch(invoice, paymentResult, true);

        //            }
        //        }


        //        //  UpdateInvoiceSerial(invoice, contract, false);
        //        //_unitOfWork.Commit();
        //        #endregion

        //        GetPayAndSubscribeOutputDto result = _mapper.Map<GetPayAndSubscribeOutputDto>(inputDto);
        //        result.PaymentInfo = invoice.PaymentInfo;

        //        return output.CreateResponse(result);


        //    }
        //    catch (Exception e)
        //    {
        //        RollBackSubscription(inputDto.CustomerSubscriptionId);
        //        return output.CreateResponse(e);
        //    }
        //}
        #endregion
        public async Task<IResponse<GetPayAndSubscribeOutputDto>> PayAndSubscribe(APIPayAndSubscribeInputDto inputDto)
        {
            var output = new Response<GetPayAndSubscribeOutputDto>();
            try
            {
                #region Check for First Invoice and First Subscription
                //Input Validation
                //Add Validators
                //Business Validation
                //TODO:check if there is invoices to roll back subscription and invoice if it failed

                bool isFirstInvoice = inputDto.CustomerSubscriptionId == 0 /*&& inputDto.VersionSubscriptionId == 0 && inputDto.AddOnSubscriptionId == 0*/;
                //check if there is a subscription already exist to
                //-qualify customer
                //-don't perform discount
                bool isFirstSubscription = _invoiceHelper.IsFirstCustomerSubscription(inputDto.CustomerId,
                  inputDto.IsAddOn ? inputDto.AddOnId.GetValueOrDefault(0) : inputDto.VersionReleaseId, inputDto.PriceLevelId, inputDto.IsAddOn);
                #endregion
                //Business 
                #region CustomerSubscription , VersionSubscriptions and AddOnSubscriptions


                #region CustomerSubscription

                //TODO:Refactor
                CustomerSubscription customerSubscription = _invoiceHelper.CreateOrGetCustomerSubscription(inputDto);
                #endregion
                #region VersionSubscription
                //create version or addon subscription if not exists
                var vsResult = _invoiceHelper.CreateOrGetVersionSubscription(inputDto, customerSubscription);
                if (!vsResult.IsSuccess)
                    return output.AppendErrors(vsResult.Errors).CreateResponse();

                VersionSubscription versionSubscription = vsResult.IsSuccess ? vsResult.Data : null;

                #endregion
                #region AddOnSubscription
                AddonSubscription addonSubscription = _invoiceHelper.CreateOrGetAddOnSubscription(inputDto, customerSubscription, versionSubscription);

                #endregion


                #endregion
                #region Contract
                var contract = await _invoiceHelper.CreateOrGetCustomerContractAsync(inputDto.CustomerId, false);

                #endregion
                #region Invoice & InvoiceDetails & Payments
                //get customer data
                inputDto.Customer = _mapper.Map<GetCustomerOutputDto>(_customerRepository.GetById(inputDto.CustomerId));

                //create invoice && invoice details
                Invoice invoice = await _invoiceHelper.CreateOrGetInvoiceAsync(inputDto, contract, customerSubscription, versionSubscription, addonSubscription, commit: true);

                //qualify customer
                //return data;

                var pResult = await _paymentBLL.TryToPay(inputDto, customerSubscription, invoice, false);
                if (!pResult.IsSuccess)
                    return output.AppendErrors(vsResult.Errors).CreateResponse();



                #endregion
                #region OutputDate with Update InvoiceSerial
                GetPayAndSubscribeOutputDto result = _mapper.Map<GetPayAndSubscribeOutputDto>(inputDto);
                result.PaymentInfo = invoice.PaymentInfo = inputDto.PaymentInfo = pResult.IsSuccess ? pResult.Data : String.Empty;
                await _invoiceHelper.UpdateInvoiceSerialAsync(invoice, contract: contract, commit: false);
                customerSubscription.Customer.TaxRegistrationNumber = inputDto.VatId;
                _unitOfWork.Commit();
                #endregion
                return output.CreateResponse(result);


            }
            catch (Exception e)
            {
                _invoiceHelper.RollBackSubscription(inputDto.CustomerSubscriptionId);
                return output.CreateResponse(e);
            }
        }

        public async Task<IResponse<APIPayAndSubscribeInputDto>> GetVersionPaymentDetails(PaymentDetailsInputDto inputDto)
        {
            var output = new Response<APIPayAndSubscribeInputDto>();

            try
            {
                //Business Validation
                //Validate against Version Exists

                var version = _versionBLL.GetVersionDataForBuyNow(inputDto);
                if (version == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Validate against Version Release Exists for customer
                var versionRelease = _versionBLL.GetCurrentVersionReleaseOrNull(inputDto.VersionId);
                if (versionRelease == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionRelease));
                //TODO:use new Created InputDto VersionReleaseId
                //var versionRelease = _versionBLL.GetVersionReleaseById(inputDto.VersionReleaseId);
                //if (versionRelease == null)
                //    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionRelease));

                //TODO:Validate against addon is related to version


                ////validate against first subscription (Forever,other)
                //bool isFirstSubscription = IsFirstCustomerSubscription(inputDto.CustomerId,
                //  inputDto.IsAddOn ? inputDto.AddOnId.GetValueOrDefault(0) : versionRelease.Id, inputDto.PriceLevelId, inputDto.IsAddOn);
                bool isFirstSubscription = _invoiceHelper.IsFirstCustomerSubscription(inputDto.VersionSubscriptionId, inputDto.AddOnSubscriptionId, inputDto.IsAddOn, inputDto.IsProductInvoice);

                var package = await _priceLevelRepository.GetByIdAsync(inputDto.PriceLevelId);

                AddOnForBuyNowOutputDto addOn = null;
                AddOnPrice addOnPrice = null;
                VersionPrice versionPrice = null;
                var defaultCountyCurrency = _countryCurrencyRepository.GetAll().FirstOrDefault(c => c.DefaultForOther);
                if (inputDto.IsAddOn)
                {


                    addOn = _addOnBLL.GetAddOnDataForBuyNow(inputDto.AddOnId.GetValueOrDefault(0), inputDto.PriceLevelId, inputDto.VersionId);
                    addOnPrice = package.AddOnPrices.Where(x => x.PriceLevelId == inputDto.PriceLevelId &&
                                                                        x.AddOnId == addOn?.AddOnId &&
                                                                       (x.CountryCurrency?.CountryId == inputDto.CountryId ||
                                                                       (x.CountryCurrency?.CountryId == null && x.CountryCurrencyId == defaultCountyCurrency.Id)))
                                                     .FirstOrDefault();

                    if (addOnPrice == null)
                        return output.CreateResponse(MessageCodes.NotAvailableInYourCountry, "Puchasing");


                }
                //else
                //{
                versionPrice = package.VersionPrices
                                                    .Where(x => x.PriceLevelId == inputDto.PriceLevelId &&
                                                                x.VersionId == version.VersionId &&
                                                                (x.CountryCurrency?.CountryId == inputDto.CountryId ||
                                                                (x.CountryCurrency?.CountryId == null && x.CountryCurrencyId == defaultCountyCurrency.Id)))
                                                     .FirstOrDefault();

                if (versionPrice == null)
                    return output.CreateResponse(MessageCodes.NotAvailableInYourCountry, "Puchasing");
                //}

                var tax = _taxBLL.GetDefaultTaxForCountry(inputDto.CountryId);
                //var currency = _countryBLL.GetByCountryIdOrDefaultAsync(inputDto.CountryId).GetAwaiter().GetResult().Data;

                #region OrderDetails
                //TODO:add in appsettings
                var renewEvery = _invoiceHelper.GetInvoiceRenewEvery(inputDto.Discriminator);
                var invoiceTypeId = _invoiceHelper.GetInvoiceType(inputDto.Discriminator, isFirstSubscription);
                //var invoiceTotal = inputDto.IsAddOn
                //    ? GetAddOnPrice(addOnPrice, tax, inputDto.Discriminator, isFirstSubscription)
                //    : GetVersionPrice(versionPrice, tax, inputDto.Discriminator, isFirstSubscription);

                var invoiceTotal = inputDto.IsAddOn
                     ? _invoiceHelper.CalculateInvoice(addOnPrice, tax, inputDto.Discriminator, isFirstSubscription)
                     : _invoiceHelper.CalculateInvoice(versionPrice, tax, inputDto.Discriminator, isFirstSubscription);

                var versionTitle = _invoiceHelper.GetConcatenatedJsonString("-", version.ApplicationTitle, version.VersionTitle);
                #endregion





                #region CalculateAddonSubscriptionInCasenotpaymentCompleted
                if (inputDto.VersionSubscriptionId != 0)
                {
                    var versionSubscription = _versionSubscriptionRepository.GetById(inputDto.VersionSubscriptionId);
                    if (versionSubscription.AddonSubscriptions.Any(ad =>
                                    !ad.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)) && inputDto.IsAddOn)
                    {

                        var addonSubscription = versionSubscription.AddonSubscriptions.FirstOrDefault(ad => ad.AddonPrice.AddOnId == inputDto.AddOnId);
                        if (addonSubscription != null)
                        {
                            var deletedAddonSubscriptions = versionSubscription.AddonSubscriptions.Where(a =>
                                a.Id != addonSubscription.Id &&
                                a.AddonPrice.AddOnId == inputDto.AddOnId);
                            inputDto.AddOnSubscriptionId = addonSubscription?.Id;
                            inputDto.CustomerSubscriptionId = addonSubscription.CustomerSubscriptionId;
                            _addonSubscriptionRepository.DeleteRange(deletedAddonSubscriptions);
                            _customerSubscriptionRepository.DeleteRange(
                                deletedAddonSubscriptions.Select(c => c.CustomerSubscription));
                            _invoiceDetailRepository.DeleteRange(versionSubscription.AddonSubscriptions
                                .Where(ad => ad.AddonPrice.AddOnId == inputDto.AddOnId)
                                .SelectMany(a => a.CustomerSubscription.Invoices).SelectMany(i => i.InvoiceDetails));
                            _invoiceRepository.DeleteRange(versionSubscription.AddonSubscriptions
                                .Where(ad => ad.AddonPrice.AddOnId == inputDto.AddOnId)
                                .SelectMany(a => a.CustomerSubscription.Invoices));
                            _unitOfWork.Commit();
                        }
                    }
                }
                #endregion






                var data = new APIPayAndSubscribeInputDto
                {
                    PriceLevelId = inputDto.PriceLevelId,
                    PriceLevelName = package.Name,
                    ApplicationTitle = inputDto.AddOnId > 0 ? addOn.AddOnTitle : version.ApplicationTitle,
                    Image = inputDto.AddOnId > 0 ? _mapper.Map<FileStorageDto>(addOn.AddOnImage) : _mapper.Map<FileStorageDto>(version.VersionImage),
                    VersionPrice = inputDto.IsAddOn ? addOn.AddOnPrice : version.VersionPrice,


                    VersionId = version.VersionId,
                    VersionTitle = versionTitle,
                    VersionPriceId = versionPrice?.Id,
                    VersionReleaseId = version.VersionRelease != null ? version.VersionRelease.Id : 0,
                    NumberOfLicenses = package.NumberOfLicenses,
                    CustomerId = inputDto.CustomerId,
                    CurrencyName = _invoiceHelper.GetCurrencyCode(versionPrice?.CountryCurrency?.Currency?.Code), //TODO:Check
                    Url = version.VersionRelease != null ? version.VersionRelease.DownloadUrl : null,


                    InvoiceId = 0,
                    InvoiceStatusId = 0,
                    PaymentMethodId = 0,
                    Details = invoiceTotal.Details,

                    VatPercentage = invoiceTotal.VatPercentage,
                    TotalVatAmount = invoiceTotal.TotalVatAmount,
                    Price = invoiceTotal.Price,
                    SubTotal = invoiceTotal.SubTotal,
                    PriceAfterDiscount = invoiceTotal.PriceAfterDiscount,
                    TotalDiscountAmount = invoiceTotal.TotalDiscountAmount,
                    Total = invoiceTotal.Total,

                    RenewEvery = renewEvery,

                    //Recheck
                    SubscriptionTypeId = version.SubscriptionTypeId,
                    InvoiceTypeId = invoiceTypeId,
                    AddOnId = inputDto.IsAddOn ? addOn.AddOnId : 0,
                    AddonTitle = inputDto.IsAddOn ? _invoiceHelper.GetConcatenatedJsonString("-", versionTitle, addOn.AddOnTitle) : "",
                    AddonPriceId = inputDto.IsAddOn ? addOnPrice.Id : 0,
                    CustomerSubscriptionId = inputDto.IsAddOn ? inputDto.CustomerSubscriptionId : 0,
                    AddOnSubscriptionId = inputDto.IsAddOn ? inputDto.AddOnSubscriptionId : 0,
                    VersionSubscriptionId = inputDto.IsAddOn ? inputDto.VersionSubscriptionId : 0,
                    ApplicationId = version.ApplicationId,
                    InvoiceSerial = inputDto.InvoiceSerial

                };

                return output.CreateResponse(data);
            }
            catch (Exception ex)
            {
                output.CreateResponse(ex);
                return output;
            }
        }

        //when customer pay auto generated invoice
        public async Task<IResponse<APIPayAndSubscribeInputDto>> GetInvoicePaymentDetails(InvoicePaymentDetailsInputDto inputDto)
        {
            bool isFirstSubscription = false;


            var output = new Response<APIPayAndSubscribeInputDto>();
            var invoice = _invoiceRepository.GetById(inputDto.InvoiceId);
            var invoiceDto = _mapper.Map<RetrieveInvoiceDto>(invoice);
            if (invoiceDto == null)
                return output.CreateResponse(MessageCodes.FailedToFetchData);


            if (invoiceDto != null && Convert.ToInt32(invoiceDto.InvoiceStatusId) != (int)InvoiceStatusEnum.Unpaid)
                return output.CreateResponse(MessageCodes.InvalidInvoiceStatus, nameof(Invoice));

            inputDto.Discriminator = _invoiceHelper.GetInvoiceDiscriminator(invoiceDto);

            var customerSubscription = _customerSubscriptionRepository.GetById(invoiceDto.CustomerSubscription.Id);

            var addOnSubscription = customerSubscription.IsAddOn ? customerSubscription.AddonSubscriptions.FirstOrDefault() : null;
            var versionSubscription = customerSubscription.IsAddOn ? addOnSubscription.VersionSubscription : customerSubscription.VersionSubscriptions.FirstOrDefault();

            #region OrderDetails
            //var renewEvery = GetInvoiceRenewEvery(inputDto.Discriminator);
            //var invoiceTypeId = GetInvoiceType(inputDto.Discriminator, isFirstSubscription);
            var invoiceTotal = _invoiceHelper.GetInvoiceSummary(invoice);//     GetVersionPrice(invoice);
            #endregion
            //disable inactive filter when trying to pay old verion 
            var versionRelease = _versionReleaseRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == versionSubscription.VersionReleaseId);
            var versionPrice = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == versionSubscription.VersionPriceId);
            var version = versionRelease?.Version;

            var addOnPrice = addOnSubscription != null ? _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == addOnSubscription.AddonPriceId) : null;
            var addOn = addOnPrice?.AddOn;

            var data = new APIPayAndSubscribeInputDto
            {
                Image = !customerSubscription.IsAddOn
                    ? _mapper.Map<FileStorageDto>(version?.Image)
                    : _mapper.Map<FileStorageDto>(addOn?.Logo),
                VersionId = version?.Id ?? 0,
                VersionTitle = version?.Title,
                VersionPriceId = versionPrice?.Id ?? 0,
                VersionReleaseId = versionRelease?.Id ?? 0,
                NumberOfLicenses = customerSubscription.NumberOfLicenses,
                CustomerId = customerSubscription.CustomerId,
                CurrencyName = _invoiceHelper.GetCurrencyCode(versionPrice?.CountryCurrency?.Currency?.Code), //TODO:Check
                InvoiceId = inputDto.InvoiceId,
                InvoiceStatusId = Convert.ToInt32(invoiceDto.InvoiceStatusId),
                PaymentMethodId = 0,
                Details = invoiceTotal.Details,
                VatPercentage = invoiceDto.VatPercentage,// invoiceTotal.VatPercentage,
                TotalVatAmount = invoiceDto.TotalVatAmount,// invoiceTotal.TotalVatAmount,
                Price = invoiceTotal.Price,
                SubTotal = invoiceTotal.SubTotal,
                PriceAfterDiscount = invoiceTotal.PriceAfterDiscount,
                TotalDiscountAmount = invoiceTotal.TotalDiscountAmount,
                Total = invoiceTotal.Total,
                RenewEvery = customerSubscription.RenewEvery,
                //Recheck
                SubscriptionTypeId = version.Application?.SubscriptionTypeId ?? 0,//TODO:Check
                InvoiceTypeId = invoiceDto.InvoiceTypeId,
                AddOnId = addOnPrice?.AddOnId,
                AddonTitle = addOn?.Title,
                AddonPriceId = customerSubscription.IsAddOn ? addOnSubscription.AddonPriceId : 0,
                CustomerSubscriptionId = customerSubscription.Id,
                AddOnSubscriptionId = addOnSubscription?.Id ?? 0,
                VersionSubscriptionId = versionSubscription?.Id ?? 0,
                AutoBill = customerSubscription.AutoBill,
                PriceLevelName = versionPrice?.PriceLevel?.Name,//TODO:Check
                ApplicationTitle = versionPrice?.Version?.Application?.Title,//TODO:Check
                //Address = invoice.Company?.Address,
                //TaxReg = invoice.Company?.TaxReg,
                Serial = invoiceDto.Serial,
                StartDate = invoiceDto.StartDate ?? DateTime.UtcNow,
                EndDate = invoiceDto.EndDate ?? DateTime.UtcNow,
                PaymentInfo = invoiceDto.PaymentInfo?.Serialize(),
                ApplicationId = version?.ApplicationId ?? 0
            };

            return output.CreateResponse(data);

        }



        public async Task<RefundPaymentResponseDto> RefundInvoiceAsync(Invoice invoice)
        {
            var response = new RefundPaymentResponseDto();

            // calling gateway for refunding.
            try
            {
                return invoice.PaymentMethod.PaymentTypeId switch
                {

                    (int)PaymentTypesEnum.PayPal => await _paymentBLL.RefundPayPal(invoice),
                    (int)PaymentTypesEnum.Fawry => await _paymentBLL.RefundFawry(invoice),
                    (int)PaymentTypesEnum.Bank => await _paymentBLL.RefundBank(invoice),
                    (int)PaymentTypesEnum.PayMob => await _paymentBLL.RefundFawry(invoice),
                    _ => throw new ArgumentOutOfRangeException(nameof(invoice.PaymentMethodId), invoice.PaymentMethodId, null)
                };
            }
            catch (Exception ex)
            {
                response.Result = ex.ToString();
                return response;
            }
        }





        #region Auto renew.

        // Todo: update default payment method to get it by customer.


        public async Task<Invoice> CreateInvoiceAsync(NewInvoiceDto oldInvoice)
        {
            try
            {
                bool isAddOn = oldInvoice.AddonSubscriptionId.HasValue && oldInvoice.AddonSubscriptionId.Value > 0;
                AddonSubscription addOnSubscription = isAddOn ? _addonSubscriptionRepository.GetById(oldInvoice.AddonSubscriptionId.Value) : null;
                VersionSubscription versionSubscription = isAddOn ? addOnSubscription.VersionSubscription : _versionSubscriptionRepository.GetById(oldInvoice.VersionSubscriptionId);
                var customer = isAddOn ? addOnSubscription.CustomerSubscription.Customer : versionSubscription.CustomerSubscription.Customer;
                var renewEvery = isAddOn ? addOnSubscription.CustomerSubscription.RenewEvery : versionSubscription.CustomerSubscription.RenewEvery;
                var contract = await _invoiceHelper.CreateOrGetCustomerContractAsync(customer.Id, false);// _customerContract.GetById(customer.Id);
                var tax = _taxBLL.GetDefaultTaxForCountry(oldInvoice.CountryId);

                var invoiceDetails = _mapper.Map<List<InvoiceDetail>>(_invoiceHelper.CalculateInvoiceDetailsFromPreviousInvoice(oldInvoice.OldInvoice, tax).ToList());
                //TODO:we need to add PriceIncludeTax n Subscription
                decimal totalDiscountAmount = invoiceDetails.Sum(x => x.Discount);
                decimal totalVatAmount = invoiceDetails.Sum(x => x.VatAmount.GetValueOrDefault(0));
                decimal vatPercentage = tax?.Percentage ?? 0; // or from old invoice
                decimal subTotal = invoiceDetails.Sum(x => x.Price);//- (tax is not null ? (!tax.PriceIncludeTax ? totalVatAmount : 0) : 0);


                var invoice = _invoiceRepository.Add(new Invoice
                {
                    PaymentMethodId = oldInvoice.PaymentMethodId,
                    CustomerSubscriptionId = _invoiceHelper.GetSubscriptionId(versionSubscription, addOnSubscription, isAddOn),
                    //CustomerSubscriptionId =  isAddOn ? addOnSubscription.CustomerSubscriptionId : versionSubscription.CustomerSubscriptionId,
                    InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid,
                    Serial = _invoiceHelper.GenerateInvoiceSerial(),
                    TaxReg = customer.TaxRegistrationNumber,
                    Address = customer.FullAddress,
                    InvoiceTypeId = _invoiceHelper.GetInvoiceTypeFromPreviousInvoice(oldInvoice.InvoiceTypeId),
                    //CreateDate = DateTime.UtcNow,
                    StartDate = oldInvoice.StartDate,// DateTime.UtcNow,
                    EndDate = oldInvoice.EndDate,//DateTime.UtcNow.AddDays(renewEvery),
                    InvoiceDetails = invoiceDetails,
                    TotalDiscountAmount = totalDiscountAmount,
                    TotalVatAmount = totalVatAmount,
                    VatPercentage = tax?.Percentage ?? 0, // or from old invoice
                    SubTotal = subTotal,
                    Total = subTotal + totalVatAmount,
                    CurrencyId = _invoiceHelper.GetSubscriptionCurrency(versionSubscription, addOnSubscription, isAddOn),
                    PaymentDate = oldInvoice.StartDate
                    //CurrencyId = isAddOn ? addOnSubscription?.AddonPrice?.CountryCurrency?.CurrencyId :
                    //   versionSubscription?.VersionPrice?.CountryCurrency?.CurrencyId
                });

                //TODO:Refactor
                //var contract = _customerContract.GetById(customer.Id);
                //var invoice = CalculateInvoiceFromPreviousInvoice(oldInvoice.OldInvoice);
                //_invoiceRepository.Add(invoice);
                _unitOfWork.Commit();

                //update invoice serial
                await _invoiceHelper.UpdateInvoiceSerialAsync(invoice, contract, true);
                return invoice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IResponse<int>> CreateUnPaidInvoice(PaymentDetailsInputDto inputDto)
        {
            //GetPayAndSubscribeOutputDto
            var output = new Response<int>();
            var paymentDetails = GetVersionPaymentDetails(inputDto).GetAwaiter().GetResult().Data;

            try
            {
                if (paymentDetails != null)
                {
                    paymentDetails.PaymentMethodId = await _paymentBLL.GetPaymentMethodId(inputDto.CustomerId);

                    #region oldInvoiceTypeId
                    //if (inputDto.IsInvoiceSupportAdmin)
                    //{
                    //    paymentDetails.InvoiceTypeId = (int)InvoiceTypes.Support;
                    //    paymentDetails.AddOnSubscriptionId = inputDto.AddOnSubscriptionId;

                    //}
                    //else if (inputDto.IsProductInvoice && inputDto.Discriminator == (int)DiscriminatorsEnum.Forever)
                    //{
                    //    paymentDetails.InvoiceTypeId = (int)InvoiceTypes.ForeverSubscription;
                    //    //paymentDetails.AddOnSubscriptionId = inputDto.AddOnSubscriptionId;

                    //}
                    //else
                    //{
                    //    paymentDetails.InvoiceTypeId = (int)InvoiceTypes.Renewal;
                    //}
                    #endregion
                    _paymentBLL.MapedPaymentData(inputDto, paymentDetails);

                    var result = await PayAndSubscribe(paymentDetails);

                    if (result.Data is null)
                        return output.CreateResponse(0);

                    return output.CreateResponse(result.Data.InvoiceId);

                }
                else
                {
                    return output.CreateResponse(MessageCodes.AlreadyExists, "Failed");
                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        //private async Task<Invoice> CreateUnPaidInvoiceAsync( PaymentDetailsInputDto inputDto )
        //{
        //    try
        //    {
        //        bool isAddOn = oldInvoice.AddonSubscriptionId.HasValue && oldInvoice.AddonSubscriptionId.Value > 0;
        //        AddonSubscription addOnSubscription = isAddOn ? _addonSubscriptionRepository.GetById(oldInvoice.AddonSubscriptionId.Value) : null;
        //        VersionSubscription versionSubscription = isAddOn ? addOnSubscription.VersionSubscription : _versionSubscriptionRepository.GetById(oldInvoice.VersionSubscriptionId);
        //        var customer = isAddOn ? addOnSubscription.CustomerSubscription.Customer : versionSubscription.CustomerSubscription.Customer;
        //        var renewEvery = isAddOn ? addOnSubscription.CustomerSubscription.RenewEvery : versionSubscription.CustomerSubscription.RenewEvery;
        //        var contract = _customerContract.GetById(customer.Id);
        //        var tax = GetDefaultTax(oldInvoice.CountryId);



        //        int invoiceTypeId = GetInvoiceTypeFromPreviousInvoice(oldInvoice.InvoiceTypeId);
        //        var invoiceDetails = _mapper.Map<List<InvoiceDetail>>(GetInvoiceDetailsFromPreviousInvoice(oldInvoice.OldInvoice).ToList());
        //        //TODO:we need to add PriceIncludeTax n Subscription
        //        decimal totalDiscountAmount = invoiceDetails.Sum(x => x.Discount);
        //        decimal totalVatAmount = invoiceDetails.Sum(x => x.VatAmount.GetValueOrDefault(0));
        //        decimal vatPercentage = tax.Percentage; // or from old invoice
        //        decimal subTotal = invoiceDetails.Sum(x => x.Price) - (tax.PriceIncludeTax ? totalVatAmount : 0);
        //        var invoice = _invoiceRepository.Add(new Invoice
        //        {
        //            PaymentMethodId = oldInvoice.PaymentMethodId,
        //            CustomerSubscriptionId = isAddOn ? addOnSubscription.CustomerSubscriptionId : versionSubscription.CustomerSubscriptionId,
        //            InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid,
        //            Serial = "INV-" + Guid.NewGuid().ToString(),
        //            TaxReg = customer.TaxRegistrationNumber,
        //            Address = customer.FullAddress,
        //            InvoiceTypeId = invoiceTypeId,
        //            CreateDate = DateTime.UtcNow,
        //            StartDate = DateTime.UtcNow,
        //            EndDate = DateTime.UtcNow.AddDays(renewEvery),
        //            InvoiceDetails = invoiceDetails,
        //            TotalDiscountAmount = totalDiscountAmount,
        //            TotalVatAmount = totalVatAmount,
        //            VatPercentage = tax.Percentage, // or from old invoice
        //            SubTotal = subTotal,
        //            Total = subTotal + totalVatAmount,
        //        });
        //        _unitOfWork.Commit();

        //        //update invoice serial
        //        UpdateInvoiceSerial(invoice, contract, true);

        //        return invoice;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion


        public async Task<IResponse<IEnumerable<TicketInvoiceLookupDto>>> GetTicketInvoicesLookupAsync(int customerId)
        {
            var response = new Response<IEnumerable<TicketInvoiceLookupDto>>();

            var invoices = await _invoiceRepository.GetManyAsync(i => i.CustomerSubscription.CustomerId == customerId && i.InvoiceStatusId != (int)InvoiceStatusEnum.Draft);

            var invoicesDto = _mapper.Map<IEnumerable<TicketInvoiceLookupDto>>(invoices);

            return response.CreateResponse(invoicesDto);
        }



        public async Task<IResponse<string>> GenerateInvoiceQrCodeAsync(int invoiceId)
        {
            var response = new Response<string>();

            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice is null)
                return response.CreateResponse(MessageCodes.NotFound, nameof(invoiceId));

            var invoiceDto = _mapper.Map<InvoicePdfDto>(invoice);

            var qrBase64 = _pdfGeneratorBLL.GenerateInoviceQRBase64(invoiceDto);

            return response.CreateResponse(qrBase64);
        }

        public bool IsVersionHasNotRefundedAddons(VersionSubscription versionSub)
        {
            if (versionSub is not null)
            {

                if (versionSub.AddonSubscriptions.Any())
                {

                    foreach (var addonSub in versionSub.AddonSubscriptions)
                    {
                        if (addonSub.CustomerSubscription.Invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded).FirstOrDefault() == null)
                            return true;
                    }
                }
            }

            return false;
        }
        #endregion

    }
}
