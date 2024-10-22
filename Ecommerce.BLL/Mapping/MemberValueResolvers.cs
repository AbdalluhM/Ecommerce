using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Addons;
using Ecommerce.BLL.Applications;
using Ecommerce.BLL.Applications.Modules;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Customers.CustomerProduct;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Customers.Reviews.Customers;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Taxes;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Settings.Enviroment;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Taxes;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Mapping
{
    public static class ContextAccessorExtensions
    {

        public static int GetUserCountryId(this IHttpContextAccessor accessor)
        {
            //get CountryId from Authorized User
            if (int.TryParse(accessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == TokenClaimTypeEnum.CountryId.ToString())?.Value, out int countryId))
            {
                return countryId;
            }
            else if (accessor?.HttpContext?.Items.Any(x => x.Key.ToString().Equals("countryId")) ?? false)
                return (int)accessor?.HttpContext?.Items["countryId"];
            else return 0;
        }
        public static GetCurrencyOutputDto GetDefaultCurrency(this IHttpContextAccessor accessor)
        {
            return (GetCurrencyOutputDto)accessor.HttpContext.Items["DefaultCurrency"] ?? new GetCurrencyOutputDto();
        }
        public static void SetDefaultCurrency(this IHttpContextAccessor accessor, GetCurrencyOutputDto defaultCurrency)
        {
            accessor.HttpContext.Items["DefaultCurrency"] = defaultCurrency;
        }
        public static string GetDefaultCurrencyCode(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Items["DefaultCurrencyCode"]?.ToString() ?? string.Empty;
        }
        public static void SetDefaultCurrencyCode(this IHttpContextAccessor accessor, string defaultCurrency)
        {
            accessor.HttpContext.Items["DefaultCurrencyCode"] = defaultCurrency;
        }
        public static int GetUserId(this IHttpContextAccessor accessor)
        {
            //get employeeId from Authorized User
            if (int.TryParse(accessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == TokenClaimTypeEnum.Id.ToString())?.Value, out int employeeId))
            {
                return employeeId;
            }

            return 0;
        }
    }
    public class AddOnMissingPricesCountResolver : IMemberValueResolver<object, object, int, int>
    {
        private readonly IAddOnBLL _addOnBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmployeeBLL _employeeBLL;

        public AddOnMissingPricesCountResolver(IAddOnBLL addOnBLL, IHttpContextAccessor httpContextAccessor, IEmployeeBLL employeeBLL)
        {
            _addOnBLL = addOnBLL;
            _httpContextAccessor = httpContextAccessor;
            _employeeBLL = employeeBLL;
        }

        public int Resolve(object source, object destination, int sourceMember, int destMember, ResolutionContext context)
        {
            var employeeId = _httpContextAccessor.GetUserId();

            var countryCurrenciesIds = _employeeBLL.GetEmployeeCountryCurrencies(employeeId);

            return _addOnBLL != null ? _addOnBLL.GetMissingPriceCount(sourceMember, countryCurrenciesIds) : 0;
        }
    }

    public class ApplicationMissionPricesCountResolver : IMemberValueResolver<object, object, int, int>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmployeeBLL _employeeBLL;

        public ApplicationMissionPricesCountResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor, IEmployeeBLL employeeBLL)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
            _employeeBLL = employeeBLL;
        }

        public int Resolve(object source, object destination, int sourceMember, int destMember, ResolutionContext context)
        {
            var employeeId = _httpContextAccessor.GetUserId();

            var countryCurrenciesIds = _employeeBLL.GetEmployeeCountryCurrencies(employeeId);

            return _versionBLL != null ? _versionBLL.GetMissingPriceCount(sourceMember, countryCurrenciesIds) : 0;
        }
    }

    public class ApplicationModulesCountResolver : IMemberValueResolver<object, object, int, int>
    {
        private readonly IVersionModuleBLL _versionModuleBLL;

        public ApplicationModulesCountResolver(IVersionModuleBLL versionModuleBLL)
        {
            _versionModuleBLL = versionModuleBLL;
        }

        public int Resolve(object source, object destination, int sourceMember, int destMember, ResolutionContext context)
        {

            return _versionModuleBLL != null ? _versionModuleBLL.GetVersionModulesCount(sourceMember) : 0;

        }
    }

    public class ApplicationVersionsCountResolver : IMemberValueResolver<object, object, int, int>
    {
        private readonly IVersionBLL _versionBLL;

        public ApplicationVersionsCountResolver(IVersionBLL versionBLL)
        {
            _versionBLL = versionBLL;
        }

        public int Resolve(object source, object destination, int sourceMember, int destMember, ResolutionContext context)
        {

            return _versionBLL != null ? _versionBLL.GetApplicationVersionsCount(sourceMember) : 0;
        }
    }
    public class ChatMessageMediaResolver : IMemberValueResolver<object, object, InputFileDto, List<MediaDto>>
    {
        private readonly IFileBLL _fileBLL;

        public ChatMessageMediaResolver(IFileBLL fileBLL)
        {
            _fileBLL = fileBLL;
        }



        public List<MediaDto> Resolve(object source, object destination, InputFileDto sourceMember, List<MediaDto> destMember, ResolutionContext context)
        {
            return _fileBLL.GetAllFiles(sourceMember);
        }
    }

    public class ChatMessageNameResolver : IMemberValueResolver<object, object, bool, string>
    {
        private IRepository<Employee> _employeeRepository;
        private IRepository<Customer> _customerRepository;

        public ChatMessageNameResolver(IRepository<Employee> employeeRepository, IRepository<Customer> customerRepository)
        {
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
        }



        public string Resolve(object source, object destination, bool sourceMember, string destMember, ResolutionContext context)
        {
            var chat = (ChatMessage)source;

            return sourceMember ? _customerRepository.GetById(chat.SenderId)?.Name : "Technical Support"/*_employeeRepository.GetById(chat.SenderId).Name*/;
        }
    }

    public class ApplicationMinimumPriceResolver : IMemberValueResolver<object, object, int, VersionPriceDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationMinimumPriceResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceDetailsDto Resolve(object source, object destination, int sourceMember, VersionPriceDetailsDto destMember, ResolutionContext context)
        {
            return _versionBLL.GetApplicationMinimumVersionPrice(sourceMember, _httpContextAccessor.GetUserCountryId());
        }
    }
    public class ApplicationMinimumPricesResolver : IMemberValueResolver<object, object, int, VersionPriceAllDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationMinimumPricesResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceAllDetailsDto Resolve(object source, object destination, int sourceMember, VersionPriceAllDetailsDto destMember, ResolutionContext context)
        {
            return _versionBLL.GetApplicationMinimumVersionPrices(sourceMember, _httpContextAccessor.GetUserCountryId());
        }
    }

    public class VersionMinimumPriceResolver : IMemberValueResolver<object, object, int, VersionPriceDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VersionMinimumPriceResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceDetailsDto Resolve(object source, object destination, int sourceMember, VersionPriceDetailsDto destMember, ResolutionContext context)
        {
            return _versionBLL.GetMinimumVersionPrice(sourceMember, _httpContextAccessor.GetUserCountryId());

        }
    }

    public class VersionPriceResolver : IMemberValueResolver<object, object, VersionPriceDto, VersionPriceDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VersionPriceResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceDetailsDto Resolve(object source, object destination, VersionPriceDto sourceMember, VersionPriceDetailsDto destMember, ResolutionContext context)
        {
            //TODO:Must put CountryId from Authorized User
            //int.TryParse(_httpContextAccessor.HttpContext.Items ["CountryId"]?.ToString() ?? string.Empty, out int countryId);
            if (sourceMember != null)
                return _versionBLL.GetMinimumVersionPrice(sourceMember.VersionId, _httpContextAccessor.GetUserCountryId(), sourceMember.PriceLevelId);
            else
                return new VersionPriceDetailsDto();

        }
    }
    public class ApplicationPriceVersionMinimumPriceResolver : IMemberValueResolver<object, object, int, VersionPriceAllDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationPriceVersionMinimumPriceResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceAllDetailsDto Resolve(object source, object destination, int sourceMember, VersionPriceAllDetailsDto destMember, ResolutionContext context)
        {
            ////TODO:Must put CountryId from Authorized User
            //int.TryParse(_httpContextAccessor.HttpContext.Items ["CountryId"]?.ToString() ?? string.Empty, out int countryId);
            return _versionBLL.GetMinimumVersionAllPrices(sourceMember, _httpContextAccessor.GetUserCountryId());

        }
    }
    public class ApplicationVersionPriceResolver : IMemberValueResolver<object, object, VersionPriceDto, VersionPriceAllDetailsDto>
    {
        private readonly IVersionBLL _versionBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationVersionPriceResolver(IVersionBLL versionBLL, IHttpContextAccessor httpContextAccessor)
        {
            _versionBLL = versionBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceAllDetailsDto Resolve(object source, object destination, VersionPriceDto sourceMember, VersionPriceAllDetailsDto destMember, ResolutionContext context)
        {

            return _versionBLL.GetMinimumVersionAllPrices(sourceMember.VersionId, _httpContextAccessor.GetUserCountryId(), sourceMember.PriceLevelId);

        }
    }
    public class CountryDefaultTaxResolver : IMemberValueResolver<object, object, int, GetTaxOutputDto>
    {
        private readonly ITaxBLL _taxBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountryDefaultTaxResolver(ITaxBLL taxBLL, IHttpContextAccessor httpContextAccessor)
        {
            _taxBLL = taxBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public GetTaxOutputDto Resolve(object source, object destination, int sourceMember, GetTaxOutputDto destMember, ResolutionContext context)
        {
            return _taxBLL.GetDefaultTaxForCustomer(new GetCountryDefaultTaxInputDto { CountryId = _httpContextAccessor.GetUserCountryId() });
        }
    }

    //public class VersionDiscrimiatorResolver : IMemberValueResolver<object, object, int, int>
    //{
    //    private readonly IInvoiceBLL _invoiceBLL;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    public VersionDiscrimiatorResolver( IInvoiceBLL invoiceBLL, IHttpContextAccessor httpContextAccessor )
    //    {
    //        _invoiceBLL = invoiceBLL;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public int Resolve( object source, object destination, int sourceMember, int destMember, ResolutionContext context )
    //    {

    //        return _invoiceBLL.GetInvoiceDiscriminator(source, _httpContextAccessor.GetUserCountryId(), destMember);

    //    }
    //}

    public class AddonMinimumPriceResolver : IMemberValueResolver<object, object, int, AddonPriceDetailsDto>
    {
        private readonly IAddOnBLL _addOnBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddonMinimumPriceResolver(IAddOnBLL addOnBLL, IHttpContextAccessor httpContextAccessor)
        {
            _addOnBLL = addOnBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public AddonPriceDetailsDto Resolve(object source, object destination, int sourceMember, AddonPriceDetailsDto destMember, ResolutionContext context)
        {
            return _addOnBLL.GetMinimumAddonPrice(sourceMember, _httpContextAccessor.GetUserCountryId());
        }
    }
    public class AddonMinimumPriceResolver_ : IMemberValueResolver<object, object, int, VersionPriceAllDetailsDto>
    {
        private readonly IAddOnBLL _addOnBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddonMinimumPriceResolver_(IAddOnBLL addOnBLL, IHttpContextAccessor httpContextAccessor)
        {
            _addOnBLL = addOnBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public VersionPriceAllDetailsDto Resolve(object source, object destination, int sourceMember, VersionPriceAllDetailsDto destMember, ResolutionContext context)
        {
            return _addOnBLL.GetMinimumAddonPrice_(sourceMember, _httpContextAccessor.GetUserCountryId());
        }
    }


    public class AddonPriceResolver : IMemberValueResolver<object, object, AddOnPriceDto, AddOnPriceAllDetailsDto>
    {
        private readonly IAddOnBLL _addOnBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddonPriceResolver(IAddOnBLL addOnBLL, IHttpContextAccessor httpContextAccessor)
        {
            _addOnBLL = addOnBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public AddOnPriceAllDetailsDto Resolve(object source, object destination, AddOnPriceDto sourceMember, AddOnPriceAllDetailsDto destMember, ResolutionContext context)
        {
            var output = _addOnBLL.GetAddonPriceByPriceLevel(sourceMember.AddOnId, _httpContextAccessor.GetUserCountryId(), sourceMember.PriceLevelId).GetAwaiter().GetResult();

            if (output.IsSuccess)
                return output.Data?.Price;
            else
                return null;
        }
    }
    public class DefaultCurrencyResolver : IMemberValueResolver<object, object, string, string>
    {
        private readonly ICountryBLL _countryBLL;
        private readonly IHttpContextAccessor _accessor;

        public DefaultCurrencyResolver(ICountryBLL countryBLL, IHttpContextAccessor accessor)
        {
            _countryBLL = countryBLL;
            _accessor = accessor;
        }

        public string Resolve(object source, object destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
            {
                return destMember = _countryBLL.GetDefaultCurrencyCode();
                //if (string.IsNullOrWhiteSpace(_accessor.GetDefaultCurrencyCode()))
                //{
                //    var defaultCurrencyCode = _countryBLL.GetDefaultCurrencyCode();
                //    _accessor.SetDefaultCurrencyCode(defaultCurrencyCode);

                //}
                //return _accessor.GetDefaultCurrencyCode();
            }
            else
                return destMember = sourceMember;



        }
    }

    public class DownloadCenterPriceResolver : IMemberValueResolver<object, object, Application, DownloadPriceDto>
    {
        private readonly IApplicationBLL _applicationBLL;

        public DownloadCenterPriceResolver(IApplicationBLL applicationBLL)
        {
            _applicationBLL = applicationBLL;
        }

        public DownloadPriceDto Resolve(object source, object destination, Application sourceMember, DownloadPriceDto destMember, ResolutionContext context)
        {
            context.Options.Items.TryGetValue("CurrencyId", out var currencyIdObj);
            var currencyId = currencyIdObj as int? ?? 4;
            var price = Task.Run(() => _applicationBLL.GetMimumPriceWithCurrencyId(sourceMember, currencyId)).GetAwaiter().GetResult();
            return price;
        }


    }
    public class DownloadCenterAddOnPriceResolver : IMemberValueResolver<object, object, AddOn, DownloadPriceDto>
    {
        private readonly IAddOnBLL _addOnBLL;

        public DownloadCenterAddOnPriceResolver(IAddOnBLL addOnBLL)
        {
            _addOnBLL = addOnBLL;
        }

        public DownloadPriceDto Resolve(object source, object destination, AddOn sourceMember, DownloadPriceDto destMember, ResolutionContext context)
        {
            context.Options.Items.TryGetValue("CurrencyId", out var currencyIdObj);
            var currencyId = currencyIdObj as int? ?? 4;
            var price = Task.Run(() => _addOnBLL.GetMimumPriceWithCurrency(sourceMember, currencyId)).GetAwaiter().GetResult();
            return price;
        }


    }
    public class CurrencyCodeResolver : IValueResolver<object, object, string>
    {
        public string Resolve(object source, object destination, string destMember, ResolutionContext context)
        {
            var currencyCode = (string)context.Items["CurrencyCode"];
            return currencyCode;
        }
    }

    //public class DownloadCenterAddOnPriceWithDiffCurrencyResolver : IMemberValueResolver<object, object, AddOn, DownloadPriceDto>
    //{
    //    private readonly IAddOnBLL _addOnBLL;
    //    //private readonly int _currencyId;

    //    public DownloadCenterAddOnPriceWithDiffCurrencyResolver(IAddOnBLL addOnBLL/*, int currencyId*/)
    //    {
    //        _addOnBLL = addOnBLL;
    //        // _currencyId = currencyId; // Pass the currency ID here
    //    }

    //    public DownloadPriceDto Resolve(object source, object destination, AddOn sourceMember, DownloadPriceDto destMember, ResolutionContext context)
    //    {
    //        //var currency = (source as AddOn)?.AddOnPrices.Select(a=>a.CountryCurrency.CurrencyId).FirstOrDefault();
    //        //var price = Task.Run(() => _addOnBLL.GetMimumPriceWithCurrency(sourceMember, currency)).GetAwaiter().GetResult(); // Use the updated method with currency ID
    //        //return price;

    //        context.Options.Items.TryGetValue("CurrencyId", out var currencyIdObj);
    //        var currencyId = currencyIdObj as int? ?? 4;
    //        var price = Task.Run(() => _addOnBLL.GetMimumPriceWithCurrency(sourceMember, currencyId)).GetAwaiter().GetResult();
    //        return price;
    //    }
    //}



    #region Review & Rates
    public class ApplicationReviewResolver : IMemberValueResolver<object, object, int, RateDto>
    {
        private readonly ICustomerReviewBLL _customerReviewBll;

        public ApplicationReviewResolver(ICustomerReviewBLL customerReviewBll)
        {
            _customerReviewBll = customerReviewBll;
        }

        public RateDto Resolve(object source, object destination, int sourceMember, RateDto destMember, ResolutionContext context)
        {
            var rate = Task.Run(() => _customerReviewBll.GetRateAsync(sourceMember, false)).GetAwaiter().GetResult();
            return rate;
        }


    }
    #endregion

    #region URL 
    public class CustomAdminURLResolver : IMemberValueResolver<object, object, string, string>
    {
        public readonly FileStorageSetting _enviromentSetting;

        public CustomAdminURLResolver(IOptions<FileStorageSetting> enviromentSetting)
        {
            _enviromentSetting = enviromentSetting.Value;
        }

        public string Resolve(object source, object destination, string sourceMember, string destinationMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
                return null;

            return _enviromentSetting.BaseUrl + sourceMember.Replace("\\", "/");
        }
        #region Comment
        //public readonly EnviromentSetting _enviromentSetting;
        //public CustomURLResolver(IOptions<EnviromentSetting> enviromentSetting )
        //{
        //    _enviromentSetting = enviromentSetting.Value;
        //}
        //public string Resolve( FileStorage source, object destination, string sourceMember, string destinationMember, ResolutionContext context )
        //{

        //    return _enviromentSetting.BaseUrl+ source.FullPath.Replace("\\", "/");
        //}
        #endregion
    }
    public class CustomAdminBlobURLResolver : IMemberValueResolver<object, object, string, string>
    {
        public readonly FileStorageSetting _enviromentSetting;

        public CustomAdminBlobURLResolver(IOptions<FileStorageSetting> enviromentSetting)
        {
            _enviromentSetting = enviromentSetting.Value;
        }

        public string Resolve(object source, object destination, string sourceMember, string destinationMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
                return null;

            return Path.Combine(_enviromentSetting.BlobBaseUrl, sourceMember).Replace("\\", "/");
        }
        #region Comment
        //public readonly EnviromentSetting _enviromentSetting;
        //public CustomURLResolver(IOptions<EnviromentSetting> enviromentSetting )
        //{
        //    _enviromentSetting = enviromentSetting.Value;
        //}
        //public string Resolve( FileStorage source, object destination, string sourceMember, string destinationMember, ResolutionContext context )
        //{

        //    return _enviromentSetting.BaseUrl+ source.FullPath.Replace("\\", "/");
        //}
        #endregion
    }
    public class BlobURLResolver : IMemberValueResolver<object, object, FileStorage, FileStorageDto>
    {
        public readonly FileStorageSetting _fileStorageSetting;

        public BlobURLResolver(IOptions<FileStorageSetting> fileStorageSetting)
        {
            _fileStorageSetting = fileStorageSetting.Value;
        }

        public FileStorageDto Resolve(object source, object destination, FileStorage sourceMember, FileStorageDto destinationMember, ResolutionContext context)
        {
            if (sourceMember != null)

            {
                destinationMember = new FileStorageDto
                {
                    FullPath = Path.Combine(_fileStorageSetting.BlobBaseUrl, sourceMember.FullPath).Replace("\\", "/"),
                    Id = sourceMember.Id,
                    ContentType = sourceMember.ContentType,
                    Extension = sourceMember.Extension,
                    FileSize = sourceMember.FileSize,
                    Name = sourceMember.Name,
                    Path = sourceMember.Path,
                    RealName = sourceMember.RealName,
                    FileType = sourceMember.FileType?.Name
                };
            }
            return destinationMember;
        }
    }
    public class CustomBlobURLResolver : IMemberValueResolver<object, object, string, string>
    {
        public readonly FileStorageSetting _fileStorageSetting;

        public CustomBlobURLResolver
           (IOptions<FileStorageSetting> fileStorageSetting)
        {
            _fileStorageSetting = fileStorageSetting.Value;
        }

        public string Resolve(object source, object destination, string sourceMember, string destinationMember, ResolutionContext context)
        {

            return Path.Combine(_fileStorageSetting.BlobBaseUrl, sourceMember).Replace("\\", "/");
        }
    }
    public class CustomCutomerURLResolver : IMemberValueResolver<object, object, FileStorage, FileStorageDto>
    {
        public readonly FileStorageSetting _enviromentSetting;

        public CustomCutomerURLResolver(IOptions<FileStorageSetting> enviromentSetting)
        {
            _enviromentSetting = enviromentSetting.Value;
        }

        public FileStorageDto Resolve(object source, object destination, FileStorage sourceMember, FileStorageDto destinationMember, ResolutionContext context)
        {
            if (sourceMember != null)

            {
                destinationMember = new FileStorageDto
                {
                    FullPath = _enviromentSetting.BaseCustomerUrl + sourceMember.FullPath.Replace("\\", "/"),
                    Id = sourceMember.Id,
                    ContentType = sourceMember.ContentType,
                    Extension = sourceMember.Extension,
                    FileSize = sourceMember.FileSize,
                    Name = sourceMember.Name,
                    Path = sourceMember.Path,
                    RealName = sourceMember.RealName,
                    FileType = sourceMember.FileType?.Name

                };

            }
            return destinationMember;
        }
        #region Comment
        //public readonly EnviromentSetting _enviromentSetting;
        //public CustomURLResolver(IOptions<EnviromentSetting> enviromentSetting )
        //{
        //    _enviromentSetting = enviromentSetting.Value;
        //}
        //public string Resolve( FileStorage source, object destination, string sourceMember, string destinationMember, ResolutionContext context )
        //{

        //    return _enviromentSetting.BaseUrl+ source.FullPath.Replace("\\", "/");
        //}
        #endregion
    }

    public class AdminCustomCutomerURLResolver : IMemberValueResolver<object, object, FileStorage, FileStorageDto>
    {
        public readonly FileStorageSetting _fileStorageSetting;
        public readonly EnviromentSetting _enviromentSetting;

        public AdminCustomCutomerURLResolver(IOptions<FileStorageSetting> fileStorageSetting, IOptions<EnviromentSetting> enviromentSetting)
        {
            _fileStorageSetting = fileStorageSetting.Value;
            _enviromentSetting = enviromentSetting.Value;
        }

        public FileStorageDto Resolve(object source, object destination, FileStorage sourceMember, FileStorageDto destinationMember, ResolutionContext context)
        {
            if (sourceMember != null)

            {
                destinationMember = new FileStorageDto
                {
                    FullPath = _enviromentSetting.CLientApiBaseUrl + sourceMember.FullPath.Replace("\\", "/"),
                    Id = sourceMember.Id,
                    ContentType = sourceMember.ContentType,
                    Extension = sourceMember.Extension,
                    FileSize = sourceMember.FileSize,
                    Name = sourceMember.Name,
                    Path = sourceMember.Path,
                    RealName = sourceMember.RealName,
                    FileType = sourceMember.FileType?.Name

                };

            }
            return destinationMember;
        }
        #region Comment
        //public readonly EnviromentSetting _enviromentSetting;
        //public CustomURLResolver(IOptions<EnviromentSetting> enviromentSetting )
        //{
        //    _enviromentSetting = enviromentSetting.Value;
        //}
        //public string Resolve( FileStorage source, object destination, string sourceMember, string destinationMember, ResolutionContext context )
        //{

        //    return _enviromentSetting.BaseUrl+ source.FullPath.Replace("\\", "/");
        //}
        #endregion
    }

    public class CustomListUrlsResolver : IMemberValueResolver<object, object, List<string>, List<string>>
    {
        public readonly FileStorageSetting _enviromentSetting;

        public CustomListUrlsResolver(IOptions<FileStorageSetting> enviromentSetting)
        {
            _enviromentSetting = enviromentSetting.Value;
        }

        public List<string> Resolve(object source, object destination, List<string> sourceMember, List<string> destinationMember, ResolutionContext context)
        {
            if (sourceMember is null || !sourceMember.Any())
                return null;

            var imagePaths = new List<string>();

            sourceMember.ForEach(p => { imagePaths.Add(Path.Combine(_enviromentSetting.BlobBaseUrl, p).Replace("\\", "/")); });

            return imagePaths;
        }
    }
    #endregion

    public class VersionSubscriptionUsedDeviceResolver : IMemberValueResolver<object, object, int, int>
    {
        private readonly ICustomerProductBLL _customerProductBLL;

        public VersionSubscriptionUsedDeviceResolver(ICustomerProductBLL customerProductBLL)
        {
            _customerProductBLL = customerProductBLL;
        }

        public int Resolve(object source, object destination, int sourceMember, int destMember, ResolutionContext context)
        {

            return _customerProductBLL != null ? _customerProductBLL.GetVersionSubscriptionUsedDevice(sourceMember) : 0;
        }
    }
    public class GetRenewalDateInvoiceResolver : IMemberValueResolver<object, object, int, DateTime>
    {
        private readonly IInvoiceHelperBLL _helper;

        public GetRenewalDateInvoiceResolver(IInvoiceHelperBLL helper)
        {
            _helper = helper;
        }

        public DateTime Resolve(object source, object destination, int sourceMember, DateTime destMember, ResolutionContext context)
        {

            return _helper.GetRenewalDate(sourceMember);
        }
    }

    public class NotificationActionDescriptionResolver : IMemberValueResolver<Notification, object, string, string>
    {
        private readonly IHttpContextAccessor _accessor;

        public NotificationActionDescriptionResolver(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Resolve(Notification source, object destination, string destMember, string sourceMember, ResolutionContext context)
        {
            try
            {
                string[] valuesForAppenddescription = new string[10];
                valuesForAppenddescription[0] = source.InvoiceId?.ToString();
                var appVerAddonNamesJsonStr = source.InvoiceId > 0 ?
                    !source.Invoice.CustomerSubscription.IsAddOn ?
                    source.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault()?.VersionName :
                    source.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault()?.AddonName :
                     source.LicenceId > 0 ? (!source.Licence.CustomerSubscription.IsAddOn ?
                  source.Licence.CustomerSubscription.VersionSubscriptions.FirstOrDefault()?.VersionName :
                    source.Licence.CustomerSubscription.AddonSubscriptions.FirstOrDefault()?.AddonName) : null;
                var appVerAddonNames = appVerAddonNamesJsonStr != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(appVerAddonNamesJsonStr) : null;
                valuesForAppenddescription[1] = appVerAddonNames?.First().Value;//version/addon en
                valuesForAppenddescription[2] = appVerAddonNames?.Skip(1)?.First().Value; //version/addon ar
                valuesForAppenddescription[3] = source.InvoiceId > 0 ?
                    source.Invoice?.CustomerSubscription?.Customer?.Contract?.Serial : source.Licence?.Serial;
                var c = source.RefundRequest;
                valuesForAppenddescription[4] = source.RefundRequest?.Reason;
                var jsonTicketsRerences = source.TicketRefrences != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(source.TicketRefrences).Values.ToArray() : null;
                if (jsonTicketsRerences?.Length >= 2)
                {
                    valuesForAppenddescription[5] = jsonTicketsRerences[0]; //for ticket no
                    valuesForAppenddescription[6] = jsonTicketsRerences[1]; //for sendername no
                }
                var jsonDescription = JsonConvert.DeserializeObject<Dictionary<string, string>>(source.NotificationAction.Description).Values;
                return
                     JsonConvert.SerializeObject(
                 new
                 {
                     defaultEn = string.Format(jsonDescription.First(), valuesForAppenddescription),
                     ar = string.Format(jsonDescription.Skip(1).First(), valuesForAppenddescription)
                 }).Replace("defaultEn", "default");
            }
            catch (Exception e)
            {

                throw;
            }

        }
    }

}
