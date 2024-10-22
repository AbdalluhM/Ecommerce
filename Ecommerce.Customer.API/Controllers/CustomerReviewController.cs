using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.Reviews.Admins;
using Ecommerce.BLL.Customers.Reviews.Customers;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerReviewController : BaseCustomerController
    {
        private readonly ICustomerReviewBLL _customerReviewBLL;
        private readonly IAdminReviewBLL _adminReviewBLL;

        public CustomerReviewController(IAdminReviewBLL adminReviewBLL,
                                        ICustomerReviewBLL customerReviewBLL,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)

        {
            _adminReviewBLL = adminReviewBLL;
            _customerReviewBLL = customerReviewBLL;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> MakeNewReviewAsync(NewReviewDto newReviewDto)
        {
            newReviewDto.CustomerId = CurrentUserId;
            var response = await _customerReviewBLL.MakeNewReviewAsync(newReviewDto);

            return Ok(response);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateReviewAsync(EditReviewDto editReviewDto)
        {
            var response = await _customerReviewBLL.UpdateReviewAsync(editReviewDto);

            return Ok(response);
        }

        [HttpPost("Delete")]
        public IActionResult DeleteReview(int id)
        {
            var output = _adminReviewBLL.Delete(new DeleteCustomerReviewInputDto { Id = id });
            return Ok(output);
        }

        [HttpGet("{appId}")]
        //ToDo:remove customerId
        public async Task<IActionResult> GetCustomerReviewAsync(int appId)
        {
            var response = await _customerReviewBLL.GetCustomerReviewAsync(appId, CurrentUserId);

            return Ok(response);
        }
        //ToDo:remove customerId
        [HttpGet("GetPagedList")]
        public IActionResult GetApplicationReviewsPagedList([FromQuery]FilteredResultRequestDto filterDto, int appId)
        {
            //ToDo
            var response = _customerReviewBLL.GetApplicationReviewsPagedList(filterDto, appId, CurrentUserId);

            return Ok(response);
        }
    }
}

