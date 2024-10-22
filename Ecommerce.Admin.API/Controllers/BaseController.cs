using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Ecommerce.BLL.Responses;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;
using System;
using System.IO;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    public class BaseAdminController : ControllerBase
    {
        private readonly FileStorageSetting _fileSetting;
        public int CurrentEmployeeId { get; set; } = 0;
        public string CurrentEmployeeLanguage { get; set; }
        public string CurrentEmployeeCuluture { get; set; }
        public GetEmployeeOutputDto CurrentEmployee;

        protected IWebHostEnvironment _webHostEnvironment;
        public BaseAdminController(IHttpContextAccessor httpContextAccessor)
        {
            CurrentEmployeeLanguage = httpContextAccessor.HttpContext.Items["CurrentEmployeeLanguage"]?.ToString() ?? string.Empty;
            CurrentEmployeeCuluture = httpContextAccessor.HttpContext.Items["CurrentEmployeeCulture"]?.ToString() ?? string.Empty;

            if (httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {


                CurrentEmployee = ((IResponse<GetEmployeeOutputDto>)httpContextAccessor.HttpContext.Items["Employee"])?.Data;
                var employee = httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                int employeeId;
                if (!string.IsNullOrWhiteSpace(employee))
                {
                    CurrentEmployeeId = int.TryParse(employee, out employeeId) ? employeeId : 0;
                }
            }
        }

        public BaseAdminController(IHttpContextAccessor httpContextAccessor, IOptions<FileStorageSetting> fileOptions)
            : this(httpContextAccessor)
        {
            if (fileOptions is not null)
                _fileSetting = fileOptions.Value;
        }
        public BaseAdminController(IHttpContextAccessor httpContextAccessor, IOptions<FileStorageSetting> fileOptions, IWebHostEnvironment webHostEnvironment)
          : this(httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            if (fileOptions is not null)
                _fileSetting = fileOptions.Value;
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
            var fileDto = new FileDto();

            if (_fileSetting is null || file is null)
                return fileDto;

            fileDto.File = file;
            fileDto.FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory;

            switch (filePathEnum)
            {
                case FilePathEnum.AddonBase:
                    fileDto.FilePath = Path.Combine(_fileSetting.Files.Admin.Addon.Path,
                                                    _fileSetting.Files.Admin.Addon.AddonBase);
                    break;
                case FilePathEnum.Feature:
                    fileDto.FilePath = _fileSetting.Files.Admin.Feature.Path;
                    break;
                case FilePathEnum.AddonSlider:
                    {
                        fileDto.FilePath = Path.Combine(_fileSetting.Files.Admin.Addon.Path,
                                                                         _fileSetting.Files.Admin.Addon.AddonSlider);
                        fileDto.ContainerName = _fileSetting.Files.Admin.License.ContainerName;
                    }
                    break;
                case FilePathEnum.Module:
                    fileDto.FilePath = _fileSetting.Files.Admin.Module.Path;
                    break;
                case FilePathEnum.ModuleSlider:
                    fileDto.FilePath = Path.Combine(_fileSetting.Files.Admin.Module.Path,
                                                    _fileSetting.Files.Admin.Module.ModuleSlider);
                    break;
                case FilePathEnum.Application:
                    fileDto.FilePath = _fileSetting.Files.Admin.Application.Path;

                    break;
                case FilePathEnum.ApplicationSlider:
                    fileDto.FilePath = Path.Combine(_fileSetting.Files.Admin.Application.Path,
                                                    _fileSetting.Files.Admin.Application.ApplicationSlider);
                    break;

                case FilePathEnum.ApplicationVersion:
                    fileDto.FilePath = Path.Combine(_fileSetting.Files.Admin.Application.Path,
                                                    _fileSetting.Files.Admin.Application.ApplicationVersion);
                    break;
                case FilePathEnum.AdminProfile:
                    fileDto.FilePath = _fileSetting.Files.Admin.AdminProfile.Path;
                    break;
                case FilePathEnum.License:
                    {
                        fileDto.FilePath = _fileSetting.Files.Admin.License.Path;
                        fileDto.ContainerName = _fileSetting.Files.Admin.License.ContainerName;
                    }
                    break;
                default:
                    return fileDto;
            }

            return fileDto;
        }
    }
}
