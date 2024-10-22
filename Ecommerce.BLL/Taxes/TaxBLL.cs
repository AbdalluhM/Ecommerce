using AutoMapper;

using Microsoft.AspNetCore.Components.Forms;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.Core.Consts.Logs;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Logs;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Taxes;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.BLL.Taxes
{
    public class TaxBLL : BaseBLL, ITaxBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Tax> _taxRepository;
        IRepository<Customer> _customerRepository;
        IRepository<AuditLogDetail> _auditLogDetailRepository;
        IRepository<Employee> _employeeRepository;

        #endregion

        #region Constructor
        public TaxBLL( IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tax> taxRepository, IRepository<Customer> customerRepository,
            IRepository<AuditLogDetail> auditLogDetailRepository ,IRepository<Employee> employeeRepository ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _taxRepository = taxRepository;
            _customerRepository = customerRepository;
            _auditLogDetailRepository = auditLogDetailRepository;
            _employeeRepository = employeeRepository;
        }

        #endregion



        /// <summary>
        /// create new Tax  
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public IResponse<GetTaxOutputDto> Create( CreateTaxInputDto inputDto )
        {
            var output = new Response<GetTaxOutputDto>();

            try
            {
                //Input Validation Validations
                var validator = new CreateTaxDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {

                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-Check if default
                if (inputDto.IsDefault == true && inputDto.IsActive == false)
                    output.CreateResponse(MessageCodes.InActiveEntity, nameof(Tax));

                // 2- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tax.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tax.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();


                //Check if no default for everey county
                if (!inputDto.IsDefault)
                {
                    if (HasAnyDefaultTaxForCountry(inputDto.CountryId))
                    {
                        inputDto.IsDefault = true;
                        inputDto.IsActive = true;
                    }
                }
                    
                var mappedInput = _mapper.Map<Tax>(inputDto);
                var entity = _taxRepository.Add(mappedInput);

                if (inputDto.IsDefault)
                {
                    var oldDefaultCountries = UpdateOldDefaultTaxForCountry(inputDto.CountryId);
                }
                _unitOfWork.Commit();

                var result = _mapper.Map<GetTaxOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        /// <summary>
        /// Update Tax
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>

        public IResponse<GetTaxOutputDto> Update( UpdateTaxInputDto inputDto )
        {
            var output = new Response<GetTaxOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateTaxDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                #region BusinessValidation
                //Business Validations
                //1-Check if default And UnActive
                if (inputDto.IsDefault == true && inputDto.IsActive == false)
                    return output.CreateResponse(MessageCodes.InActiveEntity, nameof(Tax));

                // 2- Check if Already Exists  
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Tax.Name));
                if (IsNameAlreadyExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Tax.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();


                var entity = _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 3- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Tax));

                if (entity.IsDefault && inputDto.CountryId != entity.CountryId)
                    return output.CreateResponse(MessageCodes.DefaultEntityCount, nameof(Tax));
                #endregion

                //Business
                //Check only one default in tax
                if (!inputDto.IsDefault)
                    if (HasAnyDefaultTaxForCountry(inputDto.CountryId, entity.Id))
                        return output.CreateResponse(MessageCodes.DefaultEntityCount, nameof(Tax));
                //update default for other countries in case (input is Default and Entity wasn't Default before)
                if (inputDto.IsDefault)
                {
                    var oldDefaultCountries = UpdateOldDefaultTaxForCountry(inputDto.CountryId, entity.Id);
                }

                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Tax
                entity = _taxRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetTaxOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        /// <summary>
        /// soft Delete For Tax
        /// </summary>
        /// <param name="Id"> Tax Id</param>
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

                var entity = _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//_taxRepository.GetById(inputDto.Id);
                                                                                                                                    //Business Validation
                                                                                                                                    // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Tax));
                //2-Check if default
                if (entity.IsDefault == true)
                    return output.CreateResponse(MessageCodes.DefaultEntity, nameof(Tax));


                // 2- Check if Entity has references
                var checkDto = EntityHasReferences(entity.Id, _taxRepository);
                if (checkDto.HasChildren == 0)
                {
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _taxRepository.Update(entity);
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
        /// get all Taxs active & inactive
        /// </summary>
        /// <returns></returns>
        public IResponse<List<GetTaxOutputDto>> GetAllList( )
        {
            var output = new Response<List<GetTaxOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetTaxOutputDto>>(_taxRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<PagedResultDto<GetTaxOutputDto>> GetPagedTaxList( FilteredResultRequestDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetTaxOutputDto>>();

            var result = GetPagedList<GetTaxOutputDto, Tax, int>(
                pagedDto: pagedDto, _taxRepository,
                orderExpression: x => x.Id,
                searchExpression: x => string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) && x.Name.Contains(pagedDto.SearchTerm)),
               sortDirection: pagedDto.SortingDirection,
                disableFilter: true);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<GetTaxOutputDto>> GetByIdAsync( GetTaxInputDto inputDto )
        {
            var output = new Response<GetTaxOutputDto>();
            try
            {
                //Input Validation
                var validator = new GetTaxInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetTaxOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public GetTaxOutputDto GetDefaultTaxForCustomer( GetCountryDefaultTaxInputDto inputDto )
        {
            var output = new GetTaxOutputDto();
            try
            {

                //Business Validation
                var entity = _taxRepository.Where(x => x.CountryId == inputDto.CountryId && x.IsDefault).FirstOrDefault();


                //Business
                var result = _mapper.Map<GetTaxOutputDto>(entity);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public GetTaxOutputDto GetDefaultTaxForCustomer( GetCountryDefaultTaxByCustomerIdInputDto inputDto )
        {
            var output = new GetTaxOutputDto();
            try
            {
                var countryId = _customerRepository.GetById(inputDto.CustomerId)?.CountryId;

                //Business Validation
                var entity = _taxRepository.Where(x => x.CountryId == countryId && x.IsDefault).FirstOrDefault();


                //Business
                var result = _mapper.Map<GetTaxOutputDto>(entity);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Tax GetDefaultTaxForCountry( int countryId )
        {
            var tax = _taxRepository.Where(x => x.CountryId == countryId && x.IsDefault).FirstOrDefault();
            return tax;
        }
    public async Task<IResponse<PagedResultDto<TaxActivitesDto>>> GetTaxActivitesPagedListAsync( LogFilterPagedResultDto pagedDto )
        {
            var output = new Response<PagedResultDto<TaxActivitesDto>>();

            var result = GetPagedList<TaxActivitesDto, AuditLogDetail, int>(
                pagedDto: pagedDto,

                repository: _auditLogDetailRepository, orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection,/*sortDirection:nameof(SortingDirection.DESC)*/
                searchExpression: x =>
                  x.AuditLog.TableName == nameof(Tax)
                  && (!pagedDto.Id.HasValue || (pagedDto.Id.HasValue && pagedDto.Id.Value > 0 && pagedDto.Id.Value == x.AuditLog.PrimaryKey))
                  && (!pagedDto.ActionTypeId.HasValue || (pagedDto.ActionTypeId.HasValue && pagedDto.Id.Value > 0 && pagedDto.ActionTypeId.Value == x.AuditLog.AuditActionTypeId))
                  && (!pagedDto.FromDate.HasValue || (pagedDto.FromDate.HasValue && x.AuditLog.CreateDate >= pagedDto.FromDate))
                  && (!pagedDto.ToDate.HasValue || (pagedDto.ToDate.HasValue && x.AuditLog.CreateDate <= pagedDto.ToDate))
                  &&
                string.IsNullOrWhiteSpace(pagedDto.SearchTerm)  || !string.IsNullOrWhiteSpace(pagedDto.SearchTerm)

                
                );


            if (result.Items.Any())
            {
              var auditLogIds =  result.Items.GroupBy(g => g.AuditLogId).Select(x => x.Key);
                foreach(var auditLogId in auditLogIds)
                {
                   bool isModifiedBy =  int.TryParse(result.Items.Where(
                       x => x.AuditLogId == auditLogId 
                    && (  (x.Field == LogsConst.ModifiedByColumnName && x.ActionType.Id != (int)AuditActionTypesEnum.Create)
                       || x.Field == LogsConst.CreatedByColumnName && x.ActionType.Id == (int)AuditActionTypesEnum.Create)
                      )
                       .FirstOrDefault()?.NewValue, out int modifiedById);
                    if (isModifiedBy)
                    {
                       var customer = _employeeRepository.GetById(modifiedById);
                        result.Items.Where(x=>x.AuditLogId == auditLogId).ToList().ForEach(x => x.Owner = customer?.Name ?? string.Empty);
                    }
                }
            }
            return output.CreateResponse(result);
        }

        #region Helpers
        /// <summary>
        ///  Check if Tax Name Already Exist English or arabic for insert or update 
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
                  ? _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.default") == name.Default)
                                 .WhereIf(a => a.Id != Id, Id.HasValue).Any()
                  : _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                 .Where(a => Json.Value(a.Name, "$.ar") == name.Ar)
                                  .WhereIf(a => a.Id != Id, Id.HasValue).Any();
        }
        private List<Tax> UpdateOldDefaultTaxForCountry( /*Tax inputEntity*/int countryId, int? excludedId = null, bool commit = false )
        {
            var oldDefaultTaxForCountry = _taxRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != excludedId, excludedId.HasValue && excludedId.Value > 0)
                .Where(x => /*x.Id != inputEntity.Id &&*/ x.CountryId == countryId && x.IsDefault).ToList();
            oldDefaultTaxForCountry.ForEach(oldDefault =>
            {
                oldDefault.IsDefault = false;
                _taxRepository.Update(oldDefault);

            });
            if (commit)
                _unitOfWork.Commit();
            return oldDefaultTaxForCountry;
        }
        private bool HasAnyDefaultTaxForCountry( int countryId, int? taxId = null )
        {
            return !_taxRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != taxId, taxId.HasValue)
                .Where(x => x.CountryId == countryId && x.IsDefault).Any();

        }
        #endregion


    }
}
