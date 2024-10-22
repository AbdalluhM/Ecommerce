using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.WorkSpaces;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.WorkSpaces;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.WorkSpaces
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkSpacesController : BaseCustomerController
    {
        private readonly IWorkSpaceBLL _workSpaceBLL;
        private readonly FileStorageSetting _fileStorageSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public WorkSpacesController(IHttpContextAccessor httpContextAccessor,
                                    IWorkSpaceBLL workSpaceBLL,
                                    IOptions<FileStorageSetting> fileStorageSetting,
                                    IWebHostEnvironment webHostEnvironment,
                                    IMapper mapper)
            : base(fileStorageSetting, webHostEnvironment, httpContextAccessor)
        {
            _workSpaceBLL = workSpaceBLL;
            _fileStorageSetting = fileStorageSetting.Value;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet("GetDexefCurrency")]
        public async Task<IActionResult> GetDexefCurrency()
        {
            var result = await _workSpaceBLL.GetDexefCurrency();
            return Ok(result);
        }

        [HttpGet("GetDexefCountry")]
        public async Task<IActionResult> GetDexefCountry()
        {
            var result = await _workSpaceBLL.GetDexefCountry();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetWorkspaces")]
        public async Task<IActionResult> GetWorkspaceAsync()
        {
            var result = await _workSpaceBLL.GetWorkSpacesAsync(CurrentUserId);

            return Ok(result);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpGet("GetWorkspaceDetails")]
        public async Task<IActionResult> GetWorkspaceDetailsAsync(int id)
        {
            var result = await _workSpaceBLL.GetWorkspaceDetailsAsync(id);

            return Ok(result);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreatWorkSpaceApiDto inputDto)
        {
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.DexefWorkSpace) : null;
            var mapped = _mapper.Map<CreateWorkSpaceDto>(inputDto);
            mapped.FileDto = fileDto;
            mapped.CustomerId = CurrentUserId;
            var result = await _workSpaceBLL.CreateAsync(mapped);
            return Ok(result);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateWorkSpaceApiDto inputDto)
        {
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.DexefWorkSpace) : null;
            var mapped = _mapper.Map<UpdateWorkSpaceDto>(inputDto);
            mapped.FileDto = fileDto;
            mapped.CustomerId = CurrentUserId;
            var result = await _workSpaceBLL.UpdateAsync(mapped);
            return Ok(result);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("ExtendWorkSpace")]
        public async Task<IActionResult> ExtendWorkSpace(int workSpaceId)
        {
            var result = await _workSpaceBLL.ExtendWorkSpaceAsync(workSpaceId);
            return Ok(result);
        }



        [Authorize]
        [HttpGet("GetAllSimpleDatabase")]
        public async Task<IActionResult> GetSimpleDatabaseAsync()
        {
            var result = await _workSpaceBLL.GetSimpleDatabaseAsync();

            return Ok(result);
        }


        [Authorize]
        [HttpPost("CreateSimpleDatabase")]
        public async Task<IActionResult> CreateSimpleDatabaseAsync(SimpleDatabaseDto inputDto)
        {
            var result = await _workSpaceBLL.CreateSimpleDatabaseAsync(inputDto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("SimpleDatabase/{id}")]
        public async Task<IActionResult> GetSimpleDatabaseByIdAsync(int id)
        {
            var result = await _workSpaceBLL.GetSimpleDatabaseByIdAsync(id);

            return Ok(result);
        }
        [Authorize]
        [HttpPost("DeleteSimpleDatabase")]
        public async Task<IActionResult> DeleteSimpleDatabase(int id)
        {
            var result = await _workSpaceBLL.DeleteSimpleDatabase(id);

            return Ok(result);
        }
        [Authorize]
        [HttpPost("UpdateSimpleDatabase")]
        public async Task<IActionResult> UpdateSimpleDatabase(SimpleDatabaseDto inputDto)
        {
            var result = await _workSpaceBLL.UpdateSimpleDatabase(inputDto);

            return Ok(result);
        }
    }
}
