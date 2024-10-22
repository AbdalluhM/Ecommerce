using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.Helper.Security;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Employees
{
    public class EmployeeBLL : BaseBLL, IEmployeeBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Employee> _employeeRepository;
        IRepository<EmployeeCountry> _employeeCountryRepository;
        IRepository<CountryCurrency> _countryCurrencyRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly INotificationBLL _notificationBLL;
        private readonly AuthSetting _authSetting;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRealtimeNotificationBLL _realtimeNotificationBLL;

        IPasswordHasher _passwordHasher;
        private readonly ITokenBLL _tokeBLL;
        private IFileBLL _fileBLL;

        #endregion

        #region Constructor
        public EmployeeBLL(IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IRepository<Employee> employeeRepository,
                           IRepository<EmployeeCountry> employeeCountryRepository,
                           IRepository<CountryCurrency> countryCurrencyRepository,
                           IRepository<Country> countryRepository,
                           IOptions<AuthSetting> authSetting,
                           INotificationBLL notificationBLL,
                           IPasswordHasher passwordHasher,
                           ITokenBLL tokeBLL,
                           IFileBLL fileBLL,
                           IRepository<Customer> customerRepository,
                           IRealtimeNotificationBLL realtimeNotificationBLL)
            : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _employeeCountryRepository = employeeCountryRepository;
            _countryCurrencyRepository = countryCurrencyRepository;
            _countryRepository = countryRepository;
            _authSetting = authSetting.Value;
            _notificationBLL = notificationBLL;
            _passwordHasher = passwordHasher;
            _tokeBLL = tokeBLL;
            _fileBLL = fileBLL;
            _notificationBLL = notificationBLL;
            _customerRepository = customerRepository;
            _realtimeNotificationBLL = realtimeNotificationBLL;
        }

        #endregion

        #region Employee

        #region Internal Validation Functions
        public async Task<IResponse<LoginModelOutputDto>> GetEmployee(LoginModelInputDto inputDto)
        {
            var output = new Response<LoginModelOutputDto>();

            var employee = await _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                .Where(x => x.UserName.ToLower() == inputDto.UserName.Trim().ToLower()).FirstOrDefaultAsync();

            if (employee != null)
            {
                try
                {
                    //Verify Password
                    if (_passwordHasher.VerifyHashedPassword(inputDto.Password, employee.Password))
                    {

                        var result = _mapper.Map<GetEmployeeOutputDto>(employee);
                        result.Token = _tokeBLL.BuildToken(employee);
                        return output.CreateResponse(_mapper.Map<LoginModelOutputDto>(result));

                    }
                    output.CreateResponse(MessageCodes.FailedToFetchData);
                }
                catch (Exception ex)
                {
                    output.CreateResponse(MessageCodes.InvalidPassword);

                }


            }
            else
                output.CreateResponse(MessageCodes.InvalidUserNameOrPassword);

            return output;
        }
        /// <summary>
        /// Check if Employee Already Assigned To CountryCurrency
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        private bool IsEmployeeAlreadyAssignedToCountryCurrency(AssignEmployeeToCountryInputDto inputDto)
        {
            return _employeeCountryRepository
                 .DisableFilter(nameof(DynamicFilters.IsActive))
                .Any(x => x.CountryCurrencyId == inputDto.CountryCurrencyId && x.EmployeeId == inputDto.EmployeeId);

        }
        /// <summary>
        /// Check if Country Name Already Assigned To Country Except excludedId (Updated Entity)
        /// </summary>
        /// <param name="excludedId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsEmployeeAlreadyAssignedToCountryCurrency(UpdateAssignEmployeeToCountryInputDto inputDto)
        {
            return _employeeCountryRepository
                .DisableFilter(nameof(DynamicFilters.IsActive))
                .Any(x =>
            (inputDto.Id == 0 //check for create
            || (inputDto.Id > 0 && x.Id != inputDto.Id)) //check for update
            && x.CountryCurrencyId == inputDto.CountryCurrencyId && x.EmployeeId == inputDto.EmployeeId);

        }

        private List<EmployeeValidation> IsEmployeeAlreadyExists(CreateEmployeeInputDto inputDto)
        {
            //validate username and email
            var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => (x.UserName == (inputDto.UserName ?? "").Trim() || x.Email == (inputDto.Email ?? "").Trim()));

            var result = new List<EmployeeValidation>();
            if (entity != null && inputDto != null)
            {

                if (entity.UserName.Trim().ToLower() == inputDto.UserName.Trim().ToLower())
                    result.Add(EmployeeValidation.UserName);
                if (entity.Email.Trim().ToLower() == inputDto.Email.Trim().ToLower())
                    result.Add(EmployeeValidation.Email);
                if (entity.Name.Trim().ToLower() == inputDto.UserName.Trim().ToLower())
                    result.Add(EmployeeValidation.Name);
            }
            return result;
        }
        private List<EmployeeValidation> IsEmployeeAlreadyExists(UpdateEmployeeInputDto inputDto)
        {
            //validate username and email
            var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id != inputDto.Id && (x.UserName == (inputDto.UserName ?? "").Trim() || x.Email == (inputDto.Email ?? "").Trim()));

            var result = new List<EmployeeValidation>();
            if (entity != null && inputDto != null)
            {
                if (entity.UserName.Trim().ToLower() == inputDto.UserName.Trim().ToLower())
                    result.Add(EmployeeValidation.UserName);
                if (entity.Email.Trim().ToLower() == inputDto.Email.Trim().ToLower())
                    result.Add(EmployeeValidation.Email);
                if (entity.Name.Trim().ToLower() == inputDto.UserName.Trim().ToLower())
                    result.Add(EmployeeValidation.Name);
            }
            return result;
        }

        private bool IsCountrCurrencyExists(List<int> countryCuyrrencies)
        {
            return _countryCurrencyRepository.GetAllList(true).Select(x => x.Id).Intersect(countryCuyrrencies).Count() == countryCuyrrencies.Count();

        }

        #endregion

        #region API's 
        public Task<IResponse<LoginModelOutputDto>> LoginAsync(LoginModelInputDto inputDto)
        {
            var output = new Response<LoginModelOutputDto>();
            try
            {

                //Input Validation
                var validator = new LoginModelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return Task.FromResult(output.CreateResponse(validator.Errors));
                }

                var employee = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.UserName.ToLower() == inputDto.UserName.Trim().ToLower()).FirstOrDefault();

                //Business Validation
                if (employee != null)
                {
                    try
                    {
                        //1-Verify Employee IsActive 
                        if (!employee.IsActive)
                            return Task.FromResult(output.CreateResponse(MessageCodes.EmployeeIsInActive));
                        //2-Verify Password
                        if (_passwordHasher.VerifyHashedPassword(inputDto.Password, employee.Password))
                        {

                            var result = _mapper.Map<GetEmployeeOutputDto>(employee);
                            result.Token = _tokeBLL.BuildToken(employee);
                            return Task.FromResult(output.CreateResponse(_mapper.Map<LoginModelOutputDto>(result)));

                        }
                        else
                            return Task.FromResult(output.CreateResponse(MessageCodes.InvalidPassword));

                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(output.CreateResponse(MessageCodes.InvalidPassword));

                    }
                }

                else
                    return Task.FromResult(output.CreateResponse(MessageCodes.FailedToFetchData));

            }
            catch (Exception)
            {

                return Task.FromResult(output.CreateResponse(MessageCodes.FailedToFetchData));
            }


        }
        public Task<IResponse<bool>> LogOutAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Basic CRUD 
        /// <summary>
        /// Create Employee(if not exists) and Assign Employee To CountryCurrency Asynchronously with Providing Values for  CrossTable Properties(EmployeeId,CountryCurrencyId) 
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>

        public async Task<IResponse<GetEmployeeOutputDto>> CreateAsync(CreateEmployeeInputDto inputDto)
        {
            var output = new Response<GetEmployeeOutputDto>();

            try
            {
                //Input Validations
                var validator = new CreateEmployeeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Business Validations
                //collect errors then return all
                // 1- Check if employee [ Name, UserName , Email ] already exists
                var employeeExists = IsEmployeeAlreadyExists(inputDto);
                if (employeeExists != null && employeeExists.Count() > 0)
                {
                    employeeExists.ForEach(x =>
                    {
                        if (x == EmployeeValidation.Name)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Name));
                        if (x == EmployeeValidation.Email)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Email));
                        if (x == EmployeeValidation.UserName)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.UserName));

                    });

                }

                //2-check if all countrycurrency Ids exists
                if (!IsCountrCurrencyExists(inputDto.EmployeeCountries.ToList()))
                    output.AppendError(MessageCodes.NotFound, nameof(CountryCurrency));


                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business 
                //hash password
                inputDto.Password = !string.IsNullOrWhiteSpace(inputDto.Password) ? _passwordHasher.HashPassword(inputDto.Password) : string.Empty;
                FileStorage file = null;
                if (inputDto.File != null)
                {
                    // upload image.
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File);
                    file = createdFileResult.Data;
                }

                //map entity
                var mappedInput = _mapper.Map<Employee>(inputDto);
                if (file != null)
                    mappedInput.Image = file;


                var entity = await _employeeRepository.AddAsync(mappedInput);

                _unitOfWork.Commit();
                var result = _mapper.Map<GetEmployeeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public async Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                ////Input Validations
                //var validator = new ChangePasswordInputDtoValidator().Validate(inputDto);
                //if (!validator.IsValid)
                //{
                //    return output.CreateResponse(validator.Errors);

                //}
                //map entity
                var entity = _employeeRepository.GetById(inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Employee));

                if (!_passwordHasher.VerifyHashedPassword(inputDto.OldPassword, entity.Password))
                {

                    return output.CreateResponse(MessageCodes.InvalidPassword);
                }

                //Business 
                //hash password
                inputDto.NewPassword = !string.IsNullOrWhiteSpace(inputDto.NewPassword) ? _passwordHasher.HashPassword(inputDto.NewPassword) : string.Empty;

                entity.Password = inputDto.NewPassword;
                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }


        public async Task<IResponse<GetEmployeeOutputDto>> UpdateAysnc(UpdateEmployeeInputDto inputDto)
        {
            var output = new Response<GetEmployeeOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateEmployeeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                //collect errors then return all
                // 1- Check if employee [ Name, UserName , Email ] already exists
                var employeeExists = IsEmployeeAlreadyExists(inputDto);
                if (employeeExists != null && employeeExists.Count() > 0)
                {
                    employeeExists.ForEach(x =>
                    {
                        if (x == EmployeeValidation.Name)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Name));
                        if (x == EmployeeValidation.Email)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Email));
                        if (x == EmployeeValidation.UserName)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.UserName));
                    });

                }

                if (!output.IsSuccess)
                    return output.CreateResponse();

                FileStorage file = null;
                if (inputDto.File != null)
                {
                    // upload image.
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File);
                    file = createdFileResult.Data;
                }
                //Business
                var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Employee));

                #region old
                ////2-check if all countrycurrency Ids exists
                //if (!IsCountrCurrencyExists(inputDto.EmployeeCountries?.ToList()))
                //    return output.CreateResponse(MessageCodes.NotFound, nameof(CountryCurrency));


                //var employeeCountriesDB = _employeeCountryRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.EmployeeId == inputDto.Id).ToList();

                //var employeeCountries = _mapper.Map<List<EmployeeCountry>>(inputDto.EmployeeCountries);
                //employeeCountries.ForEach(x => {
                //    var item = employeeCountriesDB.Where(cc => cc.CountryCurrencyId == x.CountryCurrencyId).FirstOrDefault();
                //    x.EmployeeId = inputDto.Id;
                //    if (item != null)
                //    {
                //        x.Id = item.Id;
                //    }
                //});
                #endregion
                //Update Entity
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Image = file;

                ////Update Child Entities
                //_employeeCountryRepository.UpdateCrossTable(employeeCountries, x => x.EmployeeId == inputDto.Id, "EmployeeCountries");

                _unitOfWork.Commit();

                ////Notifiy to all members
                //await _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                //{
                //    Channel = HubChannelEnum.RefreshTokenChannel,
                //    RecieverType = NotificationRecieverTypeEnum.Employee,
                //    Recievers = new List<int> { entity.Id },
                //    Notification = null
                //});

                var result = _mapper.Map<GetEmployeeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetEmployeeOutputDto>> UpdateCountryByAdminAysnc(UpdateEmployeeInputDto inputDto)
        {
            var output = new Response<GetEmployeeOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateEmployeeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                //collect errors then return all
                // 1- Check if employee [ Name, UserName , Email ] already exists
                var employeeExists = IsEmployeeAlreadyExists(inputDto);
                if (employeeExists != null && employeeExists.Count() > 0)
                {
                    employeeExists.ForEach(x =>
                    {
                        if (x == EmployeeValidation.Name)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Name));
                        if (x == EmployeeValidation.Email)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.Email));
                        if (x == EmployeeValidation.UserName)
                            output.AppendError(MessageCodes.AlreadyExists, nameof(Employee.UserName));
                    });

                }

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Employee));

                //2-check if all countrycurrency Ids exists
                if (!IsCountrCurrencyExists(inputDto.EmployeeCountries?.ToList()))
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CountryCurrency));


                var employeeCountriesDB = _employeeCountryRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.EmployeeId == inputDto.Id).ToList();

                var employeeCountries = _mapper.Map<List<EmployeeCountry>>(inputDto.EmployeeCountries);
                employeeCountries.ForEach(x =>
                {
                    var item = employeeCountriesDB.Where(cc => cc.CountryCurrencyId == x.CountryCurrencyId).FirstOrDefault();
                    x.EmployeeId = inputDto.Id;
                    if (item != null)
                    {
                        x.Id = item.Id;
                    }
                });
                //Update Entity
                entity.Name = inputDto.Name;
                entity.Email = inputDto.Email;
                entity.UserName = inputDto.UserName;
                entity.Password = !string.IsNullOrWhiteSpace(inputDto.Password) ? _passwordHasher.HashPassword(inputDto.Password) : entity.Password;
                entity.IsActive = inputDto.IsActive;
                entity.Mobile = inputDto.Mobile;
                entity.RoleId = inputDto.RoleId;
                entity.IsAdminForOtherCountries = inputDto.IsAdminForOtherCountries;
                //Update Child Entities
                _employeeCountryRepository.UpdateCrossTable(employeeCountries, x => x.EmployeeId == inputDto.Id, "EmployeeCountries");
                _employeeRepository.Update(entity);
                _unitOfWork.Commit();

                //Notifiy to all members
                await _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                {
                    Channel = HubChannelEnum.RefreshTokenChannel,
                    RecieverType = NotificationRecieverTypeEnum.Employee,
                    Recievers = new List<int> { entity.Id },
                    Notification = null
                });

                var result = _mapper.Map<GetEmployeeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        /// <summary>
        /// Soft Delete for Assigned Employee To Country  Asynchronously
        /// Delete will Fail if there is references in any tables Or Entity is (DefaultForOther = true)
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
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

                //Business
                // get entity
                var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                // Business Validation
                //  1 - Check if user is admin
                if (entity.IsAdmin != null && entity.IsAdmin == true)
                    return await Task.FromResult(output.CreateResponse(MessageCodes.CanNotDeleteAdminUser));

                //  2 - Check if entity has references
                var checkDto = EntityHasReferences(entity.Id, _employeeRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(EmployeeCountry)));
                if (checkDto.HasChildren == 0)
                {

                    //hard delete from cross table
                    _employeeCountryRepository.Delete(x => x.EmployeeId == inputDto.Id);
                    //   soft delete entity 
                    entity.IsDeleted = true;
                    _unitOfWork.Commit();

                    return await Task.FromResult(output.CreateResponse(true));
                }
                else
                    // reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }

        }
        public async Task<IResponse<GetEmployeeOutputDto>> GetByIdAsync(GetEmployeeInputDto inputDto)
        {
            var output = new Response<GetEmployeeOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetEmployeeInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                //var entity =  _employeeRepository.DisableFilter().Where(x => x.Id == inputDto.Id).FirstOrDefault();

                var entity = _employeeRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetEmployeeOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<Employee> GetByIdAsync(int id)
        {

            try
            {
                return await _employeeRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<IResponse<List<GetEmployeeOutputDto>>> GetAllListAsync()
        {
            var output = new Response<List<GetEmployeeOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetEmployeeOutputDto>>(_employeeRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<List<int>> GetEmployeesByCustomerIdAsync(int customerId)
        {
            var employeeIds = new List<int>();

            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer is null)
                return employeeIds;

            // get admin.
            var admins = await _employeeRepository.GetManyAsync(e => e.IsAdmin == true);

            var adminsIds = admins.Select(a => a.Id);

            employeeIds.AddRange(adminsIds);

            // get employees by country Id.
            var countryCurrency = await _countryCurrencyRepository.GetAsync(cc => cc.CountryId == customer.CountryId);

            if (countryCurrency is not null)
            {
                var employeesIds = countryCurrency.EmployeeCountries.Select(ec => ec.EmployeeId);

                employeeIds.AddRange(employeesIds);
            }
            else // get employees assigned for other countries.
            {
                var employeesForOther = await _employeeRepository.GetManyAsync(e => e.IsAdminForOtherCountries == true);

                var employeesForOtherIds = employeesForOther.Select(ec => ec.Id);

                employeeIds.AddRange(employeesForOtherIds);
            }

            return employeeIds;
        }


        /// <summary>
        /// Get Paged List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetEmployeeOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetEmployeeOutputDto>>();

            var result = GetPagedList<GetEmployeeOutputDto, Employee, int>(pagedDto: pagedDto,
                repository: _employeeRepository,
                orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection,
                //sortDirection:nameof(SortingDirection.DESC),
                searchExpression:
               x => (x.IsAdmin == null || x.IsAdmin == false) &&
                 string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) && (
                  string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (x.Name.Contains(pagedDto.SearchTerm)
                                       && (x.UserName.Contains(pagedDto.SearchTerm) || x.Email.Contains(pagedDto.SearchTerm))))),
               //x => string.IsNullOrWhiteSpace(pagedDto.SearchTerm) || (x.Name.Contains(pagedDto.SearchTerm)
               //                         && (x.UserName.Contains(pagedDto.SearchTerm) || x.Email.Contains(pagedDto.SearchTerm))),
               disableFilter: true);
            return output.CreateResponse(result);
        }
        //get assigned countries to employee
        //if employee is super admin get all countries in system
        //else get only assigned countries to employee

        public async Task<List<int>> GetEmployeeCountries(int employeeId)
        {
            var employeCountriesIds = new List<int>();
            if (employeeId == 0)
                return employeCountriesIds;

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee is null)
                return employeCountriesIds;

            // all countries
            var countryIds = _countryRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Select(x => x.Id).ToList();
            // all country in country currrency
            var countryCurrencyIds = _countryCurrencyRepository.GetAll().Select(x => x.CountryId).ToList();
            // all country assign employee
            var employeAssignCountriesIds = employee.EmployeeCountries?.Where(e=>e.CountryCurrency != null).Select(c => c.CountryCurrency.CountryId).ToList();
            // all country not assign to currency
            var countriesNotAssign = countryIds.Except(countryCurrencyIds).ToList();/*Where(x => !countryCurrencyIds.Contains(x)).ToList();*/

            //if employee is super admin get all countries in system
            if (employee.IsAdmin.HasValue && employee.IsAdmin.Value)
            {
                employeCountriesIds = countryIds;
            }
            else if (employee.IsAdminForOtherCountries == true)
            {
                countriesNotAssign.AddRange(employeAssignCountriesIds);
                employeCountriesIds = countriesNotAssign;
            }
            else
                //else get only assigned countries to employee
                employeCountriesIds = employeAssignCountriesIds;


            return employeCountriesIds;
        }

        public IEnumerable<int> GetEmployeeCountryCurrencies(int employeeId)
        {
            IEnumerable<int> employeCountriesIds = new List<int>();

            var employee = _employeeRepository.GetById(employeeId);

            if (employee is null)
                return employeCountriesIds;

            if (employee.IsAdmin.Value)
            {
                var countryCurrencies = _countryCurrencyRepository.GetAllList();

                if (!countryCurrencies.Any())
                    return employeCountriesIds;

                employeCountriesIds = countryCurrencies.Select(cc => cc.Id).ToList();
            }
            else
                employeCountriesIds = employee.EmployeeCountries.Select(c => c.CountryCurrencyId).ToList();

            return employeCountriesIds;
        }

        #endregion
        #endregion
    }
}

