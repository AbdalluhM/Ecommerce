using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Contracts.Licenses;
using Ecommerce.BLL.Customers.CustomerProduct;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.DTO.Customers.CustomerProduct;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerProductController : BaseCustomerController
    {
        private readonly ICustomerProductBLL _customerProductBLL;
        private readonly ILicensesBLL _licenceBLL;
        private readonly IMapper _mapper;

        public CustomerProductController(ICustomerProductBLL customerProductBLL,
                                         IMapper mapper,
                                         ILicensesBLL licencesBLL,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _customerProductBLL = customerProductBLL;
            _mapper = mapper;
            _mapper = mapper;
            _licenceBLL = licencesBLL;
        }

        [HttpGet("GetCustomerProductLookup")]
        public async Task<ActionResult> GetCustomerProductLookup(int deviceTypeId)
        {
            var result = await _customerProductBLL.GetAllCustomerProductsLookupAsync(CurrentUserId, deviceTypeId);

            return Ok(result);
        }

        [HttpGet("GetCustomerProductByAppIdLookup")]
        public async Task<ActionResult> GetCustomerProductByAppIdLookup(int appId)
        {
            var result = await _customerProductBLL.GetCustomerProductByAppIdLookup(CurrentUserId, appId);

            return Ok(result);
        }
        [HttpGet("GetCustomerProductsLookup")]
        public async Task<ActionResult> GetCustomerProductsLookup(int? appId = null)
        {
            var result = await _customerProductBLL.GetAllCustomerProductsLookupAsync(CurrentUserId, appId);

            return Ok(result);
        }
        [HttpGet("GetCustomerProductsWorkspacesLookup")]
        public async Task<ActionResult> GetCustomerProductsWorkspacesLookup()
        {
            var result = await _customerProductBLL.GetAllCustomerProductsWorkspacesLookupAsync(CurrentUserId);

            return Ok(result);
        }

        [HttpGet("GetCustomerApplicationsLookup")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> GetCustomerApplicationLookup()
        {
            var result = await _customerProductBLL.GetCustomerApplicationsLookupAsync(CurrentUserId);

            return Ok(result);
        }

        [HttpGet("GetCustomerSubscription")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> GetCustomerSubscriptionById(int VersionSubscriptionId)
        {
            var response = await _customerProductBLL.GetCustomerSubscriptionById(new CutomerSubscriptionInputDto
            {
                CustomerId = CurrentUserId,
                VersionSubscriptionId = VersionSubscriptionId
            });
            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("UpdateCustomerSubscription")]
        public async Task<IActionResult> UpdateCustomerSubscription(UpdateCutomerSubscriptionInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _customerProductBLL.UpdateCustomerSubscription(inputDto);
            return Ok(output);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("RefundRequestSubscription")]
        public async Task<IActionResult> RefundRequestSubscription(RefundRequestInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _customerProductBLL.RefundSubscriptionRequest(inputDto);
            return Ok(output);
        }

        [HttpGet("GetAllCustomerProducts")]
        [Produces(typeof(IResponse<List<GetCustomerProductOutputDto>>))]
        public async Task<IActionResult> GetAllCustomerProducts()
        {
            var response = await _customerProductBLL.GetAllCustomerProducts(CurrentUserId);
            return Ok(response);
        }

        [HttpGet("GetCustomerSubscriptions")]
        public async Task<IActionResult> GetCustomerSubscriptions()
        {
            var response = await _customerProductBLL.GetCustomerSubscriptions(CurrentUserId);
            return Ok(response);
        }

        [HttpGet("GetCustomerApplicationVersions")]
        public async Task<IActionResult> GetCustomerApplicationVersions(int appId)
        {
            var response = await _customerProductBLL.GetCustomerApplicationVersions(CurrentUserId, appId);
            return Ok(response);
        }
        // [HttpGet("GetCustomerApplicationVersionsAndAddons")]
        //public async Task<IActionResult> GetCustomerApplicationVersionsAndAddOns(int appId)
        //{
        //    var response = await _customerProductBLL.GetCustomerApplicationVersionsAndAddOns(CurrentUserId, appId);
        //    return Ok(response);
        //}

        [HttpGet("GetFirstRenewDate")]
        public async Task<IActionResult> GetFirstRenewDate()
        {
            var response = await _customerProductBLL.GetFirstRenewDateOfAllVersionsOfUser(CurrentUserId);
            return Ok(response);
        }


        [HttpGet("GetCustomerProduct")]
        public async Task<IActionResult> GetCustomerProduct([FromQuery] CustomerProductApIInputDto inputDto)
        {
            var input = _mapper.Map<CustomerProductInputDto>(inputDto);
            input.CustomerId = CurrentUserId;

            var response = await _customerProductBLL.GetCustomerProductById(input);
            return Ok(response);
        }

        [HttpGet("CustomerProductAddOn")]
        public async Task<IActionResult> GetCustomerProductAddOn([FromQuery] CustomerProductApIInputDto inputDto)
        {
            var input = _mapper.Map<CustomerProductInputDto>(inputDto);
            input.CustomerId = CurrentUserId;
            input.CountryId = CurrentUserCountryId;
            var output = await _customerProductBLL.GetCustomerProductAddOns(input);
            return Ok(output);
        }

        [HttpGet("GetProductAddonDetails")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> GetProductAddonDetailsAsync(int addonSubscriptionId)
        {
            var result = await _customerProductBLL.GetProductAddonDetailsAsync(addonSubscriptionId);

            return Ok(result);
        }

        [HttpPost("CancelAddonSubscription")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> CancelAddonSubscriptionAsync(CancelAddonSubscriptionDto cancelAddonSubscription)
        {
            var result = await _customerProductBLL.CancelAddonSubscriptionAsync(cancelAddonSubscription);

            return Ok(result);
        }

        [HttpGet("GetCustomerProductLicencesPagedList")]
        public async Task<IActionResult> GetCustomerProductLicencesPagedList([FromQuery] LicencesFilterInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.GetCustomerProductLicencesPagedListAsync(inputDto);
            return Ok(output);
        }
        [HttpGet("GetCustomerProductsLicencesPagedList")]
        public async Task<IActionResult> GetCustomerProductsLicencesPagedList([FromQuery] AllLicencesFilterInputDto pagedDto)
        {
            pagedDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.GetCustomerProductsLicencesPagedListAsync(pagedDto);
            return Ok(output);
        }

        [HttpGet("GetVersionLicencesLookup")]
        public async Task<IActionResult> GetVersionLicencesLookupAsync([FromQuery] CustomerProductApIInputDto inputDto)
        {
            var input = _mapper.Map<CustomerProductInputDto>(inputDto);
            input.CustomerId = CurrentUserId;
            input.CountryId = CurrentUserCountryId;
            var output = await _licenceBLL.GetVersionLicencesLookupAsync(input);
            return Ok(output);
        }

        [HttpPost(template: "ReNewActivationLicense")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> ReNewAcivationLicense(LicenseActionInputDto inputDto)
        {
            //TODO:change after Add Data & Authrorization
            inputDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.RenewVersionLicenseAsync(inputDto);
            return Ok(output);
        }

        [HttpPost(template: "ReNewAddOnActivationLicense")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> ReNewAddOnActivationLicense(AddOnLicenseActionInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.RenewAddOnLicenseAsync(inputDto);
            return Ok(output);
        }

        [HttpPost("CreateCustomerProductLicense")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> CreateCustomerProductLicense(CreateLicenseInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.CreateDeviceByCustomerAsync(inputDto);
            return Ok(output);
        }

        [HttpPost("CreateCustomerAddOnLicense")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> CreateCustomerAddOnLicense(NewAddOnLicenseDto newAddonLicense)
        {
            newAddonLicense.CustomerId = CurrentUserId;

            var result = await _licenceBLL.CreateAddOnDeviceAsync(newAddonLicense);

            return Ok(result);
        }

        [HttpGet("CustomerRequestChangeDevice")]
        public async Task<IActionResult> GetCustomerRequestChangeDevice([FromQuery] RequestChangeDeviceFilterDto inputDto)
        {
            var input = _mapper.Map<LicencesFilterInputDto>(inputDto);
            input.CustomerId = CurrentUserId;
            var response = await _licenceBLL.GetCustomerRequestChangeDevice(input/*CurrentUserId*/);
            return Ok(response);
        }

        [HttpPost("UpdateCustomerProductLicense")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> UpdateCustomerProductLicense(UpdateLicenseInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _licenceBLL.ChangeDeviceAsync(inputDto);
            return Ok(output);
        }

        [HttpGet("CustomerProductReleases")]
        public async Task<IActionResult> CustomerProductReleases([FromQuery] CustomerProductApIInputDto inputDto)
        {
            var input = _mapper.Map<CustomerProductInputDto>(inputDto);
            input.CustomerId = CurrentUserId;
            var output = await _customerProductBLL.GetCustomerProductReleases(input);
            return Ok(output);
        }

        [HttpPost("VersionReleaseDownload")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> DownloadRelease(VersionReleaseInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _customerProductBLL.DownloadRelease(inputDto);
            return Ok(output);
        }

        [HttpGet("GetReasonChangeDevice")]
        public async Task<IActionResult> GetReasonChangeDevice()
        {
            var output = await _licenceBLL.GetReasonChangeDeviceLookupAsync();
            return Ok(output);
        }




    }
}
