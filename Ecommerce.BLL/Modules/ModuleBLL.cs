using AutoMapper;
using FluentValidation;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.Modules;
using Ecommerce.BLL.Validation.Moduless;
using Ecommerce.BLL.Validation.Moduless.ModuleTags;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Modules;
using Ecommerce.DTO.Modules.ModuleBase.Outputs;
using Ecommerce.DTO.Modules.ModuleSlider;
using Ecommerce.DTO.Modules.ModuleTags;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Modules
{
    public class ModuleBLL : BaseBLL, IModuleBLL
    {
        #region Fields

        IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ModuleTag> _moduleTagRepository;
        private readonly IRepository<ModuleSlider> _moduleSliderRepository;

        private readonly IFileBLL _fileBLL;
        private readonly IRepository<CountryCurrency> _countryCurrencyRepository;

        #endregion

        #region Constructor

        public ModuleBLL(IMapper mapper,
                        IUnitOfWork unitOfWork,
                        IRepository<Module> moduleRepository,
                        IRepository<Tag> tagRepository,
                        IRepository<ModuleTag> moduleTagRepository,
                        IRepository<ModuleSlider> moduleSliderRepository,
                        IFileBLL fileBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _moduleRepository = moduleRepository;
            _tagRepository = tagRepository;
            _moduleTagRepository = moduleTagRepository;
            _moduleTagRepository = moduleTagRepository;
            _moduleSliderRepository = moduleSliderRepository;
            _fileBLL = fileBLL;
        }
        #endregion

        #region Module
        public async Task<IResponse<GetModuleOutputDto>> CreateAsync(CreateModuleInputDto inputDto)
        {
            var output = new Response<GetModuleOutputDto>();

            try
            {
                //Inputs Validation
                var validator = new CreateModuleInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation.
                //check on (name and title)
                if (IsModuleNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Name));
                if (IsModuleNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Name));
                if (IsModuleTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Title));
                if (IsModuleTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Title));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                // upload image.
                var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                if (createdFileResult.Errors.Any())
                {
                    return output.AppendErrors(createdFileResult.Errors).CreateResponse();
                }

                var mappedInput = _mapper.Map<Module>(inputDto);

                mappedInput.Image = createdFileResult.Data;
                _moduleRepository.Add(mappedInput);

                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetModuleOutputDto>(mappedInput));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetModuleOutputDto>> UpdateAsync(UpdateModuleInputDto inputDto)
        {
            var output = new Response<GetModuleOutputDto>();

            try
            {

                //TODO:Try to Refactor this
                FileStorage file = null;

                //////////////////////////////
                if (inputDto.File != null)
                {
                    // upload image
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.ModifiedBy.GetValueOrDefault(0));

                    if (createdFileResult.Errors.Any())
                    {
                        return output.AppendErrors(createdFileResult.Errors).CreateResponse();

                    }
                    file = createdFileResult.Data;
                }
                ////////////////////////////////
                //Input Validation
                var validator = new UpdateModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations

                // 1- Check if Already Exists  (Name)
                if (IsModuleNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Module.Name));
                if (IsModuleNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Module.Name));
                // 2- Check if Already Exists  (Title)
                if (IsModuleTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Module.Title));
                if (IsModuleTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Module.Title));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Module));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //update Module
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Image = file;
                entity = _moduleRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetModuleOutputDto>(entity));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var entity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                // 1- Check if Entity has references
                var checkDto = EntityHasReferences(inputDto.Id, _moduleRepository);
                if (checkDto.HasChildren == 0)
                {
                    //soft delete entity
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _unitOfWork.Commit();
                    return await Task.FromResult(output.CreateResponse(true));
                }

                else
                {
                    //reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }


        }
        /// <summary>
        /// Get  List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<List<GetModuleOutputDto>>> GetAllAsync()
        {
            var output = new Response<List<GetModuleOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetModuleOutputDto>>(_moduleRepository.GetAllList());

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetModuleOutputDto>> GetByIdAsync(GetModuleInputDto inputDto)
        {
            var output = new Response<GetModuleOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetModuleOutputDto>(entity);

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///// <summary>
        ///// Get Paged List of only Active and not Deleted records
        ///// </summary>
        ///// <param name="pagedDto"></param>
        ///// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetModuleOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetModuleOutputDto>>();

            var result = GetPagedList<GetModuleOutputDto, Module, int>(
                pagedDto: pagedDto,
                repository: _moduleRepository,
                orderExpression: x => x.Id,
                searchExpression: x => string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.Name.Contains(pagedDto.SearchTerm)
                      || x.Title.Contains(pagedDto.SearchTerm)
                      || x.MainPageUrl.Contains(pagedDto.SearchTerm)
                      || x.ShortDescription.Contains(pagedDto.SearchTerm))),
                  sortDirection: pagedDto.SortingDirection,
                  disableFilter: true);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<AppModulePopupDto>> GetAppModulePopupAsync(int moduleId)
        {
            var response = new Response<AppModulePopupDto>();

            var module = await _moduleRepository.GetByIdAsync(moduleId);

            if (module is null)
            {
                response.CreateResponse(MessageCodes.NotFound);
                return response;
            }

            var moduleDto = _mapper.Map<AppModulePopupDto>(module);

            return response.CreateResponse(moduleDto);
        }

        public async Task<IResponse<ModuleDetailsDto>> GetModuleDetailsAsync(int moduleId)
        {
            var response = new Response<ModuleDetailsDto>();

            var module = await _moduleRepository.GetByIdAsync(moduleId);

            if (module is null)
            {
                response.CreateResponse(MessageCodes.NotFound);
                return response;
            }

            var moduleDto = _mapper.Map<ModuleDetailsDto>(module);

            var versions = module.VersionModules
                                 .Where(v => v.Version?.Application != null)
                                 .Select(v => v.Version).ToList();

            foreach (var version in versions)
            {
                var versionTitle = JsonConvert.DeserializeObject<JsonLanguageModel>(version.Title);
                var appTitle = JsonConvert.DeserializeObject<JsonLanguageModel>(version.Application.Title);

                var concatenatedAppVersionTitle = new JsonLanguageModel
                {
                    Default = $"{appTitle.Default} {versionTitle.Default}",
                    Ar = $"{appTitle.Ar} {versionTitle.Ar}"
                };

                var title = JsonConvert.SerializeObject(concatenatedAppVersionTitle);

                var logoDto = _mapper.Map<FileStorageDto>(version.Image);

                moduleDto.AvailableAppVersions.Add(new AppVersionDto
                {
                    Logo = logoDto,
                    Title = title
                });
            }

            return response.CreateResponse(moduleDto);
        }
        #endregion

        #region Module Tags
        public async Task<IResponse<GetModuleTagOutputDto>> CreateModuleTagAsync(CreateModuleTagInputDto inputDto)
        {
            var output = new Response<GetModuleTagOutputDto>();

            try
            {

                //Input Validation Validations
                var validator = new CreateModuleTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-check entity already exists
                if (IsModuleTagExsistedBefore(inputDto))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(ModuleTag));
                //Business
                var mappedInput = _mapper.Map<ModuleTag>(inputDto);

                //check if this is the first record => set Featured = true;
                var firstRecord = !(_moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == inputDto.ModuleId).Any());
                if (firstRecord)
                    mappedInput.IsFeatured = true;

                _moduleTagRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetModuleTagOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public async Task<IResponse<bool>> AssignFeaturedToModuleTagAsync(int Id)
        {
            var output = new Response<bool>();

            try
            {
                //Business
                var entity = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == Id);
                // Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ModuleTag));

                //update All other ModuleTags to not featured 
                UpdateOldFeaturedModuleTagToNotFeatured(entity.ModuleId);

                entity.IsFeatured = true;
                //update ModuleTag 
                entity = _moduleTagRepository.Update(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<List<GetTagOutputDto>>> GetAllActiveTagsNotAssignToModuleIdAsync(int ModuleId)
        {
            var output = new Response<List<GetTagOutputDto>>();
            try
            {
                //Business
                var moduleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == ModuleId);
                // 1- Check if Entity Exists
                if (moduleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                var moduleTags = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == ModuleId).Select(f => f.TagId).ToList();
                var result = _mapper.Map<List<GetTagOutputDto>>(_tagRepository.Where(d => !moduleTags.Contains(d.Id)));
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetModuleTagOutputDto>> UpdateModuleTagAsync(UpdateModuleTagInputDto inputDto)
        {
            var output = new Response<GetModuleTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateModuleTagDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Tag Assigned to this Module Before
                if (IsModuleTagExsistedBefore(inputDto, inputDto.Id))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(ModuleTag));

                //Business
                var entity = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ModuleTag));
                //3-check if try to update IsFeatured to Not IsFeatured
                if (entity.IsFeatured && !inputDto.IsFeatured)
                {
                    //assign IsFeatured to the first entity
                    var FirstEntity = GetFirstModuleTag(entity.ModuleId);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                    //reject to update
                    else
                        return output.CreateResponse(MessageCodes.BusinessValidationError);
                }
                //Update Entity
                entity = _mapper.Map(inputDto, entity);
                _moduleTagRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetModuleTagOutputDto>(entity));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<bool>> DeleteModuleTagAsync(DeleteEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business
                var entity = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                //1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ModuleTag));

                //2-check if deleted Entity is Featured => Assign IsFeatured to The first Entity 
                if (entity.IsFeatured)
                {
                    var FirstEntity = GetFirstModuleTag(entity.ModuleId, entity.Id);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                }

                //3-check if entity has related data
                var checkDto = EntityHasReferences(entity.Id, _moduleTagRepository);
                if (checkDto.HasChildren == 0)
                {
                    //Hard Delete Entity
                    _moduleTagRepository.Delete(entity);
                    _unitOfWork.Commit();
                    return output.CreateResponse(true);
                }
                else
                {
                    //reject delete if entity has references in any other tables
                    return output.CreateResponse(MessageCodes.RelatedDataExist);

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetModuleTagOutputDto>> GetModuleTagByIdAsync(GetModuleTagInputDto inputDto)
        {
            var output = new Response<GetModuleTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetModuleTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetModuleTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<List<GetModuleTagOutputDto>>> GetAllModuleTagListByModuleIdAsync(int ModuleId)
        {
            var output = new Response<List<GetModuleTagOutputDto>>();
            try
            {
                //Business
                var moduleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == ModuleId);
                // 1- Check if Entity Exists
                if (moduleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                var result = _mapper.Map<List<GetModuleTagOutputDto>>(_moduleTagRepository.Where(C => C.ModuleId == ModuleId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<PagedResultDto<GetModuleTagOutputDto>>> GetModuleTagPagedListAsync(ModuleFilteredPagedResult inputDto)
        {
            var output = new Response<PagedResultDto<GetModuleTagOutputDto>>();

            var result = GetPagedList<GetModuleTagOutputDto, ModuleTag, int>(
                pagedDto: inputDto,
                repository: _moduleTagRepository,
                orderExpression: x => x.Id,
                sortDirection: inputDto.SortingDirection,
                searchExpression: x => x.ModuleId == inputDto.ModuleId && (string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                    || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm) && (x.Tag.Name.Contains(inputDto.SearchTerm)))),
                  disableFilter: true
                  );
            return output.CreateResponse(result);

        }



        #endregion

        #region Module Sliders
        public async Task<IResponse<GetModuleSliderOutputDto>> CreateModuleSliderAsync(CreateModuleSliderInputDto inputDto)
        {
            var result = new Response<GetModuleSliderOutputDto>();

            try
            {

                //Input Validation
                var validator = new CreateModuleSliderInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return result.CreateResponse(validator.Errors);
                }

                var fileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                if (!fileResult.IsSuccess)
                {
                    result.AppendErrors(fileResult.Errors);

                    return result.CreateResponse();
                }

                var mappedInput = _mapper.Map<ModuleSlider>(inputDto);

                mappedInput.Media = fileResult.Data;

                _moduleSliderRepository.Add(mappedInput);

                _unitOfWork.Commit();

                return result.CreateResponse(_mapper.Map<GetModuleSliderOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return result.CreateResponse(ex);
            }

        }

        public async Task<IResponse<PagedResultDto<GetModuleSliderOutputDto>>> GetAllModuleSlidersPagedListAsync(ModuleFilteredPagedResult pagedDto)
        {
            var result = new Response<PagedResultDto<GetModuleSliderOutputDto>>();

            try
            {
                var output = GetPagedList<GetModuleSliderOutputDto, ModuleSlider, int>(pagedDto: pagedDto,
                                                                                        repository: _moduleSliderRepository,
                                                                                        orderExpression: s => s.Id,
                                                                                        searchExpression: s => s.ModuleId == pagedDto.ModuleId,
                                                                                        sortDirection: pagedDto.SortingDirection,
                                                                                        disableFilter: true);
                return result.CreateResponse(output);
            }
            catch (Exception ex)
            {
                return result.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> DeleteModuleSliderAsync(DeleteTrackedEntityInputDto inputDto)
        {
            var result = new Response<bool>();

            try
            {

                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return result.CreateResponse(validator.Errors);
                }

                var sliderModel = _moduleSliderRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(s => s.Id == inputDto.Id);

                if (sliderModel is null)
                    return result.CreateResponse(MessageCodes.NotFound, nameof(ModuleSlider));

                //check if entity has related data
                var checkDto = EntityHasReferences(sliderModel.Id, _moduleSliderRepository);
                if (checkDto.HasChildren == 0)
                {
                    sliderModel.IsDeleted = true;
                    sliderModel.ModifiedBy = inputDto.ModifiedBy;
                    sliderModel.ModifiedDate = inputDto.ModifiedDate;
                    _moduleSliderRepository.Update(sliderModel);
                    _unitOfWork.Commit();
                    return result.CreateResponse(true);
                }
                else
                {
                    //reject delete if entity has references in any other tables
                    return result.CreateResponse(MessageCodes.RelatedDataExist);

                }
            }
            catch (Exception ex)
            {
                result.CreateResponse(ex);
            }

            return result.CreateResponse(true);
        }
        #endregion

        #region Helpers

        #region Module

        private bool IsModuleNameExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? ModuleId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                     ? _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim())
                                    .WhereIf(a => a.Id != ModuleId, ModuleId.HasValue).Any()
                     : _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim())
                                    .WhereIf(a => a.Id != ModuleId, ModuleId.HasValue).Any();
        }
        private bool IsModuleTitleExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? ModuleId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
            ? _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.default") == name.Default.Trim())
                           .WhereIf(a => a.Id != ModuleId, ModuleId.HasValue).Any()
            : _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.ar") == name.Ar.Trim())
                           .WhereIf(a => a.Id != ModuleId, ModuleId.HasValue).Any();
        }


        #endregion

        #region Module Tag
        public bool IsModuleTagExsistedBefore(ModuleTagDto dto, int? Id = null)
        {
            return _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != Id, Id.HasValue).Any(s => s.ModuleId == dto.ModuleId && s.TagId == dto.TagId);
        }
        private List<ModuleTag> UpdateOldFeaturedModuleTagToNotFeatured(int moduleId)
        {
            var oldFeatured = _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == moduleId && x.IsFeatured).ToList();
            oldFeatured.ForEach(c =>
            {
                c.IsFeatured = false;
                _moduleTagRepository.Update(c);

            });
            return oldFeatured;
        }

        private ModuleTag GetFirstModuleTag(int moduleId, int? moduleTagId = null)
        {
            return _moduleTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != moduleTagId, moduleTagId.HasValue).Where(f => f.ModuleId == moduleId).OrderBy(f => f.Id).FirstOrDefault();
        }
        #endregion

        #endregion
    }
}
