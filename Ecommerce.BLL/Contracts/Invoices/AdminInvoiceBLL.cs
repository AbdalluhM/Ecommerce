using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Pdfs;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Taxes;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.StaticNumbers;
using Ecommerce.DTO.Taxes;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Invoices.InvoiceValidator;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Customers.CustomerDto;
using Ecommerce.BLL.Notifications;
using Ecommerce.DTO.Notifications;
using Ecommerce.Core.Enums.Json;

using Ecommerce.Core.Enums.Notifications;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Addons;
using Ecommerce.BLL.Employees;
using Ecommerce.Reports;
using Ecommerce.Reports.Templts;
using Ecommerce.Core.Helpers.JsonLanguages;

namespace Ecommerce.BLL.Contracts.Invoices
{
    public class AdminInvoiceBLL : BaseBLL, IAdminInvoiceBLL
    {
        #region Field

        private readonly IMapper _mapper;
        private readonly Numbers _numbers;
        private readonly IVersionBLL _versionBLL;
        private readonly IAddOnBLL _addOnBLL;
        IRepository<Invoice> _invoiceRepository;
        IRepository<InvoiceDetail> _invoiceDetailRepository;
        IRepository<Employee> _employeeRepository;
        IRepository<VersionSubscription> _versionSubscriptionRepository;
        IRepository<AddOn> _addOnRepository;
        IRepository<PriceLevel> _priceLevelRepository;
        IRepository<VersionPrice> _versionPriceRepository;
        IRepository<Core.Entities.Version> _versionRepository;
        IRepository<VersionAddon> _versionAddOnRepository;
        IRepository<AddOnPrice> _addOnPriceRepository;
        IRepository<CustomerSubscription> _customerSubscriptionRepository;
        IRepository<AddonSubscription> _addOnSubscriptionRepository;
        IRepository<Customer> _customerRepository;
        IRepository<InvoiceDetail> _invoiceDetailsRepository;
        IRepository<Notification> _notificationRepository;
        IRepository<RefundRequest> _refundRequestRepository;
        IRepository<Currency> _currencyRepository;
        private readonly IInvoiceHelperBLL _invoiceHelper;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly ITaxBLL _taxBLL;
        private readonly IInvoiceBLL _invoiceBLL;
        private readonly IEmployeeBLL _employeeBLL;
        private readonly IReportManager _reportManger;

        IUnitOfWork _unitOfWork;
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        #endregion

        #region Constructor
        public AdminInvoiceBLL( IMapper mapper,
            IRepository<Invoice> invoiceRepository,
            IRepository<Employee> employeeRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerSubscription> customerSubscriptionRepository,
            IRepository<VersionSubscription> versionSubscriptionRepository,
            IRepository<AddonSubscription> addOnSubscriptionRepository,
            IUnitOfWork unitOfWork,
            IRepository<InvoiceDetail> invoiceDetailsRepository,
            ITaxBLL taxBLL, IRepository<AddOn> addOnRepository,
            IRepository<VersionAddon> versionAddOnRepository,
            IRepository<Core.Entities.Version> versionRepository,
            IInvoiceBLL invoiceBLL,
            IEmployeeBLL employeeBLL,
            IRepository<PriceLevel> priceLevelRepository,
            IRepository<VersionPrice> versionPriceRepository,
            IRepository<AddOnPrice> addOnPriceRepository,
            IRepository<InvoiceDetail> invoiceDetailRepository,
            IRepository<Currency> currencyRepository,
            IInvoiceHelperBLL invoiceHelper ,
            IOptions<Numbers> numbers, IRepository<RefundRequest> refundRequestRepository, IPdfGeneratorBLL pdfGeneratorBLL,
            INotificationDataBLL notificationDataBLL, IRepository<Notification> notificationRepository, IVersionBLL versionBLL, IAddOnBLL addOnBLL, IReportManager reportManger )
            : base(mapper)
        {
            _invoiceRepository = invoiceRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _customerRepository = customerRepository;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _versionSubscriptionRepository = versionSubscriptionRepository;
            _addOnSubscriptionRepository = addOnSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _taxBLL = taxBLL;
            _addOnRepository = addOnRepository;
            _versionAddOnRepository = versionAddOnRepository;
            _versionRepository = versionRepository;
            _invoiceBLL = invoiceBLL;
            _employeeBLL = employeeBLL;
            _priceLevelRepository = priceLevelRepository;
            _versionPriceRepository = versionPriceRepository;
            _addOnPriceRepository = addOnPriceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceHelper = invoiceHelper;
            _numbers = numbers.Value;
            _refundRequestRepository = refundRequestRepository;
            _currencyRepository = currencyRepository;
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _notificationDataBLL = notificationDataBLL;
            _notificationRepository = notificationRepository;
            _versionBLL = versionBLL;
            _addOnBLL = addOnBLL;
            _reportManger = reportManger;
        }
        #endregion

        #region Action
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get All Invoices by invoice type. </summary>
        ///
        /// <param name="pagedDto">          Dto contain all methods for filter . </param>
        ///                                 
        ///
        /// <returns>   A RetrieveInvoiceDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<PagedResultDto<RetrieveInvoiceDto>>> GetAllInvoicesPagedlist( FilterInvoiceByCountry pagedDto )
        {
            //if (string.IsNullOrWhiteSpace(pagedDto.Filter) || pagedDto.Filter == "{}")
            //{
            //    Dictionary<string, FilterValue> dic = new Dictionary<string, FilterValue>();
            //    dic.Add("Createdate", new FilterValue { Operator = FilterBuilder.Operation.GreaterThanOrEquals, Value = DateTime.Today.Date.AddDays(-2).ToShortDateString() });
            //    dic.Add("-R2-Createdate", new FilterValue { Operator = FilterBuilder.Operation.LessThanOrEquals, Value = DateTime.Today.Date.AddDays(-1).ToShortDateString() });
            //    pagedDto.Filter = JsonConvert.SerializeObject(dic);

            //}

           
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(pagedDto.EmployeeId);

            var output = new Response<PagedResultDto<RetrieveInvoiceDto>>();
            var result = GetPagedList<RetrieveInvoiceDto, Invoice, int>(
                pagedDto: pagedDto,
                repository: _invoiceRepository,
                orderExpression: x => x.Id,
                searchExpression: x =>
                employeCountriesIds.Contains(x.CustomerSubscription.Customer.CountryId) &&
                                            !string.IsNullOrEmpty(pagedDto.SearchTerm) ?
                                            (x.InvoiceStatusId == pagedDto.InvoiceStatusId && (x.CustomerSubscription.Customer.Name.Contains(pagedDto.SearchTerm) ||
                                            x.Serial.Contains(pagedDto.SearchTerm) ||
                                            x.CustomerSubscription.Customer.Contract.Serial.ToLower().Contains(pagedDto.SearchTerm.ToLower()))) : true
                                            && x.InvoiceStatusId == pagedDto.InvoiceStatusId,
                sortDirection: pagedDto.SortingDirection
                      //sortDirection: nameof(SortingDirection.DESC)
                      );
            foreach (var invoice in result.Items)
            {
                invoice.Discriminator = _invoiceHelper.GetInvoiceDiscriminator(invoice.InvoiceTypeId, invoice.RenwalEvery);
                invoice.StartDate = invoice.InvoiceTypeId != (int)InvoiceTypes.ForeverSubscription ? DateTime.SpecifyKind(invoice.StartDate.Value, DateTimeKind.Utc) : null;
                invoice.EndDate = invoice.InvoiceTypeId != (int)InvoiceTypes.ForeverSubscription ? DateTime.SpecifyKind(invoice.EndDate.Value, DateTimeKind.Utc) : null;
            }
            return output.CreateResponse(result);
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get invoice by id . </summary>
        ///
        /// <param name="invoiceId"      . </param>
        ///                                 
        ///
        /// <returns>   A RetrieveInvoiceDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<RetrieveInvoiceDto>> GetInvoiceById( int invoiceId )
        {
            var output = new Response<RetrieveInvoiceDto>();
            try
            {
                var invoice = await _invoiceHelper.GetInvoiceById(invoiceId);
                var mappedResult = _mapper.Map<RetrieveInvoiceDto>(invoice);
                mappedResult.Discriminator = _invoiceHelper.GetInvoiceDiscriminator(mappedResult.InvoiceTypeId, mappedResult.RenwalEvery);
                if (mappedResult.IsAddOn)
                {

                    mappedResult.AddOnSubscription = _mapper.Map<GetAddOnSubscriptionOutputDto>(_addOnSubscriptionRepository
                       .Where(x => x.CustomerSubscriptionId == mappedResult.CustomerSubscription.Id).FirstOrDefault());

                    mappedResult.VersionSubscription = _mapper.Map<GetVersionsubscriptionInvoiceOutputDto>(_versionSubscriptionRepository
                   .Where(x => x.Id == mappedResult.AddOnSubscription.VersionSubscriptionId).FirstOrDefault());

                }
                else
                {
                    mappedResult.VersionSubscription = _mapper.Map<GetVersionsubscriptionInvoiceOutputDto>(_versionSubscriptionRepository
                  .Where(x => x.CustomerSubscriptionId == mappedResult.CustomerSubscription.Id).FirstOrDefault());

                    var version = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == mappedResult.VersionSubscription.VersionId).FirstOrDefault();

                    mappedResult.VersionSubscription.PriceLevels = _mapper.Map<List<RetrievePriceLevelDto>>(version.VersionPrices
                        .Where(x => x.PriceLevel != null).Select(x => x.PriceLevel)).ToList();
                }

                return output.CreateResponse(mappedResult);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>create support invoice for customer buy products forever . </summary>
        ///
        /// <param name="inputDto"      . </param>
        /// <param name="CurrentEmployeeId"      . </param>                                
        ///
        /// <returns>   A RetrieveInvoiceDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<RetrieveInvoiceDto>> CreateSupportInvoice( PaymentDetailsInputDto inputDto, int CurrentEmployeeId )
        {
            var output = new Response<RetrieveInvoiceDto>();
            try
            {
                //Input validation 
                var validator = await new CreateInvoiceInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return output.CreateResponse(validator.Errors);

                inputDto.CountryId = (await _customerRepository.GetByIdAsync(inputDto.CustomerId))?.CountryId ?? 0;
                inputDto.IsInvoiceSupportAdmin = true;

                var result = await _invoiceBLL.CreateUnPaidInvoice(inputDto);

                var invoice = (await _invoiceHelper.GetInvoiceById(result.Data));
                if (invoice == null)
                    return output.CreateResponse(MessageCodes.Failed, nameof(Invoice));

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = inputDto.CustomerId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.TechnicalSupport;
                _notificationItem.AdminId = CurrentEmployeeId;
                _notificationItem.InvoiceId = invoice?.Id;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);


                return output.CreateResponse(_mapper.Map<RetrieveInvoiceDto>(invoice));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }

        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Update invoice support or product invoice . </summary>
        ///
        /// <param name="inputDto"      . </param>
        ///
        /// <returns>   A RetrieveInvoiceDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<RetrieveInvoiceDto>> UpdateInvoice( PaymentDetailsInputDto inputDto )
        {
            var output = new Response<RetrieveInvoiceDto>();
            try
            {
                //Input validation
                var validator = await new CreateInvoiceInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return output.CreateResponse(validator.Errors);

                var invoice = await _invoiceHelper.GetInvoiceById(inputDto.InvoiceId);
                if (invoice == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Invoice));
                
                // select only paid versionsubscription
                if(inputDto.VersionSubscriptionId > 0)
                {
                    if (!_invoiceHelper.IsSubscriptionHasAnyPaidInvoice(inputDto.VersionSubscriptionId))
                        return output.CreateResponse(MessageCodes.PaidVersionsubscription);
                }

                inputDto.InvoiceSerial = invoice.Serial; 

                inputDto.CountryId = _customerRepository.Where(x => x.Id == inputDto.CustomerId).FirstOrDefault().CountryId;

                var result = await _invoiceBLL.CreateUnPaidInvoice(inputDto);

                var retreieveInvoice = await _invoiceHelper.GetInvoiceById(result.Data);
                if (retreieveInvoice == null)
                {
                    return output.CreateResponse(MessageCodes.Failed, nameof(Invoice));
                }
                else
                {
                    if (invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Draft)
                    {
                        DeleteOldInvoice(invoice);
                        _unitOfWork.Commit();
                    }
                }

                return output.CreateResponse(_mapper.Map<RetrieveInvoiceDto>(retreieveInvoice));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }

        }

     

        private void DeleteOldInvoice(Invoice invoice)
        {
            var customerSubscription = invoice.CustomerSubscription;
            if (customerSubscription != null)
            {
                var versiionSubscription = customerSubscription.VersionSubscriptions.ToList();
                var addonSubscription = customerSubscription.AddonSubscriptions.ToList();
                if (versiionSubscription.Any())
                {
                    foreach (var item in versiionSubscription)
                    {
                        _versionSubscriptionRepository.Delete(item);
                    }
                }
                if (addonSubscription.Any())
                {
                    foreach (var item in addonSubscription)
                    {

                        _addOnSubscriptionRepository.Delete(item);
                    }
                }
                if (invoice.InvoiceDetails.Any())
                {
                    foreach (var item in invoice.InvoiceDetails)
                    {
                        _invoiceDetailsRepository.Delete(item);
                    }

                }
                if (invoice.Notifications.Any())
                {
                    foreach (var item in invoice.Notifications)
                    {
                        _notificationRepository.Delete(item);
                    }
                }
                _invoiceRepository.Delete(invoice);
                _customerSubscriptionRepository.Delete(invoice.CustomerSubscription);
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>create product invoice . </summary>
        ///
        /// <param name="inputDto"      . </param>
        /// /// <param name="CurrentEmployeeId"      . </param>
        /// <returns>   A RetrieveInvoiceDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<RetrieveInvoiceDto>> CreateInvoice( PaymentDetailsInputDto inputDto, int CurrentEmployeeId )
        {
            var output = new Response<RetrieveInvoiceDto>();
            try
            {
                //Input validation 
                var validator = await new CreateInvoiceInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return output.CreateResponse(validator.Errors);

                inputDto.CountryId = _customerRepository.GetById(inputDto.CustomerId).CountryId;
                inputDto.IsProductInvoice = true;
                var result = await _invoiceBLL.CreateUnPaidInvoice(inputDto);

                var invoice = await _invoiceHelper.GetInvoiceById(result.Data);
                if (invoice == null)
                    return output.CreateResponse(MessageCodes.Failed, nameof(Invoice));

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = inputDto.CustomerId;
                _notificationItem.AdminId = CurrentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.ProductSubscribtion;
                _notificationItem.InvoiceId = invoice.Id;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);

                return output.CreateResponse(_mapper.Map<RetrieveInvoiceDto>(invoice));
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }

        }
        public async Task<IResponse<List<GetInvoicesOutputDto>>> GetAllInvoices( int employeeId )
        {
            var output = new Response<List<GetInvoicesOutputDto>>();

            var employeeCountryIds = _employeeRepository.Where(x => x.Id == employeeId).FirstOrDefault()
               .EmployeeCountries.Select(x => x.CountryCurrencyId).ToList();
            try
            {
                return output.CreateResponse(_mapper.Map<List<GetInvoicesOutputDto>>(_invoiceRepository
                    .Where(x => employeeCountryIds.Contains(x.CustomerSubscription.Customer.Country.CountryCurrency.Id)).ToList()));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<List<GetCustomerInvoiceOutputDto>>> GetAllActiveCustomerAsync( int employeeId )
        {
            var output = new Response<List<GetCustomerInvoiceOutputDto>>();
            try
            {
                //var employeeCountryIds = _employeeRepository.Where(x => x.Id == employeeId)
                //                                            .FirstOrDefault().EmployeeCountries
                //                                            .Select(x => x.CountryCurrencyId)
                //                                            .Distinct()
                //                                            .ToList();
                var employeeCountryIds = await _employeeBLL.GetEmployeeCountries(employeeId);

                var result = await _customerRepository.Where(x => x.CustomerStatusId == (int)CustomerStatusEnum.Registered &&
                                                                  (employeeCountryIds != null && employeeCountryIds.Contains(x.CountryId)))
                                                                  .ToListAsync();

                return output.CreateResponse(_mapper.Map<List<GetCustomerInvoiceOutputDto>>(result));
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get ALL version and addon that customer purshased forever . </summary>
        ///
        /// <param name="inputDto"      . </param>
        ///
        /// <returns>   A GetCustomerVersionOrAddOnOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetCustomerVersionOrAddOnOutputDto>> GetVersionAndAddOnAsync( GetCustomerDefaultTaxInputDto inputDto )
        {
            var output = new Response<GetCustomerVersionOrAddOnOutputDto>();
            try
            {
                var tax = _taxBLL.GetDefaultTaxForCustomer(new GetCountryDefaultTaxByCustomerIdInputDto { CustomerId = inputDto.CustomerId });
                var customer = await _customerRepository.GetAsync(x => x.Id == inputDto.CustomerId
               && x.CustomerStatusId == (int)CustomerStatusEnum.Registered);
                if (customer == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));
                //get customerSubscription
                var customerSubscription = _customerSubscriptionRepository
                                                   .Where(x => customer != null && x.CustomerId == customer.Id && 
                                                    x.Invoices
                                                    .Any(x=>x.InvoiceTypeId != (int)InvoiceTypes.Renewal &&
                                                    x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid))
                                                    .Select(x => x.Id)
                                                    .ToList();
                //get versionSubscription
                var versionSubscription = _versionSubscriptionRepository
                                                              .Where(x => 
                                                               x.CustomerSubscription.Customer.CountryId == customer.CountryId
                                                               && customerSubscription.Contains(x.CustomerSubscriptionId))
                                                              .Distinct()
                                                              .ToList();
                //Get AddOnSubscription 
                var addOnSubscription = _addOnSubscriptionRepository.Where(x =>
                                                               x.CustomerSubscription.Customer.CountryId == customer.CountryId
                                                               && customerSubscription.Contains(x.CustomerSubscriptionId))
                                                               .Distinct()
                                                               .ToList();


                var result = _mapper.Map<GetCustomerVersionOrAddOnOutputDto>(customer);

                result.Version = _mapper.Map<List<GetVersionsubscriptionInvoiceOutputDto>>(versionSubscription);
                foreach (var item in result.Version)
                {
                    var SupportInvocePrice = _invoiceHelper.GetInvoicePrice(item.Price, tax);
                    item.SupportPrice = SupportInvocePrice.SupportPrice;
                    item.VatPrice = SupportInvocePrice.VatPrice;
                    item.NetPrice = SupportInvocePrice.SupportPrice;
                    item.Total = SupportInvocePrice.TotalPrice;

                    var version = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == item.VersionId).FirstOrDefault();
                    item.PriceLevels = _mapper.Map<List<RetrievePriceLevelDto>>(version.VersionPrices
                       .Where(x => x.PriceLevel != null && x.CountryCurrencyId == customer.Country.CountryCurrency.Id)
                       .Select(x => x.PriceLevel)).ToList();

                }
                result.AddOn = _mapper.Map<List<GetAddOnSubscriptionOutputDto>>(addOnSubscription);
                foreach (var item in result.AddOn)
                {
                    var SupportInvocePrice = _invoiceHelper.GetInvoicePrice(item.Price, tax);
                    item.SupportPrice = SupportInvocePrice.SupportPrice;
                    item.VatPrice = SupportInvocePrice.VatPrice;
                    item.NetPrice = SupportInvocePrice.SupportPrice;
                    item.Total = SupportInvocePrice.TotalPrice;
                }
                return output.CreateResponse(result);
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }

        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get ALL version customer purshased  . </summary>
        ///
        /// <param name="inputDto"      . </param>
        ///
        /// <returns>   A GetVersionsubscriptionInvoiceOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<List<GetVersionsubscriptionInvoiceOutputDto>>> GetCutomerVersionsAsync( GetCustomerDefaultTaxInputDto inputDto )
        {
            var output = new Response<List<GetVersionsubscriptionInvoiceOutputDto>>();
            try
            {
                var tax = _taxBLL.GetDefaultTaxForCustomer(new GetCountryDefaultTaxByCustomerIdInputDto { CustomerId = inputDto.CustomerId });
                var customer = await _customerRepository.GetAsync(x => x.Id == inputDto.CustomerId
               && x.CustomerStatusId == (int)CustomerStatusEnum.Registered);
                if (customer == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                //get customerSubscription
                var customerSubscription = _customerSubscriptionRepository
                    .Where(x => customer != null &&
                                           x.CustomerId == customer.Id &&
                                           x.Invoices.Any(x=>x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.RefundRequest.RefundRequestStatusId != (int)RefundRequestStatusEnum.Accepted))
                                          //.Select(x => x.Id)
                                          .ToList();
                
                var versionSubscriptions = customerSubscription.SelectMany(x=>x.VersionSubscriptions).ToList();

                var result = _mapper.Map<List<GetVersionsubscriptionInvoiceOutputDto>>(versionSubscriptions);

                foreach (var item in result)
                {
                    var SupportInvocePrice = _invoiceHelper.GetInvoicePrice(item.Price, tax);
                    item.SupportPrice = SupportInvocePrice.SupportPrice;
                    item.VatPrice = SupportInvocePrice.VatPrice;
                    item.NetPrice = SupportInvocePrice.SupportPrice;
                    item.Total = SupportInvocePrice.TotalPrice;
                }
                return output.CreateResponse(result);
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }

        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get ALL Addon related version customer purshased   . </summary>
        ///
        /// <param name="customerId"      . </param>
        /// <param name="versionSubscriptionId"      . </param>
        /// <returns>   A GetAddOnCanPurshasedOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<List<GetAddOnCanPurshasedOutputDto>>> GetAddOnCanPurshasedAsync( int customerId, int versionSubscriptionId )
        {
            var output = new Response<List<GetAddOnCanPurshasedOutputDto>>();
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);

                var versionPriceLevelId = _versionSubscriptionRepository.GetById(versionSubscriptionId).VersionPrice.PriceLevelId;
                
                //get Versions
                var versionId =  _versionSubscriptionRepository.Where(x => x.Id == versionSubscriptionId )
                                                                         .Select(x => x.VersionPrice.VersionId)
                                                                         .FirstOrDefault();

                //get add related version
                var addOnIds = await _versionAddOnRepository.Where(x => x.VersionId == versionId)
                                                                    .Select(x => x.AddonId)
                                                                    .ToListAsync();

                //get AddOn have price
                var addonIds = await _addOnPriceRepository.Where(x => x.PriceLevelId == versionPriceLevelId && 
                                                                                     x.CountryCurrency.CountryId == customer.CountryId &&
                                                                                     addOnIds.Contains(x.AddOnId))
                                                                                    .Select(x => x.AddOnId)
                                                                                    .ToListAsync();

                var addon = _addOnRepository.Where(x => addonIds.Contains(x.Id)).ToList();


                var result = _mapper.Map<List<GetAddOnCanPurshasedOutputDto>>(addon);

                result.ForEach(x =>
                {
                    x.Price = _addOnBLL.GetMinimumAddonPrice(x.Id, customer.CountryId);
                    x.Pricelevel = _mapper.Map<List<RetrievePriceLevelDto>>(_addOnPriceRepository.Where(a => a.CountryCurrency != null &&
                    a.CountryCurrency.CountryId == customer.CountryId && a.AddOnId == x.Id).Select(x => x.PriceLevel)).ToList();
                });

                return output.CreateResponse(result);
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get ALL version related customer country    . </summary>
        ///
        /// <param name="customerId"      . </param>
        /// <returns>   A GetVersionCanPurshasedOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public IResponse<List<GetVersionCanPurshasedOutputDto>> GetVersionCanPurshased( int customerId )
        {
            var output = new Response<List<GetVersionCanPurshasedOutputDto>>();
            try
            
            {
                var customer = _customerRepository.GetById(customerId);// _customerRepository.Where(x => x.Id == customerId).FirstOrDefault();/
                var countryCurrencyId = customer?.Country?.CountryCurrency?.Id;
                if(countryCurrencyId is not null)
                {
                    
                    var versionIds = _versionPriceRepository.WhereIf(x => x.CountryCurrencyId == countryCurrencyId, countryCurrencyId != null)
                                                 .Where(x => x.PriceLevel != null && x.Version != null )
                                                 .Select(x => x.VersionId);

                    var versions = _versionRepository.Where(x => versionIds.Contains(x.Id) && x.Application != null).ToList();
                                                                    

                    var result = _mapper.Map<List<GetVersionCanPurshasedOutputDto>>(versions);
                    result.ForEach(x =>
                    {
                        x.Price = _versionBLL.GetApplicationMinimumVersionPrice(x.ApplicationId, customer.CountryId);
                        x.PriceLevels = _mapper.Map<List<RetrievePriceLevelDto>>(_versionPriceRepository.Where(a => a.CountryCurrency != null &&
                        a.CountryCurrencyId == countryCurrencyId && a.VersionId == x.Id).Select(x => x.PriceLevel)).ToList();
                        x.Prices = _versionBLL.GetApplicationVersionPrices(x.Id  ,countryCurrencyId);
                    });

                    return output.CreateResponse(result);
                }

                return output;

            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>assign draft invoice to customer by admin and send email   . </summary>
        ///
        /// <param name="ids"      . </param>
        /// <returns>   A bool. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> AssignInvoicesToCustomer( List<int> ids )
        {
            var output = new Response<bool>();

            try
            {
                var invoices = await _invoiceRepository
                                               .Where(x => (x.InvoiceStatusId == (int)InvoiceStatusEnum.Draft) && ids.Contains(x.Id)).ToListAsync();

                invoices.ForEach(x => x.InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid);

                _unitOfWork.Commit();

                await _invoiceHelper.SendInvoicesEmails(invoices, hasAttachment: true);

                return output.CreateResponse(true);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>delete draft or anceled invoice by admin    . </summary>
        ///
        /// <param name="ids"      . </param>
        /// <returns>   A bool. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> DeleteInvoice( List<int> ids )
        {
            var output = new Response<bool>();
            try
            {
                var invoices = await _invoiceRepository
                                                .Where(x => (x.InvoiceStatusId == (int)InvoiceStatusEnum.Draft
                                                || x.InvoiceStatusId == (int)InvoiceStatusEnum.Cancelled) && ids.Contains(x.Id)).ToListAsync();
                if (invoices == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Invoice));
                foreach (var invoice in invoices)
                {
                    foreach (var invoicesDetail in invoice.InvoiceDetails)
                    {
                        //hard delte invoiceDetails
                        _invoiceDetailsRepository.Delete(invoicesDetail);
                    }

                    if (invoice.RefundRequest != null)
                        //hard delete refundRequest
                        _refundRequestRepository.Delete(invoice.RefundRequest);

                    if (invoice.Notifications != null)
                        //hard delete all notification related
                        await _notificationDataBLL.DeleteRelatedNotificationAsync(new DeleteNotificationDto
                        {
                            CustomerSubscriptionId = invoice.CustomerSubscriptionId
                        });

                }
                _invoiceRepository.DeleteRange(invoices);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>cancel unpaid invoice by admi    . </summary>
        ///
        /// <param name="inputDto"      . </param>
        /// <returns>   A bool. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> CancelInvoice( CancelInvoiceInputDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                var invoices = await _invoiceRepository
                                                .Where(x => (x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid
                                               ) && inputDto.ids.Contains(x.Id)).ToListAsync();
                invoices.ForEach(x =>
                {
                    x.InvoiceStatusId = (int)InvoiceStatusEnum.Cancelled;
                    x.CancelReason = inputDto.CancelReason;
                });

                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        
        ///-------------------------------------------------------------------------------------------------
        /// <summary>ddwnload invoice pdf   . </summary>
        ///
        /// <param name="inovicesIds"      . </param>
        /// <returns>   A DownloadInvoiceFileDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<DownloadInvoiceFileDto>> DownloadInvoicesAsync( IEnumerable<int> inovicesIds, string lang = SupportedLanguage.EN )
        {
            Response<DownloadInvoiceFileDto> response = new Response<DownloadInvoiceFileDto>();
            try
            {
                const string Base64MimeType = "data:application/octet-stream;base64,";


                var invoices = await _invoiceRepository.GetManyAsync(i => inovicesIds.Contains(i.Id));
                
                if(!invoices.Any())
                    return response.CreateResponse(MessageCodes.NotFound ,nameof(Invoice));

                var x = _mapper.Map<IEnumerable<InvoicePdfDto>>(invoices);

                var invoicesDto = _mapper.Map<IEnumerable<InvoicePdfOutputDto>>(invoices);

                var data = new DownloadInvoiceFileDto();

                var basePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                var tempPath = $@"{basePath}\Files\PDFs\Invoices\Temp";
                

                if (invoicesDto.Count() == 1)
                {
                    // get invoice 
                    var invoice = invoicesDto.FirstOrDefault();
                    var pdfBase64 = await _pdfGeneratorBLL.GenerateInvoicePdfAsync(invoice, false, lang);
                    data.FileBase64 = $"{Base64MimeType} {pdfBase64.Data}";
                    data.FileName = $"{invoice.InvoiceInfo.Serial}.pdf";
                }
                else
                {
                    //generate invoices zip file.
                    if (!System.IO.Directory.Exists(tempPath))
                        System.IO.Directory.CreateDirectory(tempPath);

                    var tempGuid = Guid.NewGuid();
                    var tempInvoicesDir = System.IO.Directory.CreateDirectory($@"{tempPath}\TempInvoices{tempGuid}");


                    foreach (var invoice in invoicesDto)
                    {
                        //Get Byte array of Pdf 
                        var arrByte = _pdfGeneratorBLL.InvoiceByteArr(lang, invoice).Data;
                        //Create path pdfs 
                        var invoicePdfPath = $@"{tempInvoicesDir.FullName}\{invoice.InvoiceInfo.Serial}.pdf";
                        //Generate Pdf
                        System.IO.File.WriteAllBytes(invoicePdfPath, arrByte);
                    }

                    // compress files.
                    var extension = ".zip";
                    var compressedFileGeneratedNameWithExtension = $"Invoices-{Guid.NewGuid()}{extension}";
                    var compressedFilesPath = System.IO.Path.Combine(tempPath, compressedFileGeneratedNameWithExtension);
                    System.IO.Compression.ZipFile.CreateFromDirectory(tempInvoicesDir.FullName, compressedFilesPath);

                    // delete temp invoices directory.
                    System.IO.Directory.Delete(tempInvoicesDir.FullName, recursive: true);

                    var compressedFileBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(compressedFilesPath));

                    System.IO.File.Delete(compressedFilesPath);


                    data.FileBase64 = $"{Base64MimeType} {compressedFileBase64}";
                    data.FileName = $"Invoices{extension}";
                }

                ///////////

                return response.CreateResponse(data);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);

            }


            // return await _pdfGeneratorBLL.GenerateInvoiceFileAsync(invoicesDto);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>accept invoice refund by customer   . </summary>
        ///
        /// <param name="invoiceId"      . </param>
        /// /// <param name="currentEmployeeId"      . </param>
        /// <returns>   A bool. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> RefundInvoiceCashAsync( int invoiceId, int currentEmployeeId )
        {
            var response = new Response<bool>();

            try
            {
                var invoice = await _invoiceHelper.GetInvoiceById(invoiceId);

                if (invoice is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(invoiceId));

                if (invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded)
                    return response.CreateResponse(MessageCodes.RequestAlreadyRefunded, nameof(invoiceId));

                if (invoice.InvoiceStatusId != (int)InvoiceStatusEnum.Paid)
                    return response.CreateResponse(MessageCodes.InvalidInvoiceStatus, nameof(invoiceId));

                if (invoice.PaymentMethodId != (int)PaymentMethodEnum.WaitCashRefund)
                    return response.CreateResponse(MessageCodes.InvalidPaymentMethod, nameof(invoiceId));

                invoice.PaymentMethodId = (int)PaymentMethodEnum.Cash;
                invoice.InvoiceStatusId = (int)InvoiceStatusEnum.Refunded;

                _invoiceRepository.Update(invoice);

                await _unitOfWork.CommitAsync();

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = invoice.CustomerSubscription.CustomerId;
                _notificationItem.AdminId = currentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.AcceptCancelation;
                _notificationItem.InvoiceId = invoice.Id;


                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }
        public async Task<IResponse<bool>> PayInvoiceByAdminAsync(int invoiceId , int paidBy)
        {
            var response = new Response<bool>();

            try
            {
                var invoice = await _invoiceHelper.GetInvoiceById(invoiceId);

                if (invoice is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(invoiceId));


                if (invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid)
                {
                    invoice.InvoiceStatusId = (int)InvoiceStatusEnum.Paid;
                    invoice.PaymentMethodId = (int)PaymentMethodEnum.Cash;
                    invoice.PaidBy = paidBy;

                    _invoiceRepository.Update(invoice);

                    await _unitOfWork.CommitAsync();

                    return response.CreateResponse(true);
                }

                return response.CreateResponse(MessageCodes.InvalidInvoiceStatus, nameof(invoiceId));
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }
        #endregion

        #region Helpers


        private List<InvoicePdfOutputDto> GetLogoPathDevExpress( IEnumerable<InvoicePdfOutputDto> invoices, string assetsPath )
        {
            var output = new List<InvoicePdfOutputDto>();
            foreach (var invoice in invoices)
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
                output.Add(invoice);

            }
            return output;
        }

        #region UnUsed
        //private string GetCurrencyCodeAsJson( string name )
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //        return string.Empty;

        //    return _currencyRepository
        //                         .Where(a => Json.Value(a.Code, "$.default") == name || Json.Value(a.Code, "$.ar") == name)
        //                         .FirstOrDefault()?.Code?.Trim() ?? string.Empty;


        //}
        //private void LocalizeInvoiceData(string lang, IEnumerable<InvoicePdfOutputDto> invoicesDto)
        //{
        //    foreach (var invoic in invoicesDto)
        //    {
        //        var isDefault = lang.ToLower() == nameof(JsonLangEnum.En).ToLower();
        //        var currency = GetCurrencyCodeAsJson(invoic.InvoiceSummary.CurrencyName);
        //        var localizedCurrencyName = isDefault ? GetJsonLanguageModelOrNull(currency)?.Default : GetJsonLanguageModelOrNull(currency)?.Ar;
        //        invoic.InvoiceSummary.CurrencyName = localizedCurrencyName;
        //        var startDate = invoic.InvoiceInfo.StartDate;
        //        invoic.InvoiceItems.ToList().ForEach(x =>
        //        {
        //            var currency = GetCurrencyCodeAsJson(x.CurrencyName);
        //            //x.PriceLevel = isDefault ? GetJsonLanguageModelOrNull(x.PriceLevel).Default : GetJsonLanguageModelOrNull(x.PriceLevel).Ar;
        //            x.ProductName = isDefault ? GetJsonLanguageModelOrNull(x.ProductName).Default : GetJsonLanguageModelOrNull(x.ProductName).Ar;
        //            x.SubscriptionType = isDefault ? GetJsonLanguageModelOrNull(x.SubscriptionType).Default : GetJsonLanguageModelOrNull(x.SubscriptionType).Ar;
        //            x.CurrencyName = localizedCurrencyName;
        //        });
        //    }
        //}
        #endregion
    }


    #endregion
}
