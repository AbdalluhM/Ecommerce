using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Addons;
using Ecommerce.BLL.Customers.CustomerSubscriptions;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Taxes;
using Ecommerce.BLL.Validation.Customer.CustomerProduct;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.CustomerProduct;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Settings.CustomerSubscriptions;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;
using Application = Ecommerce.Core.Entities.Application;

namespace Ecommerce.BLL.Customers.CustomerProduct
{
    public class CustomerProductBLL : BaseBLL, ICustomerProductBLL
    {
        #region Fields
        IMapper _mapper;
        IRepository<VersionSubscription> _versionSubscriptionRepository;
        IRepository<Workspace> _workspaceRepository;
        IRepository<Core.Entities.Version> _vesionRepository;
        IRepository<CustomerSubscription> _customerSubscriptionRepository;
        IRepository<AddonSubscription> _addOnSubscriptionRepository;
        IRepository<License> _licenseRepository;
        IRepository<AddOn> _addOnRepository;
        IRepository<VersionRelease> _versionReleaseRepository;
        IRepository<VersionAddon> _versionAddonRepository;
        IRepository<ReasonChangeDevice> _reasonChangeDeviceRepository;
        IRepository<Invoice> _invoiceRepository;
        IRepository<DownloadVersionLog> _downloadVersionLogRepository;

        IRepository<RequestActivationKey> _requestActiveionKeyRepository;
        private readonly IRepository<VersionPrice> _versionPriceRepository;
        private readonly IRepository<Core.Entities.Version> _versionRepository;
        private readonly ITaxBLL itaxBLL;

        private readonly IRepository<PriceLevel> _priceLevelRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Customer> _customerRepository;


        ICustomerSubscriptionBLL _customerSubscriptionBLL;
        IUnitOfWork _unitOfWork;
        IRepository<RefundRequest> _refundRequestRepository;
        IAddOnBLL _addOnBLL;
        private readonly SubscriptionSetting _subscriptionSetting;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IInvoiceBLL _invoiceBLL;

        #endregion

        #region Constructor
        public CustomerProductBLL(IMapper mapper,
                                  IRepository<VersionSubscription> vesionnSubscription,
                                  IRepository<License> licenseRepository,
                                  IRepository<AddonSubscription> addOnSubscriptionRepository,
                                  IRepository<VersionAddon> versionAddonRepository,
                                  IRepository<Invoice> invoiceRepository,
                                  IUnitOfWork unitOfWork,
                                  IRepository<RequestActivationKey> requestActiveionKeyRepository,
                                  IRepository<CustomerSubscription> customerSubscriptionRepository,
                                  IRepository<RefundRequest> refundRequestRepository,
                                  IRepository<ReasonChangeDevice> reasonChangeDeviceRepository,
                                  IRepository<VersionPrice> versionPriceRepository,
                                  IRepository<Core.Entities.Version> versionRepository,
                                  IRepository<PriceLevel> priceLevelRepository,
                                  IRepository<Application> applicationRepository,
                                  ICustomerSubscriptionBLL customerSubscriptionBLL,
                                  IAddOnBLL addOnBLL,
                                  IRepository<VersionRelease> versionReleaseRepository,
                                  IRepository<DownloadVersionLog> downloadVersionLogRepository,
                                  IOptions<SubscriptionSetting> subscriptionSetting,
                                  INotificationDataBLL notificationDataBLL,
                                  IRepository<AddOn> addOnRepository,
                                  IRepository<Core.Entities.Version> vesionRepository,
                                  IInvoiceBLL invoiceBLL, ITaxBLL _itaxBLL, IRepository<Customer> customerRepository
, IRepository<Workspace> workspaceRepository)
            : base(mapper)
        {
            _mapper = mapper;
            itaxBLL = _itaxBLL;
            _versionSubscriptionRepository = vesionnSubscription;
            _licenseRepository = licenseRepository;
            _addOnSubscriptionRepository = addOnSubscriptionRepository;
            _versionAddonRepository = versionAddonRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            _requestActiveionKeyRepository = requestActiveionKeyRepository;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _refundRequestRepository = refundRequestRepository;
            _reasonChangeDeviceRepository = reasonChangeDeviceRepository;
            _versionPriceRepository = versionPriceRepository;
            _versionRepository = versionRepository;
            _priceLevelRepository = priceLevelRepository;
            _applicationRepository = applicationRepository;
            _customerSubscriptionBLL = customerSubscriptionBLL;
            _versionReleaseRepository = versionReleaseRepository;
            _addOnBLL = addOnBLL;
            _downloadVersionLogRepository = downloadVersionLogRepository;
            _subscriptionSetting = subscriptionSetting.Value;
            _notificationDataBLL = notificationDataBLL;
            _addOnRepository = addOnRepository;
            _vesionRepository = vesionRepository;
            _invoiceBLL = invoiceBLL;
            _customerRepository = customerRepository;
            _workspaceRepository = workspaceRepository;
        }
        #endregion

        #region Actions

        #region CustomerProduct
        public async Task<IResponse<GetCustomerProductOutputDto>> GetCustomerProductById(CustomerProductInputDto inputDto)
        {
            var outpput = new Response<GetCustomerProductOutputDto>();
            try
            {
                var result = new GetCustomerProductOutputDto();



                var data = await _versionSubscriptionRepository
                    .Where(x => x.Id == inputDto.VersionSubscriptionId && x.CustomerSubscription.CustomerId == inputDto.CustomerId)
                    .FirstOrDefaultAsync();

                if (data.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                    return outpput.CreateResponse(MessageCodes.PageNotAllowed, "Canceld");

                if (data != null)
                    result = _mapper.Map<GetCustomerProductOutputDto>(data);
                result.NewRelease = GetNewVersionReleaseCount(data.VersionRelease.VersionId, data.VersionReleaseId, inputDto.CustomerId);
                result.UsedDevice = GetVersionSubscriptionUsedDevice(result.VersionSubscriptionId);
                if (result == null)
                    return outpput.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));
                return outpput.CreateResponse(result);
            }
            catch (Exception e)
            {
                return outpput.CreateResponse(e);
            }
        }

        private VersionPriceDetailsDto GetVersionPrice(int versionSubscriptionId)
        {
            var versionSubscription = _versionSubscriptionRepository.GetById(versionSubscriptionId);
            if (versionSubscription != null)
            {

                decimal discountPercentage = 100 * ((versionSubscription.CustomerSubscription.Price - versionSubscription.CustomerSubscription.PriceAfterDiscount) / (versionSubscription.CustomerSubscription.Price));
                return new VersionPriceDetailsDto
                {
                    CurrencySymbol = versionSubscription.CustomerSubscription.CurrencyName,
                    Discrimination = GetVersionSubscriptionDiscriminator(versionSubscription.CustomerSubscription.SubscriptionTypeId, versionSubscription.CustomerSubscription.RenewEvery),
                    DiscountPercentage = discountPercentage,
                    NetPrice = versionSubscription.CustomerSubscription.PriceAfterDiscount,
                    PriceBeforeDiscount = versionSubscription.CustomerSubscription.Price,

                };
            }

            else
            {
                return new VersionPriceDetailsDto();
            }
        }
        private string GetVersionSubscriptionDiscriminator(int subscriptionTypeId, int renewEvery)
        {
            if (subscriptionTypeId == (int)SubscriptionTypeEnum.Forever)
                return DiscriminatorsEnum.Forever.ToString();
            else if (subscriptionTypeId == (int)SubscriptionTypeEnum.Others && renewEvery == 30)
                return DiscriminatorsEnum.Monthly.ToString();
            else if (subscriptionTypeId == (int)SubscriptionTypeEnum.Others && renewEvery == 365)
                return DiscriminatorsEnum.Yearly.ToString();
            return string.Empty;

        }
        public async Task<IResponse<List<GetCustomerProductOutputDto>>> GetAllCustomerProducts(int customerId)
        {
            var outpput = new Response<List<GetCustomerProductOutputDto>>();
            try
            {

                var customerVersionSubscriptions = _versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customerId &&
                        x.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.InvoiceStatusId != (int)InvoiceStatusEnum.Refunded)).ToList();

                var result = new List<GetCustomerProductOutputDto>();

                customerVersionSubscriptions.ForEach(x =>
                {
                    var versionPrice = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionPriceId);
                    var versionRelease = _versionReleaseRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionReleaseId);
                    var version = _vesionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(v => v.Id == versionPrice.VersionId).FirstOrDefault();
                    var priceLevel = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == versionPrice.PriceLevelId);
                    var application = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == version.ApplicationId);
                    result.Add(new GetCustomerProductOutputDto
                    {
                        ApplicationName = application.Title,
                        VersionName = x.VersionName,
                        VersionDescription = version.ShortDescription,
                        Currency = x.CustomerSubscription.CurrencyName,
                        CustomerSubscriptionId = x.CustomerSubscriptionId,
                        IsDefault = false,
                        NumberOfLicenses = x.CustomerSubscription.NumberOfLicenses,
                        PriceLevel = priceLevel.Name,
                        PriceLevelId = priceLevel.Id,
                        ProductId = x.VersionPrice.VersionId,
                        ReleaseNumber = versionRelease.ReleaseNumber,
                        VersionReleaseId = versionRelease.Id,
                        VersionSubscriptionId = x.Id,
                        Logo = _mapper.Map<FileStorageDto>(version.Image),
                        RenewDate = _addOnBLL.GetVersionNextRenewDate(x),
                        UsedDevice = GetVersionSubscriptionUsedDevice(x.Id),
                        Price = GetVersionPrice(x.Id),
                        NewRelease = GetNewVersionReleaseCount(version.Id, x.VersionReleaseId, customerId),
                    });

                });



                if (result == null)
                    result = new List<GetCustomerProductOutputDto>();


                return outpput.CreateResponse(result);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }
        }
        public async Task<IResponse<NearestVersionRenewDto>> GetFirstRenewDateOfAllVersionsOfUser(int customerId)
        {

            var outpput = new Response<NearestVersionRenewDto>();
            try
            {



                var invoices = _invoiceRepository
                         .Where(x => x.CustomerSubscription.CustomerId == customerId)
                         .Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || (i.InvoiceTypeId == (int)InvoiceTypes.Support))
                         .ToList();

                if (invoices.Count == 0 || !invoices.Any())
                    return outpput.CreateResponse(new NearestVersionRenewDto());

                var nearestInvoice = invoices
                                      .GroupBy(i => i.CustomerSubscriptionId)
                                      .Select(g => g.OrderByDescending(x => x.EndDate).FirstOrDefault())
                                      .OrderBy(x => x.EndDate).FirstOrDefault();

                if (nearestInvoice.EndDate <= DateTime.UtcNow.AddDays(30) || nearestInvoice.InvoiceTypeId == (int)InvoiceTypes.Support)
                {

                    return outpput.CreateResponse(_mapper.Map<NearestVersionRenewDto>(nearestInvoice));

                }




                return outpput.CreateResponse(new NearestVersionRenewDto());

            }
            catch (Exception e)
            {
                return outpput.CreateResponse(e);
            }




        }
        public async Task<IResponse<List<GetCustomerProductOutputDto>>> GetCustomerSubscriptions(int customerId)
        {
            var outpput = new Response<List<GetCustomerProductOutputDto>>();
            try
            {

                var customerVersionSubscriptions = _versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customerId &&
                        x.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.InvoiceStatusId != (int)InvoiceStatusEnum.Refunded)).ToList();

                var result = new List<GetCustomerProductOutputDto>();
                customerVersionSubscriptions.ForEach(x =>
                {
                    var versionPrice = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionPriceId);
                    var versionRelease = _versionReleaseRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionReleaseId);
                    var version = _vesionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(v => v.Id == versionPrice.VersionId).FirstOrDefault();
                    var priceLevel = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == versionPrice.PriceLevelId);
                    var application = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == version.ApplicationId);
                    result.Add(new GetCustomerProductOutputDto
                    {
                        AppId = application.Id,
                        ApplicationName = application.Title,
                        VersionName = x.VersionName,
                        VersionDescription = version.ShortDescription,
                        Currency = x.CustomerSubscription.CurrencyName,
                        CustomerSubscriptionId = x.CustomerSubscriptionId,
                        IsDefault = false,
                        NumberOfLicenses = x.CustomerSubscription.NumberOfLicenses,
                        PriceLevel = priceLevel.Name,
                        PriceLevelId = priceLevel.Id,
                        ProductId = x.VersionPrice.VersionId,
                        ReleaseNumber = versionRelease.ReleaseNumber,
                        VersionReleaseId = versionRelease.Id,
                        VersionSubscriptionId = x.Id,
                        Logo = _mapper.Map<FileStorageDto>(version.Image),
                        RenewDate = _addOnBLL.GetVersionNextRenewDate(x),
                        UsedDevice = GetVersionSubscriptionUsedDevice(x.Id),
                        Price = GetVersionPrice(x.Id),
                        NewRelease = GetNewVersionReleaseCount(version.Id, x.VersionReleaseId, customerId),
                    });

                });
                var res = result.GroupBy(e => e.AppId).Select(g => new GetCustomerProductOutputDto
                {
                    AppId = g.Key,
                    VersionName = g.First().VersionName,
                    ApplicationName = g.First().ApplicationName,
                    VersionDescription = g.First().VersionDescription,
                    Currency = g.First().Currency,
                    CustomerSubscriptionId = g.First().CustomerSubscriptionId,
                    IsDefault = g.First().IsDefault,
                    NumberOfLicenses = g.Sum(e => e.NumberOfLicenses),
                    PriceLevel = g.First().PriceLevel,
                    PriceLevelId = g.First().PriceLevelId,
                    ProductId = g.First().ProductId,
                    ReleaseNumber = g.First().ReleaseNumber,
                    VersionReleaseId = g.First().VersionReleaseId,
                    VersionSubscriptionId = g.First().VersionSubscriptionId,
                    Logo = g.First().Logo,
                    RenewDate = g.First().RenewDate,
                    UsedDevice = g.Sum(x => x.UsedDevice),
                    Price = g.First().Price,
                    NewRelease = g.Sum(e => e.NewRelease),
                }).ToList();


                if (res == null)
                    res = new List<GetCustomerProductOutputDto>();


                return outpput.CreateResponse(res);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }
        }
        public async Task<IResponse<List<GetCustomerApplicationVersionsDto>>> GetCustomerApplicationVersions(int customerId, int appId)
        {
            var outpput = new Response<List<GetCustomerApplicationVersionsDto>>();
            try
            {

                var versions = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(v => v.ApplicationId == appId).ToList();

                var versionSubscriptions = _versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customerId &&
                        x.CustomerSubscription.Invoices.Any(
                                e => e.InvoiceStatusId == (int)InvoiceStatusEnum.Paid))
                            .Include(x => x.VersionPrice).ToList();



                var list = _mapper.Map<List<GetCustomerApplicationVersionsDto>>(versions);

                List<GetCustomerApplicationVersionsDto> result = new List<GetCustomerApplicationVersionsDto>();
                foreach (var item in list)
                {
                    IEnumerable<VersionSubscription> versionSubscription = versionSubscriptions.Where(x => x.VersionPrice.VersionId == item.Id);

                    if (!versionSubscription.Any())
                    {
                        result.Add(item);
                    }
                    else
                    {
                        result.AddRange(versionSubscription.Select(vs => new GetCustomerApplicationVersionsDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Logo = item.Logo,
                            CanCreate = vs.CustomerSubscription.NumberOfLicenses > vs.CustomerSubscription.Licenses.Count,
                            Purshased = true,
                            RenewDate = _addOnBLL.GetVersionNextRenewDate(vs),
                            VersionSubscriptionId = vs.Id,
                            LicenseCount = vs.CustomerSubscription.Licenses.Count,
                            NumberOfLicence = vs.CustomerSubscription.NumberOfLicenses,
                            SubscriptionDate = vs.CreateDate,
                            MinVersionPrice = item.MinVersionPrice,
                            LongDescription = item.LongDescription,
                            ShortDescription = item.ShortDescription,
                            SubscriptionType = item.SubscriptionType,
                            HasOpenRefundRequest = _refundRequestRepository.Where(e => e.InvoiceId == vs.CustomerSubscription.Invoices.Select(i => i.Id).LastOrDefault()).Any(i => i.RefundRequestStatusId != (int)RefundRequestStatusEnum.Refused),
                            HasPaiedAddons = vs.AddonSubscriptions.Any(a => a.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)),

                        }));
                    }


                }

                return outpput.CreateResponse(result);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }


        }

        public async Task<IResponse<List<CustomerProductLookupDto>>> GetCustomerProductByAppIdLookup(int customerId, int appId)
        {
            var response = new Response<List<CustomerProductLookupDto>>();

            try
            {
                var versionSubscriptions = _versionSubscriptionRepository
                                                                .Where(x => _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(a => a.Id ==
                                                                                             _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(v => v.Id ==
                                                                                              _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionPriceId).VersionId).ApplicationId).Id == appId &&
                                                                                              x.CustomerSubscription.CustomerId == customerId &&
                                                                        x.CustomerSubscription.Invoices
                                                                        .OrderByDescending(x => x.CreateDate)
                                                                        .FirstOrDefault().InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                                         .ToList();

                var productsDto = _mapper.Map<List<CustomerProductLookupDto>>(versionSubscriptions);



                return response.CreateResponse(productsDto);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<List<CustomerProductLookupDto>>> GetAllCustomerProductsLookupAsync(int customerId, int? appId = null)
        {
            var response = new Response<List<CustomerProductLookupDto>>();

            try
            {
                var products = await _versionSubscriptionRepository.Where(x => (appId == null || _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(a => a.Id ==
                _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(v => v.Id ==
                _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == x.VersionPriceId).VersionId).ApplicationId).Id == appId) &&
                                                                                   x.CustomerSubscription.CustomerId == customerId &&
                                                                                    x.CustomerSubscription.Invoices
                                                                                     .OrderByDescending(x => x.CreateDate)
                                                                                     .FirstOrDefault().InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                                                     .ToListAsync()
                                                                                    ;



                var versionsWithCRM = await _versionRepository
                    .DisableFilter(nameof(DynamicFilters.IsActive))
                    .Where(x =>
                           x.ProductCrmId != null &&
                           x.VersionPrices.Any(vp => products.Select(p => p.VersionPriceId)
                    .ToList()
                    .Contains(vp.Id)))
                    .ToListAsync();

                var productsDto = _mapper.Map<List<CustomerProductLookupDto>>(products);

                foreach (var item in productsDto)
                {
                    string crmId = versionsWithCRM
                     .Where(x =>
                            x.ProductCrmId != null &&
                            x.VersionPrices.Any(vp => item.VersionPriceId == vp.Id))
                     .FirstOrDefault()?.ProductCrmId;

                    item.CrmId = int.TryParse(crmId, out int currentCrmId) ? currentCrmId + "" : string.Empty;
                }

                productsDto.RemoveAll(x => string.IsNullOrWhiteSpace(x.CrmId));

                return response.CreateResponse(productsDto);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }
        public async Task<IResponse<List<CustomerProductWorkspacesDto>>> GetAllCustomerProductsWorkspacesLookupAsync(int customerId)
        {
            var response = new Response<List<CustomerProductWorkspacesDto>>();

            try
            {
                var versionSubscriptionUsedIds = _workspaceRepository.Where(w => w.CustomerId == customerId).Select(w => w.VersionSubscriptionId).AsEnumerable();
                var products = await _versionSubscriptionRepository.Where(x => !versionSubscriptionUsedIds.Contains(x.Id) &&
                                                                                   x.CustomerSubscription.CustomerId == customerId &&
                                                                                    x.CustomerSubscription.Invoices
                                                                                     .OrderByDescending(x => x.CreateDate)
                                                                                     .FirstOrDefault().InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                                                     .ToListAsync()
                                                                                    ;


                var productsDto = _mapper.Map<List<CustomerProductWorkspacesDto>>(products);

                return response.CreateResponse(productsDto);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<List<CustomerProductLookupDto>>> GetAllCustomerProductsLookupAsync(int customerId, int deviceTypeId)
        {
            var response = new Response<List<CustomerProductLookupDto>>();

            try
            {

                var products = await _versionSubscriptionRepository.Where(x =>
                                   _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                   .Where(e => e.DeviceTypeId == deviceTypeId)
                                   .SelectMany(a => _versionRepository
                                       .DisableFilter(nameof(DynamicFilters.IsActive)).Where(e => e.ApplicationId == a.Id).SelectMany(v =>
                                           _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                               .Where(vp => vp.VersionId == v.Id).Select(e => e.Id))).Contains(x.VersionPriceId) &&

                                                                               x.CustomerSubscription.CustomerId == customerId &&
                                                                               x.CustomerSubscription.Invoices.Any(e => e.InvoiceStatusId == (int)InvoiceStatusEnum.Paid))
                    .ToListAsync();


                var productsDto = _mapper.Map<List<CustomerProductLookupDto>>(products);



                return response.CreateResponse(productsDto);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<List<CustomerApplicationLookupDto>>> GetCustomerApplicationsLookupAsync(int customerId)
        {
            var response = new Response<List<CustomerApplicationLookupDto>>();
            try
            {
                var versionSubscribtions = _versionSubscriptionRepository
                .Where(a => a.CustomerSubscription.CustomerId == customerId && a.VersionPrice != null && a.VersionPrice.Version != null &&
                                             a.CustomerSubscription.Invoices
                                                                     .OrderByDescending(x => x.CreateDate)
                                                                     .FirstOrDefault().InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                .Select(vs => _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(a => a.Id ==
                _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(v => v.Id ==
                _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(vp => vp.Id == vs.VersionPriceId).VersionId).ApplicationId))
                .ToList();

                var mappedVersion = _mapper.Map<List<CustomerApplicationLookupDto>>(versionSubscribtions);

                return response.CreateResponse(mappedVersion);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }



        }
        #endregion

        #region MangeLicense         
        //TODO : add 14 days in appSetting
        private bool CanCancelSubscription(int customerSubscriptionId)
        {
            //if has more than one invoice on subscription return false
            if (_invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscriptionId).Count() > 1)
                return false;

            var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscriptionId)
                                            .FirstOrDefault(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid);

            if (invoice is null)
                return false;

            //invoice has any refund request with open status
            var hasRefundRequests = _refundRequestRepository.Any(x => x.InvoiceId == invoice.Id && x.RefundRequestStatusId != (int)RefundRequestStatusEnum.Refused);
            var dd = invoice.StartDate.Add(_subscriptionSetting.CancellationPeriod);
            return (dd > DateTime.UtcNow && !hasRefundRequests);
        }

        public async Task<IResponse<GetCustomerSubscriptionOutputDto>> GetCustomerSubscriptionById(CutomerSubscriptionInputDto inputDto)
        {
            var outpput = new Response<GetCustomerSubscriptionOutputDto>();
            try
            {

                //get version without filter
                var vbs = await _versionSubscriptionRepository.GetByIdAsync(inputDto.VersionSubscriptionId);

                if (vbs.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                    return outpput.CreateResponse(MessageCodes.PageNotAllowed, "Canceled");

                //Business Validation 
                //check entity is exisit
                if (vbs.CustomerSubscription == null)
                    return outpput.CreateResponse(MessageCodes.NotFound, nameof(CustomerSubscription));

                //Get last invoice paid
                var invoice = await _invoiceRepository
                    .Where(x => x.CustomerSubscriptionId == vbs.CustomerSubscription.Id && x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                    .OrderBy(i => i.Id)
                    .LastOrDefaultAsync();

                if (invoice is null)
                {
                    outpput.CreateResponse(MessageCodes.NotFound, nameof(invoice));
                    return outpput;
                }

                //Map entity
                var result = _mapper.Map<GetCustomerSubscriptionOutputDto>(invoice);
                result.CanCancel = CanCancelSubscription(vbs.CustomerSubscription.Id);

                if (result.EndDate < DateTime.UtcNow)
                    result.IsExpired = true;

                return outpput.CreateResponse(result);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }
        }
        public async Task<IResponse<bool>> UpdateCustomerSubscription(UpdateCutomerSubscriptionInputDto inputDto)
        {
            var outpput = new Response<bool>();
            try
            {
                var customerSubscription = await _versionSubscriptionRepository
                    .Where(x => x.Id == inputDto.VersionSubscriptionId && x.CustomerSubscription.CustomerId == inputDto.CustomerId)
                    .Select(x => x.CustomerSubscription).FirstOrDefaultAsync();
                if (customerSubscription == null)
                    return outpput.CreateResponse(MessageCodes.NotFound, nameof(CustomerSubscription));
                customerSubscription.AutoBill = inputDto.AutoBill;
                _unitOfWork.Commit();
                return outpput.CreateResponse(true);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }
        }

        public async Task<IResponse<bool>> RefundSubscriptionRequest(RefundRequestInputDto inputDto)
        {
            var outpput = new Response<bool>();
            try
            {
                //Get CustomerSubscription
                var customerSubscription = await _versionSubscriptionRepository
                    .Where(x => x.Id == inputDto.VersionSubscriptionId && x.CustomerSubscription.CustomerId == inputDto.CustomerId)
                    .Select(x => x.CustomerSubscription).FirstOrDefaultAsync();
                //Business Validation 

                //check entity is exisit
                if (customerSubscription == null)
                    return outpput.CreateResponse(MessageCodes.NotFound, nameof(CustomerSubscription));

                //check if there is related addon's not refunded
                if (_invoiceBLL.IsVersionHasNotRefundedAddons(customerSubscription.VersionSubscriptions.FirstOrDefault()))
                {
                    return outpput.CreateResponse(MessageCodes.RelatedAddonNotRefunded);
                }
                //Get First invoice paid
                var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscription.Id)
                    .FirstOrDefault(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid);
                if (CanCancelSubscription(customerSubscription.Id))
                {
                    RefundRequest _RefundRequest = new RefundRequest
                    {
                        InvoiceId = invoice.Id,
                        Reason = inputDto.Reason,
                        CreateDate = DateTime.UtcNow,
                        RefundRequestStatusId = (int)RefundRequestStatusEnum.Open,

                    };
                    _refundRequestRepository.Add(_RefundRequest);
                    invoice.InvoiceStatusId = (int)InvoiceStatusEnum.Paid;
                    _unitOfWork.Commit();


                    //push new notification db
                    var _notificationItem = new GetNotificationForCreateDto();
                    _notificationItem.CustomerId = customerSubscription.CustomerId;
                    _notificationItem.IsAdminSide = true;
                    _notificationItem.IsCreatedBySystem = false;
                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.RequestCancelation;
                    _notificationItem.NotificationActionSubTypeId = (int)RequestCancelationTypeEnum.App;
                    _notificationItem.InvoiceId = invoice.Id;
                    _notificationItem.RefundRequestId = _RefundRequest.Id;
                    await _notificationDataBLL.CreateAsync(_notificationItem);


                    return outpput.CreateResponse(true);
                }


                return outpput.CreateResponse(MessageCodes.ExceededPeriod);
            }
            catch (Exception e)
            {

                return outpput.CreateResponse(e);
            }
        }
        #endregion

        #region CustomerProductRelease
        public async Task<IResponse<List<GetReleasesOutputDto>>> GetCustomerProductReleases(CustomerProductInputDto inputDto)
        {
            var output = new Response<List<GetReleasesOutputDto>>();
            try
            {

                var versionSubscription = await _versionSubscriptionRepository.GetByIdAsync(inputDto.VersionSubscriptionId);

                if (versionSubscription.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                    return output.CreateResponse(MessageCodes.UnAuthorizedAccess);

                var release = await _versionReleaseRepository.GetByIdAsync(versionSubscription.VersionReleaseId);

                var allReleases = _versionReleaseRepository.Where(x => x.VersionId == release.VersionId).ToList();

                var downloadRelases = _downloadVersionLogRepository.Where(x => x.CustomerId == inputDto.CustomerId &&
                allReleases.Select(x => x.Id).Contains(x.VersionIdReleaseId)).OrderBy(x => x.VersionIdReleaseId).Select(x => x.VersionIdReleaseId).ToList();

                var latestReleas = _versionReleaseRepository.Where(x => x.IsCurrent == true && x.VersionId == release.VersionId)
                    .FirstOrDefault();
                var mappedResult = _mapper.Map<List<GetReleasesOutputDto>>(allReleases);
                //TODO:Upate with real values
                mappedResult.ForEach(x =>
                {
                    var invoiceStatus =
                   _customerSubscriptionBLL.GetSubscriptionStatus(versionSubscription.CustomerSubscriptionId);


                    x.SubscriptionDate = versionSubscription.CustomerSubscription.CreateDate;
                    x.RenewDate = invoiceStatus.EndDate.Value;
                    x.LicenseCount = versionSubscription.CustomerSubscription.Licenses.Count;

                    //Get IsLatest
                    if (x.Id > release.Id)
                    {
                        if (downloadRelases.Any())
                        {
                            if (x.Id > downloadRelases.LastOrDefault() && x.Id == latestReleas.Id)
                            {
                                x.IsCurrent = false;
                                x.IsDownload = false;
                                x.IsLatest = true;
                            }
                            else if (x.Id == downloadRelases.LastOrDefault())
                            {
                                x.IsDownload = true;
                                x.IsLatest = false;
                                x.IsCurrent = true;
                            }
                            else if (x.Id < downloadRelases.LastOrDefault())
                            {
                                x.IsDownload = true;
                            }
                        }
                        else if (!downloadRelases.Any() && x.Id == latestReleas.Id)
                        {
                            x.IsLatest = true;
                        }

                    }
                    else if (x.Id == release.Id)
                    {
                        if (!downloadRelases.Any())
                        {
                            x.IsDownload = true;
                            x.IsLatest = false;
                            x.IsCurrent = true;
                        }
                    }
                    else if (x.Id < release.Id)
                    {
                        x.IsLatest = false;
                        x.IsCurrent = false;
                        x.IsDownload = false;
                        if (downloadRelases.Contains(x.Id))
                        {
                            x.IsDownload = true;
                        }

                    }




                    //2- Check EndDate for Invoice
                    if (invoiceStatus.IsValid)
                    {
                        x.CanDownload = true;
                    }
                    else if (!invoiceStatus.IsValid && release.Id >= x.Id)
                    {
                        x.CanDownload = true;
                    }
                    else
                    {
                        x.CanDownload = false;
                    }

                }
                );
                return output.CreateResponse(mappedResult);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<bool>> DownloadRelease(VersionReleaseInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {

                var entity = await _versionReleaseRepository.GetByIdAsync(inputDto.VersionReleaseId);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionRelease));

                entity.Version.DownloadCount += 1;
                if (!entity.DownloadVersionLogs.Any(x => x.CustomerId == inputDto.CustomerId && x.VersionIdReleaseId == inputDto.VersionReleaseId))
                {
                    entity.DownloadVersionLogs.Add(new DownloadVersionLog
                    {
                        CustomerId = inputDto.CustomerId,
                        VersionIdReleaseId = inputDto.VersionReleaseId,
                        Ipaddress = inputDto.IpAddress,
                        CreateDate = DateTime.UtcNow
                    });
                }

                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region CustomerProductAddOn
        public async Task<IResponse<List<GetAllCustomerProductAddonOutputDto>>> GetCustomerProductAddOns(CustomerProductInputDto inputDto)
        {
            var output = new Response<List<GetAllCustomerProductAddonOutputDto>>();
            try
            {
                List<GetAllCustomerProductAddonOutputDto> allAddOns = new();
                //get versionsubscription by id
                var vps = await _versionSubscriptionRepository.GetByIdAsync(inputDto.VersionSubscriptionId);
                if (vps == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));
                //get version
                var versionId = _versionReleaseRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(vr => vr.Id == vps.VersionReleaseId)
                    .Select(x => x.VersionId).FirstOrDefault();
                var version = await _vesionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                .Where(x => x.Id == versionId).FirstOrDefaultAsync();
                ;


                if (version.Id > 0)
                {
                    var sourceAddOnsDb = await _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                        .Where(x => x.VersionId == version.Id && x.Version != null &&
                                              x.Addon != null)
                        .Select(x => x.Addon)
                        .Where(x => x.AddOnPrices.Any(a => a.PriceLevelId == vps.VersionPrice.PriceLevelId/* &&
                        a.CountryCurrency.CountryId == inputDto.CountryId*/))
                        .ToListAsync();
                    //get all addons on version
                    allAddOns = _mapper.Map<List<GetAllCustomerProductAddonOutputDto>>(sourceAddOnsDb);

                    //get all purchased addons on version for customer
                    var addOnPurshasedIds = await _addOnRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                        .Where(a => a.AddOnPrices.Any(x => x.AddonSubscriptions.Any(x =>
                                        x.CustomerSubscription.CustomerId == inputDto.CustomerId &&
                                        x.VersionSubscriptionId == inputDto.VersionSubscriptionId &&
                                        x.VersionSubscription != null && x.AddonPrice != null &&
                                        x.AddonPrice.AddOn != null /*& x.AddonPrice.CountryCurrency.CountryId == inputDto.CountryId*/ &&
                                        x.CustomerSubscription.Invoices
                                        .Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid && x.InvoiceStatusId != (int)InvoiceStatusEnum.Refunded
                                        ))))
                        .Select(x => x.Id)
                        .ToListAsync();


                    if (allAddOns.Any())
                    {
                        //flag purchased addons and leave not purchased
                        allAddOns.ForEach(async a =>
                        {
                            a.VersionId = version.Id;
                            if (addOnPurshasedIds.Contains(a.AddOnId))
                            {

                                var addOnSubscription = _addOnSubscriptionRepository
                                                                                                   .Where(aps => aps.VersionSubscriptionId == vps.Id &&
                                                                                                    aps.AddonPrice.AddOnId == a.AddOnId &&
                                                                                                    aps.CustomerSubscription.Invoices.Any(inv =>
                                                                                                    inv.InvoiceStatusId == (int)InvoiceStatusEnum.Paid &&
                                                                                                    inv.InvoiceStatusId != (int)InvoiceStatusEnum.Refunded))
                                                                                                    .FirstOrDefault();


                                var license = addOnSubscription?.CustomerSubscription?.Licenses?.OrderByDescending(o => o.CreateDate).FirstOrDefault();
                                var invoice = _invoiceRepository
                                        .Where(x => x.CustomerSubscriptionId == addOnSubscription.CustomerSubscription.Id &&
                                        (x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid))
                                        .OrderByDescending(x => x.Id)
                                        .FirstOrDefault();
                                a.AddonSubscriptionId = addOnSubscription.Id;
                                a.IsPurshased = true;
                                //Purchased AddOns
                                a.PaymentCountryId = invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonPrice.CountryCurrency.CountryId;
                                a.PurshasedData = new GetAddOnPurshasedOutputDto
                                {
                                    AddonSubscriptionId = a.AddonSubscriptionId,
                                    RenewalDate = invoice?.EndDate,
                                    LicenseRenewalDate = license?.RenewalDate,
                                    StatusId = license?.LicenseStatusId ?? default,
                                    RenwalEvery = addOnSubscription.CustomerSubscription.RenewEvery,

                                };
                                a.PurshasedData.File = _mapper.Map<FileStorageDto>(license?.ActivationFile);
                                a.Price = new AddonPriceDetailsDto
                                {
                                    PaymentCountryId = a.PaymentCountryId.GetValueOrDefault(0),
                                    DiscountPercentage =
                                    (addOnSubscription.CustomerSubscription.Price - addOnSubscription.CustomerSubscription.PriceAfterDiscount)
                                    * 100 / (addOnSubscription.CustomerSubscription.Price),

                                    PriceBeforeDiscount = addOnSubscription.CustomerSubscription.Price,
                                    NetPrice = addOnSubscription.CustomerSubscription.PriceAfterDiscount,
                                    CurrencySymbol = addOnSubscription.CustomerSubscription.CurrencyName,
                                    Discrimination = GetInvoiceDiscriminator(addOnSubscription.CustomerSubscription.Invoices.FirstOrDefault()?.InvoiceTypeId ?? 0, addOnSubscription.CustomerSubscription.RenewEvery),
                                };
                                a.CreatedDate = addOnSubscription.CreateDate;
                                a.CanCancle = CanCancelSubscription(addOnSubscription.CustomerSubscriptionId);
                            }
                            else
                            {
                                var isActiveAddOn = _addOnRepository.Where(x =>
                                x.AddOnPrices.Any(ap => ap.PriceLevelId == vps.VersionPrice.PriceLevelId && vps.VersionPrice != null && ap.PriceLevel != null)
                              && x.VersionAddons.Any(va => va.Version != null && va.Version.Application != null && va.VersionId == vps.VersionPrice.VersionId))
                                 .FirstOrDefault();
                                //Not Purchased AddOns
                                a.PaymentCountryId = inputDto.CountryId;

                                if (isActiveAddOn != null)
                                {
                                    //TODO:Recheck && Refactor this
                                    a.AllPrices = _addOnBLL.GetAddonPriceByPriceLevel(a.AddOnId, inputDto.CountryId, vps.VersionPrice.PriceLevelId).GetAwaiter().GetResult().Data.Price;
                                    //Refactored to get lowest price
                                    a.Price = a.AllPrices != null ? _mapper.Map<AddonPriceDetailsDto>(a.AllPrices?.Monthly) : null;
                                    #region Old(Get Price According To ApplicationSubscription)
                                    //if (vps.CustomerSubscription.SubscriptionTypeId == (int)SubscriptionTypeEnum.Forever)
                                    //{
                                    //    a.Price = a.AllPrices != null ? _mapper.Map<AddonPriceDetailsDto>(a.AllPrices.Forever) : null;
                                    //}
                                    //else if (vps.CustomerSubscription.SubscriptionTypeId == (int)SubscriptionTypeEnum.Others && vps.CustomerSubscription.RenewEvery == 365)
                                    //{
                                    //    a.Price = a.AllPrices != null ? _mapper.Map<AddonPriceDetailsDto>(a.AllPrices.Yearly) : null;
                                    //}
                                    //else
                                    //{
                                    //    a.Price = a.AllPrices != null ? _mapper.Map<AddonPriceDetailsDto>(a.AllPrices?.Monthly) : null;
                                    //}
                                    #endregion
                                    a.Price.PaymentCountryId = a.Price != null ? inputDto.CountryId : 0;
                                    a.PriceLevelId = a.AllPrices?.PriceLevelId ?? 0;
                                }
                                else
                                {
                                    a.IsActive = false;
                                }
                            }


                        });

                        //remove all addons that not available for buy .. not have addonprice on current user countrycurrencyid
                        allAddOns.RemoveAll(x => !x.IsPurshased && ((x.Price == null || x.PriceLevelId == 0) || !x.IsActive));
                    }
                    else
                    {
                        return output.CreateResponse(new List<GetAllCustomerProductAddonOutputDto>() { });
                    }
                }
                else
                {
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));
                }

                return output.CreateResponse(allAddOns);

            }

            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }

        public async Task<IResponse<ProductAddonDetailsDto>> GetProductAddonDetailsAsync(int addonSubscriptionId)
        {
            var response = new Response<ProductAddonDetailsDto>();
            try
            {
                var addonSubscription = await _addOnSubscriptionRepository.GetAsync(a => a.Id == addonSubscriptionId);
                if (addonSubscription is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(AddonSubscription));

                if (addonSubscription.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Refunded))
                    return response.CreateResponse(MessageCodes.PageNotAllowed, "Canceled");

                if (addonSubscription is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(addonSubscriptionId));

                var addonDetailsDto = _mapper.Map<ProductAddonDetailsDto>(addonSubscription);

                addonDetailsDto.CanCancelSubscription = CanCancelSubscription(addonSubscription.CustomerSubscriptionId);

                return response.CreateResponse(addonDetailsDto);
            }
            catch (Exception e)
            {

                return response.CreateResponse(e);
            }

        }

        public async Task<IResponse<bool>> CancelAddonSubscriptionAsync(CancelAddonSubscriptionDto cancelAddonSubscription)
        {
            var response = new Response<bool>();

            try
            {
                var validation = await new CancelAddonSubscriptionDtoValidator().ValidateAsync(cancelAddonSubscription);

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                var addonSubscription = await _addOnSubscriptionRepository.GetByIdAsync(cancelAddonSubscription.AddonSubscriptionId);

                if (addonSubscription is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(cancelAddonSubscription.AddonSubscriptionId));

                if (addonSubscription.CustomerSubscription.Invoices.Count > 1)
                    return response.CreateResponse(MessageCodes.HasMoreThanOneInvoice, nameof(cancelAddonSubscription.AddonSubscriptionId));

                if (!CanCancelSubscription(addonSubscription.CustomerSubscriptionId))
                    return response.CreateResponse(MessageCodes.ExceededPeriod, nameof(cancelAddonSubscription.AddonSubscriptionId));

                var request = _mapper.Map<RefundRequest>(cancelAddonSubscription);

                var firstInvoice = addonSubscription.CustomerSubscription.Invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                                                  .OrderBy(i => i.CreateDate)
                                                                                  .FirstOrDefault();

                request.InvoiceId = firstInvoice.Id;

                await _refundRequestRepository.AddAsync(request);

                await _unitOfWork.CommitAsync();


                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = addonSubscription.CustomerSubscription.CustomerId;
                _notificationItem.IsAdminSide = true;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.RequestCancelation;
                _notificationItem.NotificationActionSubTypeId = (int)RequestCancelationTypeEnum.Addon;
                _notificationItem.InvoiceId = request.InvoiceId;
                _notificationItem.RefundRequestId = request.Id;
                await _notificationDataBLL.CreateAsync(_notificationItem);


                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }
        #endregion

        #region Helpers
        public int GetVersionSubscriptionUsedDevice(int versionSubscriptionId)
        {
            var customerSubscription = _versionSubscriptionRepository.GetById(versionSubscriptionId)?.CustomerSubscriptionId ?? 0;
            return _licenseRepository.Where(x => x.CustomerSubscriptionId == customerSubscription).Count();
        }
        public int GetNewVersionReleaseCount(int versionId, int versionReleaseId, int customerId)
        {
            //Get AllVersionReleases
            var allReleases = _versionReleaseRepository
                   .Where(x => x.VersionId == versionId).ToList();
            //Get Purshased Release
            var release = _versionReleaseRepository.GetById(versionReleaseId);
            //Get All Releases Download
            var releasesDownload = _downloadVersionLogRepository.Where(x => x.CustomerId == customerId)
                .Select(x => x.VersionIdReleaseId).ToList();
            int result = 0;
            allReleases.ForEach(x =>
            {
                if (x.Id > release.Id)
                {
                    if (!releasesDownload.Contains(x.Id))
                    {
                        result += 1;
                    }
                    else if (x.Id > releasesDownload.LastOrDefault())
                    {
                        result += 1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
            });
            return result;
        }
        public string GetInvoiceDiscriminator(int invoiceTypeId, int renewEvery)
        {
            if (invoiceTypeId != (int)InvoiceTypes.Renewal) //this is for for support and forever subscription
                return nameof(DiscriminatorsEnum.Forever);
            else if (invoiceTypeId == (int)InvoiceTypes.Renewal && renewEvery == 30)
                return nameof(DiscriminatorsEnum.Monthly);
            else if (invoiceTypeId == (int)InvoiceTypes.Renewal && renewEvery == 365)
                return nameof(DiscriminatorsEnum.Yearly);
            return string.Empty;

        }

        public List<GetSubscriptionStatusDto> GetSubscriptionStatus(List<int> customerSubscriptionId)
        {
            var output = new List<GetSubscriptionStatusDto>();
            foreach (var customerSubscribtion in customerSubscriptionId)
            {
                var result = new GetSubscriptionStatusDto();
                var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscribtion &&
                                                                     x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                     )
                                            .OrderByDescending(x => x.CreateDate).FirstOrDefault();

                if (invoice != null && invoice.EndDate > DateTime.UtcNow)
                {
                    result.CustomerSubscriptionId = customerSubscribtion;
                    result.IsValid = true;
                    result.EndDate = invoice.EndDate;
                }
                else
                {
                    result.CustomerSubscriptionId = customerSubscribtion;
                    result.IsValid = false;
                    result.EndDate = null;
                }
                output.Add(result);
            }

            return output;
        }


        #endregion
        #endregion
    }
}
