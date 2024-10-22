using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDexef.BLL.Customers.Wishlist;
using MyDexef.DTO.Customers.WishlistApplication;

namespace MyDexef.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistApplicationController : ControllerBase
    {
        //#region Fields

        //private readonly IWishlistApplicationBLL _WishlistApplicationBLL;
        //#endregion
        //#region Constructor

        //public WishlistApplicationController(IWishlistApplicationBLL WishlistApplicationBLL)
        //{
        //    _WishlistApplicationBLL = WishlistApplicationBLL;
        //}
        //#endregion

        //#region Actions 
        //[HttpGet()]
        //[Route("Application/GetAll")]
        //public IActionResult GetAll(int customerId)
        //{
        //    var Output = _WishlistApplicationBLL.GetAllAsync(customerId);
        //    return Ok(Output);
        //}
        //[HttpPost]
        //[Route("Application/Create")]
        //public IActionResult Create(CreateWihlistApplicationInputDto inputDto)
        //{
        //    var output = _WishlistApplicationBLL.Create(inputDto);
        //    return Ok(output);

        //}

        //[Route("Application/Delete")]
        //public IActionResult Delete(DeleteWihlistApplicationInputDto inputDto)
        //{
        //    var output= _WishlistApplicationBLL.Delete(inputDto);
        //    return Ok(output);
        //}
        //#endregion
    }
}

