using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.Auth;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.DTO.Customers.Auth.Inputs;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : BaseCustomerController
    {
        private readonly IAuthBLL _authBLL;

        public SecurityController(IAuthBLL authBLL,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _authBLL = authBLL;
        }

        [Authorize]
        [HttpGet("GetEmails")]
        public async Task<IActionResult> GetEmailsAsnyc()
        {
            var response = await _authBLL.GetEmailsAsnyc(CurrentUserId);

            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("VerifyAlternativeEmail")]
        public async Task<IActionResult> VerifyAlternativeEmailAsync(AlternativeEmailDto alternativeEmailDto)
        {
            var response = await _authBLL.VerifyAlternativeEmailAsync(alternativeEmailDto);

            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("MakeEmailPrimary/{id}")]
        public async Task<IActionResult> MakeEmailPrimaryAsync(int id)
        {
            var response = await _authBLL.MakeEmailPrimaryAsync(id);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetMobiles")]
        public async Task<IActionResult> GetMobilesAsync()
        {
            var response = await _authBLL.GetMobilesAsync(CurrentUserId);

            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("VerifyAlternativeMobile")]
        public async Task<IActionResult> VerifyAlternativeMobileAsync(AlternativeMobileDto alternativeMobileDto)
        {
            var response = await _authBLL.VerifyAlternativeMobileAsync(alternativeMobileDto);

            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("MakeMobilePrimary/{id}")]
        public async Task<IActionResult> MakeMobilePrimaryAsync(int id)
        {
            var response = await _authBLL.MakeMobilePrimaryAsync(id);

            return Ok(response);
        }

        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("UpdateMobile")]
        public async Task<IActionResult> UpdateMobile(ChangeMobileDto updateMobileDto)
        {
            var response = await _authBLL.UpdateMobileAsync(updateMobileDto, CurrentUserId);

            return Ok(response);
        }
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("VerifyUpdateMobile")]
        public async Task<IActionResult> VerifyUpdateMobile(VerifyUpdateMobileDto verifyUpdateMobileDto)
        {
            var response = await _authBLL.VerifyUpdateMobile(verifyUpdateMobileDto, CurrentUserId);

            return Ok(response);
        }
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = await _authBLL.ResetPasswordAsync(resetPasswordDto);

            return Ok(response);
        }
    }
}
