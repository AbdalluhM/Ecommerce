using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.Core.Consts.Localization;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseCustomerController : ControllerBase
    {
        private readonly FileStorageSetting _fileSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string CurrentEmployeeLanguage { get; set; }
        public string CurrentEmployeeCuluture { get; set; }


        public BaseCustomerController(IHttpContextAccessor httpContextAccessor)
        {
            CurrentEmployeeLanguage = httpContextAccessor.HttpContext.Items[LanguageConsts.CurrentEmployeeLanguage]?.ToString() ?? string.Empty;
            CurrentEmployeeCuluture = httpContextAccessor.HttpContext.Items[LanguageConsts.CurrentEmployeeCulture]?.ToString() ?? string.Empty;

        }
        public BaseCustomerController(IOptions<FileStorageSetting> fileSetting, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            if (fileSetting is not null)
                _fileSetting = fileSetting.Value;
        }
        public ClaimsPrincipal CurrentUser
        {
            get
            {
                if (HttpContext.User is not null && HttpContext.User.Identity.IsAuthenticated)
                    return HttpContext.User;
                else
                    return null;
            }
        }

        public int CurrentUserId { get => CurrentUser is not null ? Convert.ToInt32(CurrentUser.FindFirstValue(TokenClaimTypeEnum.Id.ToString())) : default; }

        public int CurrentUserCountryId { get => CurrentUser is not null ? Convert.ToInt32(CurrentUser.FindFirstValue(TokenClaimTypeEnum.CountryId.ToString())) : default; }
        public string CurrentUserCurrency { get => CurrentUser is not null ? CurrentUser.FindFirstValue(TokenClaimTypeEnum.Currency.ToString()) : default; }
        public string CurrentUserName { get => CurrentUser is not null ? CurrentUser.FindFirstValue(TokenClaimTypeEnum.Name.ToString()) : default; }
        public Guid? CrmId
        {
            get
            {

                if (CurrentUser is not null)
                {
                    var crmId = CurrentUser.FindFirstValue(TokenClaimTypeEnum.CrmId.ToString());

                    if (!string.IsNullOrEmpty(crmId))
                        return Guid.Parse(crmId);
                    else
                        return null;
                }
                else
                    return null;
            }
        }


        protected int GetCountryId(string countryCode)
        {
            var countryRepository = (BaseRepository<Country>)HttpContext.RequestServices.GetService(typeof(IRepository<Country>));
            return countryRepository.GetAll().FirstOrDefault(c => c.PhoneCode == countryCode)?.Id ?? 0;
        }





        /////-------------------------------------------------------------------------------------------------
        ///// <summary>  Get file that upload. </summary>
        /////
        ///// <param name="file">            File will Upload
        ///// <param name="filePathEnum">    Path for File will Upload
        /////                                    . </param>
        /////                                 
        /////
        ///// <returns>   A File. </returns>
        /////-------------------------------------------------------------------------------------------------
        protected FileDto GetFile(IFormFile file, FilePathEnum filePathEnum)
        {
            if (_fileSetting is null || file is null)
                return null;
            var fileDto = new FileDto
            {
                File = file,
                FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory
            };

            switch (filePathEnum)
            {
                case FilePathEnum.CustomerProfile:
                    //fileDto.FilePath = Path.Combine(_fileSetting.Files.Customers.ProfileData.Path,
                    //                                _fileSetting.Files.Customers.ProfileData.Base);
                    fileDto.FilePath = _fileSetting.Files.Customers.ProfileData.Path;
                    fileDto.ContainerName = _fileSetting.Files.DexefSystem.ContainerName;

                    break;

                case FilePathEnum.DexefWorkSpace:
                    fileDto.FilePath = _fileSetting.Files.DexefSystem.Path;
                    fileDto.ContainerName = _fileSetting.Files.DexefSystem.ContainerName;
                    break;
                case FilePathEnum.Ticket:
                    fileDto.FilePath = _fileSetting.Files.Customers.Ticket.Path;
                    fileDto.ContainerName = _fileSetting.Files.Customers.Ticket.ContainerName;
                    break;
                default:
                    return null;
            }

            return fileDto;
        }


        protected List<SingleFilebaseDto> GetFile(List<IFormFile> files, FilePathEnum filePathEnum)
        {

            var list = new List<SingleFilebaseDto>();
            foreach (var file in files)
            {
                if (_fileSetting is null || file is null)
                    return null;
                switch (filePathEnum)
                {

                    case FilePathEnum.Ticket:
                        list.Add(new SingleFilebaseDto
                        {
                            FileDto = new FileDto
                            {
                                File = file,
                                ContainerName = _fileSetting.Files.Customers.Ticket.ContainerName,
                                FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory,
                                FilePath = _fileSetting.Files.Customers.Ticket.Path,
                            }
                        });

                        break;
                    default:
                        return null;
                }

            }

            return list;

        }
    }
}