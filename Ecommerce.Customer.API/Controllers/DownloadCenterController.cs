using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.DownloadCenter;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadCenterController : BaseCustomerController
    {
        #region Fields

        private readonly IDownloadCenterBLL _downloadCenterBLL;
        IHttpContextAccessor _httpContextAccessor;

        #endregion
        #region Constructor

        public DownloadCenterController(IDownloadCenterBLL downloadCenterBLL, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _downloadCenterBLL = downloadCenterBLL;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion



        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetByIdAsync(int applicationId)
        {
            var output = await _downloadCenterBLL.GetByIdAsync(applicationId);
            return Ok(output);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var output = await _downloadCenterBLL.GetAllAsync();
            return Ok(output);
        }

        [HttpGet("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var output = await _downloadCenterBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }
        [HttpGet("GetApplicationTag")]
        public async Task<IActionResult> GetApplicationTag()
        {
            var output = await _downloadCenterBLL.GetApplicationTagsAsync();
            return Ok(output);
        }

        [HttpGet("GetAddOnTag")]
        public async Task<IActionResult> GetAddOnTag()
        {
            var output = await _downloadCenterBLL.GetAddOnTagsAsync();
            return Ok(output);
        }

        [HttpGet("GetDownloadCenter")]
        public async Task<IActionResult> GetDownloadCenter([FromQuery] string searchTerm, int? applicationTagId = null, int? addonTagId = null)
        {
            if (Request.Headers.TryGetValue("countryCode", out var countryCode))
            {
                // Handle based on the value of the "countryId" header
                var countryId = GetCountryId(countryCode.ToString());
                return Ok(await _downloadCenterBLL.GetDownloadCenterAsync(searchTerm, countryId, applicationTagId, addonTagId));
            }
            else
            {
                return Ok(await _downloadCenterBLL.GetDownloadCenterAsync(searchTerm, 249, applicationTagId, addonTagId));
            }
            //    var output = await _downloadCenterBLL.GetDownloadCenterAsync(searchTerm, countryId);
            //return Ok(output);
        }




    }
}

