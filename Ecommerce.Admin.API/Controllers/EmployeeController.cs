using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Ecommerce.API.Attributes;
using Ecommerce.BLL.Customers;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Payments;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Threading.Tasks;

using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : BaseAdminController
    {
        #region Fields
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBLL _employeeBLL;
        private readonly ICustomerBLL _customerBLL;
        private readonly IMapper _mapper;
        private readonly IPaymentSetupBLL _paymentSetupBLL;
        #endregion
        #region Constructor

        public EmployeeController( ILogger<EmployeeController> logger,
            IEmployeeBLL employeeBLL,
            ICustomerBLL customerBLL, 
            IPaymentSetupBLL paymentSetupBLL,
            IHttpContextAccessor httpContextAccessor , 
            IMapper mapper, IOptions<FileStorageSetting> fileOptions,
            IWebHostEnvironment webHostEnvironment ) 
            : base(httpContextAccessor, fileOptions, webHostEnvironment)
        {
            _logger = logger;
            _employeeBLL = employeeBLL;
            _customerBLL = customerBLL;
            _mapper = mapper;
            _paymentSetupBLL = paymentSetupBLL;
        }
        #endregion
        #region Actions 
        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
       
        public async Task<IActionResult> Login( LoginModelInputDto inputDto )
        {
            var output = await _employeeBLL.LoginAsync(inputDto);
            if (output != null)
            {
                return Ok(output);
            }
            else
            {
                return Unauthorized();
            }
        }
        #region Employee
        [HttpPost]
        [Route("Create")]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.CREATE)]
        public async Task<IActionResult> Create( CreateEmployeeInputDto inputDto )
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            var output = await _employeeBLL.CreateAsync(inputDto);
            return Ok(output);
        }
        [HttpPost]
        [Route("ChangePassword")]
        [DxAuthorize(PagesEnum.SecuirtyAndLogin, ActionsEnum.UPDATE)]
        public async Task<IActionResult> ChangePassword( ChangePasswordInputDto inputDto )
        {

            inputDto.Id = CurrentEmployeeId;
            var output = await _employeeBLL.ChangePasswordAsync(inputDto);
            return Ok(output);
        }

        //[HttpGet]
        //[Route("ForgetPassword")]
        //public async Task<IActionResult> ForgetPassword( string email )
        //{
        //    return Ok();
        //    //var output = await _employeeBLL.ChangePasswordAsync(inputDto);
        //   // return Ok(output);
        //}
        //[HttpPost]
        //[Route("VerifyPassword")]
        //public async Task<IActionResult> VerifyPassword( VerifyPasswordInputDto inputDto )
        //{
        //    return Ok();
        //    //var output = await _employeeBLL.ChangePasswordAsync(inputDto);
        //    //return Ok(output);
        //}
        [HttpPost]
        [Route("Update")]
        [DxAuthorize(PagesEnum.Account, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateEmployee( [FromForm] APIUpdateEmployeeInputDto inputDto )
        {
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.AdminProfile) : null;
            var input = _mapper.Map<UpdateEmployeeInputDto>(inputDto);
            input.File = fileDto;
            input.ModifiedBy = CurrentEmployeeId;
            input.ModifiedDate = DateTime.UtcNow;
            var output = await _employeeBLL.UpdateAysnc(input);
            return Ok(output);
        }
        [HttpPost]
        [Route("UpdateEmployeeCountryAdmin")]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateEmployeeCountryAdmin( UpdateEmployeeInputDto inputDto)
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            inputDto.ModifiedDate = DateTime.UtcNow;
            var output = await _employeeBLL.UpdateCountryByAdminAysnc(inputDto);
            return Ok(output);
        }
        [HttpPost]
        [Route("Delete")]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.DELETE)]
        public async Task<IActionResult> Delete( int id )
        {
            var output = await _employeeBLL.DeleteAsync(new DeleteTrackedEntityInputDto
            {
                Id = id,
                ModifiedBy = CurrentEmployeeId
            });
            return Ok(output);
        }

        [HttpGet("{id}")]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.READ, ActionsEnum.DEFAULT)]
        public async Task<IActionResult> GetById( int id )
        {

            var output = await _employeeBLL.GetByIdAsync(new GetEmployeeInputDto { Id = id });
            return Ok(output);

        }
        [HttpGet("GetAll")]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.READ)]
        public async Task<IActionResult> GetAll( )
        {
            var result = await _employeeBLL.GetAllListAsync();

            return Ok(result);
        }
        [Route("GetPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.MyUsers, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {

            var output = await _employeeBLL.GetPagedListAsync(inputDto);
            return Ok(output);

        }
        #endregion
        #region CustomerHistory
        [Route("GetCustomerHistoryPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.RegistrationHistory, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomerHistoryPagedList([FromQuery] FilterByEmployeeCountryInputDto inputDto)
        {
            inputDto.EmployeeId = CurrentEmployeeId;
            var output = await _customerBLL.GetCustomerHistoryPagedListAsync(inputDto);
            return Ok(output);
        }
        [HttpPost]
        [Route("CustomerHistory/Update")]
        [DxAuthorize(PagesEnum.RegistrationHistory, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var output =await _customerBLL.MakeCustomerVerified(new UpdateCustomerStatusInputDto { Id=id});
            return Ok(output);


        }

        [HttpPost]
        [Route("CustomerHistory/Delete")]
        [DxAuthorize(PagesEnum.RegistrationHistory, ActionsEnum.DELETE)]
        public IActionResult DeleteCustomer(int id)
        {
            var output =  _customerBLL.Delete(new DeleteCustomerInputDto
            {
                Id = id,
            });
            return Ok(output);
        }

        [HttpPost("CustomerHistory/Export")]
        [DxAuthorize(PagesEnum.RegistrationHistory, ActionsEnum.READ)]
        public  IActionResult Export()
        {
            var customers = _customerBLL.GetAllCustomerRegisterHistory();
            return Ok(customers);
        }
        #endregion

        #region PaymentSetup
        [HttpPost("PaymentSetup/Create")]
        [DxAuthorize(PagesEnum.PayementSetup, ActionsEnum.CREATE)]
        public async Task< IActionResult>  CreatePaymentMethod(CreatePaymentSetupInputDto inputDto)
        {
            //ToDo 3 only active
            inputDto.CreatedBy = CurrentEmployeeId;
            var result = await _paymentSetupBLL.CreateAsync(inputDto);
            return Ok(result);
        }
        [HttpPost("PaymentSetup/Update")]
        [DxAuthorize(PagesEnum.PayementSetup, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdatePaymentMethod(UpdatePaymentSetupInputDto inputDto)
        {
            //ToDo 3 only active
            inputDto.ModifiedBy = CurrentEmployeeId;
            var result = await _paymentSetupBLL.UpdateAsync(inputDto);
            return Ok(result);
        }
        [HttpPost("PaymentSetup/Delete")]
        [DxAuthorize(PagesEnum.PayementSetup, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeletePaymentMethod(int Id)
        {
            var result = await _paymentSetupBLL.DeleteAsync(new DeletePayementSetupDto { Id = Id});
            return Ok(result);
        }

        [HttpGet("PaymentSetup/GetAll")]
        [DxAuthorize(PagesEnum.PayementSetup, ActionsEnum.READ)]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _paymentSetupBLL.GetAllAsync();
            return Ok(result);
        }
        #endregion

        #endregion



    }
}
