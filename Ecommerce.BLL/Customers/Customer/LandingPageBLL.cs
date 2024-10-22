using AutoMapper;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

using MyDexef.BLL.Customers.Invoices;
using MyDexef.BLL.Employees;
using MyDexef.BLL.Files;
using MyDexef.BLL.Responses;
using MyDexef.Core.Entities;
using MyDexef.Core.Enums.Admins;
using MyDexef.Core.Enums.Customers;
using MyDexef.Core.Enums.Invoices;
using MyDexef.Core.Infrastructure;
using MyDexef.DTO.Applications.ApplicationVersions;
using MyDexef.DTO.Customers.Auth.Inputs;
using MyDexef.DTO.Logs;
using MyDexef.DTO.Paging;
using MyDexef.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static MyDexef.BLL.DXConstants;
using static MyDexef.BLL.Validation.Customer.Customers.CustomerValidator;
using static MyDexef.DTO.Customers.CustomerDto;
using static MyDexef.DTO.Customers.CustomerProduct.CustomerProductDto;
using MyDexef.BLL.SignalRHubs;
using MyDexef.BLL.Countries;
using System.Data;
using MyDexef.DTO.Customers;
using FluentValidation;
using MyDexef.BLL.Validation.Customer;
using MyDexef.Helper.Security;

namespace MyDexef.BLL.Customers
{
    public class LandingPageBLL : BaseBLL, ILandingPageBLL
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
        ICountryBLL _countryBLL;
        private readonly IPasswordHasher _passwordHasher;

        IInvoiceBLL _invoiceBLL;
        IFileBLL _fileBLL;
        private readonly IEmployeeBLL _employeeBLL;
        private readonly ISignalRNotificationHub _myNotifier;
        //IWebHostEnvironment _webHostEnvironment;
        //private readonly FileStorageSetting _fileSetting;

        #endregion

        #region Constructor
        public LandingPageBLL( IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IRepository<Customer> CustomerRepository,
                           IFileBLL fileBLL,
                           IRepository<CustomerEmail> customerEmailRepository,
                           IRepository<CustomerMobile> companyMobileRepository,
                           IRepository<CustomerSubscription> customerSubscriptionRepository,
                           IRepository<Country> countryRepository,
                           IRepository<Currency> currencyRepository,
                           IRepository<CountryCurrency> countrycurrencyRepository,
                           IRepository<AuditLogDetail> auditLogDetailRepository,
                           IRepository<RefreshToken> refreshTokenRepository,
                           ICountryBLL countryBLL,
                           IPasswordHasher passwordHasher ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _customerRepository = CustomerRepository;
            _fileBLL = fileBLL;
            _customerEmailRepository = customerEmailRepository;
            _customerMobileRepository = companyMobileRepository;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _countrycurrencyRepository = countrycurrencyRepository;
            _auditLogDetailRepository = auditLogDetailRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _countryBLL = countryBLL;
            _passwordHasher = passwordHasher;
        }

        #endregion

        #region Action
        public async Task<IResponse<RegisterLandingPageOutputDto>> RegisterLandingPage( RegisterLandingPageInputDto inputDto )
        {
            var output = new Response<RegisterLandingPageOutputDto>();
            try
            {
                //Input Validation
                var validator = new RegisterLandingPageInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //Get CountryBy CountryCode
                var country = _countryRepository.Where(x => x.PhoneCode == inputDto.CountryCode).FirstOrDefault();

                //Check if Mobile Not Exists
                if (_customerMobileRepository.Where(x => x.Mobile == inputDto.Mobile).Any())
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(inputDto.Mobile));
                //Check if Email Not Exists
                if (_customerEmailRepository.Where(x => x.Email == inputDto.Email).Any())
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(inputDto.Email));

                //Add Customer 
                _customerRepository.Add(new Customer
                {
                    CustomerStatusId = (int)CustomerStatusEnum.Registered,
                    CountryId = country.Id,
                    Name = inputDto.Email,        
                    //Add CustomerMobile

                    //Add CustomerEmail

                    //HashPassword

                    Password = _passwordHasher.HashPassword(inputDto.Password),
                    CustomerEmails  = new List<CustomerEmail> {
                        new CustomerEmail
                        {
                            IsPrimary = true,
                            IsVerified = true,
                            Email = inputDto.Email
                        }
                    },
                    CustomerMobiles = new List<CustomerMobile>
                      {
                           new CustomerMobile
                           {
                                 IsPrimary = true,
                                 IsVerified = true,
                                  Mobile = inputDto.Mobile
                           }
                      },
                    TempGuid = Guid.NewGuid()
                });


                //Generate Token

                var entity = _mapper.Map<Customer>(inputDto);
                entity.CustomerStatusId = (int)CustomerStatusEnum.Registered;



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


        #region CustomerDownload

        #endregion

        #region UpdateAccount-Image




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
                                    (x.CustomerSubscriptions.Any() || x.CustomerStatusId == (int)CustomerStatusEnum.Registered ||
                                    x.CustomerStatusId == (int)CustomerStatusEnum.Suspended) &&
                                    employeCountriesIds.Contains(x.CountryId) &&
                                    (string.IsNullOrEmpty(pagedDto.SearchTerm) ||
                                    (x.Name.ToLower().Contains(pagedDto.SearchTerm.ToLower()) ||
                                    x.Contract.Serial.ToLower().Contains(pagedDto.SearchTerm.ToLower())
                                    )),
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
                                                                        .ToListAsync();
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
                // 2- Check Customer is Verified
                if (entity.CustomerStatusId == (int)CustomerStatusEnum.Registered)
                {
                    if (inputDto.Active == false)
                    {
                        //Todo after customer has invoice
                        if (HasInvoices(entity.Id))
                            return output.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(Invoice));
                        entity.CustomerStatusId = (int)CustomerStatusEnum.Suspended;
                        var customerSubscriptions = _customerSubscriptionRepository.Where(x => x.CustomerId == entity.Id).ToList();
                        customerSubscriptions.ForEach(x =>
                            x.AutoBill = false
                        );

                    }
                }
                else if (entity.CustomerStatusId == (int)CustomerStatusEnum.Suspended)
                {
                    if (inputDto.Active == true)
                        entity.CustomerStatusId = (int)CustomerStatusEnum.Registered;


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
                if (CounryChanged)
                {
                    _myNotifier.ForceCustomerLogOut(inputDto.CustomerId.ToString());
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


                //  2 - Check if entity has references
                //    string[] tables = { string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmail))
                //            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerMobile))
                //            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(RefreshToken))
                //            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerEmailVerification))
                //            , string.Format(DbSchemas.ReferenceTableName, DbSchemas.Customer, nameof(CustomerSmsverification))};
                //    var checkDto = EntityHasReferences(entity.Id, _customerRepository, String.Join(",", tables));
                //if (checkDto.HasChildren == 0)
                //{
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
                //}
                //else
                //{
                //    result = false;
                //}

                //if (result)
                //{
                _unitOfWork.Commit();
                return output.CreateResponse(true);

                //}
                //else
                //    // reject delete if entity has references in any other tables
                //    return output.CreateResponse(MessageCodes.RelatedDataExist);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region Helper



        #endregion



    }
}
