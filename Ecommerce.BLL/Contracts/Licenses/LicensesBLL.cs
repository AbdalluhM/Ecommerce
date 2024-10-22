using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Customers.CustomerSubscriptions;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Customer.CustomerProduct;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.BLL.Validation.Licenses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Enums.License;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Contracts.Licenses.Inputs;
using Ecommerce.DTO.Contracts.Licenses.Outputs;
using Ecommerce.DTO.Customers.CustomerProduct;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Customer.CustomerProduct.CustomerProductValidator;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Contracts.Licenses
{
    public class LicensesBLL : BaseBLL, ILicensesBLL
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<RequestActivationKey> _requestActivationKeyRepository;
        private readonly IRepository<RequestChangeDevice> _requestChangeDeviceRepository;
        private readonly IRepository<License> _licenseRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<VersionPrice> _versionPriceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<ViewLicenseRequest> _licenseRequestRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Core.Entities.Version> _versionRepository;
        private readonly IRepository<CustomerSubscription> _customerSubscriptionRepository;
        private readonly IRepository<RefundRequest> _refundRepository;
        private readonly IRepository<LicenseFile> _licenseFileRepository;
        private readonly IRepository<VersionSubscription> _vesionSubscriptionRepository;
        private readonly IRepository<ReasonChangeDevice> _reasonChangeDeviceRepository;
        private readonly IRepository<AddonSubscription> _addOnSubscriptionRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<LicenseLog> _licenseLogRepository;

        private readonly IInvoiceBLL _invoiceBLL;
        private readonly ICustomerSubscriptionBLL _customerSubscriptionBLL;
        private readonly IFileBLL _fileBLL;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IEmployeeBLL _employeeBLL;
        private readonly IInvoiceHelperBLL _invoiceHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FileStorageSetting _fileSetting;
        private readonly IBlobFileBLL _blobFileBLL;

        public LicensesBLL(IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IRepository<Application> applicationRepository,
                           IRepository<VersionPrice> versionPriceRepository,
        IRepository<RequestChangeDevice> requestChangeDeviceRepository,
                           IRepository<RequestActivationKey> requestActivationKeyRepository,
                           IRepository<License> licensesRepository,
                           IRepository<Employee> employeeRepository,
                           IRepository<ViewLicenseRequest> licenseRequestRepository,
                           IRepository<Customer> customerRepository,
                           IRepository<CustomerSubscription> customerSubscriptionRepository,
                           IRepository<RefundRequest> refundRepository,
                           IRepository<LicenseFile> licenseFileRepository,
                           IRepository<VersionSubscription> vesionSubscriptionRepository,
                           IRepository<ReasonChangeDevice> reasonChangeDeviceRepository,
                           IRepository<AddonSubscription> addOnSubscriptionRepository,
                           IRepository<Invoice> invoiceRepository,
                           IRepository<LicenseLog> licenseLogRepository,
                           IInvoiceBLL invoiceBLL,
                           ICustomerSubscriptionBLL customerSubscriptionBLL,
                           IFileBLL fileBLL,
                           INotificationDataBLL notificationDataBLL,
                           IRepository<Core.Entities.Version> versionRepository,
                           IEmployeeBLL employeeBLL,
                           IInvoiceHelperBLL invoiceHelper,
                           IHttpClientFactory httpClientFactory,
                           IOptions<FileStorageSetting> fileSetting,
                           IBlobFileBLL blobFileBLL)
            : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _versionPriceRepository = versionPriceRepository;
            _requestChangeDeviceRepository = requestChangeDeviceRepository;
            _requestActivationKeyRepository = requestActivationKeyRepository;
            _licenseRepository = licensesRepository;
            _employeeRepository = employeeRepository;
            _licenseRequestRepository = licenseRequestRepository;
            _customerRepository = customerRepository;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _refundRepository = refundRepository;
            _licenseFileRepository = licenseFileRepository;
            _vesionSubscriptionRepository = vesionSubscriptionRepository;
            _reasonChangeDeviceRepository = reasonChangeDeviceRepository;
            _addOnSubscriptionRepository = addOnSubscriptionRepository;
            _invoiceRepository = invoiceRepository;
            _licenseLogRepository = licenseLogRepository;

            _invoiceBLL = invoiceBLL;
            _customerSubscriptionBLL = customerSubscriptionBLL;
            _fileBLL = fileBLL;
            _notificationDataBLL = notificationDataBLL;
            _versionRepository = versionRepository;
            _employeeBLL = employeeBLL;
            _invoiceHelper = invoiceHelper;
            _httpClientFactory = httpClientFactory;
            _fileSetting = fileSetting.Value;
            _blobFileBLL = blobFileBLL;
        }

        public async Task<IResponse<List<LicenseCustomerLookupDto>>> GetCustomersLookupAsync(int currentEmployeeId)
        {
            var response = new Response<List<LicenseCustomerLookupDto>>();

            var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            if (employee is null)
                return null;

            //var employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrency?.CountryId);
            var employeeCountryIds = await _employeeBLL.GetEmployeeCountries(currentEmployeeId);

            var customers = await _customerRepository.GetManyAsync(c => c.CustomerStatusId == (int)CustomerStatusEnum.Registered &&
                                                                        c.CustomerSubscriptions.Any() &&
                                                                        (employeeCountryIds != null && employeeCountryIds.Contains(c.CountryId)));

            var customersDto = _mapper.Map<List<LicenseCustomerLookupDto>>(customers);

            return response.CreateResponse(customersDto);
        }

        public async Task<IResponse<List<LicenseProductLookupDto>>> GetProductsLookupAsync(int customerId, int currentEmployeeId)
        {
            var response = new Response<List<LicenseProductLookupDto>>();

            var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            if (employee is null)
                return null;

            var employeeCountriesIds = employee.EmployeeCountries.Select(e => e.CountryCurrency.CountryId);

            var products = await _customerSubscriptionRepository.GetManyAsync(s => s.CustomerId == customerId &&
                                                                                    !s.IsAddOn &&
                                                                                     s.Invoices.Any() &&
                                                                                    (s.Invoices.Count != 1 || s.Invoices.FirstOrDefault().InvoiceStatusId != (int)InvoiceStatusEnum.Draft) &&
                                                                                    !s.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid ||
                                                                                                         i.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded));

            var productsDto = _mapper.Map<List<LicenseProductLookupDto>>(products);

            return response.CreateResponse(productsDto);
        }

        public async Task<IResponse<List<GetReasonChangeDeviceOutputDto>>> GetReasonChangeDeviceLookupAsync()
        {
            var outpput = new Response<List<GetReasonChangeDeviceOutputDto>>();

            try
            {
                var result = await _reasonChangeDeviceRepository.GetAllListAsync();

                return outpput.CreateResponse(_mapper.Map<List<GetReasonChangeDeviceOutputDto>>(result));
            }
            catch (Exception e)
            {
                return outpput.CreateResponse(e);
            }
        }

        public async Task<IResponse<List<GetCustomerProductLicenseAddonOutputDto>>> GetVersionLicencesLookupAsync(CustomerProductInputDto inputDto)
        {
            var output = new Response<List<GetCustomerProductLicenseAddonOutputDto>>();
            try
            {
                var versionSubscription = await _vesionSubscriptionRepository.GetAsync(x => x.Id == inputDto.VersionSubscriptionId);

                var licenses = await _licenseRepository.GetManyAsync(x => x.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId && x.CustomerSubscription.IsAddOn == false);

                var licensesDto = _mapper.Map<List<GetCustomerProductLicenseAddonOutputDto>>(licenses);

                return output.CreateResponse(licensesDto);
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }
        }


        public async Task<IResponse<PagedResultDto<GetLicenseOutputDto>>> GetCustomerProductLicencesPagedListAsync(LicencesFilterInputDto pagedDto)
        {
            var response = new Response<PagedResultDto<GetLicenseOutputDto>>();

            var versionSubscription = await _vesionSubscriptionRepository.GetByIdAsync(pagedDto.VersionSubscriptionId);
            var orderExpression = GetOrderExpression<License>(pagedDto.SortBy);
            var result = GetPagedList<GetLicenseOutputDto, License, dynamic>(pagedDto: pagedDto,
                                                                         repository: _licenseRepository,
                                                                         orderExpression: orderExpression,
                                                                         searchExpression: x => x.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId
                                                                                               && x.CustomerSubscription.CustomerId == pagedDto.CustomerId
                                                                                               && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                                                                                               || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                                                                                                  (x.DeviceName.Contains(pagedDto.SearchTerm) ||
                                                                                                  x.Serial.Contains(pagedDto.SearchTerm)))),
                                                                         sortDirection: pagedDto.SortingDirection);

            if (versionSubscription.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                return response.CreateResponse(MessageCodes.UnAuthorizedAccess);
            return response.CreateResponse(result);
        }
        public async Task<IResponse<PagedResultDto<GetLicenseOutputDto>>> GetCustomerProductsLicencesPagedListAsync(AllLicencesFilterInputDto pagedDto)
        {
            var response = new Response<PagedResultDto<GetLicenseOutputDto>>();

            var orderExpression = GetOrderExpression<License>(pagedDto.SortBy);

            //    var app = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
            //        .Where(e => e.DeviceTypeId == pagedDto.DeviceTypeId).Select(a=>a.Id);
            //    var version = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
            //        .Where(e =>app.Contains(e.ApplicationId) ).Select(e=>e.Id);
            //    var versionPrice = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
            //        .Where(e => version.Contains(e.VersionId)).Select(e => e.Id);




            var result = GetPagedList<GetLicenseOutputDto, License, dynamic>(pagedDto: pagedDto,
                                                                         repository: _licenseRepository,
                                                                         orderExpression: orderExpression,
                                                                         searchExpression: x => x.CustomerSubscription.VersionSubscriptions.Any(x => _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                             .Where(e => e.DeviceTypeId == pagedDto.DeviceTypeId)
                                                                             .SelectMany(a => _versionRepository
                                                                                 .DisableFilter(nameof(DynamicFilters.IsActive)).Where(e => e.ApplicationId == a.Id).SelectMany(v =>
                                                                                     _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                                         .Where(vp => vp.VersionId == v.Id).Select(e => e.Id))).Contains(x.VersionPriceId)) &&


                                                                             x.CustomerSubscription.CustomerId == pagedDto.CustomerId
                                                                                              && x.CustomerSubscription.Invoices.Any(e => e.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                                                              && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                                                                                               || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                                                                                                  (x.DeviceName.Contains(pagedDto.SearchTerm) ||
                                                                                                  x.Serial.Contains(pagedDto.SearchTerm)))),
                                                                         disableFilter: true,
                                                                         sortDirection: pagedDto.SortingDirection);

            var versionSubscriptions = _vesionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == pagedDto.CustomerId).ToList();

            foreach (var item in result.Items)
            {
                item.VersionSubscriptionId = versionSubscriptions.Where(x => x.CustomerSubscriptionId == item.CustomerSubscriptionId).FirstOrDefault()?.Id ?? 0;
            }

            return response.CreateResponse(result);
        }


        public async Task<IResponse<PagedResultDto<GetRequestChangeDeviceOutputDto>>> GetCustomerRequestChangeDevice(LicencesFilterInputDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetRequestChangeDeviceOutputDto>>();

            var versionSubscription = await _vesionSubscriptionRepository.GetAsync(x => x.Id == pagedDto.VersionSubscriptionId);

            if (versionSubscription.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                return output.CreateResponse(MessageCodes.UnAuthorizedAccess);

            if (versionSubscription == null)
                return output.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));


            var result = GetPagedList<GetRequestChangeDeviceOutputDto, RequestChangeDevice, int>(pagedDto: pagedDto,
                                                                                                 repository: _requestChangeDeviceRepository,
                                                                                                 orderExpression: x => x.Id,
                                                                                                 searchExpression: x =>
                                                                                                 x.License.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId
                                                                                                 && x.License.CustomerSubscription.CustomerId == pagedDto.CustomerId,
                                                                                                 sortDirection: pagedDto.SortingDirection);

            return output.CreateResponse(result);
        }


        public async Task<IResponse<PagedResultDto<LicenseRequestDto>>> GetLicensesRequestsAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId)
        {
            var response = new Response<PagedResultDto<LicenseRequestDto>>();

            //var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            //if (employee is null)
            //    return null;

            //var employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrency.CountryId);
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(currentEmployeeId);
            var licensesDto = GetPagedList<LicenseRequestDto, ViewLicenseRequest, DateTime>(pagedDto: requestFilters,
                                                                                            repository: _licenseRequestRepository,
                                                                                            orderExpression: l => l.CreateDate,
                                                                                            searchExpression: l =>
                                                                                                employeCountriesIds.Contains(l.CountryId) &&
                                                                                                (!requestFilters.CustomerSubscriptionId.HasValue || (
                                                                                                    l.CustomerSubscriptionId.Equals(requestFilters.CustomerSubscriptionId)
                                                                                                )) &&
                                                                                                (string.IsNullOrEmpty(requestFilters.SearchTerm) || (
                                                                                                    l.CustomerName.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                                    l.Serial.Contains(requestFilters.SearchTerm) ||
                                                                                                    l.ContractSerial.ToLower().Contains(requestFilters.SearchTerm.ToLower())
                                                                                                )),
                                                                                             sortDirection: requestFilters.SortingDirection);

            return response.CreateResponse(licensesDto);
        }

        public async Task<IResponse<PagedResultDto<ActiveLicenseDto>>> GetActiveLicensesAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId)
        {
            var response = new Response<PagedResultDto<ActiveLicenseDto>>();

            //var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            //if (employee is null)
            //    return null;
            var productName = getParameterValue(requestFilters.Filter, "productName");

            //var employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrency.CountryId);
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(currentEmployeeId);
            var licensesDto = GetPagedList<ActiveLicenseDto, License, DateTime>(pagedDto: requestFilters,
                                                                                repository: _licenseRepository,
                                                                                orderExpression: l => l.ActivateOn.Value,
                                                                                searchExpression: l =>
                                                                                       l.LicenseStatusId == (int)LicenseStatusEnum.Generated &&
                                                                                       employeCountriesIds.Contains(l.CustomerSubscription.Customer.CountryId) &&
                                                                                       (!requestFilters.CustomerSubscriptionId.HasValue || (
                                                                                                l.CustomerSubscriptionId.Equals(requestFilters.CustomerSubscriptionId)
                                                                                       )) &&
                                                                                       (string.IsNullOrEmpty(requestFilters.SearchTerm) || (
                                                                                           l.CustomerSubscription.Customer.Name.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                           l.Serial.Contains(requestFilters.SearchTerm) ||
                                                                                           l.CustomerSubscription.Customer.Contract.Serial.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                           l.CustomerSubscriptionId.Equals(requestFilters.SearchTerm)
                                                                                       )) &&
                                                                                       (string.IsNullOrEmpty(productName) || (
                                                                                           l.CustomerSubscription.IsAddOn ? l.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName.ToLower().Contains(productName.ToLower())
                                                                                           : l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName.ToLower().Contains(productName.ToLower())
                                                                                         )),
                                                                                sortDirection: requestFilters.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/
                                                                                , excluededColumns: new List<string> { "productName" }
                                                                                );

            return response.CreateResponse(licensesDto);
        }

        public async Task<IResponse<PagedResultDto<ExpiredLicenseDto>>> GetExpiredLicensesAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId)
        {
            var response = new Response<PagedResultDto<ExpiredLicenseDto>>();

            //var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            //if (employee is null)
            //    return null;
            var productName = getParameterValue(requestFilters.Filter, "productName");
            //var employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrency.CountryId);
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(currentEmployeeId);

            var licensesDto = GetPagedList<ExpiredLicenseDto, License, DateTime>(pagedDto: requestFilters,
                                                                                 repository: _licenseRepository,
                                                                                 orderExpression: l => l.RenewalDate.Value,
                                                                                 searchExpression: l =>
                                                                                        l.LicenseStatusId == (int)LicenseStatusEnum.Expired &&
                                                                                        employeCountriesIds.Contains(l.CustomerSubscription.Customer.CountryId) &&
                                                                                        (!requestFilters.CustomerSubscriptionId.HasValue || (
                                                                                                l.CustomerSubscriptionId.Equals(requestFilters.CustomerSubscriptionId)
                                                                                        )) &&
                                                                                        (string.IsNullOrEmpty(requestFilters.SearchTerm) || (
                                                                                            l.CustomerSubscription.Customer.Name.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                            l.Serial.Contains(requestFilters.SearchTerm) ||
                                                                                            l.CustomerSubscription.Customer.Contract.Serial.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                            l.CustomerSubscriptionId.Equals(requestFilters.SearchTerm)
                                                                                        )) &&
                                                                                       (string.IsNullOrEmpty(productName) || (
                                                                                           l.CustomerSubscription.IsAddOn ? l.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName.ToLower().Contains(productName.ToLower())
                                                                                           : l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName.ToLower().Contains(productName.ToLower())
                                                                                         )),
                                                                                 sortDirection: requestFilters.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/
                                                                                 , excluededColumns: new List<string> { "productName" }
                                                                                 );

            return response.CreateResponse(licensesDto);
        }

        public async Task<IResponse<PagedResultDto<ChangeLicenseDto>>> GetChangedLicensesAsync(ChangeLicenseFilterDto requestFilters, int currentEmployeeId)
        {
            var response = new Response<PagedResultDto<ChangeLicenseDto>>();

            //var employee = await _employeeRepository.GetByIdAsync(currentEmployeeId);

            //if (employee is null)
            //    return null;
            var productName = getParameterValue(requestFilters.Filter, "productName");
            //var employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrency.CountryId);
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(currentEmployeeId);

            var changeStatusFilter = requestFilters.IsApproved ? (int)RequestChangeDeviceStatusEnum.Approved : (int)RequestChangeDeviceStatusEnum.Rejected;

            var licensesDto = GetPagedList<ChangeLicenseDto, RequestChangeDevice, DateTime>(pagedDto: requestFilters,
                                                                                            repository: _requestChangeDeviceRepository,
                                                                                            orderExpression: l => l.ModifiedDate.Value,
                                                                                            searchExpression: l =>
                                                                                                    l.RequestChangeDeviceStatusId == changeStatusFilter &&
                                                                                                    employeCountriesIds.Contains(l.License.CustomerSubscription.Customer.CountryId) &&
                                                                                                    (!requestFilters.CustomerSubscriptionId.HasValue || (
                                                                                                        l.License.CustomerSubscriptionId.Equals(requestFilters.CustomerSubscriptionId)
                                                                                                    )) &&
                                                                                                    (string.IsNullOrEmpty(requestFilters.SearchTerm) || (
                                                                                                        l.License.CustomerSubscription.Customer.Name.ToLower().Contains(requestFilters.SearchTerm.ToLower()) ||
                                                                                                        l.License.Serial.Contains(requestFilters.SearchTerm) ||
                                                                                                        l.License.CustomerSubscription.Customer.Contract.Serial.ToLower().Contains(requestFilters.SearchTerm.ToLower())
                                                                                                    )) &&
                                                                                       (string.IsNullOrEmpty(productName) || (
                                                                                           l.License.CustomerSubscription.IsAddOn ? l.License.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName.ToLower().Contains(productName.ToLower())
                                                                                           : l.License.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName.ToLower().Contains(productName.ToLower())
                                                                                         )),
                                                                                            sortDirection: requestFilters.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/
                                                                                            , excluededColumns: new List<string> { "productName" }
                                                                                            );

            return response.CreateResponse(licensesDto);
        }


        public async Task<IResponse<LicenseLogBaseInfoDto>> GetLicenseLogAsync(LicenseLogFilterDto requestFilters)
        {
            var response = new Response<LicenseLogBaseInfoDto>();

            var license = await _licenseRepository.GetByIdAsync(requestFilters.LicenseId);

            if (license is null)
                return response.CreateResponse(MessageCodes.NotFound, nameof(requestFilters.LicenseId));

            var logInfoDto = _mapper.Map<LicenseLogBaseInfoDto>(license);

            var logsDto = GetPagedList<LicenseLogDto, LicenseLog, int>(pagedDto: requestFilters,
                                                                                repository: _licenseLogRepository,
                                                                                orderExpression: l => l.Id,
                                                                                searchExpression: l => l.LicenseId == requestFilters.LicenseId,
                                                                                sortDirection: requestFilters.SortingDirection);

            logInfoDto.TotalCount = logsDto.TotalCount;
            logInfoDto.Items = logsDto.Items;

            return response.CreateResponse(logInfoDto);
        }

        public async Task<IResponse<GetLicenseOutputDto>> CreateDeviceByCustomerAsync(CreateLicenseInputDto inputDto)
        {
            var response = new Response<GetLicenseOutputDto>();

            try
            {
                var validator = new CreateLicenseInputDtoValidator().Validate(inputDto);

                //Input Validation
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var versionSubscription = await _vesionSubscriptionRepository.GetAsync(x => x.Id == inputDto.VersionSubscriptionId && x.CustomerSubscription.CustomerId == inputDto.CustomerId);

                //Business Validation
                //1-Check entity is exisit
                if (versionSubscription == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));

                var unpaidInvoice = versionSubscription.CustomerSubscription.Invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid);

                if (unpaidInvoice.Any())
                    return response.CreateResponse(MessageCodes.HasUnPaidInvoices, unpaidInvoice?.FirstOrDefault()?.Serial ?? "0");

                var invoiceStatus = _customerSubscriptionBLL.GetSubscriptionStatus(versionSubscription.CustomerSubscriptionId);

                //2- Check EndDate for Invoice
                if (!invoiceStatus.IsValid)
                    return response.CreateResponse(MessageCodes.NoPaidInvoice, nameof(Invoice));

                //3-Check NumberOfLicense is valid
                if (versionSubscription.CustomerSubscription.Licenses.Count >= versionSubscription.CustomerSubscription.NumberOfLicenses)
                    return response.CreateResponse(MessageCodes.ExceededLicensesLimit, nameof(inputDto.DeviceName));

                //4-Check Serial is Unique
                if (versionSubscription.CustomerSubscription.Licenses.Any(x => x.Serial.ToLower().Equals(inputDto.SerialNumber.ToLower())))
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(License));

                //map entity 
                var newLicense = _mapper.Map<License>(inputDto);

                newLicense.LicenseStatusId = (int)LicenseStatusEnum.InProgress;
                newLicense.CustomerSubscriptionId = versionSubscription.CustomerSubscriptionId;
                newLicense.RenewalDate = invoiceStatus.EndDate;

                var request = new RequestActivationKey
                {
                    LicenseId = newLicense.Id,
                    RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.New,
                    CreateDate = DateTime.Now
                };

                newLicense.RequestActivationKeys.Add(request);
                newLicense.ActivateOn = DateTime.UtcNow;

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = newLicense,
                    ActionTypeId = LicenseActionTypeEnum.AddDevice,
                    NewStatusId = LicenseStatusEnum.InProgress,
                    IsCreatedByAdmin = false,
                    CreatedBy = inputDto.CustomerId,
                });

                var result = await _licenseRepository.AddAsync(newLicense);

                await _unitOfWork.CommitAsync();

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = inputDto.CustomerId;
                _notificationItem.IsAdminSide = true;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.RequestKey;
                _notificationItem.LicenceId = result.Id;
                await _notificationDataBLL.CreateAsync(_notificationItem);

                // var uploadFileResult = await UploadLicenseAutomated(versionSubscription, LicenseRequestTypeEnum.New, request.Id, newLicense.Serial, invoiceStatus.EndDate);

                return response.CreateResponse(_mapper.Map<GetLicenseOutputDto>(result));
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<bool>> CreateDeviceByAdminAsync(NewDeviceDto newDeviceDto, IFileDto fileDto, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var validation = await ValidateNewDeviceAsync(newDeviceDto, fileDto);

                if (!validation.IsSuccess)
                    return response.AppendErrors(validation.Errors);

                // upload file.
                var fileResponse = await _blobFileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = fileDto }, currentEmployeeId);

                if (!fileResponse.IsSuccess)
                    return response.AppendErrors(fileResponse.Errors);

                // create license.
                var license = _mapper.Map<License>(newDeviceDto);

                license.ActivationFile = fileResponse.Data;

                // create license file.
                var licenseFile = new LicenseFile
                {
                    ActivationFile = fileResponse.Data,
                    GeneratedBy = currentEmployeeId,
                    KeyUploadedOn = DateTime.UtcNow
                };

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = license,
                    ActionTypeId = LicenseActionTypeEnum.AddDevice,
                    NewStatusId = LicenseStatusEnum.Generated,
                    IsCreatedByAdmin = true,
                    CreatedBy = currentEmployeeId,
                });

                // create approved request.
                await _requestActivationKeyRepository.AddAsync(new RequestActivationKey
                {
                    License = license,
                    RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.New,
                    LicenseFile = licenseFile,
                    CreateDate = DateTime.UtcNow,
                    IsCreatedBySystem = true
                });

                await _unitOfWork.CommitAsync();

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> CreateAddOnDeviceAsync(NewAddOnLicenseDto newAddonLicense)
        {
            var response = new Response<bool>();

            try
            {
                var validator = await new NewAddOnLicenseDtoValidator().ValidateAsync(newAddonLicense);

                //Input Validation
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var addOnSubscription = await _addOnSubscriptionRepository.GetAsync(a => a.VersionSubscriptionId == newAddonLicense.VersionSubscriptionId
                                                                                         && a.CustomerSubscription.CustomerId == newAddonLicense.CustomerId
                                                                                         && a.Id == newAddonLicense.AddOnSubscriptionId);

                //1- Check entity is exisit
                if (addOnSubscription == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(AddonSubscription));

                if (addOnSubscription.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid))
                    return response.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(newAddonLicense.VersionSubscriptionId));

                var invoiceStatus = _customerSubscriptionBLL.GetSubscriptionStatus(addOnSubscription.CustomerSubscriptionId);

                //2- Check EndDate for Invoice
                if (!invoiceStatus.IsValid)
                    return response.CreateResponse(MessageCodes.NoPaidInvoice, nameof(Invoice));

                if (await _licenseRepository.AnyAsync(l => l.CustomerSubscriptionId == addOnSubscription.CustomerSubscriptionId))
                    return response.CreateResponse(MessageCodes.ExceededLicensesLimit, nameof(newAddonLicense.LicenseId));

                var license = await _licenseRepository.GetAsync(l => l.Id == newAddonLicense.LicenseId);

                //3- Check License is Exisit
                if (license is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(newAddonLicense.LicenseId));

                var newLicense = _mapper.Map<License>(newAddonLicense);

                //Add activation key
                newLicense.RequestActivationKeys.Add(new RequestActivationKey
                {
                    LicenseId = newLicense.Id,
                    RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.New,
                    CreateDate = DateTime.Now
                });

                newLicense.CustomerSubscriptionId = addOnSubscription.CustomerSubscriptionId;
                newLicense.RenewalDate = invoiceStatus.EndDate;
                newLicense.Serial = license.Serial;
                newLicense.DeviceName = license.DeviceName;

                var result = await _licenseRepository.AddAsync(newLicense);

                await _unitOfWork.CommitAsync();


                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }


        public async Task<IResponse<bool>> RenewVersionLicenseAsync(LicenseActionInputDto renewLicense, bool isCommit = true)
        {
            var response = new Response<bool>();

            try
            {
                var license = await _licenseRepository.GetAsync(x => x.Id == renewLicense.LicenseId && x.LicenseStatusId == (int)LicenseStatusEnum.Expired);

                //Check entity Is exisit
                if (license == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(license));

                var versionSubscription = await _vesionSubscriptionRepository.GetAsync(x => x.Id == renewLicense.VersionSubscriptionId
                                                                                            && x.CustomerSubscription.CustomerId == renewLicense.CustomerId);

                // Check entity Is exisit
                if (versionSubscription == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));

                // Check paid invoice.
                var invoices = _invoiceRepository.Where(i => i.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId);
                var paidInvoices = invoices.Any(i => i.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId
                                                && i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                && DateTime.UtcNow >= i.StartDate
                                                && DateTime.UtcNow <= i.EndDate);

                var lastUnpaidInvoice = await invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid).OrderBy(x => x.CreateDate).LastOrDefaultAsync();

                if (!paidInvoices)
                {
                    return response.CreateResponse(MessageCodes.HasUnPaidInvoices, lastUnpaidInvoice.Serial ?? "0");
                }

                var invoiceStatus = _customerSubscriptionBLL.GetSubscriptionStatus(versionSubscription.CustomerSubscriptionId);

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = license,
                    ActionTypeId = LicenseActionTypeEnum.Renew,
                    OldStatusId = (LicenseStatusEnum)license.LicenseStatusId,
                    NewStatusId = LicenseStatusEnum.InProgress,
                    IsCreatedByAdmin = false,
                    CreatedBy = renewLicense.CustomerId,
                });

                //Check Invoice EndDate
                license.LicenseStatusId = (int)LicenseStatusEnum.InProgress;
                license.RenewalDate = invoiceStatus.EndDate;

                var request = new RequestActivationKey
                {
                    RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.Renew,
                    LicenseId = license.Id,
                    CreateDate = DateTime.UtcNow
                };

                license.RequestActivationKeys.Add(request);

                _licenseRepository.Update(license);

                if (isCommit)
                    await _unitOfWork.CommitAsync();

                // var uploadFileResult = await UploadLicenseAutomated(versionSubscription, LicenseRequestTypeEnum.Renew, request.Id, license.Serial, invoiceStatus.EndDate);

                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<bool>> RenewAddOnLicenseAsync(AddOnLicenseActionInputDto renewLicense)
        {
            var output = new Response<bool>();

            try
            {
                var license = await _licenseRepository.GetAsync(x => x.Id == renewLicense.LicenseId && x.LicenseStatusId == (int)LicenseStatusEnum.Expired);

                //Check entity Is exisit
                if (license == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(license));

                var addOnSubscription = await _addOnSubscriptionRepository.GetAsync(x => x.Id == renewLicense.AddOnSubscriptionId
                                                                                         && x.VersionSubscriptionId == renewLicense.VersionSubscriptionId
                                                                                         && x.CustomerSubscription.CustomerId == renewLicense.CustomerId);

                //Check entity Is exisit
                if (addOnSubscription == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddonSubscription));

                // Check paid invoice.
                if (!_invoiceRepository.Any(i => i.CustomerSubscriptionId == addOnSubscription.CustomerSubscriptionId
                                                && i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                && DateTime.UtcNow >= i.StartDate
                                                && DateTime.UtcNow <= i.EndDate))
                {
                    return output.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(renewLicense.LicenseId));
                }

                var invoiceStatus = _customerSubscriptionBLL.GetSubscriptionStatus(addOnSubscription.CustomerSubscriptionId);

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = license,
                    ActionTypeId = LicenseActionTypeEnum.Renew,
                    OldStatusId = (LicenseStatusEnum)license.LicenseStatusId,
                    NewStatusId = LicenseStatusEnum.InProgress,
                    IsCreatedByAdmin = false,
                    CreatedBy = renewLicense.CustomerId,
                });

                //Check Invoice EndDate
                license.LicenseStatusId = (int)LicenseStatusEnum.InProgress;
                license.RenewalDate = invoiceStatus.EndDate;

                license.RequestActivationKeys.Add(new RequestActivationKey
                {
                    LicenseId = license.Id,
                    RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.Renew,
                    CreateDate = DateTime.Now
                });

                _licenseRepository.Update(license);

                await _unitOfWork.CommitAsync();

                return output.CreateResponse(true);
            }
            catch (Exception e)
            {
                return output.CreateResponse(e);
            }
        }

        public async Task<List<IResponse<bool>>> RenewAllLicensesAsync(int customerSubscriptionId)
        {
            var responses = new List<IResponse<bool>>();

            var customerSubscription = await _customerSubscriptionRepository.GetByIdAsync(customerSubscriptionId);

            if (customerSubscription.AutoBill)
            {
                if (customerSubscription.IsAddOn)
                {
                    // renew addon licenses.
                    foreach (var license in customerSubscription.Licenses)
                    {
                        var licenseResult = await RenewAddOnLicenseAsync(new AddOnLicenseActionInputDto
                        {
                            LicenseId = license.Id,
                            CustomerId = customerSubscription.CustomerId,
                            VersionSubscriptionId = customerSubscription.AddonSubscriptions.FirstOrDefault().VersionSubscriptionId,
                            AddOnSubscriptionId = customerSubscription.AddonSubscriptions.FirstOrDefault().Id
                        });

                        responses.Add(licenseResult);
                    }
                }
                else
                {
                    // renew version licenses.
                    foreach (var license in customerSubscription.Licenses)
                    {
                        var licenseResult = await RenewVersionLicenseAsync(new LicenseActionInputDto
                        {
                            CustomerId = customerSubscription.CustomerId,
                            VersionSubscriptionId = customerSubscription.VersionSubscriptions.FirstOrDefault().Id,
                            LicenseId = license.Id
                        });

                        responses.Add(licenseResult);
                    }
                }
            }

            return responses;
        }


        public async Task<IResponse<bool>> UploadLicenseAsync(UploadLicenseDto uploadLicenseDto, IFileDto fileDto, int currentEmployeeId)
        {
            var response = new Response<bool>();

            var fileValidation = await new FileDtoValidator().ValidateAsync(fileDto);

            if (!fileValidation.IsValid)
                return response.CreateResponse(fileValidation.Errors);

            return (LicenseRequestTypeEnum)uploadLicenseDto.RequestTypeId switch
            {
                LicenseRequestTypeEnum.New or
                LicenseRequestTypeEnum.Renew =>
                    await UploadNewLicenseAsync(uploadLicenseDto, fileDto, currentEmployeeId),
                LicenseRequestTypeEnum.ChangeDevice =>
                    await UploadChangeDeviceLicenseAsync(uploadLicenseDto, fileDto, currentEmployeeId),
                _ => response.CreateResponse(MessageCodes.InputValidationError, nameof(uploadLicenseDto.RequestTypeId)),
            };
        }


        public async Task<IResponse<RefundDto>> AcceptRefundRequestAsync(int requestId, int currentEmployeeId)
        {
            var response = new Response<RefundDto>();

            try
            {
                // get request.
                var request = await _refundRepository.GetByIdAsync(requestId);

                var validationResult = ValidateRefundRequest(request);

                if (!validationResult.IsSuccess)
                    return response.AppendErrors(validationResult.Errors);

                var refundDto = new RefundDto();

                // check payment method.
                var paymentTypeId = (Enum.IsDefined(typeof(PaymentTypesEnum), request.Invoice.PaymentMethod.PaymentTypeId)) ?
                                                                     (PaymentTypesEnum)request.Invoice.PaymentMethod.PaymentTypeId : 0;

                if (request.Invoice.PaymentMethod.PaymentTypeId == (int)PaymentTypesEnum.PayMob)
                {
                    if (string.IsNullOrEmpty(request.Invoice.PaymentInfo))
                        return response.CreateResponse(MessageCodes.FailedToFetchData, nameof(requestId));

                    var paymentInfo = JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(request.Invoice.PaymentInfo);

                    if (!paymentInfo.Fawry.IsFawryCard)
                        paymentTypeId = PaymentTypesEnum.Cash;
                    else
                        paymentTypeId = (Enum.IsDefined(typeof(PaymentTypesEnum), request.Invoice.PaymentMethod.PaymentTypeId)) ?
                                                                     (PaymentTypesEnum)request.Invoice.PaymentMethod.PaymentTypeId : 0;
                }

                var refundResult = new RefundPaymentResponseDto();
                ;

                // call refund invoice.
                refundResult = await _invoiceBLL.RefundInvoiceAsync(request.Invoice);

                // if refund gateway failed, stop the operation and return.
                if (!refundResult.IsSuccess)
                {
                    refundDto.IsPaymentRefundFail = true;

                    return response.CreateResponse(refundDto);
                }

                // fawry ref code.
                // update invoice to be refunded.
                await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(request.InvoiceId, paymentTypeId, (int)InvoiceStatusEnum.Refunded, refundResult.Result, commit: false);

                await AcceptRefundHelperAsync(request);

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = request.Invoice.CustomerSubscription.CustomerId;
                _notificationItem.AdminId = currentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.AcceptCancelation;
                _notificationItem.InvoiceId = request.InvoiceId;
                _notificationItem.RefundRequestId = request.Id;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);

                return response.CreateResponse(refundDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> AcceptRefundRequestByCashAsync(int requestId, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var request = await _refundRepository.GetByIdAsync(requestId);

                var validationResult = ValidateRefundRequest(request);

                if (!validationResult.IsSuccess)
                    return response.AppendErrors(validationResult.Errors);

                // update payment method to be wait cash refund.
                request.Invoice.PaymentMethodId = (int)PaymentMethodEnum.WaitCashRefund;

                await AcceptRefundHelperAsync(request);

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> RejectRefundRequestAsync(int requestId, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var request = await _refundRepository.GetByIdAsync(requestId);

                if (request is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(requestId));

                request.RefundRequestStatusId = (int)RefundRequestStatusEnum.Refused;

                _refundRepository.Update(request);

                await _unitOfWork.CommitAsync();

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = request.Invoice.CustomerSubscription.CustomerId;
                _notificationItem.AdminId = currentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.RejectCancelation;
                _notificationItem.InvoiceId = request.InvoiceId;
                _notificationItem.RefundRequestId = request.Id;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        public async Task<IResponse<GetRequestChangeDeviceOutputDto>> ChangeDeviceAsync(UpdateLicenseInputDto inputDto)
        {
            var response = new Response<GetRequestChangeDeviceOutputDto>();

            try
            {
                var validator = new UpdateLicenseInputDtoValidator().Validate(inputDto);

                //Input Validation
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var versionSubscription = await _vesionSubscriptionRepository
                    .GetAsync(x => x.Id == inputDto.VersionSubscriptionId && x.CustomerSubscription.CustomerId == inputDto.CustomerId);

                //Business Validation 
                //1-Check entity is exisit
                if (versionSubscription == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));

                //2-Check no paid invoices exist.
                if (versionSubscription.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid))
                    return response.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(inputDto.VersionSubscriptionId));

                var invoiceStatus = _customerSubscriptionBLL.GetSubscriptionStatus(versionSubscription.CustomerSubscriptionId);

                //3- Check last invoice valid
                if (!invoiceStatus.IsValid)
                    return response.CreateResponse(MessageCodes.NoPaidInvoice, nameof(inputDto.DeviceId));

                var license = versionSubscription.CustomerSubscription.Licenses.FirstOrDefault(x => x.Id == inputDto.DeviceId);

                //4-Check License is exisit
                if (license == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(inputDto.DeviceId));

                //5-Check old device && new device
                if (license.Serial == inputDto.SerialNumber)
                    return response.CreateResponse(MessageCodes.OldEqualNewDevice, nameof(inputDto.SerialNumber));

                //6-Check license status is already genertaed.
                if (license.LicenseStatusId != (int)LicenseStatusEnum.Generated)
                    return response.CreateResponse(MessageCodes.IsNotGenerated, nameof(inputDto.DeviceId));

                //7-Check Serial is Unique in licenses.
                if (versionSubscription.CustomerSubscription.Licenses.Any(x => x.Serial.ToLower().Equals(inputDto.SerialNumber.ToLower())))
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(inputDto.DeviceId));


                var request = await _requestChangeDeviceRepository.AnyAsync(x => x.LicenseId == license.Id
                                                                                 && x.RequestChangeDeviceStatusId == (int)RequestChangeDeviceStatusEnum.InProgress);

                if (request)
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(RequestChangeDevice));

                //Add RequestChangeDevie

                var entity = new RequestChangeDevice
                {
                    LicenseId = license.Id,
                    OldDeviceName = license.DeviceName,
                    NewDeviceName = inputDto.DeviceName,
                    OldSerial = license.Serial,
                    NewSerial = inputDto.SerialNumber,
                    ReasonChangeDeviceId = inputDto.ReasonChangeId,
                    RequestChangeDeviceStatusId = (int)RequestChangeDeviceStatusEnum.InProgress,
                    CreateDate = DateTime.UtcNow,
                };

                var result = _requestChangeDeviceRepository.Add(entity);

                _unitOfWork.Commit();

                //var uploadFileResult = await UploadLicenseAutomated(versionSubscription, LicenseRequestTypeEnum.ChangeDevice, entity.Id, inputDto.SerialNumber, invoiceStatus.EndDate);

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = inputDto.CustomerId;
                _notificationItem.IsAdminSide = true;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.RequestKey;
                _notificationItem.LicenceId = license.Id;
                await _notificationDataBLL.CreateAsync(_notificationItem);


                return response.CreateResponse(_mapper.Map<GetRequestChangeDeviceOutputDto>(result));
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<bool>> RejectChangeDeviceRequestAsync(int requestId, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var request = await _requestChangeDeviceRepository.GetByIdAsync(requestId);

                if (request is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(requestId));

                request.RequestChangeDeviceStatusId = (int)RequestChangeDeviceStatusEnum.Rejected;
                request.ModifiedDate = DateTime.UtcNow;
                request.ModifiedBy = currentEmployeeId;

                _requestChangeDeviceRepository.Update(request);

                await _unitOfWork.CommitAsync();

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        public async Task<IResponse<bool>> ReactivateExpiredLicenseAsync(ReactivateLicenseDto reactivateLicenseDto, IFileDto fileDto, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var fileValidation = await new FileDtoValidator().ValidateAsync(fileDto);

                if (!fileValidation.IsValid)
                    return response.CreateResponse(fileValidation.Errors);

                var license = await _licenseRepository.GetByIdAsync(reactivateLicenseDto.LicenseId);

                if (license is null)
                    return response.CreateResponse(DXConstants.MessageCodes.NotFound, nameof(reactivateLicenseDto.LicenseId));

                // validate license is already expired.
                //if (license.RenewalDate >= DateTime.UtcNow)
                if (license.LicenseStatusId != (int)LicenseStatusEnum.Expired)
                    return response.CreateResponse(DXConstants.MessageCodes.LicenseNotExpiredYet, nameof(reactivateLicenseDto.LicenseId));

                //1. Check paid invoice.
                if (!license.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                    && DateTime.UtcNow >= i.StartDate
                                                                    && DateTime.UtcNow <= i.EndDate))
                {
                    return response.CreateResponse(DXConstants.MessageCodes.HasUnPaidInvoices, nameof(reactivateLicenseDto.LicenseId));
                }

                //2. upload file.
                var fileResponse = _fileBLL.UploadFile(fileDto, currentEmployeeId);

                if (!fileResponse.IsSuccess)
                    return response.AppendErrors(fileResponse.Errors);

                //3. update License status.
                license.ActivationFile = fileResponse.Data;
                license.ActivateOn = DateTime.UtcNow;
                license.LicenseStatusId = (int)LicenseStatusEnum.Generated;

                var requestExist = await _requestActivationKeyRepository.GetAsync(r => r.LicenseId == reactivateLicenseDto.LicenseId && !r.LicenseFileId.HasValue);

                var licenseFile = new LicenseFile
                {
                    ActivationFile = fileResponse.Data,
                    GeneratedBy = currentEmployeeId,
                    KeyUploadedOn = DateTime.UtcNow
                };

                if (requestExist is not null)
                {
                    requestExist.LicenseFile = licenseFile;

                    _requestActivationKeyRepository.Update(requestExist);
                }
                else
                {
                    await _requestActivationKeyRepository.AddAsync(new RequestActivationKey
                    {
                        LicenseId = reactivateLicenseDto.LicenseId,
                        CreateDate = DateTime.UtcNow,
                        IsCreatedBySystem = true,
                        LicenseFile = licenseFile,
                        RequestActivationKeyStatusId = (int)LicenseRequestTypeEnum.Renew
                    });
                }

                await _unitOfWork.CommitAsync();

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task MakeLicensesExpiredAsync(List<License> licenses, bool isCommit = true)
        {
            licenses.ForEach(license =>
            {
                license.LicenseStatusId = (int)LicenseStatusEnum.Expired;

                _licenseRepository.Update(license);
            });

            if (isCommit)
                await _unitOfWork.CommitAsync();
        }

        #region Helpers.
        public string getParameterValue(string? jsonFilter, string paramKey)
        {
            string paramValue = string.Empty;
            Dictionary<string, FilterValue> filters = null;
            if (jsonFilter != null)
            {
                filters = JsonConvert.DeserializeObject<Dictionary<string, FilterValue>>(jsonFilter);
                var item = filters.Where(x => x.Key == paramKey).FirstOrDefault();
                if (item.Value != null)
                {
                    paramValue = item.Value.Value;
                }
            }
            return paramValue;
        }

        private async Task<IResponse<bool>> UploadNewLicenseAsync(UploadLicenseDto uploadLicenseDto, IFileDto fileDto, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var request = await _requestActivationKeyRepository.GetByIdAsync(uploadLicenseDto.RequestId);

                if (request is null)
                    return response.CreateResponse(DXConstants.MessageCodes.NotFound, nameof(uploadLicenseDto.RequestId));

                //1. Check paid invoice.
                if (!request.License.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                            && DateTime.UtcNow >= i.StartDate
                                                                            && DateTime.UtcNow <= i.EndDate))
                {
                    return response.CreateResponse(DXConstants.MessageCodes.HasUnPaidInvoices, nameof(uploadLicenseDto.RequestId));
                }

                //2. upload file.
                var fileResponse = await _blobFileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = fileDto }, currentEmployeeId);

                if (!fileResponse.IsSuccess)
                    return response.AppendErrors(fileResponse.Errors);

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = request.License,
                    ActionTypeId = LicenseActionTypeEnum.UploadLicense,
                    OldStatusId = (LicenseStatusEnum)request.License.LicenseStatusId,
                    NewStatusId = LicenseStatusEnum.Generated,
                    IsCreatedByAdmin = true,
                    CreatedBy = currentEmployeeId,
                });

                //3. update License status.
                request.License.ActivationFile = fileResponse.Data;
                request.License.ActivateOn = DateTime.UtcNow;
                request.License.LicenseStatusId = (int)LicenseStatusEnum.Generated;

                request.LicenseFile = new LicenseFile
                {
                    ActivationFile = fileResponse.Data,
                    GeneratedBy = currentEmployeeId,
                    KeyUploadedOn = DateTime.UtcNow
                };

                _requestActivationKeyRepository.Update(request);

                await _unitOfWork.CommitAsync();

                //Version = request.License.CustomerSubscription.IsAddOn ? request.License.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName  : request.License.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName ,
                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = request.License.CustomerSubscription.CustomerId;
                _notificationItem.AdminId = currentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.KeyGenerated;
                _notificationItem.LicenceId = request.LicenseId;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);


                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        private async Task<IResponse<bool>> UploadChangeDeviceLicenseAsync(UploadLicenseDto uploadLicenseDto, IFileDto fileDto, int currentEmployeeId)
        {
            var response = new Response<bool>();

            try
            {
                var request = await _requestChangeDeviceRepository.GetByIdAsync(uploadLicenseDto.RequestId);

                if (request is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(uploadLicenseDto.RequestId));

                //1. Check paid invoice.
                if (!request.License.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                            && DateTime.UtcNow >= i.StartDate
                                                                            && DateTime.UtcNow <= i.EndDate))
                {
                    return response.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(uploadLicenseDto.RequestId));
                }

                //2. upload file.
                var fileResponse = await _blobFileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = fileDto }, currentEmployeeId);

                if (!fileResponse.IsSuccess)
                    return response.AppendErrors(fileResponse.Errors);

                // log license.
                await LogLicenseAsync(new NewLicenseLogDto
                {
                    License = request.License,
                    ActionTypeId = LicenseActionTypeEnum.ChangeDevice,
                    OldStatusId = (LicenseStatusEnum)request.License.LicenseStatusId,
                    NewStatusId = LicenseStatusEnum.Generated,
                    IsCreatedByAdmin = true,
                    CreatedBy = currentEmployeeId,
                });

                //3. update License status.
                request.License.ActivationFile = fileResponse.Data;
                request.License.ActivateOn = DateTime.UtcNow;
                request.License.LicenseStatusId = (int)LicenseStatusEnum.Generated;

                request.License.DeviceName = request.NewDeviceName;
                request.License.Serial = request.NewSerial;
                request.RequestChangeDeviceStatusId = (int)RequestChangeDeviceStatusEnum.Approved;
                request.ModifiedDate = DateTime.UtcNow;
                request.ModifiedBy = currentEmployeeId;

                request.LicenseFile = new LicenseFile
                {
                    ActivationFile = fileResponse.Data,
                    GeneratedBy = currentEmployeeId,
                    KeyUploadedOn = DateTime.UtcNow
                };

                _requestChangeDeviceRepository.Update(request);

                // delete related addon license to version license that has request to change.
                var relatedAddons = request.License.CustomerSubscription.VersionSubscriptions.FirstOrDefault().AddonSubscriptions;

                if (relatedAddons.Any())
                {
                    foreach (var addon in relatedAddons)
                    {
                        var relatedAddonLicense = addon.CustomerSubscription.Licenses.FirstOrDefault(l => l.Serial.ToLower().Equals(request.OldSerial.ToLower()));

                        if (relatedAddonLicense is not null)
                            _licenseRepository.Delete(relatedAddonLicense);
                    }
                }

                await _unitOfWork.CommitAsync();

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = request.License.CustomerSubscription.CustomerId;
                _notificationItem.AdminId = currentEmployeeId;
                _notificationItem.IsAdminSide = false;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.KeyGenerated;
                _notificationItem.LicenceId = request.LicenseId;
                _notificationItem.ProjectTypeEnum = ProjectTypeEnum.Admin;

                await _notificationDataBLL.CreateAsync(_notificationItem);

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        private async Task<IResponse<bool>> ValidateNewDeviceAsync(NewDeviceDto newDevice, IFileDto fileDto)
        {
            var response = new Response<bool>();

            // validate dto.
            var fileValidator = await new FileDtoValidator().ValidateAsync(fileDto);

            if (!fileValidator.IsValid)
                response.AppendErrors(fileValidator.Errors);

            var validation = await new NewDeviceDtoValidator().ValidateAsync(newDevice);

            if (!validation.IsValid)
                response.AppendErrors(validation.Errors);

            if (!response.IsSuccess)
                return response;

            //if (await _licensesRepository.AnyAsync(l => l.Serial.ToLower().Equals(newDevice.Serial.ToLower())) ||
            //    await _requestChangeDeviceRepository.AnyAsync(r => r.OldSerial.ToLower().Equals(newDevice.Serial.ToLower()) ||
            //                                                       r.NewSerial.ToLower().Equals(newDevice.Serial.ToLower())))
            //    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(newDevice.Serial));

            if (await _licenseRepository.AnyAsync(l => l.CustomerSubscriptionId == newDevice.SubscriptionId && l.Serial.ToLower().Equals(newDevice.Serial.ToLower())))
                return response.CreateResponse(MessageCodes.AlreadyExists, nameof(newDevice.Serial));

            var subscription = await _customerSubscriptionRepository.GetByIdAsync(newDevice.SubscriptionId);

            if (subscription is null)
                return response.CreateResponse(MessageCodes.InputValidationError, nameof(newDevice.SubscriptionId));

            if (subscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid))
                return response.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(newDevice.SubscriptionId));

            var lastInvoice = subscription.Invoices.Where(i => i.CustomerSubscriptionId == newDevice.SubscriptionId)
                                                   .OrderBy(i => i.CreateDate)
                                                   .LastOrDefault();

            if (lastInvoice is null)
                return response.CreateResponse(MessageCodes.NoPaidInvoice, nameof(newDevice.SubscriptionId));

            // validate license renewalDate within the valid period of paid invoice.
            if (newDevice.RenewalDate.Date < lastInvoice.StartDate.Date || newDevice.RenewalDate.Date > lastInvoice.EndDate.Date)
                return response.CreateResponse(MessageCodes.ExceededPeriod, nameof(newDevice.RenewalDate));

            // validate the number of license count.
            if (subscription.Licenses.Count == subscription.NumberOfLicenses)
                return response.CreateResponse(MessageCodes.ExceededLicensesLimit, nameof(newDevice.DeviceName));

            return response;
        }

        private async Task LogLicenseAsync(NewLicenseLogDto newLicenseLog)
        {
            var log = _mapper.Map<LicenseLog>(newLicenseLog);

            await _licenseLogRepository.AddAsync(log);
        }

        private IResponse<bool> ValidateRefundRequest(RefundRequest request)
        {
            var response = new Response<bool>();

            if (request is null)
                return response.CreateResponse(MessageCodes.NotFound, nameof(request));

            if (!request.Invoice.CustomerSubscription.IsAddOn)
            {
                var versionSub = request.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault();

                if (_invoiceBLL.IsVersionHasNotRefundedAddons(versionSub))
                {
                    return response.CreateResponse(MessageCodes.RelatedAddonNotRefunded);
                }
            }

            // check request is already refunded.
            if (request.RefundRequestStatusId == (int)RefundRequestStatusEnum.Accepted)
                return response.CreateResponse(MessageCodes.RequestAlreadyRefunded, nameof(request));

            // check request is already rejected.
            if (request.RefundRequestStatusId == (int)RefundRequestStatusEnum.Refused)
                return response.CreateResponse(MessageCodes.RequestAlreadyRejected, nameof(request));

            // check invoice is already refunded.
            if (request.Invoice.InvoiceStatusId != (int)InvoiceStatusEnum.Paid)
                return response.CreateResponse(MessageCodes.InvalidInvoiceStatus, nameof(request));

            return response.CreateResponse();
        }

        private async Task AcceptRefundHelperAsync(RefundRequest request)
        {
            try
            {
                //delete notification related invoice
                await _notificationDataBLL.DeleteRelatedNotificationAsync(new DeleteNotificationDto
                {
                    InvoiceId = request.InvoiceId,

                });
                //delete notification related license
                foreach (var license in request.Invoice.CustomerSubscription.Licenses)
                {
                    await _notificationDataBLL.DeleteRelatedNotificationAsync(new DeleteNotificationDto
                    {
                        LicenceId = license.Id,
                    });
                }
                //delete notification related refundRequest
                foreach (var refunRequest in request.Notifications)
                {
                    await _notificationDataBLL.DeleteRelatedNotificationAsync(new DeleteNotificationDto
                    {
                        RefundRequestId = refunRequest.Id,
                    });
                }
                // update request to be accepted.
                request.RefundRequestStatusId = (int)RefundRequestStatusEnum.Accepted;

                _refundRepository.Update(request);

                // get licenses.
                var licenses = request.Invoice.CustomerSubscription.Licenses.ToList();

                foreach (var license in licenses)
                {
                    var licenseFiles = new List<LicenseFile>();

                    _licenseLogRepository.DeleteRange(license.LicenseLogs);

                    licenseFiles.AddRange(license.RequestActivationKeys.Where(r => r.LicenseFile != null).Select(r => r.LicenseFile));
                    licenseFiles.AddRange(license.RequestChangeDevices.Where(r => r.LicenseFile != null).Select(r => r.LicenseFile));

                    // delete license files.
                    _licenseFileRepository.DeleteRange(licenseFiles);

                    // delete request activation
                    _requestActivationKeyRepository.DeleteRange(license.RequestActivationKeys);

                    // delete request change device.
                    _requestChangeDeviceRepository.DeleteRange(license.RequestChangeDevices);
                }

                // delete license.
                _licenseRepository.DeleteRange(licenses);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> UploadLicenseAutomated(VersionSubscription versionSubscription, LicenseRequestTypeEnum requestType, int requestId, string serial, DateTime? invoiceEndDate)
        {
            var appNameJson = versionSubscription.VersionPrice.Version.Application.Name;
            var versionNameJson = versionSubscription.VersionPrice.Version.Name;

            var appName = JsonConvert.DeserializeObject<JsonLanguageModel>(appNameJson);
            var versionName = JsonConvert.DeserializeObject<JsonLanguageModel>(versionNameJson);

            var acceptedApps = new Dictionary<string, int>
            {
                { "onepurchase", 1 },
                { "onesubscription", 2}
            };

            var acceptedVersions = new Dictionary<string, int>
            {
                {"red", 1},
                {"blue", 2},
                {"green", 3},
            };

            var trimmedAppName = appName.Default.RemoveWhitespace().ToLower();
            var trimmedVersionName = versionName.Default.RemoveWhitespace().ToLower();

            if (acceptedApps.ContainsKey(trimmedAppName) && acceptedVersions.ContainsKey(trimmedVersionName))
            {
                var appId = acceptedApps.GetValueOrDefault(trimmedAppName);
                var versionId = acceptedVersions.GetValueOrDefault(trimmedVersionName);
                var days = invoiceEndDate.HasValue ? (invoiceEndDate.Value - DateTime.UtcNow).Days : versionSubscription.CustomerSubscription.RenewEvery;
                var contractSerial = versionSubscription.CustomerSubscription.Customer.Contract.Serial;

                var file = await GenerateLicenseFileAsync(appId, days, versionId, serial, contractSerial);

                if (file is not null)
                {
                    var uploadLicenseDto = new UploadLicenseDto
                    {
                        RequestId = requestId,
                        RequestTypeId = (int)requestType,
                        File = file
                    };

                    var fileDto = new FileDto
                    {
                        File = file,
                        FileBaseDirectory = AppContext.BaseDirectory,
                        FilePath = _fileSetting.Files.Customers.License.Path,
                        ContainerName = _fileSetting.Files.Customers.License.ContainerName
                    };

                    var admin = await _employeeRepository.GetAsync(e => e.IsAdmin == true);

                    var isFileUploaded = await UploadLicenseAsync(uploadLicenseDto, fileDto, admin.Id);

                    return isFileUploaded.Data;
                }

                return false;
            }

            return false;
        }

        private async Task<IFormFile> GenerateLicenseFileAsync(int app, int days, int version, string serial, string contractSerial)
        {
            try
            {
                var url = $"http://acc111.online/api/Default?_app={app}&_days={days}&_version={version}&_serial={serial}&_contractNr={contractSerial}";

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var fileByteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync();

                var ms = new MemoryStream(fileByteArray);

                var file = new FormFile(ms, 0, fileByteArray.Length, "activation", "activation.nor")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };

                return file;

            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
