using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.DevicesTypee;
using Ecommerce.Core.Consts.Auth;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.DevicesType
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesTypeController : BaseCustomerController
    {
        private readonly IDevicesTypeBLL _devicesTypeBLL;

        public DevicesTypeController(IHttpContextAccessor httpContextAccessor , IDevicesTypeBLL devicesTypeBLL):base(httpContextAccessor)
        {
            _devicesTypeBLL = devicesTypeBLL;
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpGet("GetDevicesType")]
        public async Task<IActionResult> GetDevicesType()
        {
            var result = await _devicesTypeBLL.GetDevicesType(CurrentUserId);
            return Ok(result);
        }
    }
}
