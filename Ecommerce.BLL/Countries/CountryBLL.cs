using AutoMapper;
using HandlebarsDotNet;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Taxes;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Countries
{
    public class CountryBLL : BaseBLL, ICountryBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Country> _countryRepository;
        IRepository<Currency> _currencyRepository;
        IRepository<CountryCurrency> _countryCurrencyRepository;
        private readonly IEmployeeBLL _employeeBLL;
        #endregion

        #region Constructor
        public CountryBLL(IMapper mapper,
                          IUnitOfWork unitOfWork,
                          IRepository<Country> countryRepository,
                          IRepository<CountryCurrency> countryCurrencyRepository,
                          IRepository<Currency> currencyRepository,
                          IEmployeeBLL employeeBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _countryRepository = countryRepository;
            _countryCurrencyRepository = countryCurrencyRepository;
            _currencyRepository = currencyRepository;
            _employeeBLL = employeeBLL;
        }


        #endregion

        #region Internal Validation Functions && Helpers


        /// <summary>
        /// Check if Country Already Exists
        /// </summary>
        /// <param name="countryId">Country </param>
        /// <param name="id">CountryCurrencyId check for Create & Update(Exclude)</param>

        /// <returns></returns>
        private bool IsCountryAlreadyExists(int countryId,int? id = null)
        {

            return _countryCurrencyRepository
                .DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x=>x.Id != id ,id.HasValue && id.Value > 0)
                .Any(x => x.CountryId == countryId );

        }
        private bool IsCountryCurrencyAlreadyExists( int countryId,int currencyId, int? id = null )
        {

            return _countryCurrencyRepository
                .DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != id, id.HasValue && id.Value > 0)
                .Any(x => x.CountryId == countryId && x.CurrencyId == currencyId);

        }

        public string GetDefaultCurrencyCode( )
        {
          var entity =  _countryCurrencyRepository.Where(x => x.DefaultForOther).Select(x => x.Currency.Code).FirstOrDefault();
            if (entity == null)
                return string.Empty;

            return entity;
        }
        #endregion
        
        #region Business
        /// <summary>
        /// Remove All Assigned Currencies as Default Currency  Except Checked Entity
        /// Commit UOW in the Scope we use it
        /// </summary>
        /// <param name="inputEntity"></param>
        /// <returns></returns>

        private List<CountryCurrency> UpdateOldDefaultCountryCurrency( CountryCurrency inputEntity ,bool commit = false )
        {
            var oldDefaultCountries = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id != inputEntity.Id && x.DefaultForOther).ToList();
            oldDefaultCountries.ForEach(oldDefaultCountry =>
            {
                oldDefaultCountry.DefaultForOther = false;
                _countryCurrencyRepository.Update(oldDefaultCountry);

            });
            if (commit)
                _unitOfWork.Commit();
            return oldDefaultCountries;
        }

        #endregion
        
        #region Basic CRUD 
        /// <summary>
        ///  Assign Currency To Country Asynchronously with Providing Values for  CrossTable Properties(DefaultForOtherCountries,IsActive) 
        /// if this is the first record => set DefaultForOther = true;
        /// and Update all OldDefaultForOtherCountries(Currencies Have been as Default for this Country) if (DefaultForOtherCountries = true)
        //// if this is the first record => set DefaultForOther = true;
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> CreateAsync( AssignCurrencyToCountryInputDto inputDto )
        {
            var output = new Response<GetAssignedCurrencyToCountryOutputDto>();

            try
            {

                //Input Validation Validations
                var validator = new AssignCurrencyToCountryInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-Check if default
                if (inputDto.DefaultForOther == true && inputDto.IsActive == false)
                   return output.CreateResponse(MessageCodes.InActiveEntity, nameof(CountryCurrency));
                //2-check Country , Currency already exists
                var country = _countryRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .FirstOrDefault(x => x.Id == inputDto.CountryId);
                var currency = _currencyRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .FirstOrDefault(x => x.Id == inputDto.CurrencyId);
                if (country == null)
                     output.AppendError(MessageCodes.NotFound, nameof(Country));
                if (currency == null)
                     output.AppendError(MessageCodes.NotFound, nameof(Currency));
                if (!output.IsSuccess)
                    return output.CreateResponse();
                // 3- Check if Country Already Exists
                //if (IsCountryAlreadyExists(inputDto.CountryId))
                if (IsCountryAlreadyExists(inputDto.CountryId))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(CountryCurrency));


                //Business
                var mappedInput = _mapper.Map<CountryCurrency>(inputDto);

                //check if this is the first record => set DefaultForOther = true;
                var firstRecord = !(_countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Any());
                if (firstRecord)
                    mappedInput.DefaultForOther = true;

                var entity = await _countryCurrencyRepository.AddAsync(mappedInput);

                //update default for other countries
                if (inputDto.DefaultForOther)
                {
                    var oldDefaultCountries = UpdateOldDefaultCountryCurrency(entity);
                }

                _unitOfWork.Commit();
                var result = _mapper.Map<GetAssignedCurrencyToCountryOutputDto>(entity);
                return output.CreateResponse(result);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
      
        ///// <summary>
        ///// Assign Currency To Country synchronously with Providing Values for  CrossTable Properties(DefaultForOtherCountries,IsActive) 
        ///// and Update all OldDefaultForOtherCountries(Currencies Have been Chosen as Default for this Country) if (DefaultForOtherCountries = true)
        /////  
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
        //public IOperationResult<GetAssignedCurrencyToCountryOutputDto> AssignCurrencyToCountry( AssignCurrencyToCountryInputDto inputDto )
        //{
        //    return AsyncContext.Run(( ) => AssignCurrencyToCountryAsync(inputDto));
        //}
        /// <summary>
        /// Update Assign Currency To Country Asynchronously with Providing Values for  CrossTable Properties(DefaultForOtherCountries,IsActive) 
        /// and Update all OldDefaultForOtherCountries(Currencies have been chosen as Default for this Country) if (DefaultForOtherCountries = true)
        ///  Update Should Be Allowed only in CrossTable Properties(DefaultForOtherCountries,IsActive) 
        ///  Update will Fail if Entity is (DefaultForOther = true) and the inputDto is (DefaultForOther = false) 
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> UpdateAsync( UpdateAssignCurrencyToCountryInputDto inputDto)
        {
            var output = new Response<GetAssignedCurrencyToCountryOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateAssignCurrencyToCountryInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                //1-Check if default
                if (inputDto.DefaultForOther == true && inputDto.IsActive == false)
                   return output.CreateResponse(MessageCodes.InActiveEntity, nameof(CountryCurrency));
                //2-check Country , Currency already exists
                var country = _countryRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .FirstOrDefault(x => x.Id == inputDto.CountryId);
                var currency = _currencyRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .FirstOrDefault(x => x.Id == inputDto.CurrencyId);
                if (country == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Country));
                if (currency == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Currency));
                if (!output.IsSuccess)
                    return output.CreateResponse();
              


                //Business
                var entity = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
               
                // 3- Check if Country Already Exists
                if(entity.CountryId != inputDto.CountryId)
                {
                    if (IsCountryAlreadyExists(inputDto.CountryId,inputDto.Id))
                        return output.CreateResponse(MessageCodes.AlreadyExists, nameof(CountryCurrency));
                }
             
                // 4- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CountryCurrency));


                // 3- Check if Country Currency is (DefaultForOther=true)
                if (!inputDto.DefaultForOther && entity.DefaultForOther)
                    return output.CreateResponse(MessageCodes.DefaultForOther, nameof(CountryCurrency));



                //update default for other countries in case (input is Default and Entity wasn't Default before)
                if (inputDto.DefaultForOther && !entity.DefaultForOther)
                {
                    var oldDefaultCountries = UpdateOldDefaultCountryCurrency(entity);

                }
                entity = _mapper.Map(inputDto, entity);
                //update country currency
                entity = _countryCurrencyRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetAssignedCurrencyToCountryOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///// <summary>
        ///// Soft Delete for Assigned Currency To Country By Id Synchronously
        ///// Delete will Fail if there is references in any tables Or Entity is (DefaultForOther = true)
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
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

                //Business
                //get entity
                var entity = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business Validation
                // 1-Check if Country Currency is (DefaultForOther=true)
                if (entity.DefaultForOther)
                    return output.CreateResponse(MessageCodes.DefaultForOther,  nameof(CountryCurrency));

                //2-Check if entity has references
                var checkDto = EntityHasReferences(entity.Id, _countryCurrencyRepository);
                if (checkDto.HasChildren == 0)
                {
                    //set DefaultForOther = false;
                    entity.DefaultForOther = false;
                    //soft delete CountryCurrency
                    entity.IsDeleted = true;
                    _unitOfWork.Commit();
                    return await Task.FromResult(output.CreateResponse(true));
                }
                else
                    //reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        ///// <summary>
        ///// Get Assigned Currency To Country By Id Asynchronously(CrossTable Id)
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
        public async Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> GetByIdAsync( GetAssignedCurrencyToCountryInputDto inputDto )
        {
            var output = new Response<GetAssignedCurrencyToCountryOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetAssignedCurrencyToCountryInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);


                //Business
                var result = _mapper.Map<GetAssignedCurrencyToCountryOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> GetByCountryIdOrDefaultAsync( int? countryId = null)
        {
            var output = new Response<GetAssignedCurrencyToCountryOutputDto>();

            try
            {

                //Business
                var entity =countryId != 0 ? _countryRepository.GetById(countryId.GetValueOrDefault(0)).CountryCurrency : null;
                //get default country currency
                if (entity == null)
                    entity = _countryCurrencyRepository
                    .Where(x => x.Country != null && x.DefaultForOther)
                    .FirstOrDefault();

                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                var result = _mapper.Map<GetAssignedCurrencyToCountryOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetAssignedCurrencyToCountryOutputDto>>> GetAllListAsync( )
        {
            var output = new Response<List<GetAssignedCurrencyToCountryOutputDto>>();

            try
            {
                var countries = _countryCurrencyRepository.GetAll().ToList();
                var result = _mapper.Map<List<GetAssignedCurrencyToCountryOutputDto>>(countries);
                return output.CreateResponse(result);
            
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetAssignedCurrencyToCountryOutputDto>>> GetAllByEmployeeIdAsync(int currentEmployeeId)
        {
            var output = new Response<List<GetAssignedCurrencyToCountryOutputDto>>();

            try
            {
                var countryCurrencyIds = _employeeBLL.GetEmployeeCountryCurrencies(currentEmployeeId);

                var countries = await _countryCurrencyRepository.GetManyAsync(cc => countryCurrencyIds.Contains(cc.Id));

                var result = _mapper.Map<List<GetAssignedCurrencyToCountryOutputDto>>(countries);

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        /// <summary>
        /// Get All Assigned Currencies To Countries as a PagedList with Search Terms(CountryName,CurrencyName,CurrencySymbol) Asynchronously
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetAssignedCurrencyToCountryOutputDto>>> GetPagedListAsync( FilteredResultRequestDto inputDto )
        {
            var output = new Response<PagedResultDto<GetAssignedCurrencyToCountryOutputDto>>();
            //inputDto.MaxResultCount = int.MaxValue;
            //var countryResult = GetPagedList<GetCountryOutputDto, Country, int>(
            //   pagedDto: inputDto, // set max size
            //   repository: _countryRepository,
            //   orderExpression: x => x.Id,
            //   searchExpression: x => string.IsNullOrEmpty(inputDto.SearchTerm)
            //                                 || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm)
            //                                 && (x.Name.Contains(inputDto.SearchTerm)
            //                                 || x.Name.Contains(inputDto.SearchTerm)
            //                                 || x.CountryCurrency.Currency.Symbole.Contains(inputDto.SearchTerm)
            //    )),
            //   sortDirection: inputDto.SortingDirection,
            //   disableFilter: true,
            //   excluededColumns: new List<string> { "IsActive", "Currency." });


            //var countryCurrencyResult = GetPagedList<GetAssignedCurrencyToCountryOutputDto, CountryCurrency, int>(
            //  pagedDto: inputDto,
            //  repository: _countryCurrencyRepository,
            //  orderExpression: x => x.Id,
            //  searchExpression: null,
            //  sortDirection: inputDto.SortingDirection,
            //  disableFilter: true,
            //   excluededColumns: new List<string> { "Taxes"});


            //var countryIds = countryResult.Items.Select(x => x.Id).ToList();


            //var intersectionList = countryCurrencyResult.Items.ToList();
            //if (countryIds != null && countryIds.Count() > 0)
            //    intersectionList.RemoveAll(x => !countryIds.Contains(x.CountryId));
            //intersectionList.ForEach(cc =>
            //{
            //    cc.Taxes = countryResult.Items.Where(c => c.Id
            //    == cc.CountryId).FirstOrDefault()?.Taxes ?? new List<GetTaxOutputDto>();
            //}
            //);


            //countryCurrencyResult.Items = intersectionList.Skip(inputDto.SkipCount).Take(inputDto.MaxResultCount).ToList();
            //countryCurrencyResult.TotalCount = intersectionList.Count();

            //return output.CreateResponse(countryCurrencyResult);

            var result = GetPagedList<GetAssignedCurrencyToCountryOutputDto, CountryCurrency, int>(
               pagedDto: inputDto,
               repository: _countryCurrencyRepository,
               orderExpression: x => x.Id,
               searchExpression: x => string.IsNullOrEmpty(inputDto.SearchTerm)
                                             || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                                             && (x.Country.Name.Contains(inputDto.SearchTerm)
                                             || x.Currency.Name.Contains(inputDto.SearchTerm)
                                             || x.Currency.Symbole.Contains(inputDto.SearchTerm))),
              sortDirection: inputDto.SortingDirection,
                     disableFilter: true);
            return output.CreateResponse(result);
        }

        #region Comment

        ///// <summary>
        ///// Check if Currency Already Assigned To Country 
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
        //private bool IsCurrencyAlreadyAssignedToCountry( AssignCurrencyToCountryInputDto inputDto )
        //{
        //    return _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Any(x =>
        //     //(inputDto.Id == 0 //check for create
        //     //|| (inputDto.Id > 0 && x.Id != inputDto.Id)) && //check for update
        //     x.CountryId == inputDto.CountryId && x.CurrencyId == inputDto.CurrencyId);

        //}
        ///// <summary>
        ///// Check if Currency Already Assigned To Country Except Checked Entity
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
        //private bool IsCurrencyAlreadyAssignedToCountry( UpdateAssignCurrencyToCountryInputDto inputDto )
        //{
        //    return _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Any(x =>
        //    (inputDto.Id == 0 //check for create
        //    || (inputDto.Id > 0 && x.Id != inputDto.Id)) //check for update
        //    && x.CountryId == inputDto.CountryId && x.CurrencyId == inputDto.CurrencyId);

        //}
        /////// <summary>
        /////// Soft Delete for Assigned Currency To Country By Id Synchronously
        /////// Delete will Fail if there is references in any tables Or Entity is (DefaultForOther = true)
        /////// </summary>
        /////// <param name="inputDto"></param>
        ///// <returns></returns>
        //public IResponse<bool> DeleteAssignedCurrencyToCountry( GetAssignedCurrencyToCountryInputDto inputDto )
        //{
        //    return AsyncContext.Run(( ) => DeleteAssignedCurrencyToCountryAsync(inputDto));

        //}
        /////// <summary>
        /////// Get All Assigned Currencies To Countries as a PagedList with Search Terms(CountryName,CurrencyName,CurrencySymbol) Synchronously
        /////// </summary>
        /////// <param name="inputDto"></param>
        /////// <returns></returns>
        //public IResponse<PagedResultDto<GetAssignedCurrencyToCountryOutputDto>> GetAssignedCurrencyToCountryPagedList( FilteredResultRequestDto inputDto )
        //{

        //    return AsyncContext.Run(( ) => GetAssignedCurrencyToCountryPagedListAsync(inputDto));
        //}
        ///// <summary>
        ///// Get Assigned Currency To Country By Id Synchronously(CrossTable Id)
        ///// </summary>
        ///// <param name="inputDto"></param>
        ///// <returns></returns>
        //public IResponse<GetAssignedCurrencyToCountryOutputDto> GetAssignedCurrencyToCountryById( GetAssignedCurrencyToCountryInputDto inputDto )
        //{
        //    return AsyncContext.Run(( ) => GetAssignedCurrencyToCountryByIdAsync(inputDto));

        //}

        #endregion
        #endregion
    }
}
