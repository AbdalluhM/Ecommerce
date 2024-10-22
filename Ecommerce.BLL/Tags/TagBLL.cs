using AutoMapper;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.PriceLevels;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using Ecommerce.Repositroy.Base;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Tags
{
    public class TagBLL : BaseBLL, ITagBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Tag> _tagRepository;
        IServiceProvider _serviceProvider;

        #endregion

        #region Constructor
        public TagBLL( IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tag> tagRepository, IServiceProvider serviceProvider ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _serviceProvider = serviceProvider;
        }

        #endregion



        /// <summary>
        /// create new Tag  
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public IResponse<GetTagOutputDto> Create( CreateTagInputDto inputDto )
        {
            var output = new Response<GetTagOutputDto>(/*_serviceProvider*/);

            try
            {
                //Input Validation Validations
                var validator = new CreateTagDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {

                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tag.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tag.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();


                var mappedInput = _mapper.Map<Tag>(inputDto);
                var entity = _tagRepository.Add(mappedInput);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        /// <summary>
        /// Update Tag
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>

        public IResponse<GetTagOutputDto> Update( UpdateTagInputDto inputDto )
        {
            var output = new Response<GetTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateTagDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tag.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tag.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();


                //Business
                var entity = _tagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//_tagRepository.GetById(inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Tag));

                if (!inputDto.IsActive)
                {

                    if (entity.ApplicationTags.Any(x => x.IsFeatured))
                    {
                        return output.CreateResponse(MessageCodes.DeactivateFeaturedTag);

                    }
                    if (entity.AddOnTags.Any(x => x.IsFeatured))
                    {
                        return output.CreateResponse(MessageCodes.DeactivateFeaturedTag);

                    }
                    if (entity.ModuleTags.Any(x => x.IsFeatured))
                    {
                        return output.CreateResponse(MessageCodes.DeactivateFeaturedTag);

                    }
                   
                }

                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Tag
                entity = _tagRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        /// <summary>
        /// soft Delete For Tag
        /// </summary>
        /// <param name="Id"> Tag Id</param>
        /// <returns></returns>
        public IResponse<bool> Delete( DeleteTrackedEntityInputDto inputDto )
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

                var entity = _tagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//_tagRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(TagBLL));

                // 2- Check if Entity has references
                var checkDto = EntityHasReferences(entity.Id, _tagRepository);
                if (checkDto.HasChildren == 0)
                {
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _tagRepository.Update(entity);
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

        /// <summary>
        /// get all tags active & inactive
        /// </summary>
        /// <returns></returns>
        public IResponse<List<GetTagOutputDto>> GetAllList( )
        {
            var output = new Response<List<GetTagOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetTagOutputDto>>(_tagRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<PagedResultDto<GetTagOutputDto>> GetPagedTagList( FilteredResultRequestDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetTagOutputDto>>();

            var result = GetPagedList<GetTagOutputDto, Tag, int>(
                pagedDto: pagedDto, _tagRepository,
                orderExpression: x => x.Id,
                searchExpression: x => string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) && x.Name.Contains(pagedDto.SearchTerm)),
              sortDirection: pagedDto.SortingDirection,
              disableFilter: true);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<GetTagOutputDto>> GetByIdAsync( GetTagInputDto inputDto )
        {
            var output = new Response<GetTagOutputDto>();
            try
            {
                //Input Validation
                var validator = new GetTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _tagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        #region Helpers
        /// <summary>
        ///  Check if Tag Name Already Exist English or arabic for insert or update 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lang"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool IsNameAlreadyExist( JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? Id = null )
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                  ? _tagRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.default") == name.Default)
                                 .WhereIf(a => a.Id != Id, Id.HasValue).Any()
                  : _tagRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.ar") == name.Ar)
                                  .WhereIf(a => a.Id != Id, Id.HasValue).Any();
        }

        #endregion


    }
}
