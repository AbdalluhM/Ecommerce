using Microsoft.AspNetCore.Mvc;

using Ecommerce.BLL;
using Ecommerce.BLL.PriceLevels;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Paging;

using System.Threading.Tasks;

using System;
using Ecommerce.DTO.Features;
using Ecommerce.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.API.Attributes;
using Ecommerce.Core.Enums.Roles;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceLevelController : BaseAdminController
    {
        private readonly IPriceLevelBLL _priceLevelBLL;

        public PriceLevelController( IPriceLevelBLL priceLevelBLL, IHttpContextAccessor httpContextAccessor ) : base(httpContextAccessor)
    {
            _priceLevelBLL = priceLevelBLL;
        }

        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.CREATE)]
        public IActionResult AddPriceLevel( NewPriceLevelDto newPriceLevelDto )
        {
            newPriceLevelDto.CreatedBy = CurrentEmployeeId;
            var result = _priceLevelBLL.AddPriceLevel(newPriceLevelDto);

            return Ok(result);
        }

        [HttpPost("Update")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.UPDATE)]
        public IActionResult UpdatePriceLevel( UpdatePriceLevelDto updatePriceLevelDto )
        {

            updatePriceLevelDto.ModifiedBy = CurrentEmployeeId;
            var result = _priceLevelBLL.UpdatePriceLevel(updatePriceLevelDto);

            return Ok(result);
        }

        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.DELETE)]
        public IActionResult DeletePriceLevel( int id )
        {
            var result = _priceLevelBLL.DeletePriceLevel(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(result);
        }
        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.READ)]
        public async Task<IActionResult> GetByIdAsync( int id )
        {

            var output = await _priceLevelBLL.GetByIdAsync(new GetPriceLevelInputDto { Id = id });
            return Ok(output);

        }
        [HttpGet("GetAll")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.READ)]
        public IActionResult GetAllPriceLevels( )
        {
            var result = _priceLevelBLL.GetAllList();

            return Ok(result);
        }
        [HttpGet]
        [Route("GetPagedList")]
        [DxAuthorize(PagesEnum.PriceLevels, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {
            var output = _priceLevelBLL.GetAllPriceLevelsPagedList(inputDto);
            return Ok(output);


        }
    }
}
