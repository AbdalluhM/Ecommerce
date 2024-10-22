using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Taxes;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Taxes;

using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : BaseAdminController
    {
        #region Fields

        private readonly ITaxBLL _taxBLL;
        #endregion
        #region Constructor

        public TaxController( ITaxBLL taxBLL, IHttpContextAccessor httpContextAccessor ) : base(httpContextAccessor)
        {
            _taxBLL = taxBLL;
        }
        #endregion


        #region Actions 
        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.CREATE)]
        public IActionResult Create( CreateTaxInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId ;
            var output = _taxBLL.Create(inputDto);
            return Ok(output);

        }

        [HttpPost("Update")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.UPDATE)]
        public IActionResult Update( UpdateTaxInputDto inputDto )
        {
            inputDto.ModifiedBy = CurrentEmployeeId ;
            var output = _taxBLL.Update(inputDto);
            return Ok(output);

        }

        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.DELETE)]
        public IActionResult Delete( [FromQuery] int id )
        {

            var output = _taxBLL.Delete(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);
        }

        #endregion

        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.READ)]
        public async Task<IActionResult> GetByIdAsync( int id )
        {

            var output = await _taxBLL.GetByIdAsync(new GetTaxInputDto { Id = id });
            return Ok(output);

        }

        [HttpGet("GetAll")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.READ)]
        public IActionResult GetAll( )
        {
            var output = _taxBLL.GetAllList();
            return Ok(output);
        }

        [HttpGet("GetPagedList")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.READ)]
        public IActionResult GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {
            var output = _taxBLL.GetPagedTaxList(inputDto);
            return Ok(output);
        }


        [HttpGet("GetTaxActivitesPagedList")]
        [DxAuthorize(PagesEnum.Taxes, ActionsEnum.READ)]
        public async Task<IActionResult> GetTaxActivitesPagedList( [FromQuery] LogFilterPagedResultDto inputDto )
        {
            var output = await _taxBLL.GetTaxActivitesPagedListAsync(inputDto);
            return Ok(output);
        }

    }
}
