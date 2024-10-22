using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Addons;
using Ecommerce.Core.Consts.Auth;

using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.Addons
{
    [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
    [Route("api/[controller]")]
    [ApiController]
    public class AddonController : BaseCustomerController
    {
        private readonly IAddOnBLL _addonBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddonController(IAddOnBLL addonBLL,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _addonBLL = addonBLL;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAddonDetails(int id)
        {
            if (Request.Headers.TryGetValue("countryCode", out var countryCode) && CurrentUserId == 0)
            {
                // Handle based on the value of the "countryId" header
                var countryId = GetCountryId(countryCode.ToString());
                _httpContextAccessor.HttpContext.Items.Add(nameof(countryId), countryId);
                return Ok(await _addonBLL.GetAddonDetailsAsync(id, CurrentUserId));
            }
            if (CurrentUserId == 0)
                return Unauthorized();

            var response = await _addonBLL.GetAddonDetailsAsync(id, CurrentUserId);

            return Ok(response);
        }
        [HttpGet("AddOn/{addOnId}/{priceLevelId:int?}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVersionDetailsAsync(int addOnId, int? priceLevelId = null)
        {
            if (Request.Headers.TryGetValue("countryCode", out var countryCode) && CurrentUserId == 0)
            {
                // Handle based on the value of the "countryId" header
                var countryId = GetCountryId(countryCode.ToString());
                _httpContextAccessor.HttpContext.Items.Add(nameof(countryId), countryId);
                return Ok(await _addonBLL.GetAddonPriceByPriceLevel(addOnId, countryId, priceLevelId));
            }

            if (CurrentUserCountryId == 0)
                return Unauthorized();
            var response = await _addonBLL.GetAddonPriceByPriceLevel(addOnId, CurrentUserCountryId, priceLevelId);

            return Ok(response);

        }
    }
}
