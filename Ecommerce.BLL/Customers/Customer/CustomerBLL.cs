using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Customers.Auth;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Customer;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.Helper.Security;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Customer.Customers.CustomerValidator;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Customers
{
    public class CustomerBLL : BaseBLL, ICustomerBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Core.Entities.Version> _versionRepository;
        IRepository<AddOn> _addOnRepository;
        IRepository<Customer> _customerRepository;
        IRepository<Industry> _industryRepository;
        IRepository<CompanySize> _companySizeRepository;
        IRepository<CustomerEmail> _customerEmailRepository;
        IRepository<CustomerMobile> _customerMobileRepository;
        IRepository<RefreshToken> _refreshTokenRepository;
        IRepository<CustomerEmailVerification> _customerEmailVerficationRepository;
        IRepository<CustomerSmsverification> _customerSmsVerficationRepository;
        IRepository<CustomerSubscription> _customerSubscriptionRepository;
        IRepository<Invoice> _invoiceRepository;
        IRepository<VersionSubscription> _versionSubscriptionRepository;
        IRepository<AddonSubscription> _addonSubscriptionRepository;
        IRepository<License> _licenseRepository;
        IRepository<Employee> _employeeRepository;
        IRepository<Country> _countryRepository;
        IRepository<Currency> _currencyRepository;
        IRepository<CountryCurrency> _countrycurrencyRepository;
        IRepository<AuditLogDetail> _auditLogDetailRepository;
        IRepository<Workspace> _workSpaceRepository;

        ICountryBLL _countryBLL;
        private readonly IRealtimeNotificationBLL _realtimeNotificationBLL;
        private readonly IBlobFileBLL _blobFileBLL;

        IInvoiceBLL _invoiceBLL;
        IFileBLL _fileBLL;
        private readonly IEmployeeBLL _employeeBLL;
        private readonly IAuthBLL _authBLL;
        private readonly IRepository<CustomerMobile> _mobileRepository;
        private readonly IPasswordHasher _passwordHasher;

        //IWebHostEnvironment _webHostEnvironment;
        //private readonly FileStorageSetting _fileSetting;

        #endregion

        #region Constructor
        public CustomerBLL( IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IRepository<Customer> CustomerRepository,
                           IRepository<AddOn> addOnRepository,
                           IFileBLL fileBLL,
                           IRepository<Industry> industryRepository,
                           IRepository<CompanySize> companySizeRepository,
                           IRepository<CustomerEmail> customerEmailRepository,
                           IRepository<CustomerMobile> companyMobileRepository,
                           IRepository<CustomerEmailVerification> customerEmailVerficationRepository,
                           IRepository<CustomerSmsverification> customerSmsVerficationRepository,
                           //IWebHostEnvironment webHostEnvironment,
                           //IOptions<FileStorageSetting> fileSetting,
                           IRepository<Core.Entities.Version> versionRepository,
                           IRepository<CustomerSubscription> customerSubscriptionRepository,
                           IRepository<Invoice> invoiceRepository,
                           IRepository<VersionSubscription> versionSubscriptionRepository,
                           IRepository<License> licenseRepository,
                           IInvoiceBLL invoiceBLL,
                           IRepository<Employee> employeeRepository,
                           IRepository<AddonSubscription> addonSubscriptionRepository,
                           IRepository<Country> countryRepository,
                           IRepository<Currency> currencyRepository,
                           IRepository<CountryCurrency> countrycurrencyRepository,
                           IRepository<AuditLogDetail> auditLogDetailRepository,
                           IEmployeeBLL employeeBLL,
                           IRepository<RefreshToken> refreshTokenRepository,
                           ICountryBLL countryBLL,
                           IRealtimeNotificationBLL realtimeNotificationBLL,
                           IAuthBLL authBLL,
                           IRepository<CustomerMobile> mobileRepository,
                           IPasswordHasher passwordHasher,
                           IRepository<Workspace> workSpaceRepository,
                           IBlobFileBLL blobFileBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _customerRepository = CustomerRepository;
            _fileBLL = fileBLL;
            _industryRepository = industryRepository;
            _companySizeRepository = companySizeRepository;
            _customerEmailRepository = customerEmailRepository;
            _customerMobileRepository = companyMobileRepository;
            _customerEmailVerficationRepository = customerEmailVerficationRepository;
            _customerSmsVerficationRepository = customerSmsVerficationRepository;
            //_webHostEnvironment = webHostEnvironment;
            //_fileSetting = fileSetting.Value;
            _versionRepository = versionRepository;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _invoiceRepository = invoiceRepository;
            _versionSubscriptionRepository = versionSubscriptionRepository;
            _licenseRepository = licenseRepository;
            _invoiceBLL = invoiceBLL;
            _employeeRepository = employeeRepository;
            _addonSubscriptionRepository = addonSubscriptionRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _countrycurrencyRepository = countrycurrencyRepository;
            _auditLogDetailRepository = auditLogDetailRepository;
            _employeeBLL = employeeBLL;
            _refreshTokenRepository = refreshTokenRepository;
            _countryBLL = countryBLL;
            _realtimeNotificationBLL = realtimeNotificationBLL;
            _authBLL = authBLL;
            _mobileRepository = mobileRepository;
            _passwordHasher = passwordHasher;
            _workSpaceRepository = workSpaceRepository;
            _blobFileBLL = blobFileBLL;
        }

        #endregion

        #region Action

        #region CustomerDownload
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Update DownloadVersionCount. </summary>
        ///
        /// <param name="inputDto">          Dto contain versionId and
        ///                                  Customer make Download.    </param>
        ///                                 
        ///
        /// <returns>  Boolean. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> UpdateVersionDownloadCount( UpdateVersionDownloadCountInputDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                var validator = await new UpdateVersionDownloadCountInputDtoValidator().ValidateAsync(inputDto);

                //Input validation
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var entity = await _versionRepository.GetByIdAsync(inputDto.VersionId);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Core.Entities.Version));

                entity.DownloadCount += 1;

                var versionRelease = entity.VersionReleases.FirstOrDefault(x => x.IsCurrent == true);
                if (versionRelease == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionRelease));

                entity.VersionReleases.FirstOrDefault(x => x.IsCurrent == true).DownloadVersionLogs.Add(new DownloadVersionLog
                {
                    CustomerId = inputDto.CustomerId != null ? inputDto.CustomerId : null,
                    VersionIdReleaseId = versionRelease.Id,
                    Ipaddress = inputDto.IpAddress,
                    CreateDate = DateTime.UtcNow
                });

                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Update AddonDownloadCount. </summary>
        ///
        /// <param name="inputDto">          Dto contain AddOnId and
        ///                                  Customer make Download.    </param>
        ///                                 
        ///
        /// <returns>  Boolean. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> UpdateAddOnDownloadCount( UpdateAddOnDownloadCountInputDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                var validator = await new UpdateAddOnDownloadCountInputDtoValidator().ValidateAsync(inputDto);

                //Input validation
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //ToDo after scafold
                var entity = await _addOnRepository.GetByIdAsync(inputDto.AddOnId);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                entity.DownloadCount += 1;

                //entity.DownloadAddOnLogs.Add(new DownloadAddOnLog
                //{
                //    CustomerId = inputDto.CustomerId,
                //    AddonId = entity.Id,
                //    Ipaddress = inputDto.IpAddress,
                //    CreateDate = DateTime.UtcNow
                //});
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region UpdateAccount-Image

        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Get Customer login. </summary>
        ///
        /// <param name="inputDto">         Dto contain Token Customer
        ///                                 login . </param>
        ///                                 
        ///
        /// <returns>   A GetCustomerOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetCustomerOutputDto>> GetByIdAsync( GetCustomerInputDto inputDto )
        {
            var output = new Response<GetCustomerOutputDto>();
            try
            {
                var validator = await new GetCustomerInputDtoValidator().ValidateAsync(inputDto);
                //Input validation
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var entity = await _customerRepository.GetByIdAsync(inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));
                var customer = _mapper.Map<GetCustomerOutputDto>(entity);
                return output.CreateResponse(customer);
            }
            catch (Exception ex)
            {

                return output.CreateResponse(ex);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary> Upload Image Customer Registred. </summary>
        ///
        /// <param name="inputDto">        Dto contain Token Customer
        ///                                login and image will upload . </param>
        ///                                 
        ///
        /// <returns>   A GetCustomerImageOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetCustomerImageOutputDto>> UploadImage( UploadCustomerImageInputDto inputDto )
        {
            var output = new Response<GetCustomerImageOutputDto>();
            try
            {
                //var validator = await new UploadCustomerImageInputDtoValidator().ValidateAsync(inputDto);
                ////Input validation
                //if (!validator.IsValid)
                //{
                //    return output.CreateResponse(validator.Errors);
                //}

                FileStorage file = null;
                if (inputDto.File != null)
                {
                    // upload image.
                    // var createdFileResult = _fileBLL.UploadFile(inputDto.File);

                    //upload on blob 
                    var storedImage = await _blobFileBLL.UploadFileAsync(inputDto.File);

                    if (storedImage.IsSuccess)
                        file = storedImage.Data;
                    else
                    {
                        output.AppendErrors(storedImage.Errors);
                        return output.CreateResponse();
                    }
                }


                var entity = await _customerRepository.GetByIdAsync(inputDto.Id);//GetCustomerById

                // Business Validaton 
                //1- Check customer is exisit
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                //Map inputDto to Entity
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Image = file;
                //Update Customer
                entity =  _customerRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetCustomerImageOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {

                return output.CreateResponse(ex);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Update Data for customer registred. </summary>
        ///
        /// <param name="inputDto">          Dto contain Token Customer
        ///                                  login and Data will update . </param>
        ///                                 
        ///
        /// <returns>   A GetCustomerOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetCustomerOutputDto>> UpdateAysnc( UpdateCustomerInputDto inputDto )
        {
            var output = new Response<GetCustomerOutputDto>();
            try
            {
                var validator = new UpdateCustomerInputDtoValidator().Validate(inputDto);
                //Input validation
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                // 1- Check Industry Esisit
                if (inputDto.IndustryId is not null)
                {
                    var industry = await _industryRepository.GetByIdAsync(inputDto.IndustryId.Value);
                    if (industry == null)
                        return output.CreateResponse(MessageCodes.NotFound, nameof(Industry));
                }

                // 2- Check CompanySize Esisit
                if (inputDto.CompanySizeId is not null)
                {
                    var companySize = await _companySizeRepository.GetByIdAsync(inputDto.CompanySizeId.Value);
                    if (companySize == null)
                        return output.CreateResponse(MessageCodes.NotFound, nameof(CompanySize));
                }


                var entity = await _customerRepository.GetByIdAsync(inputDto.Id);//GetCustomerById

                // Business Validaton 
                //1- Check customer is exisit
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                //Map inputDto to Entity
                entity = _mapper.Map(inputDto, entity);

                //Update Customer
                _customerRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetCustomerOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {

                return output.CreateResponse(ex);
            }
        }
        public async Task<GetCustomerOutputDto> CreateAsync( PreregisterDto inputDto )
        {
            var output = new Response<GetCustomerOutputDto>();
            try
            {
                //Map inputDto to Entity
                var entity = _mapper.Map<Customer>(inputDto);
                entity.CustomerStatusId = (int)CustomerStatusEnum.Pending;
                entity.CountryId = 1;
                //Update Customer
                await _customerRepository.AddAsync(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetCustomerOutputDto>(entity);
                return result;
            }
            catch (Exception ex)
            {

                return new GetCustomerOutputDto();
            }
        }

        /////-------------------------------------------------------------------------------------------------
        ///// <summary>  Get file tht upload. </summary>
        /////
        ///// <param name="file">            File will Upload
        ///// <param name="filePathEnum">    Path for File will Upload
        /////                                    . </param>
        /////                                 
        /////
        ///// <returns>   A File. </returns>
        /////-------------------------------------------------------------------------------------------------
        //public FileDto GetFile(IFormFile file, FilePathEnum filePathEnum)
        //{
        //    if (_fileSetting is null || file is null)
        //        return null;

        //    var fileDto = new FileDto
        //    {
        //        File = file,
        //        FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory
        //    };

        //    switch (filePathEnum)
        //    {
        //        case FilePathEnum.CustomerProfile:
        //            //fileDto.FilePath =  _fileSetting.Files.Customers.ProfileData.Path;
        //            Path.Combine(_fileSetting.Files.Customers.ProfileData.Path,
        //                                            _fileSetting.Files.Customers.ProfileData.Base);
        //            break;

        //        default:
        //            return null;
        //    }

        //    return fileDto;
        //}
        #endregion

        #region CustomerHistoryRegistered
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Get All customer and filter them by name , country , status , phone 
        /// registration date and email. </summary>
        ///
        /// <param name="pagedDto">          Dto contain all methods for filter . </param>
        ///                                 
        ///
        /// <returns>   A GetCustomerRegistrationHistoryOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<PagedResultDto<GetCustomerRegistrationHistoryOutputDto>>> GetCustomerHistoryPagedListAsync( FilterByEmployeeCountryInputDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetCustomerRegistrationHistoryOutputDto>>();
            ////GetCustomers Yesterday
            //if (string.IsNullOrWhiteSpace(pagedDto.Filter) || pagedDto.Filter == "{}")
            //{
            //    Dictionary<string, FilterValue> dic = new Dictionary<string, FilterValue>();
            //    dic.Add("Createdate", new FilterValue { Operator = FilterBuilder.Operation.GreaterThanOrEquals, Value = DateTime.Today.Date.AddDays(-2).ToShortDateString() });
            //    dic.Add("-R2-Createdate", new FilterValue { Operator = FilterBuilder.Operation.LessThanOrEquals, Value = DateTime.Today.Date.AddDays(-1).ToShortDateString() });
            //    pagedDto.Filter = JsonConvert.SerializeObject(dic);
            //}
            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(pagedDto.EmployeeId);
            var result = GetPagedList<GetCustomerRegistrationHistoryOutputDto, Customer, int>(
                pagedDto: pagedDto,
                repository: _customerRepository, orderExpression: x => x.Id,
                searchExpression: x =>
                        employeCountriesIds.Contains(x.CountryId) &&
                                    (string.IsNullOrEmpty(pagedDto.SearchTerm) ||
                                    (x.Name.ToLower().Contains(pagedDto.SearchTerm.ToLower()) ||
                                    x.Contract.Serial.ToLower().Contains(pagedDto.SearchTerm.ToLower()))),
                sortDirection: pagedDto.SortingDirection);
            return output.CreateResponse(result);
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary> Make Customer not complete register by some problem Verfied. </summary>
        ///
        /// <param name="inputDto">          Dto contain Identifier 
        ///                                   . </param>
        ///                                 
        ///
        /// <returns>   A Boolean. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> MakeCustomerVerified( UpdateCustomerStatusInputDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = await new UpdateCustomerStatusInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = await _customerRepository.GetByIdAsync(inputDto.Id);

                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                // 2- Check Customer is Verified
                if (entity.CustomerStatusId == (int)CustomerStatusEnum.UnVerified)
                {
                    var customerMobiles = entity.CustomerMobiles.ToList();

                    if (customerMobiles != null)
                    {
                        foreach (var customerMobile in customerMobiles)
                        {
                            customerMobile.IsVerified = true;
                            customerMobile.SendCount = 0;
                            customerMobile.BlockForSendingSmsuntil = null;
                            var verifyMobile = customerMobile.CustomerSmsverifications;
                            if (verifyMobile != null)
                                foreach (var item in verifyMobile)
                                {
                                    _customerSmsVerficationRepository.Delete(item);
                                }
                        }
                    }

                    var customerEmails = entity.CustomerEmails.ToList();

                    if (customerEmails != null)
                    {
                        foreach (var customerEmail in customerEmails)
                        {
                            customerEmail.IsVerified = true;
                            customerEmail.SendCount = 0;
                            customerEmail.BlockForSendingEmailUntil = null;
                            var customerEmailVerfiy = customerEmail.CustomerEmailVerifications;
                            if (customerEmailVerfiy != null)
                                foreach (var item in customerEmailVerfiy)
                                {
                                    _customerEmailVerficationRepository.Delete(item);
                                }
                        }
                    }

                    entity.CustomerStatusId = (int)CustomerStatusEnum.Registered;

                    var email = entity.CustomerEmails.FirstOrDefault(ce => ce.IsPrimary);

                    if (email is not null)
                        await _authBLL.CreateCrmLeadAsync(email);
                }
                else
                {
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));
                }

                _customerRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Get all customers for download csv file 
        /// for admin check them . </summary>
        /// <returns>   A GetCustomerRegistrationHistoryOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------

        public IResponse<List<GetCustomerRegistrationHistoryOutputDto>> GetAllCustomerRegisterHistory( )
        {
            var output = new Response<List<GetCustomerRegistrationHistoryOutputDto>>();
            try
            {
                var customers = _mapper.Map<List<GetCustomerRegistrationHistoryOutputDto>>(_customerRepository.GetAllList());
                return output.CreateResponse(customers);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Delete Customer UnVerfied . </summary>
        ///
        /// <param name="inputDto">         Dto contain Identifier for Cutomer
        ///                                 will delete. </param>
        ///                                 
        ///
        /// <returns>   Boolean. </returns>
        ///-------------------------------------------------------------------------------------------------
        public IResponse<bool> Delete( DeleteCustomerInputDto inputDto )
        {

            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = new DeleteCustomerInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _customerRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));
                bool result = true;
                // Check Customer is Verified
                if (entity.CustomerStatusId == (int)CustomerStatusEnum.UnVerified || entity.CustomerStatusId == (int)CustomerStatusEnum.Pending)
                {

                    //  2 - Check if entity has references
                    string [] tables = { string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmail))
                            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerMobile))
                            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmailVerification))
                            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerSmsverification))};
                    var checkDto = EntityHasReferences(entity.Id, _customerRepository, String.Join(",", tables));
                    if (checkDto.HasChildren == 0)
                    {
                        // 2- Check if Entity has references
                        var customerEmails = entity.CustomerEmails.ToList();
                        // Check Customer Has Email
                        if (customerEmails != null)
                        {
                            foreach (var customerEmail in customerEmails)
                            {
                                //Check Email Has Verification
                                if (customerEmail.CustomerEmailVerifications.Any())
                                    // Hard Delete
                                    _customerEmailVerficationRepository.Delete(x => x.CustomerEmailId == customerEmail.Id);
                                // Hard Delete
                                _customerEmailRepository.Delete(customerEmail);
                            }
                        }
                        var customerMobiles = entity.CustomerMobiles.ToList();
                        // Check Customer Has Mobile
                        if (customerMobiles != null)
                        {
                            foreach (var customerMobile in customerMobiles)
                            {
                                // hard Mobile Has Verfication
                                if (customerMobile.CustomerSmsverifications.Any())
                                    // Hard Delete
                                    _customerSmsVerficationRepository.Delete(x => x.CustomerMobileId == customerMobile.Id);
                                // Hard Delete
                                _customerMobileRepository.Delete(customerMobile);
                            }
                        }
                        _customerRepository.Delete(entity);

                        //Notifiy to all members
                        _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                        {
                            Channel = HubChannelEnum.RefreshTokenChannel,
                            RecieverType = NotificationRecieverTypeEnum.Customer,
                            Recievers = new List<int> { entity.Id },
                            Notification = null
                        });
                    }
                    else
                    {
                        result = false;
                    }

                }
                else
                {
                    return output.CreateResponse(MessageCodes.BusinessValidationError, nameof(Customer));
                }
                if (result)
                {
                    _unitOfWork.Commit();



                    return output.CreateResponse(true);

                }
                else
                    // reject delete if entity has references in any other tables
                    return output.CreateResponse(MessageCodes.RelatedDataExist);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }



        #endregion


        #region Contract
        #region Customer
        public async Task<IResponse<PagedResultDto<GetContractCustomerOutputDto>>> GetContractustomerPagedListAsync( FilterByEmployeeCountryInputDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetContractCustomerOutputDto>>();

            ////GetCustomers Yesterday
            //if (string.IsNullOrWhiteSpace(pagedDto.Filter) || pagedDto.Filter == "{}")
            //{
            //    Dictionary<string, FilterValue> dic = new Dictionary<string, FilterValue>();
            //    dic.Add("Createdate", new FilterValue { Operator = FilterBuilder.Operation.GreaterThanOrEquals, Value = DateTime.Today.Date.AddDays(-2).ToShortDateString() });
            //    dic.Add("-R2-Createdate", new FilterValue { Operator = FilterBuilder.Operation.LessThanOrEquals, Value = DateTime.Today.Date.AddDays(-1).ToShortDateString() });
            //    pagedDto.Filter = JsonConvert.SerializeObject(dic);
            //}

            //var employee = await _employeeRepository.GetByIdAsync(pagedDto.EmployeeId);

            //if (employee is null)
            //    return null;

            var employeCountriesIds = await _employeeBLL.GetEmployeeCountries(pagedDto.EmployeeId); //employee.EmployeeCountries.Select(c => c.CountryCurrency.CountryId);

            var result = GetPagedList<GetContractCustomerOutputDto, Customer, int>(
                pagedDto: pagedDto,
                repository: _customerRepository,
                orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/,
                searchExpression: x =>
                                    (x.CustomerSubscriptions.Any() || x.CustomerStatusId == (int)CustomerStatusEnum.Registered
                                    || x.CustomerStatusId == (int)CustomerStatusEnum.Suspended)
                                    && employeCountriesIds.Contains(x.CountryId)
                                    && (string.IsNullOrEmpty(pagedDto.SearchTerm)
                                    || (x.Name.ToLower().Contains(pagedDto.SearchTerm.ToLower())
                                    || x.Contract.Serial.ToLower().Contains(pagedDto.SearchTerm.ToLower())
                                    || x.CustomerEmails.FirstOrDefault(e => e.IsPrimary).Email.ToLower().Equals(pagedDto.SearchTerm.ToLower()))),
                disableFilter: true);

            var defaultCurrency = _countryBLL.GetDefaultCurrencyCode();
            result.Items.ToList().ForEach(x =>
            {
                x.Currency = string.IsNullOrWhiteSpace(x.Currency) ? defaultCurrency : x.Currency;
            });

            return output.CreateResponse(result);
        }
        public async Task<IResponse<PagedResultDto<CustomerReguesttDto>>> GetCustomerRequestByProductPagedList( LicencesFilterInputDto pagedDto )
        {
            var output = new Response<PagedResultDto<CustomerReguesttDto>>();
            var versionSubscription = await _versionSubscriptionRepository.Where(x => x.Id == pagedDto.VersionSubscriptionId).FirstOrDefaultAsync();
            var version = await _versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == pagedDto.CustomerId)
                   .FirstOrDefaultAsync();
            var result = GetPagedList<CustomerReguesttDto, License, int>(
                pagedDto: pagedDto,
                repository: _licenseRepository,
                orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/,
                searchExpression: x => x.CustomerSubscriptionId == versionSubscription.CustomerSubscriptionId
                && x.CustomerSubscription.CustomerId == pagedDto.CustomerId && string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || !string.IsNullOrWhiteSpace(pagedDto.SearchTerm))
                ;

            return output.CreateResponse(result);
        }
        public async Task<IResponse<GetCustomerReferencesOutputDto>> GetCustomerByDetails( int customerId )
        {
            var output = new Response<GetCustomerReferencesOutputDto>();
            try
            {
                var entity = await _customerRepository.GetByIdAsync(customerId);

                var customer = _mapper.Map<GetCustomerReferencesOutputDto>(_customerRepository.GetById(customerId));
                customer.CustomerApplications = new List<GetCustomerProductOutputDto>();
                //check customer has subscriptions
                if (entity.CustomerSubscriptions.Any())
                {
                    //get all versionSubscription 
                    var versions = await _versionSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customer.Id &&
                                                                    x.CustomerSubscription.Invoices.Any(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                    && x.InvoiceStatusId != (int)InvoiceStatusEnum.Refunded))
                                                                        .Distinct()
                                                                        .ToListAsync()
                                                                        ;
                    // //Get all AddonSubscription
                    // var addons = _addonSubscriptionRepository.Where(x => x.CustomerSubscription.CustomerId == customer.Id)
                    //.ToList();
                    //get all licences
                    //var licenses = _licenseRepository.Where(x => x.CustomerSubscription.CustomerId == customer.Id).ToList();
                    if (versions.Count > 0)
                    {
                        customer.CustomerApplications = _mapper.Map<List<GetCustomerProductOutputDto>>(versions);
                        customer.CustomerApplications?.ToList().ForEach(x =>
                        {
                            x.Price = GetVersionPrice(x.VersionSubscriptionId);
                        });
                        customer.CustomerApplications.FirstOrDefault().IsDefault = true;
                    }
                    else
                    {
                        customer.CustomerApplications = new List<GetCustomerProductOutputDto>();
                    }
                }
                var result = _mapper.Map<GetCustomerReferencesOutputDto>(customer);
                result.CurrencyName = string.IsNullOrWhiteSpace(result.CurrencyName) ? _countryBLL.GetDefaultCurrencyCode() : result.CurrencyName;
                return output.CreateResponse(result);

            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }

        public IResponse<bool> UpdateCustomerByAdmin( UpdateCustomerByAdminInputDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                bool CounryChanged = false;
                //Input Validation
                var validator = new UpdateCustomerByAdminInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _customerRepository.GetById(inputDto.CustomerId);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                var isStatusChanged = false;

                // 2- Check Customer is Verified
                if (entity.CustomerStatusId == (int)CustomerStatusEnum.Registered)
                {
                    if (inputDto.Active == false)
                    {
                        //Todo after customer has invoice
                        if (HasInvoices(entity.Id))
                            return output.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(Invoice));

                        entity.CustomerStatusId = (int)CustomerStatusEnum.Suspended;

                        isStatusChanged = true;

                        var customerSubscriptions = _customerSubscriptionRepository.Where(x => x.CustomerId == entity.Id).ToList();

                        customerSubscriptions.ForEach(x => x.AutoBill = false);
                    }
                }
                else if (entity.CustomerStatusId == (int)CustomerStatusEnum.Suspended)
                {
                    if (inputDto.Active == true)
                    {
                        entity.CustomerStatusId = (int)CustomerStatusEnum.Registered;

                        isStatusChanged = true;
                    }
                }
                if (inputDto.CountryId != 0)
                {
                    //check changeing of country
                    if (inputDto.CountryId != entity.CountryId)
                    {
                        //update 
                        CounryChanged = true;
                    }

                    var invoices = _invoiceRepository.Where(x => x.CustomerSubscription.CustomerId == entity.Id
                                                                 && x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid).ToList();

                    if (HasInvoices(entity.Id))
                        return output.CreateResponse(MessageCodes.HasUnPaidInvoicesCount, invoices.Count().ToString());

                    entity.CountryId = inputDto.CountryId;
                }

                entity = _mapper.Map(inputDto, entity);
                _customerRepository.Update(entity);
                _unitOfWork.Commit();

                //check changeing of country
                if (CounryChanged || isStatusChanged)
                {
                    _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                    {
                        Channel = HubChannelEnum.RefreshTokenChannel,
                        RecieverType = NotificationRecieverTypeEnum.Customer,
                        Recievers = new List<int> { inputDto.CustomerId },
                        Notification = null
                    });
                }

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<PagedResultDto<CustomerActivitesDto>>> GetCustomerActivitesPagedListAsync( LogFilterPagedResultDto pagedDto )
        {
            var output = new Response<PagedResultDto<CustomerActivitesDto>>();

            var result = GetPagedList<CustomerActivitesDto, AuditLogDetail, int>(
                pagedDto: pagedDto,
                repository: _auditLogDetailRepository,
                sortDirection: pagedDto.SortingDirection/*sortDirection: nameof(SortingDirection.DESC)*/,
                orderExpression: x => x.Id,
                searchExpression: x =>
                  x.AuditLog.TableName == nameof(Customer)
                  && (!pagedDto.Id.HasValue || (pagedDto.Id.HasValue && pagedDto.Id.Value > 0 && pagedDto.Id.Value == x.AuditLog.PrimaryKey))
                  && (!pagedDto.ActionTypeId.HasValue || (pagedDto.ActionTypeId.HasValue && pagedDto.Id.Value > 0 && pagedDto.ActionTypeId.Value == x.AuditLog.AuditActionTypeId))
                  && (!pagedDto.FromDate.HasValue || (pagedDto.FromDate.HasValue && x.AuditLog.CreateDate >= pagedDto.FromDate))
                   && (!pagedDto.ToDate.HasValue || (pagedDto.ToDate.HasValue && x.AuditLog.CreateDate <= pagedDto.ToDate))

                  &&
                string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || !string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                         )
                      ;

            int customerId = int.TryParse(result.Items.FirstOrDefault()?.Id, out customerId) ? customerId : 0;
            var customer = customerId > 0 ? _customerRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == customerId) : null;
            if (result.Items.Any())
            {
                if (customer != null)
                {
                    result.Items.ToList().ForEach(x => x.Owner = customer.Name);
                }
            }


            return output.CreateResponse(result);
        }

        public IResponse<List<GetContractCustomerOutputDto>> GetAlltCustomersInContract( int employeeId )
        {
            var output = new Response<List<GetContractCustomerOutputDto>>();
            try
            {
                var employeCountriesIds = _employeeBLL.GetEmployeeCountries(employeeId).GetAwaiter().GetResult();
                var customers = _mapper.Map<List<GetContractCustomerOutputDto>>(
                                                          _customerRepository.Where(x => employeCountriesIds.Contains(x.CountryId)).ToList());
                return output.CreateResponse(customers);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetAllCountriesOutputDto>>> GetAllCountriesAsync( )
        {
            var output = new Response<List<GetAllCountriesOutputDto>>();
            try
            {
                var countries = await _countryRepository.GetAllListAsync();
                var defualtCurrency = _countrycurrencyRepository.Where(x => x.DefaultForOther).FirstOrDefault()?.Currency?.Name;
                var result = _mapper.Map<List<GetAllCountriesOutputDto>>(countries);

                result.ForEach(x =>
                {
                    x.Currency =
                    _countrycurrencyRepository.Where(cc => cc.CountryId == x.Id).FirstOrDefault()?.Currency?.Name ?? defualtCurrency;

                });

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<int>> CreateCustomerAsync(NewCustomerByAdminDto newCustomerDto, int currentEmployeeId)
        {
            var response = new Response<int>();

            try
            {
                var validator = await new RegisterLandingPageInputDtoValidator().ValidateAsync(newCustomerDto);

                if (!validator.IsValid)
                    response.AppendErrors(validator.Errors);

                //Get CountryBy CountryCode
                var country = await _countryRepository.GetAsync(x => x.PhoneCode == newCustomerDto.CountryCode);

                if (country == null)
                    response.AppendError(MessageCodes.NotFound, nameof(newCustomerDto.CountryCode));

                var emailAndMobileValidator = await _authBLL.ValidateEmailAndMobileAsync(newCustomerDto);

                if (!emailAndMobileValidator.IsSuccess)
                    response.AppendErrors(emailAndMobileValidator.Errors);

                var existMobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(newCustomerDto.Mobile));

                // Handle verified mobile.
                if (existMobile is not null && existMobile.IsVerified)
                    response.AppendError(MessageCodes.AlreadyExists, nameof(newCustomerDto.Mobile));

                if (!response.IsSuccess)
                    return response.CreateResponse();

                var mobile = _mapper.Map<CustomerMobile>(newCustomerDto);

                mobile.PhoneCountryId = country.Id;

                var email = _mapper.Map<CustomerEmail>(newCustomerDto);
                var customer = _mapper.Map<Customer>(newCustomerDto);

                customer.CountryId = country.Id;
                customer.CreatedBy = currentEmployeeId;
                customer.Password = _passwordHasher.HashPassword(newCustomerDto.Password);

                customer.CustomerMobiles.Add(mobile);
                customer.CustomerEmails.Add(email);

                await _customerRepository.AddAsync(customer);

                // Create lead in crm and set lead id in customer.
                await _authBLL.CreateCrmLeadAsync(email);

                await _unitOfWork.CommitAsync();

                return response.CreateResponse(customer.Id);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }
        public async Task<IResponse<bool>> DeleteCustomerAsync(int customerId)
        {
            var response = new Response<bool>();

            try
            {
                var entity = await _customerRepository.GetByIdAsync(customerId);

                if (entity is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(Customer));

                string[] tables = ExcludedCustomerReference();

                // 1- Check if Entity has references
                var checkDto = EntityHasReferences(entity.Id, _customerRepository, String.Join(",", tables));
                if (checkDto.HasChildren == 0)
                {
                    //Delete customerRefernces

                    DeleteRefreshTokens(entity);

                    DeleteCustomerEmails(entity);

                    DeleteCustomerMobiles(entity);

                    _customerRepository.Delete(entity);

                    _unitOfWork.Commit();

                    return response.CreateResponse(true);
                }

                return response.CreateResponse(MessageCodes.RelatedDataExist, nameof(Customer));



            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }




        #endregion
        #endregion

        #region TestValidateCustomer

        #endregion
        public async Task<IResponse<bool>> TestValidateMobile( int customerMobileId )
        {
            var output = new Response<bool>();
            try
            {
                var customerMobile = await _customerMobileRepository.GetByIdAsync(customerMobileId);

                if (customerMobile is null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerMobile));

                if (customerMobile.CustomerSmsverifications.Any())
                {
                    foreach (var customerSmsverf in customerMobile.CustomerSmsverifications)
                    {
                        _customerSmsVerficationRepository.Delete(customerSmsverf);
                    }
                }
                customerMobile.SendCount = 0;
                customerMobile.IsVerified = true;
                customerMobile.Customer.CustomerStatusId = (int)CustomerStatusEnum.Verified;
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> TestValidateEmail( int customerEmailId )
        {
            var output = new Response<bool>();
            try
            {
                var customerEmail = await _customerEmailRepository.GetByIdAsync(customerEmailId);

                if (customerEmail is null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerMobile));

                if (customerEmail.CustomerEmailVerifications.Any())
                {
                    foreach (var customerEmailVerf in customerEmail.CustomerEmailVerifications)
                    {
                        _customerEmailVerficationRepository.Delete(customerEmailVerf);
                    }
                }
                customerEmail.SendCount = 0;
                customerEmail.IsVerified = true;
                customerEmail.Customer.CustomerStatusId = (int)CustomerStatusEnum.Registered;
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> TestDeleteCustomer( int customerId )
        {
            var output = new Response<bool>();
            try
            {
                var entity = await _customerRepository.GetByIdAsync(customerId);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Customer));
                bool result = true;



                var refreshToken = entity.RefreshTokens.ToList();
                if (refreshToken != null)
                    _refreshTokenRepository.DeleteRange(entity.RefreshTokens);

                // 2- Check if Entity has references
                var customerEmails = entity.CustomerEmails.ToList();
                // Check Customer Has Email
                if (customerEmails != null)
                {
                    foreach (var customerEmail in customerEmails)
                    {
                        //Check Email Has Verification
                        if (customerEmail.CustomerEmailVerifications.Any())
                            // Hard Delete
                            _customerEmailVerficationRepository.Delete(x => x.CustomerEmailId == customerEmail.Id);
                        // Hard Delete
                        _customerEmailRepository.Delete(customerEmail);
                    }
                }
                var customerMobiles = entity.CustomerMobiles.ToList();
                // Check Customer Has Mobile
                if (customerMobiles != null)
                {
                    foreach (var customerMobile in customerMobiles)
                    {
                        // hard Mobile Has Verfication
                        if (customerMobile.CustomerSmsverifications.Any())
                            // Hard Delete
                            _customerSmsVerficationRepository.Delete(x => x.CustomerMobileId == customerMobile.Id);
                        // Hard Delete
                        _customerMobileRepository.Delete(customerMobile);
                    }
                }

                var customerWorkSpaces = entity.Workspaces.ToList();

                if (customerWorkSpaces != null)
                    _workSpaceRepository.DeleteRange(customerWorkSpaces);

                _customerRepository.Delete(entity);

                _unitOfWork.Commit();

                return output.CreateResponse(true);


            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<bool>> DeleteCustomerByEmailOrPhonrAsync(string userName)
        {
            var response = new Response<bool>();
            try
            {
                int customerId = 0;
                if (userName.Contains("@"))
                {
                    var customerEmail = await _customerEmailRepository.GetAsync(x => x.Email.Equals(userName));

                    if (customerEmail is null)
                        return response.CreateResponse(MessageCodes.NotFound, userName);

                    customerId = customerEmail.CustomerId;
                }
                else
                {
                    var customerMobile = await _customerMobileRepository.GetAsync(x => x.Mobile.Equals(userName));

                    if (customerMobile is null)
                        return response.CreateResponse(MessageCodes.NotFound, userName);

                    customerId = customerMobile.CustomerId;
                }

                var deleteCustomer = await TestDeleteCustomer(customerId);

                if (!deleteCustomer.IsSuccess)
                    response.AppendErrors(deleteCustomer.Errors);

                if (!response.IsSuccess)
                    response.CreateResponse();

                return response.CreateResponse(true);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }
        }


        #endregion

        #region Helper
        private bool HasInvoices( int customerId )
        {
            var invoice = _invoiceRepository
                .Where(x => x.CustomerSubscription.CustomerId == customerId && x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();
            if (invoice == null)
                return false;
            if (invoice != null && invoice.EndDate > DateTime.UtcNow)
                return false;

            return true;

        }
        private VersionPriceDetailsDto GetVersionPrice( int versionSubscriptionId )
        {
            var versionSubscription = _versionSubscriptionRepository.Where(v => v.Id == versionSubscriptionId).FirstOrDefault();
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
        private string GetVersionSubscriptionDiscriminator( int subscriptionTypeId, int renewEvery )
        {
            if (subscriptionTypeId == (int)SubscriptionTypeEnum.Forever)
                return DiscriminatorsEnum.Forever.ToString();
            else if (subscriptionTypeId == (int)SubscriptionTypeEnum.Others && renewEvery == 30)
                return DiscriminatorsEnum.Monthly.ToString();
            else if (subscriptionTypeId == (int)SubscriptionTypeEnum.Others && renewEvery == 365)
                return DiscriminatorsEnum.Yearly.ToString();
            return string.Empty;

        }

        private void DeleteCustomerMobiles(Customer entity)
        {
            var customerMobiles = entity.CustomerMobiles.ToList();

            if (customerMobiles != null)
            {
                foreach (var customerMobile in customerMobiles)
                {
                    if (customerMobile.CustomerSmsverifications.Any())
                        _customerSmsVerficationRepository.Delete(x => x.CustomerMobileId == customerMobile.Id);
                    _customerMobileRepository.Delete(customerMobile);
                }
            }
        }

        private void DeleteCustomerEmails(Customer entity)
        {
            var customerEmails = entity.CustomerEmails.ToList();
            if (customerEmails != null)
            {
                foreach (var customerEmail in customerEmails)
                {
                    if (customerEmail.CustomerEmailVerifications.Any())
                        _customerEmailVerficationRepository.Delete(x => x.CustomerEmailId == customerEmail.Id);
                    _customerEmailRepository.Delete(customerEmail);
                }
            }
        }

        private void DeleteRefreshTokens(Customer entity)
        {
            var refreshToken = entity.RefreshTokens.ToList();
            if (refreshToken != null)
                _refreshTokenRepository.DeleteRange(entity.RefreshTokens);
        }
        private static string[] ExcludedCustomerReference()
        {
            string[] tables = {
                string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmail))
                                        ,
                string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerMobile))
                                        ,
                string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(RefreshToken))
                                        ,
                string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmailVerification))
                                        ,
                string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerSmsverification))
            };
            return tables;
        }
        #endregion
    }
}
