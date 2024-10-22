using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Features;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;

using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : BaseAdminController
    {
        private readonly IFeatureBLL _featureBLL;
        private readonly ILogger<FeatureController> _logger;
        private readonly IMapper _mapper;
        public FeatureController( IFeatureBLL featureBLL, IOptions<FileStorageSetting> fileOptions, IMapper mapper, ILogger<FeatureController> logger, IHttpContextAccessor httpContextAccessor , IWebHostEnvironment webHostEnvironment ) : base(httpContextAccessor, fileOptions, webHostEnvironment)
        {
            _featureBLL = featureBLL;
            _logger = logger;
            _mapper = mapper;

        }

        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.CREATE)]
        public async Task<IActionResult> Create( [FromForm] CreateFeatureAPIInputDto inputDto )
        {
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.Feature) : null;
            var input = _mapper.Map<CreateFeatureInputDto>(inputDto);
            input.File = fileDto;
            input.CreatedBy = CurrentEmployeeId;
            var output = await _featureBLL.CreateAsync(input);
            return Ok(output);

        }


        [HttpPost("Update")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.UPDATE)]
        public async Task<IActionResult> Update( [FromForm] UpdateFeatureAPIInputDto inputDto )
        {
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.Feature) : null;
            var input = _mapper.Map<UpdateFeatureInputDto>(inputDto);
            input.File = fileDto;
            input.ModifiedBy =  CurrentEmployeeId;
            var output = await _featureBLL.UpdateAsync(input);
            return Ok(output);
        }

        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.DELETE)]
        public async Task<IActionResult> Delete( int id )
        {
            var output = await _featureBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);

        }
        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.READ)]
        public async Task<IActionResult> GetByIdAsync( int id )
        {

            var output = await _featureBLL.GetByIdAsync(new GetFeatureInputDto { Id = id });
            return Ok(output);

        }
        [HttpGet("GetAll")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.READ)]
        public async Task<IActionResult> GetAll( )
        {
            var result = await _featureBLL.GetAllAsync();

            return Ok(result);
        }


        [HttpGet("GetPagedList")]
        [DxAuthorize(PagesEnum.Features, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {

            var output = await _featureBLL.GetPagedListAsync(inputDto);
            return Ok(output);


        }
    }
}
