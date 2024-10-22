using AutoMapper;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.PriceLevels;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Roles.RoleDto;

namespace Ecommerce.BLL.PriceLevels
{
    public class PriceLevelBLL : BaseBLL, IPriceLevelBLL
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PriceLevel> _priceLevelRepository;

        public PriceLevelBLL( IMapper mapper,
                             IRepository<PriceLevel> priceLevelRepository,
                             IUnitOfWork unitOfWork )
            : base(mapper)
        {
            _mapper = mapper;
            _priceLevelRepository = priceLevelRepository;
            _unitOfWork = unitOfWork;
        }

        public IResponse<bool> AddPriceLevel( NewPriceLevelDto newPriceLevel )
        {
            var output =new Response<bool>();

            try
            {
                // => inputs validation.
                var validator = new NewPriceLevelValidator().Validate(newPriceLevel);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                // => business validation.
                if (IsPriceLevelExist(GetJsonLanguageModelOrNull(newPriceLevel.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(PriceLevel.Name));
                if (IsPriceLevelExist(GetJsonLanguageModelOrNull(newPriceLevel.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(PriceLevel.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                var priceLevelModel = _mapper.Map<PriceLevel>(newPriceLevel);


                _priceLevelRepository.Add(priceLevelModel);

                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public IResponse<bool> UpdatePriceLevel( UpdatePriceLevelDto updatePriceLevelDto )
        {
            var output =new Response<bool>();

            try
            {
                // => inputs validation.
                var validator = new UpdatePriceLevelValidator().Validate(updatePriceLevelDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                // => business validation.
                // 1- Check if Already Exists  
                if (IsPriceLevelExist(GetJsonLanguageModelOrNull(updatePriceLevelDto.Name), LangEnum.Default, updatePriceLevelDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(PriceLevel.Name));
                if (IsPriceLevelExist(GetJsonLanguageModelOrNull(updatePriceLevelDto.Name), LangEnum.Ar, updatePriceLevelDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(PriceLevel.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                var priceLevelModel = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == updatePriceLevelDto.Id);

                // 2. validate id existence.
                if (priceLevelModel == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(PriceLevel));

                priceLevelModel = _mapper.Map(updatePriceLevelDto, priceLevelModel);

                _priceLevelRepository.Update(priceLevelModel);

                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public IResponse<bool> DeletePriceLevel( DeleteTrackedEntityInputDto inputDto)
        {
            var output =new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                // => business validation.
                var priceLevelModel = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 1. validate id existence.
                if (priceLevelModel == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(PriceLevel));

                var referenceModels = EntityHasReferences(inputDto.Id, _priceLevelRepository);

                if (referenceModels.HasChildren is not 0)
                {
                    return output.CreateResponse(MessageCodes.RelatedDataExist);
                }

                priceLevelModel.IsDeleted = true;
                priceLevelModel.ModifiedDate = inputDto.ModifiedDate;
                priceLevelModel.ModifiedBy = inputDto.ModifiedBy;

                _priceLevelRepository.Update(priceLevelModel);

                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<List<RetrievePriceLevelDto>> GetAllList( )
        {

            var output =new Response<List<RetrievePriceLevelDto>>();
            try
            {

                var result = _mapper.Map<List<RetrievePriceLevelDto>>(_priceLevelRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<PagedResultDto<RetrievePriceLevelDto>> GetAllPriceLevelsPagedList( FilteredResultRequestDto filterDto )
        {
            var output =new Response<PagedResultDto<RetrievePriceLevelDto>>();

            var priceLevelsDto = GetPagedList<RetrievePriceLevelDto, PriceLevel, int>(
                pagedDto: filterDto,
                _priceLevelRepository, 
                orderExpression: pl => pl.Id,
                searchExpression: x => string.IsNullOrWhiteSpace(filterDto.SearchTerm) || (!string.IsNullOrWhiteSpace(filterDto.SearchTerm) && x.Name.Contains(filterDto.SearchTerm)),
                sortDirection: filterDto.SortingDirection,
                disableFilter: true);

            return output.CreateResponse(priceLevelsDto);
        }

        public async Task<IResponse<RetrievePriceLevelDto>> GetByIdAsync( GetPriceLevelInputDto inputDto )
        {
            var output =new Response<RetrievePriceLevelDto>();
            try
            {
                //Input Validation
                var validator = new GetPriceLevelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<RetrievePriceLevelDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
       
    #region Validation Helpers.
        private bool IsPriceLevelExist( JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? Id = null )
        {
            if (name == null )
                return false;

            return lang == LangEnum.Default
                  ? _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim())
                                 .WhereIf(a => a.Id != Id, Id.HasValue).Any()
                  : _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim())
                                  .WhereIf(a => a.Id != Id, Id.HasValue).Any();
        }
        #endregion
    }
}
