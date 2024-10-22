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
using Ecommerce.DTO.Applications;
using Ecommerce.BLL.Applications;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Applications.ApplicationSlider;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Applications.Features;
using System;
using Ecommerce.BLL.Applications.Modules;
using Ecommerce.BLL.Applications.AddOns;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Applications.VersionModules;
using Ecommerce.DTO.Applications.VersionAddOns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.API.Attributes;
using Ecommerce.Core.Enums.Roles;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : BaseAdminController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationController> _logger;

        private readonly IApplicationBLL _applicationBLL;
        private readonly IVersionBLL _version;
        private readonly IVersionFeatureBLL _featureBLL;
        private readonly IVersionModuleBLL _moduleBLL;
        private readonly IVersionAddOnBLL _addOnBLL;

        public ApplicationController( IApplicationBLL applicationBLL,
                               IVersionBLL versionBLL,
                               IVersionFeatureBLL featureBLL,
                               IVersionModuleBLL moduleBLL,
                               IVersionAddOnBLL addOnBLL,
                               IMapper mapper,
                               ILogger<ApplicationController> logger,
                               IHttpContextAccessor httpContextAccessor,
                               IOptions<FileStorageSetting> fileOptions, IWebHostEnvironment webHostEnvironment )
            : base(httpContextAccessor, fileOptions,webHostEnvironment)
        {
            _applicationBLL = applicationBLL;
            _version = versionBLL;
            _featureBLL = featureBLL;
            _moduleBLL = moduleBLL;
            _addOnBLL = addOnBLL;
            _logger = logger;
            _mapper = mapper;
        }

        #region Application
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync( [FromForm] CreateApplicationAPIInputDto inputDto )
        {
            var fileDto = GetFile(inputDto.File, FilePathEnum.Application);
            var input = _mapper.Map<CreateApplicationInputDto>(inputDto);
            input.File = fileDto;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _applicationBLL.CreateAsync(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync( [FromForm] UpdateApplicationAPIInputDto inputDto )
        {

            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.Application) : null;
            var input = _mapper.Map<UpdateApplicationInputDto>(inputDto);
            input.File = fileDto;
            input.ModifiedBy = CurrentEmployeeId;
            var result = await _applicationBLL.UpdateAsync(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAsync( int Id )
        {
            var result = await _applicationBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetApplicationByIdAsync( int id )
        {
            var output = await _applicationBLL.GetByIdAsync(new GetApplicationInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedListAsync( [FromQuery] FilteredResultRequestDto inputDto )
        {
            var output = await _applicationBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }

        /// <summary>
        /// ////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllAsync( )
        {
            var result = await _applicationBLL.GetAllAsync();
            return Ok(result);
        }
        #endregion

        #region Application Label

        [HttpPost]
        [Route("Label/Create")]

        public async Task<IActionResult> AddApplicationLabel( CreateApplicationLabelInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = await _applicationBLL.CreateApplicationLabelAsync(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Label/Update")]
        public async Task<IActionResult> UpdateApplicationLabel( UpdateApplicationLabelInputDto inputDto )
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            var result = await _applicationBLL.UpdateApplicationLabelAsync(inputDto);

            return Ok(result);
        }
        [HttpPost]
        [Route("Label/Delete")]
        public async Task<IActionResult> DeleteApplicationLabel( int ApplicationId )
        {
            var result = await _applicationBLL.DeleteApplicationLabelAsync(new DeleteTrackedEntityInputDto
            {
                Id = ApplicationId,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Label/{ApplicationId}")]
        public async Task<IActionResult> GetApplicationLabelByIdAsync( int ApplicationId )
        {
            var output = await _applicationBLL.GetApplicationLabelByApplicationIdAsync(new GetApplicationLabelInputDto { ApplicationId = ApplicationId });
            return Ok(output);
        }
        [HttpGet]
        [Route("Label/GetAll")]
        public async Task<IActionResult> GetAllApplicationLabelListByApplicationId( int ApplicationId )
        {
            var result = await _applicationBLL.GetAllApplicationLabelListByApplicationIdAsync(new GetApplicationLabelInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }
        [HttpGet]
        [Route("Label/GetPagedList")]
        public async Task<IActionResult> GetLabelPagedList( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
            var output = await _applicationBLL.GetApplicationLabelPagedListAsync(inputDto);
            return Ok(output);


        }
        #endregion

        #region Application Tag

        [HttpPost]
        [Route("Tag/Create")]

        public async Task<IActionResult> CreateApplicationTagAsync( CreateApplicationTagInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = await _applicationBLL.CreateApplicationTagAsync(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Update")]

        public async Task<IActionResult> UpdateApplicationTagAsync( UpdateApplicationTagInputDto inputDto )
        {
            var result = await _applicationBLL.UpdateApplicationTagAsync(inputDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/AssignFeatured")]

        public async Task<IActionResult> AssignFeaturedToApplicationTagAsync( int Id )
        {
            var result = await _applicationBLL.AssignFeaturedToApplicationTagAsync(Id);

            return Ok(result);
        }

        [HttpPost]
        [Route("Tag/Delete")]
        public async Task<IActionResult> DeleteApplicationTagAsync( int Id )
        {
            var result = await _applicationBLL.DeleteApplicationTagAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/{id}")]
        public async Task<IActionResult> GetApplicationTagByIdAsync( int id )
        {
            var output = await _applicationBLL.GetApplicationTagByIdAsync(new GetApplicationTagInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("Tag/GetAll")]
        public async Task<IActionResult> GetAllApplicationTagListByApplicationIdAsync( int ApplicationId )
        {
            var result = await _applicationBLL.GetAllApplicationTagListByApplicationIdAsync(ApplicationId);

            return Ok(result);
        }
        [HttpGet]
        [Route("Tag/GetAllUnAssigned")]

        public async Task<IActionResult> GetAllActiveTagsNotAssignToApplicationIdAsync( int ApplicationId )
        {
            var output = await _applicationBLL.GetAllActiveTagsNotAssignToApplicationIdAsync(ApplicationId);
            return Ok(output);

        }
        [HttpGet]
        [Route("Tag/GetPagedList")]
        public async Task<IActionResult> GetPagedListAsync( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
            var output = await _applicationBLL.GetApplicationTagPagedListAsync(inputDto);
            return Ok(output);
        }
        #endregion

        #region Application Slider
        [HttpPost("Slider/Create")]
        public async Task<IActionResult> CreateModuleSliderAsync( [FromForm] CreateApplicationSliderAPIInputDto inputDto )
        {
            var file = GetFile(inputDto.File, FilePathEnum.ApplicationSlider);
            var input = _mapper.Map<CreateApplicationSliderInputDto>(inputDto);
            input.File = file;
            input.IsActive = true;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _applicationBLL.CreateApplicationSliderAsync(input);
            return Ok(result);
        }

        [HttpGet("Slider/GetPagedList")]
        public async Task<IActionResult> GetAllApplicationSlidersPagedList( [FromQuery] ApplicationFilteredPagedResult pagedDto )
        {
            var result = await _applicationBLL.GetAllApplicationSlidersPagedListAsync(pagedDto);

            return Ok(result);
        }

        [HttpPost("Slider/Delete/{id}")]
        public async Task<IActionResult> DeleteApplicationSliderAsync( int id )
        {
            var result = await _applicationBLL.DeleteApplicationSliderAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        #endregion

        #region Application Version

        [HttpPost]
        [Route("Version/Create")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateApplicationVersionAsync( [FromForm] CreateVersionAPIInputDto inputDto )
        {
            var file = GetFile(inputDto.File, FilePathEnum.ApplicationVersion);
            var input = _mapper.Map<CreateVersionInputDto>(inputDto);
            input.File = file;
            input.CreatedBy = CurrentEmployeeId;
            var result = await _version.CreateAsync(input);
            return Ok(result);
        }

        [HttpPost]
        [Route("Version/Update")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateApplicationVersionAsync( [FromForm] UpdateVersionAPIInputDto inputDto )
        {
            var file = GetFile(inputDto.File, FilePathEnum.ApplicationVersion);
            var input = _mapper.Map<UpdateVersionInputDto>(inputDto);
            if (inputDto.File != null)
            {
                input.File = file;
            }
            else
            {
                input.File = null;
            }
            input.ModifiedBy = CurrentEmployeeId;
            var result = await _version.UpdateAsync(input);

            return Ok(result);
        }



        [HttpPost]
        [Route("Version/Delete")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteApplicationVersionAsync( int Id )
        {
            var result = await _version.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = Id,
                ModifiedBy = CurrentEmployeeId
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Version/{id}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetApplicationVersionById( int id )
        {
            var output = await _version.GetByIdAsync(new GetVersionInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("Version/GetAll")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllApplicationVersionListByApplicationIdAsync( int? ApplicationId = null )
        {
            var result = await _version.GetAllAsync(ApplicationId);

            return Ok(result);
        }

        [HttpGet]
        [Route("Version/GetPagedList")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetApplicationVersionsPagedListAsync( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
            var output = await _version.GetPagedListAsync(inputDto);
            return Ok(output);
        }
        #endregion

        #region Application Version Price
        [HttpPost]
        [Route("Price/Create")]
        public async Task<IActionResult> CreateVersionPrice( CreateVersionPriceInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var output = await _version.CreateVersionPriceAsync(inputDto);
            return Ok(output);

        }
        [HttpPost]
        [Route("Price/Update")]

        public async Task<IActionResult> UpdateVersionPrice( UpdateVersionPriceInputDto inputDto )
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            var output = await _version.UpdateVersionPriceAsync(inputDto);
            return Ok(output);


        }

        //[Route("DeleteVersionPrice")]
        [HttpPost]
        [Route("Price/Delete")]
        public async Task<IActionResult> DeleteVersionPrice( int id )
        {
            var output = await _version.DeleteVersionPriceAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);


        }

        [HttpGet]
        [Route("Price/{id}")]

        public async Task<IActionResult> GetVersionPriceByIdAsync( int id )
        {
            var output = await _version.GetVersionPriceByIdAsync(new GetVersionPriceInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet]
        [Route("Price/GetAllExisting")]
        public async Task<IActionResult> GetAllExistingVersionPricesAsync( [FromQuery] GetApplicationVersionPricesInputDto inputDto )
        {


            var output = await _version.GetAllExistingVersionPricesAsync(inputDto);
            return Ok(output);


        }
       
        [HttpGet]
        [Route("Price/GetAllMissing")]
        public async Task<IActionResult> GetAllMissingVersionPricesAsync( [FromQuery] GetApplicationVersionPricesInputDto inputDto )
        {
            var output = await _version.GetAllMissingVersionPricesAsync(inputDto);
            
            return Ok(output);
        }
        
        [HttpGet]
        [Route("Price/Missing/GetPagedList")]
        public async Task<IActionResult> GetAllMissingPricesPagedList([FromQuery] ApplicationFilteredPagedResult inputDto)
        {
            var output = await _version.GetAllMissingVersionPricesPagedListAsync(inputDto, CurrentEmployeeId);
         
            return Ok(output);
        }

        [HttpGet]
        [Route("Price/Existing/GetPagedList")]
        public async Task<IActionResult> GetAllExistingPricesPagedList([FromQuery] ApplicationFilteredPagedResult inputDto)
        {
            var output = await _version.GetAllExistingVersionPricePagedListAsync(inputDto, CurrentEmployeeId);

            return Ok(output);
        }

        #endregion

        #region Application Version Feature
        [HttpPost("Feature/Versions/Create")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateFeatureVersionsAsync(  CreateVersionFeatureAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<CreateVersionFeatureInputDto>(inputDto);
                input.CreatedBy = CurrentEmployeeId;//       CurrentEmployeeId;
                input.Versions.ForEach(x => x.CreatedBy = input.CreatedBy);
                var result = await _featureBLL.CreateAsync(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost("Feature/Versions/Update")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateFeatureVersionsAsync( UpdateVersionFeatureAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<UpdateVersionFeatureInputDto>(inputDto);
                input.ModifiedBy = CurrentEmployeeId;//CurrentEmployeeId;
                #region comment
                //input.Versions.ForEach(x => x.CreatedBy =  input.ModifiedBy.GetValueOrDefault(0));
                //input.Versions.ForEach(x => x.ModifiedBy = input.ModifiedBy);
                #endregion
                var result = await _featureBLL.UpdateAysnc(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        [Route("Feature/Versions/Delete")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteVersionFeature( int ApplicationId,int FeatureId )
        {
            var result = await _featureBLL.DeleteAsync(new DeleteApplicatinFeatureInputDto
            {
                ApplicationId = ApplicationId,
                FeatureId = FeatureId,
                ModifiedBy = CurrentEmployeeId,
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Feature/Versions/{ApplicationId}/{FeatureId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionFeatureByIdAsync( int ApplicationId, int FeatureId )
        {
            var output = await _featureBLL.GetByIdAsync(new GetVersionFeatureInputDto {
                ApplicationId = ApplicationId,FeatureId = FeatureId });
            return Ok(output);
        }
        [HttpGet]
        [Route("Feature/Versions/GetAll/{ApplicationId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllVersionFeaturesListByApplicationId( int ApplicationId)
        {
            var result = await _featureBLL.GetAllAsync(new GetAllVersionFeatureInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }
        [HttpGet]
        [Route("Feature/GetAllUnAsigned")]
        public async Task<IActionResult> GetAllFeatureUnAsigned(int ApplicationId)
        {
            var result = await _featureBLL.GetAllUnAsignedFeatureAsync(new GetApplicationByIdInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }

        [HttpGet]
        [Route("Feature/Versions/GetPagedList")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionFeaturePagedList( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
                var output = await _featureBLL.GetPagedListAsync(inputDto);
                return Ok(output);
        }
        #endregion

        #region Application Version Module
        [HttpPost("Module/Versions/Create")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateModuleVersionsAsync( CreateVersionModuleAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<CreateVersionModuleInputDto>(inputDto);
                input.CreatedBy = CurrentEmployeeId;//       CurrentEmployeeId;
                input.Versions.ForEach(x => x.CreatedBy = input.CreatedBy);
                var result = await _moduleBLL.CreateAsync(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost("Module/Versions/Update")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateModuleVersionsAsync( UpdateVersionModuleAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<UpdateVersionModuleInputDto>(inputDto);
                input.ModifiedBy = CurrentEmployeeId;//       CurrentEmployeeId;
                #region comment
                //input.Versions.ForEach(x => x.CreatedBy = input.ModifiedBy.GetValueOrDefault(0));
                //input.Versions.ForEach(x => x.ModifiedBy = input.ModifiedBy);
                #endregion
                var result = await _moduleBLL.UpdateAysnc(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        [Route("Module/Versions/Delete")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteVersionModule( int ApplicationId, int ModuleId )
        {
            var result = await _moduleBLL.DeleteAsync(new DeleteApplicatinModuleInputDto
            {
                ApplicationId = ApplicationId,
                ModuleId = ModuleId,
                ModifiedBy = CurrentEmployeeId,
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("Module/Versions/{ApplicationId}/{ModuleId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionModuleByIdAsync( int ApplicationId, int ModuleId )
        {
            var output = await _moduleBLL.GetByIdAsync(new GetVersionModuleInputDto
            {
                ApplicationId = ApplicationId,
                ModuleId = ModuleId
            });
            return Ok(output);
        }
        [HttpGet]
        [Route("Module/Versions/GetAll/{ApplicationId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllVersionModulesListByApplicationId( int ApplicationId )
        {
            var result = await _moduleBLL.GetAllAsync(new GetAllVersionModuleInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }

        [HttpGet]
        [Route("Module/Versions/GetPagedList")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionModulePagedList( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
            var output = await _moduleBLL.GetPagedListAsync(inputDto);
            return Ok(output);


        }
        [HttpGet]
        [Route("Module/GetAllUnAsigned")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllModuleUnAsigned(int ApplicationId)
        {
            var result = await _moduleBLL.GetAllUnAsignedModuleAsync(new GetApplicationByIdInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }
        #endregion

        #region Application Version AddOn
        [HttpPost("AddOn/Versions/Create")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateAddOnVersionsAsync( CreateVersionAddOnAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<CreateVersionAddOnInputDto>(inputDto);
                input.CreatedBy = CurrentEmployeeId;//       CurrentEmployeeId;
                input.Versions.ForEach(x => x.CreatedBy = input.CreatedBy);
                var result = await _addOnBLL.CreateAsync(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost("AddOn/Versions/Update")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateAddOnVersionsAsync( UpdateVersionAddOnAPIInputDto inputDto )
        {
            try
            {
                var input = _mapper.Map<UpdateVersionAddOnInputDto>(inputDto);
                input.ModifiedBy = CurrentEmployeeId;//       CurrentEmployeeId;
                #region comment
                //input.Versions.ForEach(x => x.CreatedBy = input.ModifiedBy.GetValueOrDefault(0));
                //input.Versions.ForEach(x => x.ModifiedBy = input.ModifiedBy);

                //input.Versions.ForEach(x => {
                //    if(x.id)
                //    x.CreatedBy = input.ModifiedBy.GetValueOrDefault(0);
                //    x.ModifiedBy = l
                //    });
                #endregion
                var result = await _addOnBLL.UpdateAysnc(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        [Route("AddOn/Versions/Delete")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteVersionAddOn( int ApplicationId, int AddOnId )
        {
            var result = await _addOnBLL.DeleteAsync(new DeleteApplicatinAddOnInputDto
            {
                ApplicationId = ApplicationId,
                AddOnId = AddOnId,
                ModifiedBy = CurrentEmployeeId,
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("AddOn/Versions/{ApplicationId}/{AddOnId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionAddOnByIdAsync( int ApplicationId, int AddOnId )
        {
            var output = await _addOnBLL.GetByIdAsync(new GetVersionAddOnInputDto
            {
                ApplicationId = ApplicationId,
                AddOnId = AddOnId
            });
            return Ok(output);
        }
        [HttpGet]
        [Route("AddOn/Versions/GetAll/{ApplicationId}")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllVersionAddOnsListByApplicationId( int ApplicationId )
        {
            var result = await _addOnBLL.GetAllAsync(new GetApplicationByIdInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }

        [HttpGet]
        [Route("AddOn/Versions/GetPagedList")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionAddOnPagedList( [FromQuery] ApplicationFilteredPagedResult inputDto )
        {
            var output = await _addOnBLL.GetPagedListAsync(inputDto);
            return Ok(output);


        }
        [HttpGet]
        [Route("AddOn/GetAllUnAsigned")]
        [DxAuthorize(PagesEnum.Versions, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllUnAsignedAddOn( int ApplicationId)
        {
            var result = await _addOnBLL.GetAllUnAsignedAddOnAsync(new GetApplicationByIdInputDto { ApplicationId = ApplicationId });

            return Ok(result);
        }
        #endregion

        #region DevicesType
        [HttpGet]
        [Route("GetAllDevicesTypeLookupAsync")]
        public async Task<IActionResult> GetAllDevicesTypeLookupAsync()
        {
            try
            {
                var result = await _applicationBLL.GetAllDevicesTypeAsync();
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e);
            }
            
        }
        #endregion
    }
}

