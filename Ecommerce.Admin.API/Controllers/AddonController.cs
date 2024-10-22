using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Addons;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO;
using Ecommerce.DTO.Addons;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonSliders.Inputs;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddonController : BaseAdminController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddonController> _logger;

        private readonly IAddOnBLL _AddonBLL;

        public AddonController(IAddOnBLL AddonBLL,
                               IMapper mapper,
                               ILogger<AddonController> logger,
                               IHttpContextAccessor httpContextAccessor,
                               IOptions<FileStorageSetting> fileOptions, IWebHostEnvironment webHostEnvironment)
            : base(httpContextAccessor, fileOptions, webHostEnvironment)
        {
            _AddonBLL = AddonBLL;
            _logger = logger;
            _mapper = mapper;
        }

        #region AddOn
        //[HttpPost("Create")]
        //public IActionResult Create([FromForm] NewAddonDto newAddonDto)
        //{ 
        //    var fileDto = GetFile(newAddonDto.File, FilePathEnum.AddonBase);
        //    var result = _AddonBLL.CreateAddon(fileDto, newAddonDto, CurrentEmployeeId);

        //    return Ok(result);
        //}
        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.CREATE)]
        public async Task<IActionResult> Create([FromForm] CreateAddOnAPIInputDto inputDto)
        {
            var fileDto = GetFile(inputDto.File, FilePathEnum.AddonBase);
            var input = _mapper.Map<CreateAddOnInputDto>(inputDto);
            input.File = fileDto;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _AddonBLL.CreateAsync(input);

            return Ok(result);
        }
        [HttpPost]
        [Route("Update")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateAddOn([FromForm] UpdateAddOnAPIInputDto inputDto)
        {

            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.AddonBase) : null;
            var input = _mapper.Map<UpdateAddOnInputDto>(inputDto);
            input.File = fileDto;
            input.ModifiedBy = CurrentEmployeeId;
            var result = await _AddonBLL.UpdateAsync(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("Delete")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteAddOn(int Id)
        {
            var result = await _AddonBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAddOnIdAsync(int id)
        {
            var output = await _AddonBLL.GetByIdAsync(new GetAddOnInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var output = await _AddonBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }

        /// <summary>
        /// ////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _AddonBLL.GetAllAsync();
            return Ok(result);
        }
        #endregion

        #region AddonTag

        [HttpPost]
        [Route("Tag/Create")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.CREATE)]

        public IActionResult AddAddonTag(CreateAddonTagInputDto inputDto)
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = _AddonBLL.AddAddonTag(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Update")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.UPDATE)]
        public IActionResult UpdateAddonTag(UpdateAddonTagDto inputDto)
        {
            var result = _AddonBLL.UpdateAddonTag(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/AssignFeatured")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.UPDATE)]
        public IActionResult AssignFeaturedToAddonTag(int Id)
        {
            var result = _AddonBLL.AssignFeaturedToAddonTag(Id);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Delete")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.DELETE)]
        public IActionResult DeleteAddonTag(int Id)
        {
            var result = _AddonBLL.DeleteAddonTag(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/{id}")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAddOnTagByIdAsync(int id)
        {
            var output = await _AddonBLL.GetAddOnTagByIdAsync(new GetAddOnTagInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("Tag/GetAll")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public IActionResult GetAllAddOnTagListByAddonId(int AddonId)
        {
            var result = _AddonBLL.GetAllAddOnTagListByAddonId(AddonId);

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/GetAllUnAssigned")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public IActionResult GetAllActiveTagsNotAssignToAddonId(int AddonId)
        {
            var output = _AddonBLL.GetAllActiveTagsNotAssignToAddonId(AddonId);
            return Ok(output);

        }
        [HttpGet]
        [Route("Tag/GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList([FromQuery] AddOnFilteredPagedResult inputDto)
        {
            var output = await _AddonBLL.GetAddOnTagPagedListAsync(inputDto);
            return Ok(output);
        }
        #endregion

        #region AddonLabel

        [HttpPost]
        [Route("Label/Create")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.CREATE)]
        public IActionResult AddAddonLabel(CreateAddonLabelInputDto inputDto)
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = _AddonBLL.CreateAddonLabel(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Label/Update")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.UPDATE)]
        public IActionResult UpdateAddonLabel(UpdateAddonLabelInputDto inputDto)
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            var result = _AddonBLL.UpdateAddonLabel(inputDto);

            return Ok(result);
        }
        [HttpPost]
        [Route("Label/Delete")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.DELETE)]
        public IActionResult DeleteAddonLabel(int addonId)
        {
            var result = _AddonBLL.DeleteAddonLabel(new DeleteTrackedEntityInputDto
            {
                Id = addonId,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Label/{addonId}")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAddOnLabelByIdAsync(int addonId)
        {
            var output = await _AddonBLL.GetAddOnLabelByAddOnIdAsync(new GetAddOnLabelInputDto { AddOnId = addonId });
            return Ok(output);
        }
        [HttpGet]
        [Route("Label/GetAll")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public IActionResult GetAllAddOnLabelListByAddonId(int addonId)
        {
            var result = _AddonBLL.GetAllAddOnLabelListByAddonId(new GetAddOnLabelInputDto { AddOnId = addonId });

            return Ok(result);
        }
        [HttpGet]
        [Route("Label/GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetLabelPagedList([FromQuery] AddOnFilteredPagedResult inputDto)
        {
            var output = await _AddonBLL.GetAddOnLabelPagedListAsync(inputDto);
            return Ok(output);


        }
        #endregion

        #region AddonPrice
        //[Route("CreateAddOnPrice")]
        [HttpPost]
        [Route("Price/Create")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateAddOnPrice(CreateAddOnPriceInputDto inputDto)
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var output = await _AddonBLL.CreateAddOnPriceAsync(inputDto);
            return Ok(output);

        }
        //[Route("UpdateAddOnPrice")]
        [HttpPost]
        [Route("Price/Update")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.UPDATE)]

        public async Task<IActionResult> UpdateAddOnPrice(UpdateAddOnPriceInputDto inputDto)
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            var output = await _AddonBLL.UpdateAddOnPriceAsync(inputDto);
            return Ok(output);


        }

        //[Route("DeleteAddOnPrice")]
        [HttpPost]
        [Route("Price/Delete")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteAddOnPrice(int id)
        {
            var output = await _AddonBLL.DeleteAddOnPriceAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);


        }

        [HttpGet]
        [Route("Price/{id}")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAddOnPriceByIdAsync(int id)
        {
            var output = await _AddonBLL.GetAddOnPriceByIdAsync(new GetAddOnPriceInputDto { Id = id });

            return Ok(output);
        }

        [HttpGet]
        [Route("Price/GetAllExisting")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllExistingAddOnPricesAsync([FromQuery] GetAddOnPricesInputDto inputDto)
        {
            var output = await _AddonBLL.GetAllExistingAddOnPricesAsync(inputDto);

            return Ok(output);
        }

        [HttpGet]
        [Route("Price/GetAllMissing")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllMissingAddOnPricesAsync([FromQuery] GetAddOnPricesInputDto inputDto)
        {
            var output = await _AddonBLL.GetAllMissingAddOnPricesAsync(inputDto);

            return Ok(output);
        }

        [HttpGet]
        [Route("Price/Missing/GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllMissingPricesPagedList([FromQuery] AddOnFilteredPagedResult inputDto)
        {
            var output = await _AddonBLL.GetAllMissingAddOnPricesPagedListAsync(inputDto, CurrentEmployeeId);

            return Ok(output);
        }

        [HttpGet]
        [Route("Price/Existing/GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllExistingPricesPagedList([FromQuery] AddOnFilteredPagedResult inputDto)
        {
            var output = await _AddonBLL.GetAllExistingAddOnPricePagedListAsync(inputDto, CurrentEmployeeId);

            return Ok(output);
        }
        #endregion

        #region Addon Sliders.
        [HttpPost("Slider/Create")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.CREATE)]
        public async Task<IActionResult> AddAddonSlider([FromForm] NewAddonSliderDto newAddonSliderDto)
        {
            var file = GetFile(newAddonSliderDto.Image, FilePathEnum.AddonSlider);
            var result = await _AddonBLL.AddAddoneSlider(file, newAddonSliderDto, CurrentEmployeeId);

            return Ok(result);
        }

        [HttpGet("Slider/GetPagedList")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.READ)]
        public IActionResult GetAllAddonSlidersPagedList([FromQuery] AddOnFilteredPagedResult pagedDto)
        {
            var result = _AddonBLL.GetAllAddonSlidersPagedList(pagedDto);

            return Ok(result);
        }

        [HttpPost("Slider/Delete/{id}")]
        [DxAuthorize(PagesEnum.AddOns, ActionsEnum.DELETE)]
        public IActionResult DeleteAddonSlider(int id)
        {
            var result = _AddonBLL.DeleteAddonSlider(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        #endregion
    }
}

