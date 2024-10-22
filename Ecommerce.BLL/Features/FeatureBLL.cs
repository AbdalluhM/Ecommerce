using AutoMapper;

using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.Features;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Features
{
    public class FeatureBLL : BaseBLL, IFeatureBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        private readonly IFileBLL _fileBLL;
        private readonly IRepository<Feature> _featureRepository;
        //private readonly IStringLocalizer<FeatureBLL> _stringLocalizer;
        #endregion

        #region Constructor

        public FeatureBLL( IMapper mapper, IUnitOfWork unitOfWork,
                        IFileBLL fileBLL, IRepository<Feature> featureRepository /*, IStringLocalizer<FeatureBLL> stringLocalizer */) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileBLL = fileBLL;
            _featureRepository = featureRepository;
            //_stringLocalizer = stringLocalizer;
        }


        #endregion
        #region Business

        public async Task<IResponse<GetFeatureOutputDto>> CreateAsync( CreateFeatureInputDto inputDto/*, FileDto fileDto*/ )
        {
            var output = new Response<GetFeatureOutputDto>();
            try
            {
                FileStorage file = null;
                //////////////////////////////

                if (inputDto.File != null)
                {

                    // upload image.
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                    if (createdFileResult.Errors.Any())
                    {
                        //return output.AppendError(createdFileResult.Errors);

                    }

                    file = createdFileResult.Data;
                }

                ////////////////////////////////
                //Input Validation Validations
                var validator = new CreateFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                //1-check entity already exists Name
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Feature.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr,  nameof(Feature.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _mapper.Map<Feature>(inputDto);
                entity.Logo = file;//     createdFileResult.Data;
                await _featureRepository.AddAsync(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetFeatureOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);


            }
        }
        public async Task<IResponse<GetFeatureOutputDto>> UpdateAsync( UpdateFeatureInputDto inputDto/*, FileDto fileDto*/ )
        {
            var output = new Response<GetFeatureOutputDto>();

            try
            {
               // return output.CreateResponse(MessageCodes.Success);
                //TODO:Try to Refactor this
                FileStorage file = null;
                //////////////////////////////

                if (inputDto.File != null)
                {
                   
                    // upload image.
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.ModifiedBy.GetValueOrDefault(0));

                    if (createdFileResult.Errors.Any())
                    {
                        //return output.AppendError(createdFileResult.Errors);

                    }
                    file = createdFileResult.Data;

                }
                ////////////////////////////////
                //Input Validation
                var validator = new UpdateFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.Id))
                 output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));


                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Logo = file;
                //update 
                entity = _featureRepository.Update(entity);
                _unitOfWork.Commit();

                var result = _mapper.Map<GetFeatureOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<bool>> DeleteAsync( DeleteTrackedEntityInputDto inputDto )
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
                var entity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));

                var checkDto = EntityHasReferences(inputDto.Id, _featureRepository);
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
        public async Task<IResponse<List<GetFeatureOutputDto>>> GetAllAsync( )
        {
            var output = new Response<List<GetFeatureOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetFeatureOutputDto>>(_featureRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetFeatureOutputDto>> GetByIdAsync( GetFeatureInputDto inputDto )
        {
            var output = new Response<GetFeatureOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetFeatureOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        /// <summary>
        /// Get Paged List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetFeatureOutputDto>>> GetPagedListAsync( FilteredResultRequestDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetFeatureOutputDto>>();

            var result = GetPagedList<GetFeatureOutputDto, Feature, int>(
                pagedDto: pagedDto,
                repository: _featureRepository, orderExpression: x => x.Id,
                searchExpression: x =>
                  string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) && (
                  string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (x.Name.Contains(pagedDto.SearchTerm)
                      || (x.Description.Contains(pagedDto.SearchTerm))))),
                sortDirection: pagedDto.SortingDirection,
                disableFilter: true);
            return output.CreateResponse(result);
        }
        #endregion
        #region Helpers


        private bool IsNameAlreadyExist( JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? Id = null )
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                  ? _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.default") == name.Default)
                                 .WhereIf(a => a.Id != Id, Id.HasValue).Any()
                  : _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.ar") == name.Ar)
                                  .WhereIf(a => a.Id != Id, Id.HasValue).Any();
        }

        #endregion

    }
}
