using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDexef.BLL.Customers.Wishlist;
using MyDexef.DTO.Customers.Wishlist;
using MyDexef.DTO.Customers.WishlistApplication;

namespace MyDexef.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistAddOnController : ControllerBase
    {
        #region Fields

        private readonly IWishlistAddonBLL _WishlistAddOnBLL;
        #endregion
        #region Constructor

        public WishlistAddOnController(IWishlistAddonBLL wishlistAddonBLL)
        {
            _WishlistAddOnBLL = wishlistAddonBLL;
        }
        #endregion

        #region Actions 
        [HttpGet]
        [Route("AddOn/GetAll")]
        public IActionResult GetAll(int customerId)
        {
            var Output = _WishlistAddOnBLL.GetAllAsync(customerId);
            return Ok(Output);
        }

        [HttpPost]
        [Route("AddOn/Create")]

        public IActionResult Create(CreateWihlistAddOnInputDto inputDto)
        {
            var output = _WishlistAddOnBLL.Create(inputDto);
            return Ok(output);

        }
        [HttpPost]
        [Route("AddOn/Delete")]
        public IActionResult Delete(DeleteWihlistAddOnInputDto inputDto)
        {
            var output= _WishlistAddOnBLL.Delete(inputDto);
            return Ok(output);
        }
        #endregion
    }
}

