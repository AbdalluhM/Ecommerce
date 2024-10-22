using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Ecommerce.API.Attributes;
using Ecommerce.BLL.LookUps;
using Ecommerce.Core.Enums.Roles;

using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LookUpController : BaseAdminController
    {
        #region Fields
        private readonly ILogger<LookUpController> _logger;
        private readonly ILookUpBLL _lookUpBLL;

        #endregion

        #region Constructor
        public LookUpController(ILogger<LookUpController> logger, ILookUpBLL lookUpBLL, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _logger = logger;
            _lookUpBLL = lookUpBLL;
        }

        #endregion

        #region Actions
       #region Countries
        [HttpGet]
        [Route("Country/GetAll")]
        public async Task<IActionResult> GetAllCountries()
        {
            var output = await _lookUpBLL.GetAllCountriesAsync();

            return Ok(output);
        }
        #endregion

        #region Currencies


        [HttpGet]
        //  [Route("GetAllCurrencies")]
        [Route("Currency/GetAll")]

        public async Task<IActionResult> GetAllCurrencies()
        {
            var output = await _lookUpBLL.GetAllCurrenciesAsync();
            return Ok(output);
        }
        #endregion
        #region EmployeeTypes

        [HttpGet]
        [Route("EmployeeType/GetAll")]
        public async Task<IActionResult> GetAllEmployeeTypes()
        {
            var output = await _lookUpBLL.GetAllEmployeeTypesAsync();
            return Ok(output);
        }

        #endregion
        #region DashBoard
        [HttpGet]
        [Route("DashBoard/GetAll")]
        public async Task<IActionResult> GetAllDashboardCounts()
        {
            var output = await _lookUpBLL.GetAllDashboardCounts();
            return Ok(output);
        }
        #endregion
        #region SubscriptionType

        [HttpGet]
        [Route("SubscriptionType/GetAll")]
        public async Task<IActionResult> GetAllSubscriptionTypes()
        {
            var output = await _lookUpBLL.GetAllSubscriptionTypesAsync();
            return Ok(output);
        }

        #endregion

        #endregion
    }
}
