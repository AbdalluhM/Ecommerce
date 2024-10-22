using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Customers;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.API.Controllers.Contracts.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerBLL _customerBLL;
        private readonly IMapper _mapper;
        #endregion
        #region Constructor

        public CustomerController(ICustomerBLL customerBLL, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor)
        {
            _customerBLL = customerBLL;
            _mapper = mapper;
        }


        #endregion
        #region Actions
        [Route("GetAllCustomersInContract")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public IActionResult GetAllCustomersInContract()
        {
            var output =  _customerBLL.GetAlltCustomersInContract(CurrentEmployeeId);
            return Ok(output);
        }
        [Route("GetContractCustomerPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public async Task<IActionResult> GetContractCustomerPagedList( [FromQuery] FilteredResultRequestDto inputDto )
        {

            var input = _mapper.Map<FilterByEmployeeCountryInputDto>(inputDto);
            input.EmployeeId = CurrentEmployeeId;

            var output = await _customerBLL.GetContractustomerPagedListAsync(input);
            return Ok(output);
        }
        [Route("GetCustomerByDetails/{customerId}")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomerByDetails(int customerId)
        {
            var output = await _customerBLL.GetCustomerByDetails(customerId);
            return Ok(output);
        }

        [HttpPost]
        [Route("UpdateCustomerStatus")]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.UPDATE)]
        public IActionResult UpdateCustomer(UpdateCustomerByAdminInputDto inputDto)
        {
            var output = _customerBLL.UpdateCustomerByAdmin(inputDto);
            return Ok(output);
        }

        [Route("GetCustomerActivitesPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomerActivitesPagedList([FromQuery] LogFilterPagedResultDto inputDto )
        {
            var output = await _customerBLL.GetCustomerActivitesPagedListAsync(inputDto);
            return Ok(output);
        }
        [Route("GetCustomerRequestsByProductPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomerRequestsByProductPagedList([FromQuery] LicencesFilterInputDto inputDto)
        {
            var output = await _customerBLL.GetCustomerRequestByProductPagedList(inputDto);
            return Ok(output);
        }

        [Route("GetAllCountries")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllCountries()
        {
            var output =await _customerBLL.GetAllCountriesAsync();
            return Ok(output);
        }
        
        [HttpPost("CreateCustomer")]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateCustomerAsync(NewCustomerByAdminDto newCustomerDto)
        {
            var response = await _customerBLL.CreateCustomerAsync(newCustomerDto, CurrentEmployeeId);
            
            return Ok(response);
        }
        [HttpPost("DeleteCustomer")]
        [DxAuthorize(PagesEnum.Customers, ActionsEnum.CREATE)]
        public async Task<IActionResult> DeleteCustomerAsync(int customerId)
        {
            var response = await _customerBLL.DeleteCustomerAsync(customerId);

            return Ok(response);
        }
        #endregion

    }
}
