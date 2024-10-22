using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ecommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.DTO.Modules;
using Ecommerce.BLL.Modules;
using Ecommerce.DTO.Modules.ModuleTags;
using Ecommerce.DTO.Modules.ModuleSlider;
using Microsoft.AspNetCore.Hosting;
using Ecommerce.API.Attributes;
using Ecommerce.Core.Enums.Roles;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : BaseAdminController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ModuleController> _logger;

        private readonly IModuleBLL _moduleBLL;

        public ModuleController( IModuleBLL moduleBLL,
                               IMapper mapper,
                               ILogger<ModuleController> logger,
                               IHttpContextAccessor httpContextAccessor,
                               IOptions<FileStorageSetting> fileOptions, IWebHostEnvironment webHostEnvironment )
            : base(httpContextAccessor, fileOptions,webHostEnvironment)
        {
            _moduleBLL = moduleBLL;
            _logger = logger;
            _mapper = mapper;
        }

        #region Module
        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateAsync( [FromForm] CreateModuleAPIInputDto inputDto)
        {
            var fileDto =  GetFile(inputDto.File, FilePathEnum.Module);
            var input = _mapper.Map<CreateModuleInputDto>(inputDto);
            input.File = fileDto;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _moduleBLL.CreateAsync(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("Update")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateAsync( [FromForm] UpdateModuleAPIInputDto inputDto )
        {

            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.Module) : null;
            var input = _mapper.Map<UpdateModuleInputDto>(inputDto);
            input.File = fileDto;
            input.ModifiedBy = CurrentEmployeeId;
            var result = await _moduleBLL.UpdateAsync(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("Delete")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteAsync( int Id)
        {
            var result = await _moduleBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
        });
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetModuleByIdAsync(int id)
        {
            var output = await _moduleBLL.GetByIdAsync(new GetModuleInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("GetPagedList")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedListAsync( [FromQuery] FilteredResultRequestDto inputDto)
        {
            var output = await _moduleBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }

        /// <summary>
        /// ////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _moduleBLL.GetAllAsync();
            return Ok(result);
        }
        #endregion
        #region Module Tag

        [HttpPost]
        [Route("Tag/Create")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateModuleTagAsync( CreateModuleTagInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = await _moduleBLL.CreateModuleTagAsync(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Update")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateModuleTagAsync( UpdateModuleTagInputDto inputDto )
        {
            var result = await _moduleBLL.UpdateModuleTagAsync(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/AssignFeatured")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.UPDATE)]

        public async Task<IActionResult> AssignFeaturedToModuleTagAsync( int Id )
        {
            var result = await _moduleBLL.AssignFeaturedToModuleTagAsync(Id);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Delete")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteModuleTagAsync( int Id )
        {
            var result = await _moduleBLL.DeleteModuleTagAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/{id}")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetModuleTagByIdAsync( int id )
        {
            var output = await _moduleBLL.GetModuleTagByIdAsync(new GetModuleTagInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("Tag/GetAll")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllModuleTagListByModuleIdAsync( int ModuleId )
        {
            var result = await _moduleBLL.GetAllModuleTagListByModuleIdAsync(ModuleId);

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/GetAllUnAssigned")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]

        public async Task<IActionResult> GetAllActiveTagsNotAssignToModuleIdAsync( int ModuleId )
        {
            var output =  await _moduleBLL.GetAllActiveTagsNotAssignToModuleIdAsync(ModuleId);
            return Ok(output);

        }
        [HttpGet]
        [Route("Tag/GetPagedList")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedListAsync( [FromQuery] ModuleFilteredPagedResult inputDto )
        {
            var output = await _moduleBLL.GetModuleTagPagedListAsync(inputDto);
            return Ok(output);
        }
        #endregion
        #region Module Slider
        [HttpPost("Slider/Create")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateModuleSliderAsync( [FromForm] CreateModuleSliderAPIInputDto inputDto )
        {
            var file = GetFile(inputDto.File, FilePathEnum.ModuleSlider);
            var input = _mapper.Map<CreateModuleSliderInputDto>(inputDto);
            input.File = file;
            input.IsActive = true;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _moduleBLL.CreateModuleSliderAsync(input);
            return Ok(result);
        }

        [HttpGet("Slider/GetPagedList")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllModuleSlidersPagedList( [FromQuery] ModuleFilteredPagedResult pagedDto )
        {
            var result = await _moduleBLL.GetAllModuleSlidersPagedListAsync(pagedDto);

            return Ok(result);
        }

        [HttpPost("Slider/Delete/{id}")]
        [DxAuthorize(PagesEnum.Mosules, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteModuleSliderAsync( int id )
        {
            var result = await _moduleBLL.DeleteModuleSliderAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        #endregion
    }
}

