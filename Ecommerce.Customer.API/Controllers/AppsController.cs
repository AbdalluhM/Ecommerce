using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Applications;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Customers.Apps;
using Ecommerce.BLL.Features;
using Ecommerce.BLL.Modules;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Features;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
    [Route("api/[controller]")]
    [ApiController]
    public class AppsController : BaseCustomerController
    {
        private readonly IAppsBLL _appsBLL;
        private readonly IApplicationBLL _applicationBLL;
        private readonly IVersionBLL _versionBLL;
        private readonly IFeatureBLL _featureBLL;

        private readonly IModuleBLL _moduleBLL;
        IHttpContextAccessor _httpContextAccessor;

        public AppsController(IAppsBLL appsBLL,
                              IApplicationBLL applicationBLL,
                              IVersionBLL versionBLL,
                              IModuleBLL moduleBLL, IFeatureBLL featureBLL,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _featureBLL = featureBLL;
            _appsBLL = appsBLL;
            _applicationBLL = applicationBLL;
            _versionBLL = versionBLL;
            _moduleBLL = moduleBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpGet("{applicationId}")]
        //public async Task<IActionResult> GetByIdAsync( int applicationId )
        //{

        //    var output = await _appsBLL.GetByIdAsync(applicationId);
        //    return Ok(output);

        //}

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string searchTerm)
        {
            var output = await _appsBLL.GetAllAsync(searchTerm);
            return Ok(output);
        }
        [HttpGet("GetPriceRange")]
        public async Task<IActionResult> GetPriceRange(int? ApplicationId)
        {
            var output = await _appsBLL.GetPriceRangeAsync(ApplicationId);
            return Ok(output);
        }
        [HttpGet("FilterApps")]
        public async Task<IActionResult> FilterApps([FromQuery] FilterAppsInputDto inputDto)
        {
            //TODO:Get this from Authorization
            inputDto.CurrentCustomerId = CurrentUserId;
            var output = await _appsBLL.FilterApps(inputDto);
            return Ok(output);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppDetailsAsync(int id)
        {
            var countryId = 0;
            if (Request.Headers.TryGetValue("countryCode", out var countryCode) && CurrentUserId == 0)
            {
                // Handle based on the value of the "countryId" header
                countryId = GetCountryId(countryCode.ToString());
                _httpContextAccessor.HttpContext.Items.Add(nameof(countryId), countryId);
                return Ok(await _applicationBLL.GetAppDetailsAsync(id, countryId));

            }

            if (CurrentUserCountryId == 0)
                return Unauthorized();

            var response = await _applicationBLL.GetAppDetailsAsync(id, countryId, currentUserId: CurrentUserId);

            return Ok(response);
        }

        [HttpGet("Version/{appId}/{versionId:int?}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVersionDetailsAsync(int appId, int? versionId = null)
        {
            if (Request.Headers.TryGetValue("countryCode", out var countryCode) && CurrentUserId == 0)
            {
                // Handle based on the value of the "countryId" header
                var countryId = GetCountryId(countryCode.ToString());
                _httpContextAccessor.HttpContext.Items.Add(nameof(countryId), countryId);
                return Ok(await _versionBLL.GetVersionDetailsAsync(appId, countryId, versionId));
            }

            if (CurrentUserCountryId == 0)
                return Unauthorized();

            return Ok(await _versionBLL.GetVersionDetailsAsync(appId, CurrentUserCountryId, versionId));
        }

        [HttpGet("Module/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppModulePopupInfo(int id)
        {
            var response = await _moduleBLL.GetAppModulePopupAsync(id);

            return Ok(response);
        }

        [HttpGet("AppDetailsPricingAndPlans/{appId}")]
        public async Task<IActionResult> GetAppDetailsPricingAndPlans(int appId)
        {
            var result = await _applicationBLL.GetAppDetailsPricingAndPlans(new GetAppDetailsPricingAndPlansInputDto { ApplicationId = appId, CountryId = CurrentUserCountryId });

            return Ok(result);
        }

        [HttpGet("AppDetailsPricingAndPlans")]
        public async Task<IActionResult> GetAppDetailsPricingAndPlans(int versionId, int subscriptionTypeId)
        {
            var result = await _applicationBLL.GetPackagesAsync(versionId, subscriptionTypeId, CurrentUserCountryId);

            return Ok(result);
        }
        [HttpGet("GetAvailableVersionsByApplication/{appId}/{priceLevelId:int?}")]
        public async Task<IActionResult> GetAvailableVersionsByApplicationAndPricing(int appId, int? priceLevelId = null)
        {
            var result = await _versionBLL.GetAvailableVersionsByApplicationAndPricing(
                new GetAvailableVersionsByApplicationAndPricingInputDto
                {
                    ApplicationId = appId,
                    PriceLevelId = priceLevelId
                });

            return Ok(result);
        }
        [HttpGet("GetAvailableVersionsByApplication")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableVersionsByApplication(int appId)
        {
            if (Request.Headers.TryGetValue("countryCode", out var countryCode) && CurrentUserId == 0)
            {
                // Handle based on the value of the "countryId" header
                var countryId = GetCountryId(countryCode.ToString());
                _httpContextAccessor.HttpContext.Items.Add(nameof(countryId), countryId);
                return Ok(await _versionBLL.GetAvailableVersionsByApplication(appId, countryId));
            }

            if (CurrentUserCountryId == 0)
                return Unauthorized();

            var result = await _versionBLL.GetAvailableVersionsByApplication(appId, CurrentUserCountryId);

            return Ok(result);
        }

        [HttpGet("Feature/{id}")]
        public async Task<IActionResult> GetFeatureByIdAsync(int id)
        {

            var output = await _featureBLL.GetByIdAsync(new GetFeatureInputDto { Id = id });
            return Ok(output);

        }
    }
}

