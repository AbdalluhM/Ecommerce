using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Customers.Reviews.Admins;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : BaseAdminController
    {
        #region Fields

        private readonly IAdminReviewBLL _customerReviewBLL;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public ReviewController(IAdminReviewBLL customerReviewBLL,
                                        IMapper mapper,
                                         IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)

        {
            _customerReviewBLL = customerReviewBLL;
            _mapper = mapper;
        }
        #endregion

        #region Actions 

        [HttpGet("GetPagedList")]
        [DxAuthorize(PagesEnum.Reviews, ActionsEnum.READ)]
        public async Task<IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var output =await _customerReviewBLL.GetPagedListAsync(inputDto);
            return Ok(output);
        }
        [HttpPost("Submit")]
        [DxAuthorize(PagesEnum.Reviews, ActionsEnum.UPDATE)]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitCustomerReviewApiInputDto inputDto)
        {
            
            var input = _mapper.Map<SubmitCustomerReviewInputDto>(inputDto);
            input.ApprovedBy = CurrentEmployeeId;
            var result = await _customerReviewBLL.SubmitAsync(input);

            return Ok(result);
        }
        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.Reviews, ActionsEnum.DELETE)]
        public IActionResult DeleteReview(int id)
        {
            var output = _customerReviewBLL.Delete(new DeleteCustomerReviewInputDto { Id = id });
            return Ok(output);
        }
        #endregion
    }
}
