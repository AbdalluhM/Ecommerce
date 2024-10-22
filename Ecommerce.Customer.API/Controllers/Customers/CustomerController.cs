using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Ecommerce.BLL.Customers;
using Ecommerce.BLL.Files;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;
using System.Threading.Tasks;

using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.Customer.API.Controllers.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseCustomerController
    {
        private readonly ICustomerBLL _customerBLL;
        private readonly IMapper _mapper;
        private readonly IFileBLL _fileBLL;
        public CustomerController(IFileBLL fileBLL, ICustomerBLL customerBLL, IOptions<FileStorageSetting> fileOptions,
            IWebHostEnvironment webHostEnvironment, IMapper mapper, IHttpContextAccessor contextAccessor) : base(fileOptions, webHostEnvironment, contextAccessor)
        {
            _customerBLL = customerBLL;
            _mapper = mapper;
            _fileBLL = fileBLL;
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateCustomerInputDto inputDto)
        {
            inputDto.Id = CurrentUserId;
            var output = await _customerBLL.UpdateAysnc(inputDto);
            return Ok(output);

        }


        [Authorize]
        [HttpGet("GetCurrentCustomer")]
        public async Task<IActionResult> GetCurrentCustomer()
        {

            var output = await _customerBLL.GetByIdAsync(new GetCustomerInputDto { Id = CurrentUserId });
            return Ok(output);

        }



        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromQuery] UploadCustomerImageAptInputDto inputDto)
        {
            //ToDo After Registration 
            var fileDto = inputDto.File != null ? GetFile(inputDto.File, FilePathEnum.CustomerProfile) : null;
            var input = _mapper.Map<UploadCustomerImageInputDto>(inputDto);
            input.File = new SingleFilebaseDto { FileDto = new FileDto { 
                                                                        File = fileDto.File , 
                                                                        ContainerName = fileDto.ContainerName ,
                                                                        FileBaseDirectory = fileDto.FileBaseDirectory ,
                                                                        FilePath = fileDto.FilePath 
                                                                        } };
            
            input.Id = CurrentUserId;
            var output = await _customerBLL.UploadImage(input);
            return Ok(output);

        }




        [HttpPost("VersionDownloadCount")]
        public async Task<IActionResult> UpdateVersionDownloadCount(UpdateVersionDownloadCountInputDto inputDto)
        {
            if (inputDto.CustomerId != null)
                inputDto.CustomerId = CurrentUserId;
            var output = await _customerBLL.UpdateVersionDownloadCount(inputDto);
            return Ok(output);

        }

        [HttpPost("AddOnDownloadCount")]
        public async Task<IActionResult> UpdateAddOnDownloadCount(UpdateAddOnDownloadCountInputDto inputDto)
        {
            if (inputDto.CustomerId != null)
                inputDto.CustomerId = CurrentUserId;
            var output = await _customerBLL.UpdateAddOnDownloadCount(inputDto);
            return Ok(output);
        }


        #region TestCustomerValidate
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Test/customer/verfied/{customerMobileId}")]
        public async Task<IActionResult> TestMakeCustomerVerfied(int customerMobileId)
        {
            var output = await _customerBLL.TestValidateMobile(customerMobileId);
            return Ok(output);

        }
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Test/customer/registered/{customerEmailId}")]
        public async Task<IActionResult> TestMakeCustomerRegistered(int customerEmailId)
        {
            var output = await _customerBLL.TestValidateEmail(customerEmailId);
            return Ok(output);

        }
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Test/customer/delete/{customerId}")]
        public async Task<IActionResult> TestDeleteCustomer(int customerId)
        {
            var output = await _customerBLL.TestDeleteCustomer(customerId);
            return Ok(output);

        }
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("Test/customer/Delete")]
        public async Task<IActionResult> TestDeleteCustomerByMobileOrEmail(string userName)
        {
            var output = await _customerBLL.DeleteCustomerByEmailOrPhonrAsync(userName);
            return Ok(output);

        }
        #endregion
        //[HttpPost("TestLogCreateCustomer")]
        //public async Task<IActionResult> TestLogCreateCustomer( PreregisterDto inputDto )
        //{
        //    var output = await _customerBLL.CreateAsync(inputDto);
        //    return Ok(output);

        //}
        //[HttpPost("TestLogUpdateCustomer")]
        //public async Task<IActionResult> TestLogUpdateCustomer( UpdateCustomerInputDto inputDto )
        //{
        //    inputDto.Id = CurrentUserId;
        //    var output = await _customerBLL.UpdateAysnc(inputDto);
        //    return Ok(output);

        //}
        //[HttpPost("TestLogDeleteCustomer")]
        //public async Task<IActionResult> TestLogDeleteCustomer( DeleteCustomerInputDto inputDto )
        //{
        //    inputDto.Id = inputDto.Id;
        //    var output = _customerBLL.Delete(inputDto);
        //    return Ok(output);

        //}
    }
}
