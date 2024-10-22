using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Countries;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CountryController : BaseAdminController
    {
        #region Fields

        private readonly ILogger<CountryController> _logger;
        private readonly ICountryBLL _countryBLL;
        IHttpContextAccessor _accessor;
        #endregion
      
        #region Constructor

        public CountryController( ILogger<CountryController> logger, ICountryBLL countryBLL, IHttpContextAccessor httpContextAccessor ) : base(httpContextAccessor)
        {
            _logger = logger;
            _countryBLL = countryBLL;
            _accessor = httpContextAccessor;
            
        }
        #endregion
        
        #region Actions 

        #region Country Currency
        [HttpPost]
        [Route("Create")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateCountryCurrency( AssignCurrencyToCountryInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var output = await _countryBLL.CreateAsync(inputDto);
            return Ok(output);
        }
        [HttpPost]
        [Route("Update")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateCountryCurrency( UpdateAssignCurrencyToCountryInputDto inputDto )
        {

            inputDto.ModifiedBy = CurrentEmployeeId;
            var output = await _countryBLL.UpdateAsync(inputDto);
            return Ok(output);

        }

        [HttpPost]
        [Route("Delete")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteCountryCurrency( int id )
        {
            var output = await _countryBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);

        }

        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.READ)]
        public async Task<IActionResult> GetCountryCurrency( int id )
        {
            var output = await _countryBLL.GetByIdAsync(new GetAssignedCurrencyToCountryInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet("GetAll")]
        // [DxAuthorize(PagesEnum.Countries, ActionsEnum.READ)]
        [Authorize]
        public async Task<IActionResult> GetAll( )
        {
            var result = await _countryBLL.GetAllListAsync();

            return Ok(result);
        }
        
        [HttpGet("GetAllByEmployeeId")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllByEmployeeIdAsync()
        {
            var result = await _countryBLL.GetAllByEmployeeIdAsync(CurrentEmployeeId);

            return Ok(result);
        }
        
        [HttpGet]
        [Route("GetPagedList")]
        [DxAuthorize(PagesEnum.Countries, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {
            var output = await _countryBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }
        #endregion

        #endregion
    }
}
