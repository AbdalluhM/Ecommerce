using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.HomePage;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.HomePage.HomePageDto;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomePageController : BaseCustomerController
    {
        private readonly IHomePageBLL _homePageBLL;
        private readonly IMapper _mapper;
        public HomePageController(IHomePageBLL homePageBLL, IMapper mapper,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _homePageBLL = homePageBLL;
            _mapper = mapper;
        }
        
        [HttpGet("GetPagedList/AppTagsCustomer")]
        public async Task<IActionResult> GetPagedList([FromQuery] FilterCustomerAppTagsApiInputDto inputDto)
        {
            var input = _mapper.Map<FilterCustomerAppTagsInputDto>(inputDto);
            input.CustomerId = CurrentUserId;
            input.CountryId = CurrentUserCountryId;
            var output = await _homePageBLL.GetCustomerAppTagsoRHighlighted(input);
            return Ok(output);


        }
    }
}
