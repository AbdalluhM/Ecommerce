using AutoMapper;
using Dexef.Notification.Enums;
using Dexef.Notification.Enums.VictoryLink;
using Dexef.Notification.Models.Mailing;
using Dexef.Notification.Models.Sms;
using Dexef.System.Validations;
using FluentValidation;
using Google;
using Google.Apis.Oauth2.v2;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Ecommerce.BLL.Customers.Crm;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Customer;
using Ecommerce.BLL.Validation.Customer.Auth;
using Ecommerce.BLL.WorkSpaces;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using Ecommerce.DTO.Customers.Auth.Outputs;
using Ecommerce.DTO.Customers.Crm;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.Helper.Security;
using Ecommerce.Repositroy.Base;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

using IO = System.IO;

namespace Ecommerce.BLL.Customers.Auth
{
    public class AuthBLL : BaseBLL, IAuthBLL
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerMobile> _mobileRepository;
        private readonly IRepository<CustomerSmsverification> _mobileVerificationRepository;
        private readonly IRepository<CustomerEmail> _emailRepository;
        private readonly IRepository<CustomerEmailVerification> _emailVerificationRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly AuthSetting _authSetting;
        private readonly INotificationBLL _notificationBLL;
        private readonly ITwilioBLL _twilioBLL;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICrmBLL _crmBLL;
        //private readonly ICustomerBLL _customerBLL;
        private readonly IWorkSpaceBLL _workSpaceBLL;
        private readonly EmailTemplateSetting _emailTemplateSetting;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<CountryCurrency> _countryCurrencyRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthBLL(IMapper mapper,
                       IUnitOfWork unitOfWork,
                       IRepository<Customer> customerRepository,
                       IRepository<CustomerMobile> mobileRepository,
                       IRepository<CustomerSmsverification> mobileverificationRepository,
                       IRepository<CustomerEmail> emailRepository,
                       IRepository<CustomerEmailVerification> emailVerificationRepository,
                       IRepository<RefreshToken> refreshTokenRepository,
                       IOptions<AuthSetting> authSetting,
                       INotificationBLL notificationBLL,
                       IPasswordHasher passwordHasher,
                       ICrmBLL crmBLL,
                       IOptions<EmailTemplateSetting> emailTemplateSetting,
                       IRepository<Country> countryRepository,
                       IHttpClientFactory httpClientFactory,
                       IWorkSpaceBLL workSpaceBLL
,
                       ITwilioBLL twilioBLL
,
                       IRepository<CountryCurrency> countryCurrencyRepository
/* ICustomerBLL customerBLL*/)
            : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _mobileRepository = mobileRepository;
            _mobileVerificationRepository = mobileverificationRepository;
            _emailRepository = emailRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _authSetting = authSetting.Value;
            _notificationBLL = notificationBLL;
            _passwordHasher = passwordHasher;
            _crmBLL = crmBLL;
            _emailTemplateSetting = emailTemplateSetting.Value;
            _countryRepository = countryRepository;
            _httpClientFactory = httpClientFactory;
            _workSpaceBLL = workSpaceBLL;
            _twilioBLL = twilioBLL;
            _countryCurrencyRepository = countryCurrencyRepository;
            //_customerBLL = customerBLL;
        }

        public async Task<IResponse<CustomerEmailExistDto>> IsEmailExistAsync(CheckEmailExistDto checkEmailExistDto)
        {
            var response = new Response<CustomerEmailExistDto>();

            try
            {
                var validator = await new CheckEmailExistDtoValidator().ValidateAsync(checkEmailExistDto);

                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var existEmail = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(checkEmailExistDto.Email.ToLower()));

                var customerData = new CustomerEmailExistDto();

                if (existEmail is not null)
                {
                    customerData.IsEmailExist = true;

                    var relatedMobile = existEmail.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary);

                    if (relatedMobile is not null)
                    {
                        customerData.IsMobileVerified = relatedMobile.IsVerified;
                        customerData.MobileId = relatedMobile.Id;
                    }
                }

                return response.CreateResponse(customerData);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<CustomerMobileExistDto>> IsMobileExistAsync(CheckMobileExistDto checkMobileExistDto)
        {
            var response = new Response<CustomerMobileExistDto>();

            try
            {
                var validator = await new CheckMobileExistDtoValidator().ValidateAsync(checkMobileExistDto);

                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var mobileExist = await _mobileRepository.GetAsync(e => e.Mobile.Equals(checkMobileExistDto.Mobile));

                var customerMobileDto = new CustomerMobileExistDto();

                if (mobileExist is not null)
                {
                    customerMobileDto.IsMobileExist = true;
                    customerMobileDto.MobileId = mobileExist.Id;

                    customerMobileDto.IsMobileVerified = mobileExist.IsVerified;
                }

                return response.CreateResponse(customerMobileDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<PreregisterResultDto>> PreregisterLandingPageAsync(PreregisterLandingPageDto preregisterLandingPageDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                //Input Validation
                var validator = new RegisterLandingPageInputDtoValidator().Validate(preregisterLandingPageDto);

                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                //Get CountryBy CountryCode
                var country = await _countryRepository.GetAsync(x => x.PhoneCode == preregisterLandingPageDto.CountryCode);

                if (country == null)
                    response.AppendError(MessageCodes.NotFound, nameof(preregisterLandingPageDto.CountryCode));

                var emailAndMobileValidator = await ValidateEmailAndMobileHelperAsync(preregisterLandingPageDto);

                if (!emailAndMobileValidator.IsSuccess)
                    response.AppendErrors(emailAndMobileValidator.Errors);

                //if (!response.IsSuccess)
                //    return response.CreateResponse();

                var registerResult = new PreregisterResultDto();


                // Handle verified mobile.
                var existMobiles = await _mobileRepository.GetManyAsync(m => m.Mobile.Equals(preregisterLandingPageDto.Mobile)/*&& m.IsVerified*/);

                // Handle verified mobile.
                if (existMobiles.Any() && existMobiles.Any(m => m.IsVerified))
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(preregisterLandingPageDto.Mobile));

                var lastMobileNotVerified = existMobiles.Where(m => m.Customer.CustomerEmails.Any(e => e.Email.Equals(preregisterLandingPageDto.Email))).OrderBy(x => x.Id).LastOrDefault();
                // Handle unverified mobile.
                if (lastMobileNotVerified is not null && !lastMobileNotVerified.IsVerified)
                {
                    if (lastMobileNotVerified.IsPrimary)
                        return await SendMobileCodeHelperAsync(lastMobileNotVerified);
                    else
                    {
                        response.AppendError(MessageCodes.AlreadyExists, nameof(preregisterLandingPageDto.Email));
                    }
                }
                if (!response.IsSuccess)
                    return response.CreateResponse();



                var mobile = _mapper.Map<CustomerMobile>(preregisterLandingPageDto);
                var email = _mapper.Map<CustomerEmail>(preregisterLandingPageDto);
                var customer = _mapper.Map<Customer>(preregisterLandingPageDto);

                mobile.PhoneCountryId = country.Id;
                customer.CountryId = country.Id;
                customer.Password = _passwordHasher.HashPassword(preregisterLandingPageDto.Password);

                customer.CustomerMobiles.Add(mobile);
                customer.CustomerEmails.Add(email);

                await _customerRepository.AddAsync(customer);
                _unitOfWork.Commit();

                // map retrieve dto.
                registerResult.Id = customer.Id;
                registerResult.MobileId = mobile.Id;
                registerResult.Status = CustomerStatusEnum.UnVerified;


                return response.CreateResponse(registerResult);




            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }
        public async Task<IResponse<PreregisterResultDto>> SendSmsVerificationAsync(int mobileId, bool isWatsApp, string lang)
        {
            var response = new Response<PreregisterResultDto>();
            try
            {
                var existMobile = await _mobileRepository.GetByIdAsync(mobileId);
                var result = await SendMobileCodeHelperAsync(existMobile, isWatsApp: isWatsApp, lang: lang);

                return result;
            }
            catch (Exception e)
            {

                return response.CreateResponse(e);
            }
        }
        private async Task<IResponse<PreregisterResultDto>> SendMobileCodeHelperAsync(CustomerMobile mobile, SmsTypeEnum smsTypeEnum = SmsTypeEnum.Verification, bool isWatsApp = true, string lang = SupportedLanguage.EN)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var registerResult = new PreregisterResultDto
                {
                    Id = mobile.CustomerId,
                    MobileId = mobile.Id,
                    Status = CustomerStatusEnum.UnVerified
                };

                var isExpiryExist = false;
                var isBlockExist = false;

                // last expiry date exist.
                var codeNotExpiredYetExist = mobile?.CustomerSmsverifications.FirstOrDefault(v => v.ExpireDate >= DateTime.UtcNow);
                if (codeNotExpiredYetExist is not null)
                {
                    registerResult.ExpiryDate = DateTime.SpecifyKind(codeNotExpiredYetExist.ExpireDate, DateTimeKind.Utc);
                    isExpiryExist = true;
                }

                if (mobile.BlockForSendingSmsuntil.HasValue)
                {
                    // return block till date.
                    if (mobile.BlockForSendingSmsuntil.Value >= DateTime.UtcNow)
                    {
                        registerResult.BlockTillDate = DateTime.SpecifyKind(mobile.BlockForSendingSmsuntil.Value, DateTimeKind.Utc);
                        isBlockExist = true;
                    }
                    else
                    {
                        // reset sendCount & block sending till date.
                        mobile.SendCount = default;
                        mobile.BlockForSendingSmsuntil = null;
                    }
                }

                if (isExpiryExist || isBlockExist)
                    return response.CreateResponse(registerResult);

                // if sendCount < limit by 1, add block datetime.
                var sendLimit = _authSetting.Sms.SendCountLimit;
                if (mobile.SendCount == --sendLimit)
                    mobile.BlockForSendingSmsuntil = DateTime.UtcNow.Add(_authSetting.Sms.BlockSendingTime)
                                                                    .Add(_authSetting.Sms.CodeExpiryTime);

                mobile.SendCount++;
                mobile.ModifiedDate = DateTime.UtcNow;

                var verificationCode = await GenerateMobileCodeAsync(mobile.CustomerId);

                var mobileVerification = new CustomerSmsverification
                {
                    VerificationCode = verificationCode,
                    CreateDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime),
                    SmstypeId = (int)smsTypeEnum
                };

                mobile.CustomerSmsverifications.Add(mobileVerification);

                _mobileRepository.Update(mobile);


                // send sms.
                if (!isWatsApp)
                    await SendVerificationCodeSmsAsync(mobile, verificationCode);
                else
                    await _twilioBLL.SendWhatsAppVerificationCodeAsync(mobile, verificationCode, lang);


                await _unitOfWork.CommitAsync();

                registerResult.ExpiryDate = mobileVerification.ExpireDate;
                registerResult.BlockTillDate = mobile.BlockForSendingSmsuntil.HasValue ? DateTime.SpecifyKind(mobile.BlockForSendingSmsuntil.Value, DateTimeKind.Utc) : null;

                return response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }

        }

        public async Task<IResponse<PreregisterResultDto>> PreregisterAsync(PreregisterDto preRegisterDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                // inputs validation.
                var validation = await new PreregisterDtoValidator().ValidateAsync(preRegisterDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                //Get CountryBy CountryCode
                var mobileCountry = await _countryRepository.GetAsync(x => x.PhoneCode == preRegisterDto.MobileCountryCode);

                if (mobileCountry == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(preRegisterDto.MobileCountryCode));

                var registerResult = new PreregisterResultDto();

                var existMobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(preRegisterDto.Mobile));

                // Handle verified mobile.
                if (existMobile is not null && existMobile.IsVerified)
                {
                    switch ((CustomerStatusEnum)existMobile.Customer.CustomerStatusId)
                    {
                        case CustomerStatusEnum.Verified:
                            {
                                registerResult.Id = existMobile.CustomerId;
                                registerResult.MobileId = existMobile.Id;
                                registerResult.Status = (CustomerStatusEnum)existMobile.Customer.CustomerStatusId;

                                response.CreateResponse(registerResult);
                                return response;
                            }
                        case CustomerStatusEnum.Pending:
                            {
                                var customerEmail = await _emailRepository.GetAsync(e => e.CustomerId == existMobile.CustomerId);

                                if (customerEmail is null)
                                {
                                    response.CreateResponse(MessageCodes.NotFound);
                                    return response;
                                }

                                if (customerEmail.IsVerified)
                                {
                                    response.CreateResponse(MessageCodes.EmailAlreadyVerified);
                                    return response;
                                }

                                return await SendEmailLinkHelperAsync(customerEmail, EmailLinkEnum.ActivationLink);
                            }
                        case CustomerStatusEnum.Registered:
                            return response.CreateResponse(MessageCodes.AlreadyExists, nameof(preRegisterDto.Mobile));
                        default:
                            return response.CreateResponse(MessageCodes.Failed);
                    }
                }

                // Handle unverified mobile.
                if (existMobile is not null && !existMobile.IsVerified)
                {
                    if (existMobile.IsPrimary)
                        return await SendMobileCodeHelperAsync(existMobile);
                    else // Todo-Sully: add error message ALternative.
                    {
                        response.CreateResponse(MessageCodes.AlreadyExists, nameof(preRegisterDto.Mobile));
                        return response;
                    }
                }

                // generate sms verification code.
                var verificationCode = await GenerateMobileCodeAsync();

                var mobileVerification = new CustomerSmsverification
                {
                    VerificationCode = verificationCode,
                    CreateDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime)
                };

                var mobile = _mapper.Map<CustomerMobile>(preRegisterDto);

                mobile.PhoneCountryId = mobile.PhoneCountryId;

                var customer = _mapper.Map<Customer>(preRegisterDto);

                mobile.CustomerSmsverifications.Add(mobileVerification);
                customer.CustomerMobiles.Add(mobile);

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.CommitAsync();

                // map retrieve dto.
                registerResult.Id = customer.Id;
                registerResult.MobileId = mobile.Id;
                registerResult.ExpiryDate = mobileVerification.ExpireDate;
                registerResult.Status = CustomerStatusEnum.UnVerified;

                // send sms.
                await SendVerificationCodeSmsAsync(mobile, verificationCode);

                response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> ResendMobileCodeAsync(ResendMobileCodeDto resendMobileCodeDto)
        {
            var response = new Response<PreregisterResultDto>();

            var mobile = await _mobileRepository.GetAsync(m => m.Id == resendMobileCodeDto.MobileId);

            if (mobile is null)
            {
                response.CreateResponse(MessageCodes.NotFound);
                return response;
            }

            switch (resendMobileCodeDto.SmsType)
            {
                case SmsTypeEnum.Verification:
                    {
                        if (mobile.IsVerified)
                            return response.CreateResponse(MessageCodes.MobileAlreadyVerified);
                    }
                    break;
                case SmsTypeEnum.ForgetPassword:
                    {
                        if (!mobile.IsVerified)
                            return response.CreateResponse(MessageCodes.MobileNotVerified);

                        if (mobile.Customer.CustomerStatusId != (int)CustomerStatusEnum.Registered)
                            return response.CreateResponse(MessageCodes.RegistrationProcessIncompleted);
                    }
                    break;
                default:
                    break;
            }

            return await SendMobileCodeHelperAsync(mobile, resendMobileCodeDto.SmsType, isWatsApp: resendMobileCodeDto.isWhatsapp);
        }

        public async Task<IResponse<PreregisterResultDto>> VerifyMobileAsync(VerifyMobileDto verifyMobileDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var registerResult = new PreregisterResultDto();

                var validator = await new VerifyMobileDtoValidator().ValidateAsync(verifyMobileDto);

                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var mobile = await _mobileRepository.GetByIdAsync(verifyMobileDto.MobileId);

                if (mobile is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(mobile));

                var existCode = mobile.CustomerSmsverifications.FirstOrDefault(v => v.VerificationCode.Equals(verifyMobileDto.Code));

                if (existCode is null)
                    return response.CreateResponse(MessageCodes.InvalidCode, nameof(verifyMobileDto.Code));

                if (DateTime.UtcNow > existCode.ExpireDate)
                    return response.CreateResponse(MessageCodes.PhoneCodeExpired);

                // remove previos generated codes.
                _mobileVerificationRepository.DeleteRange(mobile.CustomerSmsverifications);

                // reset sendCount & block sending till date.
                mobile.SendCount = default;
                mobile.BlockForSendingSmsuntil = null;
                mobile.IsVerified = true;
                mobile.ModifiedDate = DateTime.UtcNow;

                // map retrieve dto.
                registerResult.Id = mobile.CustomerId;
                registerResult.MobileId = mobile.Id;

                // update user status.
                if (verifyMobileDto.IsPrimaryMobile)
                {
                    mobile.Customer.CustomerStatusId = verifyMobileDto.FromLandingPage ? (int)CustomerStatusEnum.Registered : (int)CustomerStatusEnum.Verified;
                    mobile.Customer.ModifiedDate = DateTime.UtcNow;
                }

                var mobileUnverified = await _mobileRepository.GetManyAsync(m => m.Mobile.ToLower().Equals(mobile.Mobile) && !m.IsVerified && m.Id != mobile.Id);

                _mobileVerificationRepository.DeleteRange(mobileUnverified.SelectMany(m => m.CustomerSmsverifications));
                _mobileRepository.DeleteRange(mobileUnverified);
                _unitOfWork.Commit();

                _mobileRepository.Update(mobile);
                _unitOfWork.Commit();

                if (verifyMobileDto.IsPrimaryMobile && verifyMobileDto.FromLandingPage)
                {
                    var email = mobile.Customer.CustomerEmails.FirstOrDefault(e => e.IsPrimary);

                    var newLead = new CrmNewLeadDto
                    {
                        CustomerId = email.Customer.Id,
                        Email = email.Email,
                        Mobile = email.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary && m.IsVerified)?.Mobile,
                        Name = email.Customer.Name,
                        CompanyName = email.Customer?.CompanyName,
                        CompanySize = email.Customer?.CompanySize?.Crmid ?? (int)CompanySizeCrmEnum.OneToFive,
                        CreatedDate = email.Customer.CreateDate,
                        CountryId = email.Customer?.Country?.Crmid,
                        CurrencyId = email.Customer?.Country?.CountryCurrency?.Currency?.Crmid,
                        TaxRegistrationNumber = int.TryParse(email?.Customer?.TaxRegistrationNumber, out int taxReg) ? taxReg : 0,
                        Website = email.Customer?.CompanyWebsite,
                        IndustryCode = email.Customer?.Industry?.Crmid ?? (int)IndustryCodeCrmEnum.Accounting,
                        SoftwareId = _authSetting.Crm.SoftwareId,
                        FollowType = _authSetting.Crm.FollowType,
                        LeadSourceCode = _authSetting.Crm.LeadSourceCode,
                    };

                    BackgroundJob.Enqueue(() => CreateCrmLeadHelperAsync(newLead));
                }

                return response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> VerifyForgetPasswordMobileCodeAsync(ForgetPasswordVerifyMobileDto verifyMobileDto)
        {
            var response = new Response<bool>();

            try
            {
                var validator = await new ForgetPasswordVerifyMobileDtoValidator().ValidateAsync(verifyMobileDto);

                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var mobile = await _mobileRepository.GetByIdAsync(verifyMobileDto.MobileId);

                if (mobile is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(mobile));

                var existCode = mobile.CustomerSmsverifications.FirstOrDefault(v => v.VerificationCode.Equals(verifyMobileDto.Code));

                if (existCode is null)
                    return response.CreateResponse(MessageCodes.InvalidCode, nameof(verifyMobileDto.Code));

                if (existCode.IsVerifiedFromFrogetPassword)
                    return response.CreateResponse(MessageCodes.MobileAlreadyVerified, nameof(verifyMobileDto.Code));

                if (DateTime.UtcNow > existCode.ExpireDate)
                    return response.CreateResponse(MessageCodes.PhoneCodeExpired);

                existCode.IsVerifiedFromFrogetPassword = true;

                _mobileRepository.Update(mobile);
                await _unitOfWork.CommitAsync();

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<PreregisterResultDto>> ChangeMobileAsync(ChangeMobileDto newMobileDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validator = await new ChangeMobileDtoValidator().ValidateAsync(newMobileDto);

                if (!validator.IsValid)
                {
                    response.CreateResponse(validator.Errors);
                    return response;
                }

                var newMobileCountry = await _countryRepository.GetAsync(c => c.PhoneCode.Equals(newMobileDto.NewMobileCountryCode));

                if (newMobileCountry == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(newMobileDto.NewMobileCountryCode));

                var exesitMobile = _mobileRepository.Where(m => m.Mobile.Equals(newMobileDto.NewMobile)).ToList();


                if (exesitMobile is not null && exesitMobile.Any(e => e.IsVerified))
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(newMobileDto.NewMobile));


                var oldMobile = await _mobileRepository.GetByIdAsync(newMobileDto.MobileId);

                // change mobile validation.
                var validation = ValidateChangeMobile(newMobileDto, oldMobile);

                if (!validation.IsSuccess)
                {
                    response.AppendErrors(validation.Errors)
                            .CreateResponse();

                    return response;
                }
                //oldMobile.CustomerId != exesitMobile.FirstOrDefault().CustomerId
                //delete old customer not Verfied
                if (exesitMobile.Count > 0 && !exesitMobile.Any(e => e.IsVerified))
                {
                    _mobileVerificationRepository.DeleteRange(exesitMobile.SelectMany(e => e.CustomerSmsverifications));
                    _mobileRepository.DeleteRange(exesitMobile.Where(e => e.CustomerId != oldMobile.CustomerId));
                    _emailVerificationRepository.DeleteRange(exesitMobile.Where(e => e.CustomerId != oldMobile.CustomerId).SelectMany(e => e.Customer.CustomerEmails.FirstOrDefault().CustomerEmailVerifications));
                    _emailRepository.DeleteRange(exesitMobile.Where(e => e.CustomerId != oldMobile.CustomerId).SelectMany(e => e.Customer.CustomerEmails));
                    _customerRepository.DeleteRange(exesitMobile.Where(e => e.CustomerId != oldMobile.CustomerId).Select(e => e.Customer));
                }

                if (!newMobileDto.NewMobileCountryCode.Equals(oldMobile.PhoneCode))
                {
                    oldMobile.PhoneCode = newMobileDto.NewMobileCountryCode;
                    oldMobile.PhoneCountryId = newMobileCountry?.Id;
                }

                // change mobile.
                oldMobile.Mobile = newMobileDto.NewMobile;

                // resend code, validate sendCount, blockSendingDate, etc...
                return await SendMobileCodeHelperAsync(oldMobile, isWatsApp: newMobileDto.IsWatsApp);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> RegisterAsync(RegisterDto registerDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new RegisterDtoValidator().ValidateAsync(registerDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                // validate email is not exist in verified emails.
                if (await _emailRepository.AnyAsync(e => e.Email.ToLower().Equals(registerDto.Email.ToLower()) && e.IsVerified))
                {
                    response.CreateResponse(MessageCodes.AlreadyExists, nameof(registerDto.Email));
                    return response;
                }

                var customer = await _customerRepository.GetByIdAsync(registerDto.Id);

                if (customer is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(registerDto.Id));
                    return response;
                }

                var code = await GenerateEmailCodeAsync(registerDto.Id);

                customer = _mapper.Map(registerDto, customer);
                var email = _mapper.Map<CustomerEmail>(registerDto);

                // hash password.
                customer.Password = _passwordHasher.HashPassword(registerDto.Password);

                // assign email to customer.
                customer.CustomerEmails.Add(email);
                _customerRepository.Update(customer);

                var emailVerification = new CustomerEmailVerification
                {
                    CustomerEmail = email,
                    VerificationCode = code,
                    CreateDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.Add(_authSetting.Email.CodeExpiryTime)
                };

                _emailVerificationRepository.Add(emailVerification);
                await _unitOfWork.CommitAsync();

                // send email.
                await SendEmailAsync(new VerificationLinkDto { EmailId = email.Id, Email = email.Email, Code = code, ExpiryDate = emailVerification.ExpireDate, Name = customer.Name }, EmailLinkEnum.ActivationLink);

                // map retrieve dto.
                var registerResult = new PreregisterResultDto
                {
                    Id = customer.Id,
                    EmailId = email.Id,
                    ExpiryDate = emailVerification.ExpireDate,
                    Status = CustomerStatusEnum.Pending
                };

                response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var verifuEmailValidation = await new VerifyEmailDtoValidator().ValidateAsync(verifyEmailDto);

                if (!verifuEmailValidation.IsValid)
                {
                    response.CreateResponse(verifuEmailValidation.Errors);
                    return response;
                }

                var linkByteArray = Convert.FromBase64String(verifyEmailDto.Token);

                var jsonToken = Encoding.UTF8.GetString(linkByteArray);

                var verificationLinkDto = JsonConvert.DeserializeObject<VerificationLinkDto>(jsonToken);

                var verificationLinkValidation = await new VerificationLinkDtoValidator().ValidateAsync(verificationLinkDto);

                if (!verifuEmailValidation.IsValid)
                {
                    response.CreateResponse(verifuEmailValidation.Errors);
                    return response;
                }

                var email = await _emailRepository.GetAsync(e => e.Id == verificationLinkDto.EmailId);

                if (email is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(verificationLinkDto.EmailId));
                    return response;
                }

                var existCode = email.CustomerEmailVerifications.FirstOrDefault(v => v.VerificationCode.Equals(verificationLinkDto.Code));

                if (existCode is null)
                {
                    response.CreateResponse(MessageCodes.InvalidVerificationLink, nameof(verifyEmailDto.Token));
                    return response;
                }

                // map retrieve dto.
                var registerResult = new PreregisterResultDto();

                registerResult.Id = email.CustomerId;
                registerResult.EmailId = email.Id;

                if (DateTime.UtcNow > existCode.ExpireDate)
                {
                    registerResult.ExpiryDate = DateTime.SpecifyKind(existCode.ExpireDate, DateTimeKind.Utc);
                    registerResult.Status = (CustomerStatusEnum)email.Customer.CustomerStatusId;

                    response.CreateResponse(registerResult);
                    return response;
                }

                // validate provided password in verification link  (primary and alternative) case.
                if (verifyEmailDto.IsVerificationLink)
                {
                    if (string.IsNullOrEmpty(verifyEmailDto.Password))
                    {
                        response.CreateResponse(MessageCodes.Required, nameof(verifyEmailDto.Password));
                        return response;
                    }

                    if (!_passwordHasher.VerifyHashedPassword(verifyEmailDto.Password, email.Customer.Password))
                    {
                        response.CreateResponse(MessageCodes.InvalidPasswordOrLink, nameof(verifyEmailDto.Password));
                        return response;
                    }
                }

                // remove previos generated codes.
                _emailVerificationRepository.DeleteRange(email.CustomerEmailVerifications);

                // reset sendCount & block sending till date.
                email.SendCount = default;
                email.BlockForSendingEmailUntil = null;
                email.IsVerified = true;
                email.ModifiedDate = DateTime.UtcNow;

                // update user status.
                email.Customer.ModifiedDate = DateTime.UtcNow;
                email.Customer.CustomerStatusId = (int)CustomerStatusEnum.Registered;

                _emailRepository.Update(email);

                await _unitOfWork.CommitAsync();

                if (email.IsPrimary && string.IsNullOrEmpty(email.Customer.CustomerCrmid))
                {
                    // Create customer in CRM as lead.
                    CreateCrmLeadHelperAsync(email);
                }

                registerResult.Status = (CustomerStatusEnum)email.Customer.CustomerStatusId;

                response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> ResendVerificationEmailLinkAsync(int emailId)
        {
            var response = new Response<PreregisterResultDto>();

            var email = await _emailRepository.GetAsync(m => m.Id == emailId);

            if (email is null)
            {
                response.CreateResponse(MessageCodes.NotFound);
                return response;
            }

            if (email.IsVerified)
            {
                response.CreateResponse(MessageCodes.EmailAlreadyVerified);
                return response;
            }

            return await SendEmailLinkHelperAsync(email, EmailLinkEnum.ActivationLink);
        }

        public async Task<IResponse<PreregisterResultDto>> RegisterByGoogleAsync(OAuthRegisterDto googleRegisterDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new GoogleLoginDtoValidator().ValidateAsync(googleRegisterDto);

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                // Get CountryBy CountryCode
                var country = await _countryRepository.GetAsync(x => x.PhoneCode == googleRegisterDto.CountryCode);

                if (country is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(googleRegisterDto.CountryCode));

                // call Google api and fetch data.
                var userInfoRequest = new Oauth2Service().Userinfo.Get();
                userInfoRequest.OauthToken = googleRegisterDto.Token;
                var googleUserInfo = await userInfoRequest.ExecuteAsync();

                if (!(googleUserInfo is not null && googleUserInfo.VerifiedEmail.HasValue && googleUserInfo.VerifiedEmail.Value))
                    return response.CreateResponse(MessageCodes.EmailNotVerified, nameof(googleRegisterDto.Token));

                // check user account exist.
                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals((object)googleUserInfo.Email.ToLower()) && e.IsVerified);

                // check user account exist.
                var existMobiles = await _mobileRepository.GetManyAsync(e => e.Mobile.ToLower().Equals((object)googleRegisterDto.Mobile.ToLower()));

                // Handle verified mobile.
                if (existMobiles is not null && existMobiles.Any(m => m.IsVerified))
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));

                var lastMobileNotVerified = existMobiles.Where(m => m.Customer.CustomerEmails.Any(e => e.Email.Equals(googleRegisterDto.Email))).OrderBy(x => x.Id).LastOrDefault();


                var registerDto = new PreregisterLandingPageDto
                {
                    Email = googleUserInfo.Email,
                    CountryCode = googleRegisterDto.CountryCode,
                    Name = googleUserInfo.Name,
                    CompanyName = googleUserInfo.Name,
                    Mobile = googleRegisterDto.Mobile,
                    SourceId = (int)googleRegisterDto.SourceId
                };

                var emailAndMobileValidator = await ValidateEmailAndMobileHelperAsync(registerDto);

                if (!emailAndMobileValidator.IsSuccess)
                    response.AppendErrors(emailAndMobileValidator.Errors);

                if (!response.IsSuccess)
                    return response.CreateResponse();


                // Handle unverified mobile.
                if (email is not null && lastMobileNotVerified is not null && !lastMobileNotVerified.IsVerified)
                {
                    if (lastMobileNotVerified.CustomerId == email.CustomerId)
                    {
                        if (lastMobileNotVerified.IsPrimary)
                            return await SendMobileCodeHelperAsync(lastMobileNotVerified);
                        else
                        {
                            response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));
                            return response;
                        }

                    }
                    else
                    {

                        //_mobileVerificationRepository.DeleteRange(mobile.CustomerSmsverifications);
                        //_mobileRepository.Delete(mobile);
                        //_emailRepository.DeleteRange(mobile.Customer.CustomerEmails);
                        //_customerRepository.Delete(mobile.Customer);
                    }

                }




                //// generate sms verification code.
                //var verificationCode = await GenerateMobileCodeAsync();

                //var mobileVerification = new CustomerSmsverification
                //{
                //    SmstypeId = (int)SmsTypeEnum.Verification,
                //    VerificationCode = verificationCode,
                //    CreateDate = DateTime.UtcNow,
                //    ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime)
                //};

                var customerMobile = _mapper.Map<CustomerMobile>(registerDto);
                var customerEmail = _mapper.Map<CustomerEmail>(registerDto);
                var customer = _mapper.Map<Customer>(registerDto);

                customerMobile.PhoneCountryId = country.Id;
                customer.CountryId = country.Id;
                customer.OauthTypeId = (int)OAuthTypeEnum.Google;
                customer.OauthResponse = JsonConvert.SerializeObject(new OAuthResponseDto { Google = new OAuthDataDto { Id = googleUserInfo.Id } });

                //customerMobile.CustomerSmsverifications.Add(mobileVerification);
                customer.CustomerMobiles.Add(customerMobile);
                customer.CustomerEmails.Add(customerEmail);

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.CommitAsync();

                var registerResult = new PreregisterResultDto();

                // map retrieve dto.
                registerResult.Id = customer.Id;
                registerResult.MobileId = customerMobile.Id;
                //registerResult.ExpiryDate = mobileVerification.ExpireDate;
                registerResult.Status = CustomerStatusEnum.UnVerified;

                // send sms.
                // await SendVerificationCodeSmsAsync(customerMobile, verificationCode);

                return response.CreateResponse(registerResult);
            }
            catch (GoogleApiException ex)
            {
                return response.CreateResponse(MessageCodes.InvalidToken);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        //public async Task<IResponse<PreregisterResultDto>> RegisterByAppleAsync(AppleRegisterDto appleRegisterDto)
        //{
        //    var response = new Response<PreregisterResultDto>();

        //    try
        //    {
        //        var validation = await new AppleRegisterDtoValidator().ValidateAsync(appleRegisterDto);

        //        if (!validation.IsValid)
        //            return response.CreateResponse(validation.Errors);

        //         //var claims = new JwtSecurityTokenHandler().ReadJwtToken(appleRegisterDto.Token).Claims;

        //        var userInfo = new AppleUserInfo
        //        {
        //            Id = appleRegisterDto.TokenId,
        //            Email = appleRegisterDto.Email,
        //            Name = appleRegisterDto.Name,
        //            //VerifiedEmail =  bool.Parse(claims.FirstOrDefault(x => x.Type ==nameof(TokenClaimTypeEnum.email_verified))?.Value)
        //        };
        //        // Get CountryBy CountryCode
        //        var country = await _countryRepository.GetAsync(x => x.PhoneCode == appleRegisterDto.CountryCode);

        //        if (country is null)
        //            return response.CreateResponse(MessageCodes.NotFound, nameof(appleRegisterDto.CountryCode));

        //        // call Google api and fetch data.

        //        //if (!(userInfo is not null && userInfo.VerifiedEmail ))
        //        //    return response.CreateResponse(MessageCodes.EmailNotVerified, nameof(appleRegisterDto.Email));

        //        // check user account exist.
        //        var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals((object)userInfo.Email.ToLower()) && e.IsVerified);

        //        // check user account exist.
        //        var mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals((object)appleRegisterDto.Mobile.ToLower()));

        //        // Handle verified mobile.
        //        if (mobile is not null && mobile.IsVerified)
        //            return response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));

        //        var registerDto = new PreregisterLandingPageDto
        //        {
        //            Email = userInfo.Email,
        //            CountryCode = appleRegisterDto.CountryCode,
        //            Name = userInfo.Name /*?? userInfo.Email.Split("@")[0].ToString()*/,
        //            CompanyName = userInfo.Name,
        //            Mobile = appleRegisterDto.Mobile,
        //            SourceId = (int)appleRegisterDto.SourceId
        //        };

        //        var emailAndMobileValidator = await ValidateEmailAndMobileHelperAsync(registerDto);

        //        if (!emailAndMobileValidator.IsSuccess)
        //            response.AppendErrors(emailAndMobileValidator.Errors);

        //        if (!response.IsSuccess)
        //            return response.CreateResponse();


        //        // Handle unverified mobile.
        //        if (email is not null && mobile is not null && !mobile.IsVerified)
        //        {
        //            if (mobile.CustomerId == email.CustomerId)
        //            {
        //                if (mobile.IsPrimary)
        //                    return await SendMobileCodeHelperAsync(mobile);
        //                else
        //                {
        //                    response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));
        //                    return response;
        //                }

        //            }
        //            else
        //            {

        //                //_mobileVerificationRepository.DeleteRange(mobile.CustomerSmsverifications);
        //                //_mobileRepository.Delete(mobile);
        //                //_emailRepository.DeleteRange(mobile.Customer.CustomerEmails);
        //                //_customerRepository.Delete(mobile.Customer);
        //            }

        //        }




        //        //// generate sms verification code.
        //        //var verificationCode = await GenerateMobileCodeAsync();

        //        //var mobileVerification = new CustomerSmsverification
        //        //{
        //        //    SmstypeId = (int)SmsTypeEnum.Verification,
        //        //    VerificationCode = verificationCode,
        //        //    CreateDate = DateTime.UtcNow,
        //        //    ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime)
        //        //};

        //        var customerMobile = _mapper.Map<CustomerMobile>(registerDto);
        //        var customerEmail = _mapper.Map<CustomerEmail>(registerDto);
        //        var customer = _mapper.Map<Customer>(registerDto);

        //        customerMobile.PhoneCountryId = country.Id;
        //        customer.CountryId = country.Id;
        //        customer.OauthTypeId = (int)OAuthTypeEnum.Apple;
        //        customer.OauthResponse = JsonConvert.SerializeObject(new OAuthResponseDto { Apple = new OAuthDataDto { Id = userInfo.Id } });

        //        //customerMobile.CustomerSmsverifications.Add(mobileVerification);
        //        customer.CustomerMobiles.Add(customerMobile);
        //        customer.CustomerEmails.Add(customerEmail);

        //        await _customerRepository.AddAsync(customer);
        //        await _unitOfWork.CommitAsync();

        //        var registerResult = new PreregisterResultDto();

        //        // map retrieve dto.
        //        registerResult.Id = customer.Id;
        //        registerResult.MobileId = customerMobile.Id;

        //        //registerResult.ExpiryDate = mobileVerification.ExpireDate;
        //        registerResult.Status = CustomerStatusEnum.UnVerified;

        //        // send sms.
        //        // await SendVerificationCodeSmsAsync(customerMobile, verificationCode);

        //        return response.CreateResponse(registerResult);
        //    }
        //    catch (GoogleApiException ex)
        //    {
        //        return response.CreateResponse(MessageCodes.InvalidToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        return response.CreateResponse(ex);
        //    }
        //}


        //public async Task<IResponse<TokenResultDto>> LoginByAppleAsync(AppleLoginDto loginDto)
        //{
        //    var response = new Response<TokenResultDto>();
        //    try
        //    {
        //        var validation = await new AppleLoginDtoValidator().ValidateAsync(loginDto);

        //        if (!validation.IsValid)
        //            return response.CreateResponse(validation.Errors);

        //        // call Google api and fetch data.
        //        //var claims = new JwtSecurityTokenHandler().ReadJwtToken(loginDto.Token).Claims;

        //        var userInfo = new AppleUserInfo
        //        {
        //            Id = loginDto.TokenId,
        //            Email = loginDto.Email
        //            //Name = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
        //            //VerifiedEmail = bool.Parse(claims.FirstOrDefault(x => x.Type == nameof(TokenClaimTypeEnum.email_verified))?.Value)
        //        };

        //        if (userInfo is null)
        //            return response.CreateResponse(MessageCodes.InvalidToken);

        //        var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(userInfo.Email.ToLower()) &&
        //                                                         e.IsPrimary &&
        //                                                         e.IsVerified &&
        //                                                         e.Customer.OauthTypeId == (int)OAuthTypeEnum.Apple);


        //        if (email is null)
        //            return response.CreateResponse(MessageCodes.NotFound, nameof(CustomerEmail));

        //        var mobile = await _mobileRepository.GetAsync(x => x.CustomerId == email.CustomerId && x.IsPrimary);

        //        if (mobile is null) return response.CreateResponse(MessageCodes.InvalidLoginCredentials);

        //        if (!mobile.IsVerified && mobile.Customer.OauthTypeId == (int)OAuthTypeEnum.Apple)
        //            return response.CreateResponse(MessageCodes.MobileNotVerified, $"{mobile.Id}");

        //        if (!mobile.IsVerified && mobile.Customer.OauthTypeId != (int)OAuthTypeEnum.Apple)
        //            return response.CreateResponse(MessageCodes.InvalidLoginCredentials);



        //        var generatedJwtToken = await GenerateJwtTokenAsync(email);

        //        var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, email.CustomerId);

        //        var tokenResultDto = new TokenResultDto
        //        {
        //            Token = generatedJwtToken.Token,
        //            RefreshToken = generatedRefreshToken
        //        };

        //        response.CreateResponse(tokenResultDto);
        //    }
        //    catch (GoogleApiException ex)
        //    {
        //        return response.CreateResponse(MessageCodes.InvalidToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.CreateResponse(ex);
        //    }

        //    return response;
        //}

        public async Task<IResponse<PreregisterResultDto>> RegisterByAppleAsync(OAuthRegisterDto appleRegisterDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new AppleLoginDtoValidator().ValidateAsync(appleRegisterDto);

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                var claims = new JwtSecurityTokenHandler().ReadJwtToken(appleRegisterDto.Token).Claims;

                var userInfo = new AppleUserInfo
                {
                    Id = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
                    Email = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value,
                    Name = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
                    VerifiedEmail = bool.Parse(claims.FirstOrDefault(x => x.Type == nameof(TokenClaimTypeEnum.email_verified))?.Value)
                };
                // Get CountryBy CountryCode
                var country = await _countryRepository.GetAsync(x => x.PhoneCode == appleRegisterDto.CountryCode);

                if (country is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(appleRegisterDto.CountryCode));

                // call Google api and fetch data.

                if (!(userInfo is not null && userInfo.VerifiedEmail))
                    return response.CreateResponse(MessageCodes.EmailNotVerified, nameof(appleRegisterDto.Token));

                // check user account exist.
                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals((object)userInfo.Email.ToLower()) && e.IsVerified);

                // check user account exist.
                var mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals((object)appleRegisterDto.Mobile.ToLower()) && e.IsVerified);

                // Handle verified mobile.
                if (mobile is not null)
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));

                var registerDto = new PreregisterLandingPageDto
                {
                    Email = userInfo.Email,
                    CountryCode = appleRegisterDto.CountryCode,
                    Name = userInfo.Name ?? userInfo.Email.Split("@")[0].ToString(),
                    CompanyName = userInfo.Name,
                    Mobile = appleRegisterDto.Mobile,
                    SourceId = (int)appleRegisterDto.SourceId
                };

                var emailAndMobileValidator = await ValidateEmailAndMobileHelperAsync(registerDto);

                if (!emailAndMobileValidator.IsSuccess)
                    response.AppendErrors(emailAndMobileValidator.Errors);

                if (!response.IsSuccess)
                    return response.CreateResponse();


                // Handle unverified mobile.
                if (email is not null && mobile is not null && !mobile.IsVerified)
                {
                    if (mobile.CustomerId == email.CustomerId)
                    {
                        if (mobile.IsPrimary)
                            return await SendMobileCodeHelperAsync(mobile);
                        else
                        {
                            response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));
                            return response;
                        }

                    }

                }

                var customerMobile = _mapper.Map<CustomerMobile>(registerDto);
                var customerEmail = _mapper.Map<CustomerEmail>(registerDto);
                var customer = _mapper.Map<Customer>(registerDto);

                customerMobile.PhoneCountryId = country.Id;
                customer.CountryId = country.Id;
                customer.OauthTypeId = (int)OAuthTypeEnum.Apple;
                customer.OauthResponse = JsonConvert.SerializeObject(new OAuthResponseDto { Apple = new OAuthDataDto { Id = userInfo.Id } });

                //customerMobile.CustomerSmsverifications.Add(mobileVerification);
                customer.CustomerMobiles.Add(customerMobile);
                customer.CustomerEmails.Add(customerEmail);

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.CommitAsync();

                var registerResult = new PreregisterResultDto();

                // map retrieve dto.
                registerResult.Id = customer.Id;
                registerResult.MobileId = customerMobile.Id;

                //registerResult.ExpiryDate = mobileVerification.ExpireDate;
                registerResult.Status = CustomerStatusEnum.UnVerified;

                // send sms.
                // await SendVerificationCodeSmsAsync(customerMobile, verificationCode);

                return response.CreateResponse(registerResult);
            }
            catch (GoogleApiException ex)
            {
                return response.CreateResponse(MessageCodes.InvalidToken);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        public async Task<IResponse<TokenResultDto>> LoginByAppleAsync(OAuthLoginDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {
                var validation = await new OAuthLoginDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                // call Google api and fetch data.
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(loginDto.Token).Claims;

                var userInfo = new AppleUserInfo
                {
                    Id = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
                    Email = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value,
                    Name = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
                    VerifiedEmail = bool.Parse(claims.FirstOrDefault(x => x.Type == nameof(TokenClaimTypeEnum.email_verified))?.Value)
                };

                if (userInfo is null)
                    return response.CreateResponse(MessageCodes.InvalidToken);

                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(userInfo.Email.ToLower()) &&
                                                                 e.IsPrimary &&
                                                                 e.IsVerified &&
                                                                 e.Customer.OauthTypeId == (int)OAuthTypeEnum.Apple);


                if (email is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(CustomerEmail));

                var mobile = await _mobileRepository.GetAsync(x => x.CustomerId == email.CustomerId && x.IsPrimary);

                if (mobile is null) return response.CreateResponse(MessageCodes.InvalidLoginCredentials);

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId == (int)OAuthTypeEnum.Apple)
                    return response.CreateResponse(MessageCodes.MobileNotVerified, $"{mobile.Id}");

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId != (int)OAuthTypeEnum.Apple)
                    return response.CreateResponse(MessageCodes.InvalidLoginCredentials);



                var generatedJwtToken = await GenerateJwtTokenAsync(email);

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, email.CustomerId);

                var tokenResultDto = new TokenResultDto
                {
                    Token = generatedJwtToken.Token,
                    RefreshToken = generatedRefreshToken
                };

                response.CreateResponse(tokenResultDto);
            }
            catch (GoogleApiException ex)
            {
                return response.CreateResponse(MessageCodes.InvalidToken);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<TokenResultDto>> LoginByGoogleAsync(OAuthLoginDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {
                var validation = await new OAuthLoginDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                // call Google api and fetch data.
                var userInfoRequest = new Oauth2Service().Userinfo.Get();
                userInfoRequest.OauthToken = loginDto.Token;
                var googleUserInfo = await userInfoRequest.ExecuteAsync();

                if (googleUserInfo is null)
                    return response.CreateResponse(MessageCodes.InvalidToken);

                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(googleUserInfo.Email.ToLower()) &&
                                                                 e.IsPrimary &&
                                                                 e.IsVerified &&
                                                                 e.Customer.OauthTypeId == (int)OAuthTypeEnum.Google);


                if (email is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(CustomerEmail));

                var mobile = await _mobileRepository.GetAsync(x => x.CustomerId == email.CustomerId && x.IsPrimary);

                if (mobile is null) return response.CreateResponse(MessageCodes.InvalidLoginCredentials);

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId == (int)OAuthTypeEnum.Google)
                    return response.CreateResponse(MessageCodes.MobileNotVerified, $"{mobile.Id}");

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId != (int)OAuthTypeEnum.Google)
                    return response.CreateResponse(MessageCodes.InvalidLoginCredentials);



                var generatedJwtToken = await GenerateJwtTokenAsync(email);

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, email.CustomerId);

                var tokenResultDto = new TokenResultDto
                {
                    Token = generatedJwtToken.Token,
                    RefreshToken = generatedRefreshToken
                };

                response.CreateResponse(tokenResultDto);
            }
            catch (GoogleApiException ex)
            {
                return response.CreateResponse(MessageCodes.InvalidToken);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<TokenResultDto>> LoginByFaceBookAsync(OAuthLoginDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {
                var validation = await new OAuthLoginDtoValidator().ValidateAsync(loginDto);
                var userInfoUrl = "https://graph.facebook.com/me?fields=id,email,name&access_token=";

                if (!validation.IsValid)
                    return response.CreateResponse(validation.Errors);

                // call Google api and fetch data.
                var formatUrl = $"{userInfoUrl}{loginDto.Token}";
                var result = await _httpClientFactory.CreateClient().GetAsync(formatUrl);

                if (!result.IsSuccessStatusCode)
                    return response.CreateResponse(MessageCodes.UnAuthorizedAccess);

                var userJson = await result.Content.ReadAsStringAsync();
                var userInfoRequest = JObject.Parse(userJson);

                var customer = _customerRepository.Where(x => x.OauthTypeId == (int)OAuthTypeEnum.Facebook &&
                                                                             x.OauthResponse == JsonConvert.SerializeObject(
                                                                                                                       new OAuthResponseDto
                                                                                                                       {
                                                                                                                           Facebook = new OAuthDataDto
                                                                                                                           {
                                                                                                                               Id = userInfoRequest["id"].ToString()
                                                                                                                           }
                                                                                                                       }
                                                                                                                            ) &&
                                                                                                                            x.CustomerMobiles
                                                                                                                            .Any(x => x.IsPrimary && x.IsVerified))
                                                         .FirstOrDefault();



                if (customer is null)
                    return response.CreateResponse(MessageCodes.NotFound);

                var mobile = await _mobileRepository.GetAsync(x => x.CustomerId == customer.Id && x.IsPrimary);

                if (mobile is null) return response.CreateResponse(MessageCodes.InvalidLoginCredentials);

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId == (int)OAuthTypeEnum.Facebook)
                    return response.CreateResponse(MessageCodes.MobileNotVerified, $"{mobile.Id}");

                if (!mobile.IsVerified && mobile.Customer.OauthTypeId != (int)OAuthTypeEnum.Facebook)
                    return response.CreateResponse(MessageCodes.InvalidLoginCredentials);



                var generatedJwtToken = await GenerateJwtTokenAsync(customer.CustomerEmails.FirstOrDefault());

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, customer.Id);

                var tokenResultDto = new TokenResultDto
                {
                    Token = generatedJwtToken.Token,
                    RefreshToken = generatedRefreshToken
                };

                response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> RegisterByFaceBookAsync(OAuthRegisterDto loginDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var userInfoUrl = "https://graph.facebook.com/me?fields=id,email,name&access_token=";
                var validation = await new GoogleLoginDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                // Get CountryBy CountryCode
                var country = await _countryRepository.GetAsync(x => x.PhoneCode == loginDto.CountryCode);

                if (country is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(loginDto.CountryCode));

                // call Facebook api and fetch data.
                var formatUrl = $"{userInfoUrl}{loginDto.Token}";
                var result = await _httpClientFactory.CreateClient().GetAsync(formatUrl);

                if (!result.IsSuccessStatusCode)
                    return response.CreateResponse(MessageCodes.UnAuthorizedAccess);

                var userJson = await result.Content.ReadAsStringAsync();
                var userInfoRequest = JObject.Parse(userJson);

                if (userInfoRequest is null)
                    return response.CreateResponse(MessageCodes.EmailNotVerified, nameof(loginDto.Token));


                var customerExcist = _customerRepository.Where(x => x.OauthTypeId == (int)OAuthTypeEnum.Facebook &&
                                                                             x.OauthResponse == JsonConvert.SerializeObject(
                                                                                                                       new OAuthResponseDto
                                                                                                                       {
                                                                                                                           Facebook = new OAuthDataDto
                                                                                                                           {
                                                                                                                               Id = userInfoRequest["id"].ToString()
                                                                                                                           }
                                                                                                                       }
                                                                                                                            )).FirstOrDefault();

                var mobileExcist = customerExcist?.CustomerMobiles?.FirstOrDefault(x => x.IsVerified && x.IsPrimary) != null;

                if (customerExcist is not null && mobileExcist)
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(loginDto.Email));

                // check user account exist.
                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(loginDto.Email) && e.IsVerified);

                // check user account exist.
                var mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals((object)loginDto.Mobile.ToLower()));

                // Handle verified mobile.
                if (mobile is not null && mobile.IsVerified)
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));

                var registerDto = new PreregisterLandingPageDto
                {
                    Email = loginDto.Email,
                    CountryCode = loginDto.CountryCode,
                    Name = userInfoRequest["name"].ToString(),
                    CompanyName = userInfoRequest["name"].ToString(),
                    Mobile = loginDto.Mobile,
                    SourceId = (int)loginDto.SourceId
                };

                var emailAndMobileValidator = await ValidateEmailAndMobileHelperAsync(registerDto);

                if (!emailAndMobileValidator.IsSuccess)
                    response.AppendErrors(emailAndMobileValidator.Errors);

                if (!response.IsSuccess)
                    return response.CreateResponse();



                // Handle unverified mobile.
                if (email is not null && mobile is not null && !mobile.IsVerified)
                {
                    if (mobile.CustomerId == email.CustomerId)
                    {
                        if (mobile.IsPrimary)
                            return await SendMobileCodeHelperAsync(mobile);
                        else
                        {
                            response.CreateResponse(MessageCodes.AlreadyExists, nameof(OAuthRegisterDto.Mobile));
                            return response;
                        }

                    }
                    else
                    {

                        //_mobileVerificationRepository.DeleteRange(mobile.CustomerSmsverifications);
                        //_mobileRepository.Delete(mobile);
                        //_emailRepository.DeleteRange(mobile.Customer.CustomerEmails);
                        //_customerRepository.Delete(mobile.Customer);
                    }

                }



                // generate sms verification code.
                //var verificationCode = await GenerateMobileCodeAsync();

                //var mobileVerification = new CustomerSmsverification
                //{
                //    SmstypeId = (int)SmsTypeEnum.Verification,
                //    VerificationCode = verificationCode,
                //    CreateDate = DateTime.UtcNow,
                //    ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime)
                //};

                var customerMobile = _mapper.Map<CustomerMobile>(registerDto);
                var customerEmail = _mapper.Map<CustomerEmail>(registerDto);
                var customer = _mapper.Map<Customer>(registerDto);

                customerMobile.PhoneCountryId = country.Id;
                customer.CountryId = country.Id;
                customer.OauthTypeId = (int)OAuthTypeEnum.Facebook;
                customer.OauthResponse = JsonConvert.SerializeObject(new OAuthResponseDto { Facebook = new OAuthDataDto { Id = userInfoRequest["id"].ToString() } });

                // customerMobile.CustomerSmsverifications.Add(mobileVerification);
                customer.CustomerMobiles.Add(customerMobile);
                customer.CustomerEmails.Add(customerEmail);

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.CommitAsync();

                var registerResult = new PreregisterResultDto();

                // map retrieve dto.
                registerResult.Id = customer.Id;
                registerResult.MobileId = customerMobile.Id;
                //registerResult.ExpiryDate = mobileVerification.ExpireDate;
                registerResult.Status = CustomerStatusEnum.UnVerified;

                // send sms.
                // await SendVerificationCodeSmsAsync(customerMobile, verificationCode);

                return response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<TokenResultDto>> LoginAsync(LoginDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {
                var validation = await new LoginDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var tokenResultDto = new TokenResultDto();

                CustomerMobile mobile = null;
                CustomerEmail email = null;
                Customer customer = null;

                if (loginDto.Email.Contains("@"))
                {
                    email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(loginDto.Email.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified);

                    if (email is null || email?.Customer?.OauthTypeId != (int)OAuthTypeEnum.Normal)
                        return response.CreateResponse(MessageCodes.InvalidLoginCredentials);

                    customer = email.Customer;
                }
                else
                {
                    mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals(loginDto.Email.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified);

                    if (mobile is null || mobile?.Customer?.OauthTypeId != (int)OAuthTypeEnum.Normal)
                        return response.CreateResponse(MessageCodes.InvalidLoginCredentials);


                    customer = mobile.Customer;

                    email = customer.CustomerEmails.FirstOrDefault(e => e.IsPrimary && e.IsVerified);
                }


                switch ((CustomerStatusEnum)customer.CustomerStatusId)
                {
                    case CustomerStatusEnum.UnVerified:
                    case CustomerStatusEnum.Suspended:
                    case CustomerStatusEnum.Pending:
                        return response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                }

                if (!_passwordHasher.VerifyHashedPassword(loginDto.Password, customer.Password))
                {
                    response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                    return response;
                }

                var generatedJwtToken = await GenerateJwtTokenAsync(email);

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, customer.Id);

                tokenResultDto.Token = generatedJwtToken.Token;
                tokenResultDto.RefreshToken = generatedRefreshToken;

                response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }
        public async Task<IResponse<ValidateEmailResultDto>> ValidateEmailAsync(string userName)
        {
            var response = new Response<ValidateEmailResultDto>();
            try
            {
                CustomerMobile mobile = null;
                CustomerEmail email = null;

                if (userName.Contains("@"))
                {
                    email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(userName.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified);

                    if (email is null)
                        return response.CreateResponse(MessageCodes.NotFound, nameof(CustomerEmail));

                    return response.CreateResponse(GetOAuthType(email.Customer.OauthTypeId, email.Email));

                }
                else
                {
                    mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals(userName.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified);

                    if (mobile is null)
                        return response.CreateResponse(MessageCodes.NotFound, nameof(CustomerMobile));

                    return response.CreateResponse(GetOAuthType(mobile.Customer.OauthTypeId, mobile.Mobile));
                }
            }

            catch (Exception e)
            {

                throw;
            }
        }

        private ValidateEmailResultDto GetOAuthType(int oAuthTypeId, string userName)
        {
            return oAuthTypeId switch
            {
                (int)OAuthTypeEnum.Google => new ValidateEmailResultDto { Email = userName, Type = OAuthTypeEnum.Google.ToString() },
                (int)OAuthTypeEnum.Apple => new ValidateEmailResultDto { Email = userName, Type = OAuthTypeEnum.Apple.ToString() },
                _ => new ValidateEmailResultDto { Email = userName, Type = OAuthTypeEnum.Normal.ToString() }
            };
        }
        public async Task<IResponse<TokenResultDto>> LoginByMobileAsync(LoginMobileDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {
                var validation = await new LoginByMobileDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }


                CustomerMobile mobile = null;
                CustomerEmail email = null;
                Customer customer = null;
                if (loginDto.UserName.Contains("@"))
                {
                    email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(loginDto.UserName.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified &&
                                                                (e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Registered ||
                                                                e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Suspended));


                    if (email is null)
                    {
                        response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                        return response;
                    }
                    customer = email.Customer;

                }
                else
                {
                    mobile = await _mobileRepository.GetAsync(e => e.Mobile.ToLower().Equals(loginDto.UserName.ToLower()) &&
                                                                e.IsPrimary &&
                                                                e.IsVerified &&
                                                                (e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Registered ||
                                                                e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Suspended));

                    if (mobile is null)
                    {
                        response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                        return response;
                    }

                    customer = mobile.Customer;
                    email = customer.CustomerEmails.Where(e => e.IsPrimary && e.IsVerified &&
                                                                  (e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Registered ||
                                                                   e.Customer.CustomerStatusId == (int)CustomerStatusEnum.Suspended)).FirstOrDefault();

                }






                if (!_passwordHasher.VerifyHashedPassword(loginDto.Password, customer.Password))
                {
                    response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                    return response;
                }

                var generatedJwtToken = await GenerateJwtTokenAsync(email);

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, customer.Id);

                var tokenResultDto = new TokenResultDto
                {
                    Token = generatedJwtToken.Token,
                    RefreshToken = generatedRefreshToken
                };

                response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }


        public async Task<IResponse<TokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, TokenValidationParameters tokenValidationParameters)
        {
            var response = new Response<TokenResultDto>();

            try
            {
                var validation = await new RefreshTokenDtoValidator().ValidateAsync(refreshTokenDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var verifyTokenResult = await VerifyTokenAsync(refreshTokenDto, tokenValidationParameters);

                if (!verifyTokenResult.IsSuccess)
                {
                    response.AppendErrors(verifyTokenResult.Errors)
                            .CreateResponse();

                    return response;
                }

                var storedToken = verifyTokenResult.Data;

                // generate new tokens.
                var email = await _emailRepository.GetAsync(e => e.CustomerId == storedToken.CustomerId && e.IsPrimary);

                if (email is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(email));
                    return response;
                }

                var newJwtToken = await GenerateJwtTokenAsync(email);

                var newRefreshToken = await UpdateRefreshTokenAsync(storedToken, newJwtToken.Jti);

                var tokenResultDto = new TokenResultDto
                {
                    Token = newJwtToken.Token,
                    RefreshToken = newRefreshToken
                };

                response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<bool>> LogoutAsync(LogoutDto logoutDto)
        {
            var response = new Response<bool>();

            try
            {
                var validation = await new LogoutDtoValidator().ValidateAsync(logoutDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var existRefreshToken = await _refreshTokenRepository.GetAsync(rt => rt.Token == logoutDto.RefreshToken);

                if (existRefreshToken is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(logoutDto.RefreshToken));
                    return response;
                }

                _refreshTokenRepository.Delete(existRefreshToken);
                await _unitOfWork.CommitAsync();

                response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }


        public async Task<IResponse<PreregisterResultDto>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new ForgetPasswordDtoValidator().ValidateAsync(forgetPasswordDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(forgetPasswordDto.Email.ToLower()) && e.IsVerified && e.IsPrimary);

                if (email is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(forgetPasswordDto.Email));
                    return response;
                }

                if (email.Customer.OauthTypeId != (int)OAuthTypeEnum.Normal)
                    return response.CreateResponse(MessageCodes.NotAllowed);

                return await SendEmailLinkHelperAsync(email, EmailLinkEnum.ForgetPassword);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> ForgetPasswordByMobileAsync(ForgetPasswordByMobileDto forgetPasswordDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new ForgetPasswordByMobileDtoValidator().ValidateAsync(forgetPasswordDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                CustomerMobile mobile = null;

                if (forgetPasswordDto.Username.Contains("@"))
                {
                    var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(forgetPasswordDto.Username.ToLower()));

                    if (email is null)
                        return response.CreateResponse(MessageCodes.NotFound, nameof(forgetPasswordDto.Username));

                    if (!email.IsVerified)
                        return response.CreateResponse(MessageCodes.EmailNotVerified, nameof(forgetPasswordDto.Username));

                    mobile = email.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary && m.IsVerified);
                }
                else
                    mobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(forgetPasswordDto.Username));

                if (mobile is null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(forgetPasswordDto.Username));

                if (mobile.Customer.OauthTypeId != (int)OAuthTypeEnum.Normal)
                    return response.CreateResponse(MessageCodes.NotAllowed);

                if (!mobile.IsVerified)
                    return response.CreateResponse(MessageCodes.MobileNotVerified, nameof(forgetPasswordDto.Username));

                if (mobile.Customer.CustomerStatusId != (int)CustomerStatusEnum.Registered)
                    return response.CreateResponse(MessageCodes.RegistrationProcessIncompleted, nameof(forgetPasswordDto.Username));

                return await SendMobileCodeHelperAsync(mobile, SmsTypeEnum.ForgetPassword, isWatsApp: forgetPasswordDto.IsWatsApp);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> ResendForgetPasswordEmailLinkAsync(int emailId)
        {
            var response = new Response<PreregisterResultDto>();

            var email = await _emailRepository.GetAsync(m => m.Id == emailId);

            if (email is null)
            {
                response.CreateResponse(MessageCodes.NotFound);
                return response;
            }

            if (!email.IsVerified)
            {
                response.CreateResponse(MessageCodes.EmailNotVerified);
                return response;
            }

            return await SendEmailLinkHelperAsync(email, EmailLinkEnum.ForgetPassword);
        }

        public async Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto /*, TokenValidationParameters tokenValidationParameters*/)
        {
            var response = new Response<bool>();

            var validation = await new ChangePasswordDtoValidator().ValidateAsync(changePasswordDto);

            if (!validation.IsValid)
            {
                response.CreateResponse(validation.Errors);
                return response;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(changePasswordDto.Token);
                if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                {
                    response.CreateResponse(MessageCodes.VerificationLinkExpired, nameof(changePasswordDto.Token));
                    return response;
                };
                //var claimsPrincipal = handler.ValidateToken(changePasswordDto.Token, tokenValidationParameters, out var validatedToken);

                //var exp =  claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value;


                //var linkByteArray = Convert.FromBase64String(changePasswordDto.Token);

                //var jsonToken = Encoding.UTF8.GetString(linkByteArray);

                //var verificationLinkDto = JsonConvert.DeserializeObject<VerificationLinkDto>(jsonToken);

                //if (DateTime.UtcNow > verificationLinkDto.ExpiryDate)
                //{
                //    response.CreateResponse(MessageCodes.VerificationLinkExpired, nameof(changePasswordDto.Token));
                //    return response;
                //}

                var customer = await _customerRepository.GetByIdAsync(changePasswordDto.CustomerId);

                if (customer is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(changePasswordDto.CustomerId));
                    return response;
                }

                return await ChangePasswordHelperAsync(customer, changePasswordDto.Password);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IResponse<bool>> ChangePasswordByMobileCodeAsync(ChangePasswordByMobileCodeDto changePasswordDto)
        {
            var response = new Response<bool>();

            var validation = await new ChangePasswordByMobileCodeDtoValidator().ValidateAsync(changePasswordDto);

            if (!validation.IsValid)
                return response.CreateResponse(validation.Errors);

            var mobile = await _mobileRepository.GetByIdAsync(changePasswordDto.MobileId);

            if (mobile is null)
                return response.CreateResponse(MessageCodes.NotFound, nameof(mobile));

            var existCode = mobile.CustomerSmsverifications.FirstOrDefault(v => v.VerificationCode.Equals(changePasswordDto.Code));

            if (existCode is null || !existCode.IsVerifiedFromFrogetPassword)
                return response.CreateResponse(MessageCodes.InvalidCode, nameof(changePasswordDto.Code));

            mobile.SendCount = default;
            mobile.BlockForSendingSmsuntil = null;
            mobile.ModifiedDate = DateTime.UtcNow;

            _mobileVerificationRepository.DeleteRange(mobile.CustomerSmsverifications);

            return await ChangePasswordHelperAsync(existCode.CustomerMobile.Customer, changePasswordDto.Password);
        }

        public async Task<IResponse<IEnumerable<AlternativeEmailResultDto>>> GetEmailsAsnyc(int customerId)
        {
            var response = new Response<IEnumerable<AlternativeEmailResultDto>>();

            var emails = await _emailRepository.GetManyAsync(e => e.CustomerId == customerId);

            var emailsDto = _mapper.Map<IEnumerable<AlternativeEmailResultDto>>(emails);

            return response.CreateResponse(emailsDto);
        }

        public async Task<IResponse<PreregisterResultDto>> VerifyAlternativeEmailAsync(AlternativeEmailDto alternativeEmailDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new AlternativeEmailDtoValidator().ValidateAsync(alternativeEmailDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                // validate email is not exist in verified emails.
                if (await _emailRepository.AnyAsync(e => e.Email.ToLower().Equals(alternativeEmailDto.Email.ToLower()) && e.IsVerified))
                {
                    response.CreateResponse(MessageCodes.AlreadyExists, nameof(alternativeEmailDto.Email));
                    return response;
                }

                // add email for first time, send first email verification, set count = 1
                if (alternativeEmailDto.EmailId == default)
                {
                    var createdEmail = await AddAlternativeEmailAsync(alternativeEmailDto);
                    response.CreateResponse(createdEmail);
                    return response;
                }
                else // get exist email, update email, set email unverified, resend email, increment count by 1, check if there's expire or block
                {
                    var email = await _emailRepository.GetByIdAsync(alternativeEmailDto.EmailId);

                    if (email is null)
                    {
                        response.CreateResponse(MessageCodes.NotFound, nameof(alternativeEmailDto.EmailId));
                        return response;
                    }

                    email.Email = alternativeEmailDto.Email;
                    email.IsVerified = false;

                    return await SendEmailLinkHelperAsync(email, EmailLinkEnum.AlternativeEmailLink);
                }
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }

        public async Task<IResponse<bool>> MakeEmailPrimaryAsync(int emailId)
        {
            var response = new Response<bool>();

            try
            {
                var email = await _emailRepository.GetByIdAsync(emailId);

                if (email is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(emailId));
                    return response;
                }

                if (!email.IsVerified)
                {
                    response.CreateResponse(MessageCodes.EmailNotVerified, nameof(emailId));
                    return response;
                }

                if (email.IsPrimary)
                {
                    response.CreateResponse(MessageCodes.EmailNotVerified, nameof(emailId));
                    return response;
                }

                email.IsPrimary = true;
                email.Customer.CustomerEmails.Where(e => e.Id != emailId)
                                             .ToList()
                                             .ForEach(e => e.IsPrimary = false);

                _emailRepository.Update(email);
                await _unitOfWork.CommitAsync();

                response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }


        public async Task<IResponse<IEnumerable<AlternativeMobileResultDto>>> GetMobilesAsync(int customerId)
        {
            var response = new Response<IEnumerable<AlternativeMobileResultDto>>();

            var mobiles = await _mobileRepository.GetManyAsync(m => m.CustomerId == customerId);

            var mobilesDto = _mapper.Map<IEnumerable<AlternativeMobileResultDto>>(mobiles);

            return response.CreateResponse(mobilesDto);
        }

        public async Task<IResponse<PreregisterResultDto>> VerifyAlternativeMobileAsync(AlternativeMobileDto alternativeMobileDto)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var validation = await new AlternativeMobileDtoValidator().ValidateAsync(alternativeMobileDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                //Get CountryBy CountryCode
                var mobileCountry = await _countryRepository.GetAsync(x => x.PhoneCode.Equals(alternativeMobileDto.MobileCountryCode));

                if (mobileCountry == null)
                    response.AppendError(MessageCodes.NotFound, nameof(alternativeMobileDto.MobileCountryCode));

                // validate email is not exist in verified emails.
                if (await _mobileRepository.AnyAsync(e => e.Mobile.Equals(alternativeMobileDto.Mobile) && e.IsVerified))
                    response.AppendError(MessageCodes.AlreadyExists, nameof(alternativeMobileDto.Mobile));

                if (!response.IsSuccess)
                    return response;

                // add mobile for first time, send first mobile code verification, set count = 1
                if (alternativeMobileDto.MobileId == default)
                {
                    var mobileResult = await AddAlternativeMobileAsync(alternativeMobileDto, mobileCountry.Id);

                    response.CreateResponse(mobileResult);
                    return response;
                }
                else // get exist mobile, update mobile, set mobile unverified, resend code verification, increment count by 1, check if there's expire or block
                {
                    var mobile = await _mobileRepository.GetByIdAsync(alternativeMobileDto.MobileId);

                    if (mobile is null)
                    {
                        response.CreateResponse(MessageCodes.NotFound, nameof(alternativeMobileDto.MobileId));
                        return response;
                    }

                    mobile.IsVerified = false;
                    mobile.Mobile = alternativeMobileDto.Mobile;

                    if (!alternativeMobileDto.MobileCountryCode.Equals(mobile.PhoneCode))
                    {
                        mobile.PhoneCode = alternativeMobileDto.MobileCountryCode;
                        mobile.PhoneCountryId = mobileCountry.Id;
                    }

                    return await SendMobileCodeHelperAsync(mobile);
                }
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }

        public async Task<IResponse<bool>> MakeMobilePrimaryAsync(int mobileId)
        {
            var response = new Response<bool>();

            try
            {
                var mobile = await _mobileRepository.GetByIdAsync(mobileId);

                if (mobile is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(mobileId));
                    return response;
                }

                if (!mobile.IsVerified)
                {
                    response.CreateResponse(MessageCodes.EmailNotVerified, nameof(mobileId));
                    return response;
                }

                if (mobile.IsPrimary)
                {
                    response.CreateResponse(MessageCodes.EmailNotVerified, nameof(mobileId));
                    return response;
                }

                mobile.IsPrimary = true;
                mobile.Customer.CustomerMobiles.Where(e => e.Id != mobileId)
                                               .ToList()
                                               .ForEach(e => e.IsPrimary = false);

                _mobileRepository.Update(mobile);
                await _unitOfWork.CommitAsync();

                response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }
        public async Task<IResponse<PreregisterResultDto>> UpdateMobileAsync(ChangeMobileDto newMobileDto, int customerId)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                // inputs validation.
                var validation = await new ChangeMobileDtoValidator().ValidateAsync(newMobileDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                //Get CountryBy CountryCode
                var mobileCountry = await _countryRepository.GetAsync(x => x.PhoneCode == newMobileDto.NewMobileCountryCode);

                if (mobileCountry == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(newMobileDto.NewMobileCountryCode));

                var checkExistMobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(newMobileDto.NewMobile) && m.IsVerified);

                if (checkExistMobile is not null && checkExistMobile.CustomerId != customerId)
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(newMobileDto.NewMobile));


                CustomerMobile addNewMobile = new();

                var customerMobile = await _mobileRepository.GetByIdAsync(newMobileDto.MobileId);
                var customerMobilesNotVerified = customerMobile.Customer.CustomerMobiles.Where(cm => !cm.IsPrimary && !cm.IsVerified).ToList();
                _mobileVerificationRepository.DeleteRange(customerMobilesNotVerified.SelectMany(m => m.CustomerSmsverifications));
                _mobileRepository.DeleteRange(customerMobilesNotVerified);

                if (checkExistMobile == null)
                {

                    addNewMobile = await _mobileRepository.AddAsync(new CustomerMobile()
                    {
                        CustomerId = customerMobile.CustomerId,
                        BlockForSendingSmsuntil = customerMobile.BlockForSendingSmsuntil,
                        CreateDate = DateTime.UtcNow,
                        SendCount = 0,
                        PhoneCountryId = mobileCountry.Id,
                        PhoneCode = newMobileDto.NewMobileCountryCode,
                        Mobile = newMobileDto.NewMobile,
                        IsVerified = false,
                        PhoneCountry = mobileCountry,
                        Customer = customerMobile.Customer,
                        IsPrimary = false,
                    });
                }
                else
                {
                    addNewMobile = checkExistMobile;
                }
                await _unitOfWork.CommitAsync();




                // send code, validate sendCount, blockSendingDate, etc...
                return await SendMobileCodeHelperAsync(addNewMobile, isWatsApp: newMobileDto.IsWatsApp);


            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        public async Task<IResponse<PreregisterResultDto>> VerifyUpdateMobile(VerifyUpdateMobileDto verifyMobileDto, int customerId)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {

                var checkOldMobile = _mobileRepository.GetById(verifyMobileDto.OldMobileId);
                if (checkOldMobile.CustomerId != customerId)
                    return response.CreateResponse(MessageCodes.InvalidOldMobileId, nameof(verifyMobileDto.OldMobileId));

                var result = await VerifyMobileAsync(new VerifyMobileDto()
                {
                    IsPrimaryMobile = verifyMobileDto.IsPrimaryMobile,
                    FromLandingPage = verifyMobileDto.FromLandingPage,
                    Code = verifyMobileDto.Code,
                    MobileId = verifyMobileDto.MobileId
                });

                if (result.IsSuccess)
                {
                    await MakeMobilePrimaryAsync(verifyMobileDto.MobileId);
                    _mobileVerificationRepository.Delete(e => e.CustomerMobileId == verifyMobileDto.OldMobileId);
                    _mobileRepository.Delete(e => e.Id == verifyMobileDto.OldMobileId);
                    await _unitOfWork.CommitAsync();
                }
                return result;

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }

        }

        public async Task<IResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = new Response<bool>();

            var resetValidation = await new ResetPasswordDtoValidator().ValidateAsync(resetPasswordDto);

            if (!resetValidation.IsValid)
            {
                response.CreateResponse(resetValidation.Errors);
                return response;
            }

            if (!resetValidation.IsValid)
            {
                response.CreateResponse(resetValidation.Errors);
                return response;
            }

            var customer = await _customerRepository.GetByIdAsync(resetPasswordDto.CustomerId);

            if (customer is null)
            {
                response.CreateResponse(MessageCodes.NotFound, nameof(resetPasswordDto.CustomerId));
                return response;
            }

            if (!_passwordHasher.VerifyHashedPassword(resetPasswordDto.CurrentPassword, customer.Password))
            {
                response.CreateResponse(MessageCodes.InvalidPassword);
                return response;
            }

            return await ChangePasswordHelperAsync(customer, resetPasswordDto.Password);
        }


        public async Task<string> CreateCrmLeadAsync(int customerId)
        {
            try
            {
                CustomerEmail email = _emailRepository.Where(x => x.CustomerId == customerId).FirstOrDefault(m => m.IsPrimary && m.IsVerified);

                if (email == null)
                    return null;

                return await CreateCrmLeadHelperAsync(email);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> CreateCrmLeadAsync(CustomerEmail email) =>
            await CreateCrmLeadHelperAsync(email, commitChanges: false);


        public async Task<IResponse<bool>> ValidateEmailAndMobileAsync(PreregisterLandingPageDto landingPageDto)
        {
            return await ValidateEmailAndMobileHelperAsync(landingPageDto);
        }




        #region Helpers.
        private async Task SendVerificationCodeSmsAsync(CustomerMobile mobile, string code)
        {
            var smsResult = new OperationResult<List<SmsResult>>();

            var countryId = mobile.PhoneCountryId.HasValue ? mobile.PhoneCountryId.Value : mobile.Customer.CountryId;

            switch ((CountryEnum)countryId)
            {
                case CountryEnum.Egypt:
                    {
                        smsResult = await _notificationBLL.SendSmsAsync(SmsProvider.VictoryLink,
                                        new SmsContent
                                        {
                                            From = _authSetting.Sms.SmsTemplate.VictoryLinkTemplate.FromPhone,
                                            ToPhones = new List<string> { mobile.Mobile },
                                            LanguageId = (int)VictoryLinkLanguageEnum.E,
                                            MessageContent = _authSetting.Sms.SmsTemplate.VictoryLinkTemplate.Content + code
                                        });

                        var result = smsResult.Response.Select(x => x.Status);

                        if (smsResult.Errors.Any())
                            goto default;
                    }
                    break;
                //case CountryEnum.SaudiArabia:
                //    {
                //        smsResult = await _notificationBLL.SendSmsAsync(SmsProvider.Taqnyat,
                //                        new SmsContent
                //                        {
                //                            From = _authSetting.Sms.SmsTemplate.TaqnyatTemplate.FromPhone,
                //                            ToPhones = new List<string> { mobile.Mobile },
                //                            MessageContent = _authSetting.Sms.SmsTemplate.TaqnyatTemplate.Content + code
                //                        });

                //        if (smsResult.Errors.Any())
                //            goto default;
                //    }
                //    break;
                default:
                    {
                        smsResult = await _notificationBLL.SendSmsAsync(SmsProvider.Twilio,
                                        new SmsContent
                                        {
                                            From = _authSetting.Sms.SmsTemplate.TwilioTemplate.FromPhone,
                                            ToPhones = new List<string> { mobile.Mobile },
                                            MessageContent = _authSetting.Sms.SmsTemplate.TwilioTemplate.Content + code
                                        });
                    }
                    break;
            }
        }

        private async Task<string> GenerateMobileCodeAsync(int? customerId = null)
        {
            var code = GenerateRandom(_authSetting.VerificationCode.Length);

            if (customerId.HasValue)
            {
                while (await _mobileVerificationRepository.AnyAsync(m => m.CustomerMobile.CustomerId == customerId.Value &&
                                                                         m.VerificationCode.Equals(code.ToString())))
                {
                    code = GenerateRandom(_authSetting.VerificationCode.Length);
                }
            }

            return code;
        }

        private async Task<string> GenerateEmailCodeAsync(int? customerId = null)
        {
            var code = GenerateRandom(_authSetting.VerificationCode.Length);

            if (customerId.HasValue)
            {
                while (await _emailVerificationRepository.AnyAsync(m => m.CustomerEmail.CustomerId == customerId.Value &&
                                                                        m.VerificationCode.Equals(code.ToString())))
                {
                    code = GenerateRandom(_authSetting.VerificationCode.Length);
                }
            }

            return code;
        }

        private async Task<OperationResult<MailResult>> SendEmailAsync(VerificationLinkDto verificationLinkDto, EmailLinkEnum emailLinkEnum)
        {
            return emailLinkEnum switch
            {
                EmailLinkEnum.ActivationLink or EmailLinkEnum.AlternativeEmailLink
                                             => await SendVerificationLinkEmailAsync(verificationLinkDto.Email,
                                                      await BuildEmailTemlateAsync(verificationLinkDto, emailLinkEnum)),
                EmailLinkEnum.ForgetPassword => await SendForgetPasswordLinkEmailAsync(verificationLinkDto.Email,
                                                      await BuildEmailTemlateAsync(verificationLinkDto, emailLinkEnum)),
                _ => null
            };
        }

        private async Task<OperationResult<MailResult>> SendVerificationLinkEmailAsync(string email, string template)
        {
            return await _notificationBLL.SendEmailAsync(EmailProvider.Smtp,
                                                         new MailContent
                                                         {
                                                             FromEmail = _authSetting.Email.VerifyEmail.FromEmail,
                                                             ToEmails = new List<string> { email },
                                                             Subject = _authSetting.Email.VerifyEmail.Subject,
                                                             DisplayName = _authSetting.Email.VerifyEmail.DisplayName,
                                                             Body = template
                                                         });
        }

        private async Task<OperationResult<MailResult>> SendForgetPasswordLinkEmailAsync(string email, string template)
        {
            return await _notificationBLL.SendEmailAsync(EmailProvider.Smtp,
                                                         new MailContent
                                                         {
                                                             FromEmail = _authSetting.Email.ForgetPassword.FromEmail,
                                                             ToEmails = new List<string> { email },
                                                             Subject = _authSetting.Email.ForgetPassword.Subject,
                                                             DisplayName = _authSetting.Email.ForgetPassword.DisplayName,
                                                             Body = template
                                                         });
        }

        private async Task<string> BuildEmailTemlateAsync(VerificationLinkDto verificationLinkDto, EmailLinkEnum emailLinkEnum)
        {
            return emailLinkEnum switch
            {
                EmailLinkEnum.ActivationLink or EmailLinkEnum.AlternativeEmailLink
                                             => await BuildVerifyEmailTemplateAsync(GenerateEmailLink(verificationLinkDto, emailLinkEnum), verificationLinkDto.Name),
                EmailLinkEnum.ForgetPassword => await BuildForgetPasswordEmailTemplateAsync(GenerateEmailLink(verificationLinkDto, emailLinkEnum), verificationLinkDto.Email, verificationLinkDto.Name),
                _ => null
            };
        }

        private async Task<string> BuildVerifyEmailTemplateAsync(string link, string name)
        {
            var assemblyFolder = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var file = await IO.File.ReadAllTextAsync(IO.Path.Combine(assemblyFolder,
                                                                      _emailTemplateSetting.BaseFilesPath,
                                                                      _emailTemplateSetting.Folder.Activation.Name,
                                                                      _emailTemplateSetting.Folder.Activation.HtmlFile));

            var replacedFile = file.Replace("%customerName%", name)
                                   .Replace("%verifyLink%", link)
                                   .Replace("%contactUsLink%",
                                        IO.Path.Combine(_emailTemplateSetting.CLientBaseUrl,
                                                        _emailTemplateSetting.ClientMethod.ContactUs))
                                   .Replace("%dexefLogoImg%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Image.Name,
                                                        _emailTemplateSetting.Folder.Image.Logo))
                                   .Replace("%tajawalBoldFont%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Font.Name,
                                                        _emailTemplateSetting.Folder.Font.TajawalBold))
                                   .Replace("%tajawalRegularFont%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Font.Name,
                                                        _emailTemplateSetting.Folder.Font.TajawalRegular));

            return replacedFile;
        }

        private async Task<string> BuildForgetPasswordEmailTemplateAsync(string link, string email, string name)
        {
            var assemblyFolder = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var file = await IO.File.ReadAllTextAsync(IO.Path.Combine(assemblyFolder,
                                                                      _emailTemplateSetting.BaseFilesPath,
                                                                      _emailTemplateSetting.Folder.ChangePassword.Name,
                                                                      _emailTemplateSetting.Folder.ChangePassword.HtmlFile));

            var replacedFile = file.Replace("%customerName%", name)
                                   .Replace("%customerEmail%", email)
                                   .Replace("%verifyLink%", link)
                                   .Replace("%contactUsLink%",
                                        $"{_emailTemplateSetting.CLientBaseUrl}/{_emailTemplateSetting.ClientMethod.ContactUs}")
                                   .Replace("%dexefLogoImg%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Image.Name,
                                                        _emailTemplateSetting.Folder.Image.Logo))
                                   .Replace("%tajawalBoldFont%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Font.Name,
                                                        _emailTemplateSetting.Folder.Font.TajawalBold))
                                   .Replace("%tajawalRegularFont%",
                                        IO.Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                        _emailTemplateSetting.BaseFilesPath,
                                                        _emailTemplateSetting.Folder.Font.Name,
                                                        _emailTemplateSetting.Folder.Font.TajawalRegular));

            return replacedFile;
        }

        private string GenerateEmailLink(VerificationLinkDto verificationLinkDto, EmailLinkEnum emailLinkEnum)
        {
            var jsonLink = JsonConvert.SerializeObject(verificationLinkDto);

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonLink));

            return emailLinkEnum switch
            {
                EmailLinkEnum.ActivationLink => $"{_authSetting.Email.ClientBaseUrl}/{_authSetting.Email.VerifyEmail.ClientMethod}/{token}",
                EmailLinkEnum.ForgetPassword => $"{_authSetting.Email.ClientBaseUrl}/{_authSetting.Email.ForgetPassword.ClientMethod}/{token}",
                EmailLinkEnum.AlternativeEmailLink => $"{_authSetting.Email.ClientBaseUrl}/{_authSetting.Email.VerifyAlternativeEmail.ClientMethod}/{token}",
                _ => null,
            };
        }

        private string GenerateRandom(int length)
        {
            var random = new Random();

            return new string(Enumerable.Repeat(_authSetting.VerificationCode.Chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        private IResponse<PreregisterResultDto> ValidateChangeMobile(ChangeMobileDto newMobileDto, CustomerMobile oldMobile)
        {
            var response = new Response<PreregisterResultDto>();

            if (oldMobile is null)
            {
                response.CreateResponse(MessageCodes.NotFound, nameof(newMobileDto.MobileId));
                return response;
            }

            if (oldMobile.IsVerified)
            {
                response.CreateResponse(MessageCodes.MobileAlreadyVerified, nameof(newMobileDto.MobileId));
                return response;
            }

            if (oldMobile.Mobile.Equals(newMobileDto.NewMobile))
            {
                response.CreateResponse(MessageCodes.AlreadyExists, nameof(newMobileDto.NewMobile));
                return response;
            }

            return response;
        }

        private async Task<IResponse<bool>> ChangePasswordHelperAsync(Customer customer, string password)
        {
            var response = new Response<bool>();

            try
            {
                if (customer.Password is not null)
                {
                    if (_passwordHasher.VerifyHashedPassword(password, customer.Password))
                    {
                        response.CreateResponse(MessageCodes.NewPasswordAlreadyDefined, nameof(password));
                        return response;
                    }
                }


                customer.Password = _passwordHasher.HashPassword(password);
                customer.ModifiedDate = DateTime.UtcNow;
                customer.LastPasswordUpdate = DateTime.UtcNow;
                _customerRepository.Update(customer);
                await _unitOfWork.CommitAsync();

                response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.Failed);
            }

            return response;
        }


        private async Task<IResponse<PreregisterResultDto>> SendEmailLinkHelperAsync(CustomerEmail email, EmailLinkEnum emailLinkEnum)
        {
            var response = new Response<PreregisterResultDto>();

            try
            {
                var registerResult = new PreregisterResultDto
                {
                    Id = email.CustomerId,
                    EmailId = email.Id,
                    Status = (CustomerStatusEnum)email.Customer.CustomerStatusId
                };

                if (email.BlockForSendingEmailUntil.HasValue)
                {
                    // return block till date.
                    if (email.BlockForSendingEmailUntil.Value > DateTime.UtcNow)
                    {
                        registerResult.BlockTillDate = DateTime.SpecifyKind(email.BlockForSendingEmailUntil.Value, DateTimeKind.Utc);

                        response.CreateResponse(registerResult);
                        return response;
                    }
                    else
                    {
                        // reset sendCount & block sending till date.
                        email.SendCount = default;
                        email.BlockForSendingEmailUntil = null;
                    }
                }

                // return old expiry date.
                var codeNotExpiredYetExist = email.CustomerEmailVerifications.FirstOrDefault(v => v.ExpireDate > DateTime.UtcNow);
                if (codeNotExpiredYetExist is not null)
                {
                    registerResult.ExpiryDate = DateTime.SpecifyKind(codeNotExpiredYetExist.ExpireDate, DateTimeKind.Utc);

                    response.CreateResponse(registerResult);
                    return response;
                }

                // if sendCount < limit by 1, add block datetime.
                var sendLimit = _authSetting.Email.SendCountLimit;
                if (email.SendCount == --sendLimit)
                    email.BlockForSendingEmailUntil = DateTime.UtcNow.Add(_authSetting.Email.BlockSendingTime)
                                                                     .Add(_authSetting.Email.CodeExpiryTime);

                email.SendCount++;
                email.ModifiedDate = DateTime.UtcNow;

                var code = await GenerateEmailCodeAsync(email.CustomerId);

                var emailVerification = new CustomerEmailVerification
                {
                    VerificationCode = code,
                    CreateDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.Add(_authSetting.Email.CodeExpiryTime)
                };

                email.CustomerEmailVerifications.Add(emailVerification);

                _emailRepository.Update(email);

                await _unitOfWork.CommitAsync();

                // send email.
                var emailResult = await SendEmailAsync(new VerificationLinkDto { EmailId = email.Id, Email = email.Email, Code = code, ExpiryDate = emailVerification.ExpireDate, Name = email.Customer.Name }, emailLinkEnum);

                registerResult.ExpiryDate = emailVerification.ExpireDate;
                registerResult.BlockTillDate = email.BlockForSendingEmailUntil.HasValue ? DateTime.SpecifyKind(email.BlockForSendingEmailUntil.Value, DateTimeKind.Utc) : null;

                response.CreateResponse(registerResult);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
            }

            return response;
        }

        private async Task<PreregisterResultDto> AddAlternativeEmailAsync(AlternativeEmailDto alternativeEmailDto)
        {
            var code = await GenerateEmailCodeAsync(alternativeEmailDto.CustomerId);

            var email = _mapper.Map<CustomerEmail>(alternativeEmailDto);

            var emailVerification = new CustomerEmailVerification
            {
                CustomerEmail = email,
                VerificationCode = code,
                CreateDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.Add(_authSetting.Email.CodeExpiryTime)
            };

            email.CustomerEmailVerifications.Add(emailVerification);

            await _emailRepository.AddAsync(email);
            await _unitOfWork.CommitAsync();

            _emailRepository.RefreshEntity(email, e => e.Customer);

            // send email.
            await SendEmailAsync(new VerificationLinkDto { EmailId = email.Id, Email = email.Email, Code = code, ExpiryDate = emailVerification.ExpireDate, Name = email.Customer.Name }, EmailLinkEnum.AlternativeEmailLink);

            // map retrieve dto.
            return new PreregisterResultDto
            {
                Id = email.CustomerId,
                EmailId = email.Id,
                ExpiryDate = emailVerification.ExpireDate
            };
        }

        private async Task<PreregisterResultDto> AddAlternativeMobileAsync(AlternativeMobileDto alternativeMobileDto, int mobileCountryId)
        {
            // generate sms verification code.
            var verificationCode = await GenerateMobileCodeAsync();

            var mobileVerification = new CustomerSmsverification
            {
                VerificationCode = verificationCode,
                CreateDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.Add(_authSetting.Sms.CodeExpiryTime)
            };

            var mobile = _mapper.Map<CustomerMobile>(alternativeMobileDto);

            mobile.PhoneCountryId = mobileCountryId;

            mobile.CustomerSmsverifications.Add(mobileVerification);

            await _mobileRepository.AddAsync(mobile);
            await _unitOfWork.CommitAsync();

            await _mobileRepository.RefreshEntityReferencesAsync(mobile);

            // send sms.
            await SendVerificationCodeSmsAsync(mobile, verificationCode);

            return new PreregisterResultDto
            {
                Id = mobile.CustomerId,
                MobileId = mobile.Id,
                ExpiryDate = mobileVerification.ExpireDate
            };
        }

        private async Task<string> CreateCrmLeadHelperAsync(CustomerEmail email, bool commitChanges = true)
        {
            if (email == null)
                return string.Empty;

            //var crmLeadId = string.Empty;

            // Create customer in CRM as lead.
            var crmLeadId = await _crmBLL.CreateLeadAsync(new CrmNewLeadDto
            {
                CustomerId = email.Customer.Id,
                Email = email.Email,
                Mobile = email.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary && m.IsVerified)?.Mobile,
                Name = email.Customer.Name,
                CompanyName = email.Customer?.CompanyName,
                CompanySize = email.Customer?.CompanySize?.Crmid ?? (int)CompanySizeCrmEnum.OneToFive,
                CreatedDate = email.Customer.CreateDate,
                CountryId = email.Customer?.Country?.Crmid,
                CurrencyId = email.Customer?.Country?.CountryCurrency?.Currency?.Crmid,
                TaxRegistrationNumber = int.TryParse(email?.Customer?.TaxRegistrationNumber, out int taxReg) ? taxReg : 0,
                Website = email.Customer?.CompanyWebsite,
                IndustryCode = email.Customer?.Industry?.Crmid ?? (int)IndustryCodeCrmEnum.Accounting,
                SoftwareId = _authSetting.Crm.SoftwareId,
                FollowType = _authSetting.Crm.FollowType,
                LeadSourceCode = _authSetting.Crm.LeadSourceCode,
            });

            email.Customer.CrmleadId = !string.IsNullOrEmpty(crmLeadId) ? crmLeadId : null;

            //_customerRepository.Update(email.Customer);

            if (commitChanges)
                _unitOfWork.Commit();

            return crmLeadId;
        }

        public async Task CreateCrmLeadHelperAsync(CrmNewLeadDto newLeadDto)
        {
            // Create customer in CRM as lead.
            var crmLeadId = await _crmBLL.CreateLeadAsync(newLeadDto);

            if (!string.IsNullOrEmpty(crmLeadId))
            {
                var customer = await _customerRepository.GetByIdAsync(newLeadDto.CustomerId);

                customer.CrmleadId = crmLeadId;

                await _unitOfWork.CommitAsync();
            }
        }

        private async Task<IResponse<bool>> ValidateEmailAndMobileHelperAsync(PreregisterLandingPageDto landingPageDto)
        {
            var response = new Response<bool>();

            var existEmail = await GetEmailOrMobileHelperAsync(landingPageDto.Mobile, isMobile: true);
            var existMobile = await GetEmailOrMobileHelperAsync(landingPageDto.Email, isMobile: false);

            if (existEmail is not null && existEmail.Item2 && existMobile is null)
                return response.CreateResponse(MessageCodes.AlreadyExists, nameof(landingPageDto.Mobile));


            // must check mobile on email isVerified but if do this will throw exception because email unique constraint
            if (existMobile is not null && existEmail is null)
                return response.CreateResponse(MessageCodes.AlreadyExists, nameof(landingPageDto.Email));

            if (existEmail is not null && existMobile is not null)
            {
                //if (existMobile.Item1 != landingPageDto.Mobile && existEmail.Item2)
                //    return response.CreateResponse(MessageCodes.MobileBelongsToOtherEmail, nameof(landingPageDto.Mobile));

                if (existEmail.Item1 != landingPageDto.Email.ToLower())
                    return response.CreateResponse(MessageCodes.EmailBelongsToOtherMobile, nameof(landingPageDto.Email));

            }

            return response.CreateResponse(true);
        }

        private async Task<Tuple<string, bool>> GetEmailOrMobileHelperAsync(string input, bool isMobile = true)
        {
            if (isMobile)
            {
                var mobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(input));

                if (mobile is null)
                    return null;

                var email = mobile.Customer.CustomerEmails.FirstOrDefault(e => e.IsPrimary);

                return Tuple.Create(email.Email.ToLower(), mobile.IsVerified);
            }
            else
            {
                var email = await _emailRepository.GetAsync(m => m.Email.ToLower().Equals(input.ToLower()));

                if (email is null)
                    return null;

                var mobile = email.Customer.CustomerMobiles.FirstOrDefault(e => e.IsPrimary);

                return Tuple.Create(mobile.Mobile, email.IsVerified);
            }
        }



        #region refacator
        //private async Task<IResponse<bool>> ValidateEmailAndMobileHelperAsync(PreregisterLandingPageDto landingPageDto)
        //{
        //    var response = new Response<bool>();

        //    var existingMobileInfo = await GetContactInfoAsync(landingPageDto.Mobile, isMobile: true);
        //    var existingEmailInfo = await GetContactInfoAsync(landingPageDto.Email, isMobile: false);

        //    if (IsMobileExistsButEmailNotExists(existingMobileInfo, existingEmailInfo))
        //        return response.CreateResponse(MessageCodes.AlreadyExists, nameof(landingPageDto.Email));

        //    if (IsEmailExistsButMobileNotExists(existingEmailInfo, existingMobileInfo))
        //        return response.CreateResponse(MessageCodes.AlreadyExists, nameof(landingPageDto.Mobile));

        //    if (BothMobileAndEmailExists(existingMobileInfo, existingEmailInfo))
        //    {
        //        if (existingEmailInfo.UserName != landingPageDto.Email.ToLower())
        //            return response.CreateResponse(MessageCodes.EmailBelongsToOtherMobile, nameof(landingPageDto.Email));
        //    }

        //    return response.CreateResponse(true);
        //}

        //private async Task<ContactInfoDto> GetContactInfoAsync(string input, bool isMobile)
        //{
        //    if (isMobile)
        //    {
        //        var mobile = await _mobileRepository.GetAsync(m => m.Mobile.Equals(input));
        //        return mobile == null ? null : new ContactInfoDto(mobile.IsVerified, mobile.Customer.CustomerEmails.FirstOrDefault(e => e.IsPrimary)?.Email.ToLower());
        //    }
        //    else
        //    {
        //        var email = await _emailRepository.GetAsync(e => e.Email.ToLower().Equals(input.ToLower()));
        //        return email == null ? null : new ContactInfoDto(email.IsVerified, email.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary)?.Mobile);
        //    }
        //}

        //private bool IsMobileExistsButEmailNotExists(ContactInfoDto mobileInfo, ContactInfoDto emailInfo) => mobileInfo != null && emailInfo == null;

        //private bool IsEmailExistsButMobileNotExists(ContactInfoDto emailInfo, ContactInfoDto mobileInfo) => emailInfo != null && mobileInfo == null;

        //private bool BothMobileAndEmailExists(ContactInfoDto mobileInfo, ContactInfoDto emailInfo) => mobileInfo != null && emailInfo != null;
        #endregion





        #endregion

        #region Jwt Token Helpers.
        private async Task<JwtTokenDto> GenerateJwtTokenAsync(CustomerEmail email)
        {
            return await Task.Run(() =>
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_authSetting.Jwt.Secret);



                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _authSetting.Jwt.Issuer,
                    Subject = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(TokenClaimTypeEnum.Id.ToString(), email.CustomerId.ToString()),
                        new Claim(TokenClaimTypeEnum.DexefCountryId.ToString(),_workSpaceBLL.GetCustomerCountryCurrencyId(email.CustomerId).ToString()??"0"),
                        new Claim(JwtRegisteredClaimNames.Email, email.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, email.Email),
                        new Claim(TokenClaimTypeEnum.CountryId.ToString(), email.Customer.CountryId.ToString()),
                        new Claim(TokenClaimTypeEnum.Currency.ToString(), email.Customer.Country?.CountryCurrency?.Currency?.Code??GetDefaultCurrencyCode()),
                        new Claim(TokenClaimTypeEnum.StatusId.ToString(), email.Customer.CustomerStatusId.ToString()),
                        new Claim(TokenClaimTypeEnum.Name.ToString(), email.Customer.Name),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    }),
                    Expires = DateTime.UtcNow.Add(_authSetting.Jwt.TokenExpiryTime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var mobile = email.Customer.CustomerMobiles.FirstOrDefault(m => m.IsPrimary && m.IsVerified);

                if (mobile is not null)
                    tokenDescriptor.Subject.AddClaim(new Claim(TokenClaimTypeEnum.Mobile.ToString(), mobile.Mobile));

                if (!string.IsNullOrEmpty(email.Customer.CustomerCrmid))
                    tokenDescriptor.Subject.AddClaim(new Claim(TokenClaimTypeEnum.CrmId.ToString(), email.Customer.CustomerCrmid));

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);

                var jwtToken = jwtTokenHandler.WriteToken(token);

                return new JwtTokenDto
                {
                    Jti = token.Id,
                    Token = jwtToken,
                };
            });
        }

        private string GetDefaultCurrencyCode()
        {
            return _countryCurrencyRepository.Where(c => c.DefaultForOther).FirstOrDefault().Currency.Code;
        }
        private async Task<string> GenerateRefreshTokenAsync(string jti, int customerId)
        {
            var refreshToken = new RefreshToken
            {
                Jti = jti,
                CustomerId = customerId,
                ExpireDate = DateTime.UtcNow.AddMonths(_authSetting.Jwt.RefreshToken.RefreshTokenExpiryInMonths),
                CreateDate = DateTime.UtcNow,
                Token = $"{GenerateRandom(_authSetting.Jwt.RefreshToken.TokenLength)}{Guid.NewGuid()}"
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return refreshToken.Token;
        }

        private async Task<string> UpdateRefreshTokenAsync(RefreshToken refreshToken, string jti)
        {
            refreshToken.Jti = jti;
            refreshToken.Token = $"{GenerateRandom(_authSetting.Jwt.RefreshToken.TokenLength)}{Guid.NewGuid()}";
            refreshToken.ModifiedDate = DateTime.UtcNow;

            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.CommitAsync();

            return refreshToken.Token;
        }

        private async Task<IResponse<RefreshToken>> VerifyTokenAsync(RefreshTokenDto refreshTokenDto, TokenValidationParameters tokenValidationParameters)
        {
            var response = new Response<RefreshToken>();

            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                // prevent validate token lifetime.
                tokenValidationParameters.ValidateLifetime = false;

                // 01- Validate token is a propper jwt token formatting.
                var claimsPrincipal = jwtTokenHandler.ValidateToken(refreshTokenDto.Token, tokenValidationParameters, out var validatedToken);

                // 02- Validate token has been encrypted using the encryption that we've specified. 
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var isSameEncryption = jwtSecurityToken.Header
                                                           .Alg
                                                           .Equals(SecurityAlgorithms.HmacSha256,
                                                                   StringComparison.InvariantCultureIgnoreCase);

                    if (!isSameEncryption)
                    {
                        response.CreateResponse(MessageCodes.InvalidToken);

                        // reset lifetime to valdiate it.
                        tokenValidationParameters.ValidateLifetime = true;
                        return response;
                    }
                }

                // Todo-Sully: check validated token is not jwtSecurityToken.

                // 03- Validate token expiry date.
                var utcLongExpiryDate = long.Parse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcLongExpiryDate);

                // Comment validate token expiry time.
                //if (expiryDate > DateTime.UtcNow)
                //{
                //    response.CreateResponse(MessageCodes.TokenNotExpiredYet);

                //    // reset lifetime to valdiate it.
                //    tokenValidationParameters.ValidateLifetime = true;
                //    return response;
                //}

                // 04- Validate actual token stored in database.
                var storedToken = await _refreshTokenRepository.GetAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

                if (storedToken is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(refreshTokenDto.RefreshToken));

                    // reset lifetime to valdiate it.
                    tokenValidationParameters.ValidateLifetime = true;
                    return response;
                }

                // 07- Validate jwt token Jti matches refresh token jti in database.
                var jti = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                if (!storedToken.Jti.Equals(jti))
                {
                    response.CreateResponse(MessageCodes.TokensDoNotMatch);

                    // reset lifetime to valdiate it.
                    tokenValidationParameters.ValidateLifetime = true;
                    return response;
                }

                response.CreateResponse(storedToken);
            }
            catch (Exception ex)
            {
                response.CreateResponse(MessageCodes.InvalidToken);
            }

            // reset lifetime to valdiate it.
            tokenValidationParameters.ValidateLifetime = true;
            return response;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // utc time is an integer (long) number of seconds from the 1970/1/1 till now. 
            var datetimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return datetimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
        }
        #endregion
    }



    public class AppleUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool VerifiedEmail { get; set; }
    }
}
