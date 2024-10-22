using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.PaymentSetup;
using Ecommerce.Repositroy.Base;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.PaymentSetup.PaymentSetupValidator;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.BLL.Payments
{
    public class PaymentSetupBLL : BaseBLL, IPaymentSetupBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<PaymentMethod> _paymentMethodRepository;
        IRepository<PaymentType> _paymentTypeRepository;
        IRepository<CountryPaymentMethod> _countryPaymentRepository;
        IRepository<Country> _countryRepository;
        IRepository<Employee> _employeeRepository;

        #endregion

        #region Constructor
        public PaymentSetupBLL( IMapper mapper,
                               IUnitOfWork unitOfWork,
                               IRepository<PaymentMethod> paymentMethodRepository,
                               IRepository<Employee> employeeRepository,
                               IRepository<PaymentType> paymentTypeRepository,
                               IRepository<CountryPaymentMethod> countryPaymentRepository,
                               IRepository<Country> countryRepository ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentTypeRepository = paymentTypeRepository;
            _countryPaymentRepository = countryPaymentRepository;
            _countryRepository = countryRepository;
            _employeeRepository = employeeRepository;
        }

        #endregion

        #region Action

        ///-------------------------------------------------------------------------------------------------
        /// <summary> Add PaymentMethod and CounryPaymentMethod. </summary>
        ///
        /// <param name="inputDto">          Dto contain All data need in payment method.    </param>
        ///                                 
        ///
        /// <returns>  GetPaymentSetupOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetPaymentSetupOutputDto>> CreateAsync( CreatePaymentSetupInputDto inputDto )
        {
            var output = new Response<GetPaymentSetupOutputDto>();
            try
            {
                //Input Validation
                var validator = await new CreatePaymentSetupInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                //1- Check Entity is exisit
                var paymentType = await _paymentTypeRepository.GetByIdAsync(inputDto.PaymentTypeId);
                if (paymentType == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(PaymentType));
                //2- Check PaymentType
                if (CheckPaymentType(null, inputDto.PaymentTypeId))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(PaymentType));
                //3- Check is UnActive and Default
                if (inputDto.IsDefault && !inputDto.IsActive)
                    return output.CreateResponse(MessageCodes.InActiveEntity, nameof(PaymentMethod));
                //4-Check has one Default at least
                if (!inputDto.IsDefault)
                {
                    if (HasAnyDefaultPaymentMethod())
                    {
                        inputDto.IsDefault = true;
                        inputDto.IsActive = true;
                    }
                }
                //5- Make one default
                if (inputDto.IsDefault == true)
                    MakeOnlyOneDefault();
                //Map entity
                var entity = _mapper.Map<PaymentMethod>(inputDto);
                _paymentMethodRepository.Add(entity);
                _unitOfWork.Commit();
                //2-Check has country
                if (inputDto.CountryId != 0)
                {
                    var country = await _countryRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                .FirstOrDefaultAsync(x => x.Id == inputDto.CountryId);
                    if (country == null)
                        return output.CreateResponse(MessageCodes.NotFound, nameof(Country));
                    //Save CountryPayment
                    SaveCountryPaymentMethod(inputDto.CountryId, entity.Id);
                }
                var result = _mapper.Map<GetPaymentSetupOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {

                return output.CreateResponse(ex);
            }

        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Delete PaymentMethod By ID. </summary>
        ///
        /// <param name="inputDto">          Dto contain ID paymentMethod delete.    </param>
        ///                                 
        ///
        /// <returns>  Boolean. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<bool>> DeleteAsync( DeletePayementSetupDto inputDto )
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = await new DeletePaymentSetupInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return output.CreateResponse(validator.Errors);
                //Business Validation
                var entity = await _paymentMethodRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                     .FirstOrDefaultAsync(x => x.Id == inputDto.Id);
                //1-Check entity is exisit
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(PaymentMethod));
                //2- Check is Default
                if (entity.IsDefault)
                    return output.CreateResponse(MessageCodes.DefaultEntity);
                // Check entity has references
                bool result = true;
                var checkDto = EntityHasReferences(entity.Id, _paymentMethodRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(CountryPaymentMethod)));
                if (checkDto.HasChildren == 0)
                {
                    var paymentCountry = entity.CountryPaymentMethods;
                    foreach (var country in paymentCountry)
                    {
                        _countryPaymentRepository.Delete(country);
                    }
                    result = true;
                }
                else
                {
                    result = false;
                }
                if (result)
                {
                    //Soft deleted
                    entity.IsDeleted = true;
                    _unitOfWork.Commit();
                    return output.CreateResponse(true);
                }
                else
                    // reject delete if entity has references in any other tables
                    return output.CreateResponse(MessageCodes.RelatedDataExist);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Update PaymentMethod and CounryPaymentMethod. </summary>
        ///
        /// <param name="inputDto">          Dto contain All data need in payment method.    </param>
        ///                                 
        ///
        /// <returns>  GetPaymentSetupOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<GetPaymentSetupOutputDto>> UpdateAsync( UpdatePaymentSetupInputDto inputDto )
        {
            var output = new Response<GetPaymentSetupOutputDto>();
            try
            {
                // Input Validation
                var validator = await new UpdatePaymentSetupInputDtoValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return output.CreateResponse(validator.Errors);

                //BusinessValidation
                // 1- Check entity is exisit
                var entity = await _paymentMethodRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                     .FirstOrDefaultAsync(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(PaymentMethod));
                //2- Check PaymentType
                if (CheckPaymentType(entity.Id, inputDto.PaymentTypeId))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(PaymentType));
                //3- Check is UnActive and Default
                if (inputDto.IsDefault && !inputDto.IsActive)
                    return output.CreateResponse(MessageCodes.InActiveEntity, nameof(PaymentMethod));
                //4-Check has one active at least
                if (!inputDto.IsDefault)
                    if (HasAnyDefaultPaymentMethod(entity.Id))
                        return output.CreateResponse(MessageCodes.DefaultEntityCount, nameof(PaymentMethod));
                //5-Check for Credential
                var paymentCredential = JsonConvert.DeserializeObject<PaymentSetupCredential>(entity.Credential);
                if (!string.IsNullOrWhiteSpace(inputDto.PaymentSetupCredential.SercretKey) && inputDto.PaymentSetupCredential.SercretKey != paymentCredential.SercretKey)
                    paymentCredential.SercretKey = inputDto.PaymentSetupCredential.SercretKey;
                if (!string.IsNullOrWhiteSpace(inputDto.PaymentSetupCredential.ApiKey) && inputDto.PaymentSetupCredential.ApiKey != paymentCredential.ApiKey)
                    paymentCredential.ApiKey = inputDto.PaymentSetupCredential.ApiKey;
                if (!string.IsNullOrWhiteSpace(inputDto.PaymentSetupCredential.BaseUrl) && inputDto.PaymentSetupCredential.BaseUrl != paymentCredential.BaseUrl)
                    paymentCredential.BaseUrl = inputDto.PaymentSetupCredential.BaseUrl;
                inputDto.PaymentSetupCredential = paymentCredential;

                //6- Check Default
                if (inputDto.IsDefault == true)
                    MakeOnlyOneDefault();
                // 2- Check has country
                //if (inputDto.CountryId != null)
                //{
                //    ////Check PaymentCounry is exisit
                //var country = inputDto.CountryId != null 
                //            ? await _countryRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                //                                            .FirstOrDefaultAsync(x => x.Id == inputDto.CountryId)
                //            :null;
                ////if (country == null)
                ////    return output.CreateResponse(MessageCodes.NotFound, nameof(Country));
                inputDto.CountryId = inputDto.CountryId == 0 ? null : inputDto.CountryId;
                if (!entity.CountryPaymentMethods.Any())
                {
                    SaveCountryPaymentMethod(inputDto.CountryId, entity.Id);
                }
                else
                {
                    var countryPayment = entity.CountryPaymentMethods.FirstOrDefault();
                    countryPayment.CountryId = inputDto.CountryId;
                    _countryPaymentRepository.Update(countryPayment);
                    _unitOfWork.Commit();

                }
                //}
                entity = _mapper.Map(inputDto, entity);
                _paymentMethodRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetPaymentSetupOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Get PaymentMethod by ID. </summary>
        ///
        /// <param name="inputDto">         Dto contain ID paymentMethod.    </param>
        ///                                 
        ///
        /// <returns>  GetPaymentSetupOutputDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        public async Task<IResponse<List<GetPaymentSetupOutputDto>>> GetAllAsync()
        {
            var ouput = new Response<List<GetPaymentSetupOutputDto>>();
            try
            {
                var payments = await _paymentMethodRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .Where(x => x.IsCreatedBySystem == false && x.IsRefundMethod == false).ToListAsync();
                var result = _mapper.Map<List<GetPaymentSetupOutputDto>>(payments);
                return ouput.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return ouput.CreateResponse(ex);
            }
        }
        public async Task<IResponse<List<GetPaymentMethodsOutputDto>>> GetAllPaymentMethodsAsync( int countryId )
        {
            var ouput = new Response<List<GetPaymentMethodsOutputDto>>();
            try
            {
                List<int> availablePaymentMethods = new List<int>();
                var countryPaymentMethods = _countryPaymentRepository.GetAllList();
                if (countryPaymentMethods.Any())
                {
                    foreach (var countryPayment in countryPaymentMethods)
                    {
                        if (countryPayment.CountryId == null || (countryPayment.CountryId == countryId))
                            availablePaymentMethods.Add(countryPayment.PaymentMethodId);
                    }
                }
                var paymentMethodQuery = _paymentMethodRepository.Where(x => !x.IsCreatedBySystem && !x.IsRefundMethod && (x.PaymentTypeId == (int)PaymentTypesEnum.PayPal || x.PaymentTypeId == (int)PaymentTypesEnum.PayMob));
                var availablePayments = countryPaymentMethods.Count() == 0
                                      ? paymentMethodQuery.Where(x => availablePaymentMethods.Contains(x.Id)).ToList()
                                      : paymentMethodQuery.ToList();
                var result = _mapper.Map<List<GetPaymentMethodsOutputDto>>(availablePayments);
                return ouput.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return ouput.CreateResponse(ex);
            }
        }
        public async Task<PaymentTypeDto> GetPaymentTypeByPaymentMethod( int paymentMethodId )
        {
            try
            {
                var payments = await _paymentMethodRepository.GetByIdAsync(paymentMethodId);
                var result = payments!= null ?_mapper.Map<PaymentTypeDto>(payments.PaymentType) : null;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GetPaymentMethodsOutputDto> GetPaymentMethodByPaymentType( PaymentTypesEnum paymentType )
        {
            try
            {
                var payments = await _paymentMethodRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefaultAsync(m => m.PaymentTypeId == (int)paymentType);

                var result = _mapper.Map<GetPaymentMethodsOutputDto>(payments);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IResponse<GetPaymentMethodsOutputDto>> GetPaymentMethodByPaymentTypeWithResponse( PaymentTypesEnum paymentType )
        {
            var response = new Response<GetPaymentMethodsOutputDto>();

            try
            {
                var paymentMethod = await GetPaymentMethodByPaymentType(paymentType);

                return response.CreateResponse(paymentMethod);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

      

        public async Task<GetPaymentMethodsOutputDto> GetPaymentMethodById(int paymentMethodId)
        {
            try
            {
                var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId);
                if (paymentMethod == null)
                    paymentMethod = await _paymentMethodRepository.Where(x => x.PaymentTypeId == (int)PaymentTypesEnum.Fawry).FirstOrDefaultAsync();
                var result = _mapper.Map<GetPaymentMethodsOutputDto>(paymentMethod);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Helper
        //Add Country for Payment
        private void SaveCountryPaymentMethod( int? countryId, int paymentMethodId )
        {

            _countryPaymentRepository.Add(new CountryPaymentMethod
            {
                CountryId = countryId,
                PaymentMethodId = paymentMethodId
            });
            _unitOfWork.Commit();
        }
        // Make only one is default
        private void MakeOnlyOneDefault( int? PaymentSetupId = null )
        {
            var payments = _paymentMethodRepository
                  .DisableFilter(nameof(DynamicFilters.IsActive))
                  .WhereIf(x => x.Id != PaymentSetupId, PaymentSetupId.HasValue)
                  .Where(x => x.IsDefault).ToList();
            foreach (var payment in payments)
            {
                payment.IsDefault = false;
            }
        }
        // Check type is exisit 
        private bool CheckPaymentType( int? paymentId, int PayementId )
        {
            var check = _paymentMethodRepository
                 .WhereIf(x => x.Id != paymentId, paymentId.HasValue)
                 .Where(x => x.PaymentTypeId == PayementId).Any();
            if (check)
                return true;
            return false;
        }

        private bool HasAnyDefaultPaymentMethod( int? paymentMethodId = null )
        {
            return !_paymentMethodRepository
                .DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != paymentMethodId, paymentMethodId.HasValue)
                .Where(x => x.IsDefault).Any();
        }
        #endregion

    }


}
