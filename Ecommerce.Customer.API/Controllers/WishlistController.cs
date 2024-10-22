using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.Wishlist;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : BaseCustomerController
    {
        #region Fields

        private readonly IWishlistApplicationBLL _WishlistApplicationBLL;
        private readonly IWishlistAddonBLL _WishlistAddonBLL;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WishListController(IWishlistApplicationBLL WishlistApplicationBLL, IWishlistAddonBLL WishlistAddonBLL, IMapper mapper,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _WishlistApplicationBLL = WishlistApplicationBLL;
            _WishlistAddonBLL = WishlistAddonBLL;
            _mapper = mapper;
        }
        #endregion

        #region Actions 
        #region Application
        [Authorize]
        [HttpGet]
        [Route("Application/GetAll")]
        public async Task<IActionResult> GetAllApplications([FromQuery] WishlistApplicationSearchApiInputDto inputDto)
        {
            var result = _mapper.Map<WishlistApplicationSearchInputDto>(inputDto);
            result.CustomerId = CurrentUserId;
            var Output = await _WishlistApplicationBLL.GetAllAsync(result);
            return Ok(Output);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost]
        [Route("Application/Create")]
        public IActionResult CreateApplication([FromBody]CreateWishlistApplicationInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = _WishlistApplicationBLL.Create(inputDto);
            return Ok(output);

        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost]
        [Route("Application/Delete")]
        public IActionResult DeleteApplication( int ApplicationId )
        {
            var output = _WishlistApplicationBLL.Delete(new DeleteWishlistApplicationInputDto { ApplicationId = ApplicationId ,
            CustomerId=CurrentUserId});
            return Ok(output);
        }
        //[HttpGet]
        //[Route("Application/GetPagedList")]
        //public async Task<IActionResult> GetPagedListAaaplicationAsync([FromQuery] CustomerFilteredResultRequestDto inputDto)
        //{
        //    var output = await _WishlistApplicationBLL.GetPagedListAsync(inputDto);
        //    return Ok(output);
        //}
        #endregion
        #region AddOn
        [Authorize]
        [HttpGet]
        [Route("AddOn/GetAll")]
        public async Task<IActionResult> GetAllAddOns([FromQuery] WishlistAddOnSearchApIInputDto inputDto)
        {
            var result =_mapper.Map<WishlistAddOnSearchInputDto>(inputDto); 
            result.CustomerId=CurrentUserId;
            var Output = await _WishlistAddonBLL.GetAllAsync(result);
            return Ok(Output);
        }
        
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost]
        [Route("AddOn/Create")]
        public IActionResult CreateAddOn([FromBody]CreateWishlistAddOnInputDto inputDto)
        {

            inputDto.CustomerId = CurrentUserId;
            var output = _WishlistAddonBLL.Create(inputDto);
            return Ok(output);

        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost]
        [Route("AddOn/Delete")]
        public IActionResult DeleteAddOn(int AddOnId)
        {
            var output = _WishlistAddonBLL.Delete(new DeleteWishlistAddOnInputDto { AddOnId = AddOnId ,
            CustomerId= CurrentUserId
            } );
            return Ok(output);
        }
        //[HttpGet]
        //[Route("AddOn/GetPagedList")]
        //public async Task<IActionResult> GetPagedListAddOnAsync([FromQuery] CustomerFilteredResultRequestDto inputDto)
        //{
        //    var output = await _WishlistAddonBLL.GetPagedListAsync(inputDto);
        //    return Ok(output);
        //}
        #endregion
        #endregion
    }
}

