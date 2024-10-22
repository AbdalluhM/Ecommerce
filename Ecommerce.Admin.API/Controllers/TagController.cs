using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.API.Attributes;
using Ecommerce.BLL;
using Ecommerce.BLL.Tags;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : BaseAdminController
    {
        #region Fields

        private readonly ITagBLL _TagBLL;
        #endregion
        #region Constructor

        public TagController( ITagBLL TagBLL, IHttpContextAccessor httpContextAccessor ) : base(httpContextAccessor)
        {
            _TagBLL = TagBLL;
        }
        #endregion


        #region Actions 
        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.Tags, ActionsEnum.CREATE)]
        public IActionResult Create( CreateTagInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var output = _TagBLL.Create(inputDto);
            return Ok(output);

        }

        [HttpPost("Update")]
       [DxAuthorize(PagesEnum.Tags, ActionsEnum.UPDATE)]
        public IActionResult Update( UpdateTagInputDto inputDto )
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            var output = _TagBLL.Update(inputDto);
            return Ok(output);

        }

        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.Tags, ActionsEnum.DELETE)]
        public IActionResult Delete( [FromQuery] int id )
        {

            var output = _TagBLL.Delete(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);
        }

        #endregion

        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.Tags, ActionsEnum.READ)]
        public async Task<IActionResult> GetByIdAsync( int id )
        {

            var output = await _TagBLL.GetByIdAsync(new GetTagInputDto { Id = id });
            return Ok(output);

        }

        [HttpGet("GetAll")]
        [DxAuthorize(PagesEnum.Tags, ActionsEnum.READ)]
        public IActionResult GetAll( )
        {
            var output = _TagBLL.GetAllList();
            return Ok(output);
        }

        [HttpGet("GetPagedList")]
        [DxAuthorize(PagesEnum.Tags, ActionsEnum.READ)]
        public IActionResult GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {
            var output = _TagBLL.GetPagedTagList(inputDto);
            return Ok(output);
        }


        /// for test asmaa 
        #region ForTest
        [HttpPost("Create/Asmaa")]
        public IActionResult CreateAsmaa(CreateTagInputDto inputDto)
        {
            inputDto.CreatedBy =  inputDto.CreatedBy;
            var output = _TagBLL.Create(inputDto);
            return Ok(output);

        }
        [HttpPost("Update/Asmaa")]
        public IActionResult UpdateAsmaa(UpdateTagInputDto inputDto)
        {
            inputDto.ModifiedBy =  inputDto.CreatedBy;
            var output = _TagBLL.Update(inputDto);
            return Ok(output);

        }
        #endregion

    }
}
