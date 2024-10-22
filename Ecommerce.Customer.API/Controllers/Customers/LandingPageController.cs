using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using MyDexef.BLL.Customers;
using MyDexef.Core.Consts.Auth;
using MyDexef.Core.Enums.Settings.Files;
using MyDexef.DTO.Customers;
using MyDexef.DTO.Customers.Auth.Inputs;
using MyDexef.DTO.Settings.Files;

using System.Threading.Tasks;
using static MyDexef.DTO.Customers.CustomerDto;

namespace MyDexef.Customer.API.Controllers.Customers
{
    [Route("api/LandingPage")]
    [ApiController]
    public class LandingPageController : BaseCustomerController
    {
        private readonly ILandingPageBLL _landingPageBLL;

        public LandingPageController( ILandingPageBLL landingPageBLL ,IHttpContextAccessor contextAccessor):base ( contextAccessor)
        {
            _landingPageBLL = landingPageBLL;
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task< IActionResult> Update( RegisterLandingPageInputDto inputDto )
        {
            var output =await _landingPageBLL.UpdateAysnc(inputDto);
            return Ok(output);

        }

   
  
    }
}
