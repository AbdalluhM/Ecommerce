using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Contracts.Licenses;
using Ecommerce.BLL.Files;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Contracts.Licenses.Inputs;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers.Contracts.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LicenseController : BaseAdminController
    {
        private readonly ILicensesBLL _licensesBLL;
        private readonly IBlobFileBLL _blobFileBLL;

        public LicenseController(ILicensesBLL licensesBLL,
                                 IHttpContextAccessor httpContextAccessor,
                                 IOptions<FileStorageSetting> fileOptions,
                                 IWebHostEnvironment webHostEnvironment,
                                 IBlobFileBLL blobFileBLL)
            : base(httpContextAccessor,
                   fileOptions,
                   webHostEnvironment)
        {
            _licensesBLL = licensesBLL;
            _blobFileBLL = blobFileBLL;
        }

        [HttpGet("GetCustomersLookup")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomersLookupAsync()
        {
            var result = await _licensesBLL.GetCustomersLookupAsync(CurrentEmployeeId);

            return Ok(result);
        }

        [HttpGet("GetProductsLookup")]
        //[DxAuthorize(PagesEnum.Licences, ActionsEnum.READ)]
        [Authorize]
        public async Task<IActionResult> GetProductsLookupAsync(int customerId)
        {
            var result = await _licensesBLL.GetProductsLookupAsync(customerId, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpGet]
        [Route("GetLicensesRequests")]
        [DxAuthorize(new PagesEnum[] { PagesEnum.Customers, PagesEnum.Licences }, ActionsEnum.READ)]
        public async Task<IActionResult> GetLicensesRequestsAsync([FromQuery] LicenseRequestFilterDto requestFilters)
        {
            var result = await _licensesBLL.GetLicensesRequestsAsync(requestFilters, CurrentEmployeeId);

            return Ok(result);
        }



        [HttpGet("GetActiveLicenses")]
        [DxAuthorize(new PagesEnum[] { PagesEnum.Customers, PagesEnum.Licences }, ActionsEnum.READ)]
        public async Task<IActionResult> GetActiveLicensesAsync([FromQuery] LicenseRequestFilterDto requestFilters)
        {
            var result = await _licensesBLL.GetActiveLicensesAsync(requestFilters, CurrentEmployeeId);

            return Ok(result);
        }

        [HttpGet("GetExpiredLicenses")]
        [DxAuthorize(new PagesEnum[] { PagesEnum.Customers, PagesEnum.Licences }, ActionsEnum.READ)]
        public async Task<IActionResult> GetExpiredLicensesAsync([FromQuery] LicenseRequestFilterDto requestFilters)
        {
            var result = await _licensesBLL.GetExpiredLicensesAsync(requestFilters, CurrentEmployeeId);

            return Ok(result);
        }

        [HttpGet("GetChangedLicenses")]
        [DxAuthorize(new PagesEnum[] { PagesEnum.Customers, PagesEnum.Licences }, ActionsEnum.READ)]
        public async Task<IActionResult> GetChangedLicensesAsync([FromQuery] ChangeLicenseFilterDto requestFilters)
        {
            var result = await _licensesBLL.GetChangedLicensesAsync(requestFilters, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpGet("GetLicenseLog")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.READ)]
        public async Task<IActionResult> GetLicenseLogAsync([FromQuery] LicenseLogFilterDto requestFilters)
        {
            var result = await _licensesBLL.GetLicenseLogAsync(requestFilters);

            return Ok(result);
        }


        [HttpPost("CreateDevice")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateDeviceAsync([FromForm] NewDeviceDto newDeviceDto)
        {
            var fileDto = GetFile(newDeviceDto.File, FilePathEnum.License);

            var result = await _licensesBLL.CreateDeviceByAdminAsync(newDeviceDto, fileDto, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpPost("UploadLicense")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UploadLicenseAsync([FromForm] UploadLicenseDto uploadLicenseDto)
        {
            var fileDto = GetFile(uploadLicenseDto.File, FilePathEnum.License);

            var result = await _licensesBLL.UploadLicenseAsync(uploadLicenseDto, fileDto, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpPost("AcceptRefundRequest")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> AcceptRefundRequestAsync(int requestId)
        {
            var result = await _licensesBLL.AcceptRefundRequestAsync(requestId, CurrentEmployeeId);

            return Ok(result);
        }

        [HttpPost("AcceptRefundRequestByCash")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> AcceptRefundRequestByCashAsync(int requestId)
        {
            var result = await _licensesBLL.AcceptRefundRequestByCashAsync(requestId, CurrentEmployeeId);

            return Ok(result);
        }

        [HttpPost("RejectRefundRequest")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> RejectRefundRequestAsync(int requestId)
        {
            var result = await _licensesBLL.RejectRefundRequestAsync(requestId, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpPost("RejectChangeDeviceRequest")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> RejectChangeDeviceRequestAsync(int requestId)
        {
            var result = await _licensesBLL.RejectChangeDeviceRequestAsync(requestId, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpPost("ReactivateExpiredLicense")]
        [DxAuthorize(PagesEnum.Licences, ActionsEnum.UPDATE)]
        public async Task<IActionResult> ReactivateExpiredLicenseAsync([FromForm] ReactivateLicenseDto reactivateLicenseDto)
        {
            var fileDto = GetFile(reactivateLicenseDto.File, FilePathEnum.License);

            var result = await _licensesBLL.ReactivateExpiredLicenseAsync(reactivateLicenseDto, fileDto, CurrentEmployeeId);

            return Ok(result);
        }


        [HttpPost("Upload")]
        [AllowAnonymous]
        public async Task<IActionResult> Upload([FromForm] FileObject fileObject)
        {
            var fileDto = GetFile(fileObject.File, FilePathEnum.License);

            var result = await _blobFileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = new FileDto { File = fileObject.File } });

            return Ok(result);
        }

        public class FileObject
        {
            public IFormFile File { get; set; }
        }
    }
}
