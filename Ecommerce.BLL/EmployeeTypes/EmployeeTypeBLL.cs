using AutoMapper;
using FluentValidation;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.EmployeeTypes;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.Repositroy.Base;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.EmployeeTypes
{
    public class EmployeeTypeBLL : BaseBLL, IEmployeeTypeBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<EmployeeType> _employeeTypeRepository;
        //IServiceProvider _serviceProvider;

        #endregion

        #region Constractor
        public EmployeeTypeBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<EmployeeType> employeeTypeRepository/*, IServiceProvider serviceProvider*/) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeTypeRepository = employeeTypeRepository;
            //_serviceProvider = serviceProvider;
        }
        #endregion

        #region Actions

        /// <summary>
        /// create GetAll EmployeeType  
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public IResponse<List<GetEmployeeTypeOutputDto>> GetAllEmployeeTypes()
        {
            return AsyncContext.Run(() => GetAllEmployeeTypesAsync());
        }

        public async Task<IResponse<List<GetEmployeeTypeOutputDto>>> GetAllEmployeeTypesAsync()
        {
            var output = new Response<List<GetEmployeeTypeOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetEmployeeTypeOutputDto>>(_employeeTypeRepository.GetAllList());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }



        /// <summary>
        /// create new EmployeeType  
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public IResponse<GetEmployeeTypeOutputDto> Create(CreateEmployeeTypeInputDto inputDto)
        {
            var output = new Response<GetEmployeeTypeOutputDto>();

            try
            {
                //Input Validation Validations
                var validator = new CreateEmployeeTypeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {

                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Type), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tag.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Type), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tag.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                var mappedInput = _mapper.Map<EmployeeType>(inputDto);
                var entity = _employeeTypeRepository.Add(mappedInput);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetEmployeeTypeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        /// <summary>
        /// Update EmployeeType
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>

        public IResponse<GetEmployeeTypeOutputDto> Update(UpdateEmployeeTypeInputDto inputDto)
        {
            var output = new Response<GetEmployeeTypeOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateEmployeeTypeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //Business Validations
                // 1- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Type), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tag.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Type), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tag.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _employeeTypeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//_EmployeeTypeRepository.GetById(inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(EmployeeType));


                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Entity
                entity = _employeeTypeRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetEmployeeTypeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        /// <summary>
        /// soft Delete For EmployeeType
        /// </summary>
        /// <param name="Id"> EmployeeType Id</param>
        /// <returns></returns>
        public IResponse<bool> Delete(DeleteEntityInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = new DeleteEmployeeTypeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _employeeTypeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//_EmployeeTypeRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(EmployeeType));

                // 2- Check if Entity has references
                var checkDto = EntityHasReferences(entity.Id, _employeeTypeRepository);
                if (checkDto.HasChildren == 0)
                {
                    entity.IsDeleted = true;
                    _employeeTypeRepository.Update(entity);
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
        #endregion


        #region Helpers
        /// <summary>
        ///  Check if Tag Name Already Exist English or arabic for insert or update 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lang"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool IsNameAlreadyExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? Id = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                  ? _employeeTypeRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Type, "$.default") == name.Default)
                                 .WhereIf(a => a.Id != Id, Id.HasValue).Any()
                  : _employeeTypeRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Type, "$.ar") == name.Ar)
                                  .WhereIf(a => a.Id != Id, Id.HasValue).Any();
        }

        #endregion

    }
}
