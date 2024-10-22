using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.BLL;
using Ecommerce.BLL.Customers.Auth;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthBLL _authBLL;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthController(IAuthBLL authBLL, TokenValidationParameters tokenValidationParameters, IHttpClientFactory httpClientFactory)
        {
            _authBLL = authBLL;
            _tokenValidationParameters = tokenValidationParameters;
            _httpClientFactory = httpClientFactory;
        }
        //[HttpGet("apple-signin")]
        //public IActionResult AppleSignIn()
        //{
        //    var authorizationUrl = GenerateAuthorizationUrl();
        //    return Redirect(authorizationUrl);
        //}
        //[HttpPost("redirect")]
        //public IActionResult redirect([FromQuery] string code, [FromQuery] string IdToken)
        //{
        //    return Ok($"code :{code}+  token : {IdToken}");

        //}


        //        private string GenerateAuthorizationUrl()
        //{
        //    var baseUrl = "https://appleid.apple.com/auth/authorize";
        //    var responseType = "code id_token";
        //    var scope = "name email";
        //    var responseMode = "query"; // You can also use "web_message" if needed

        //    var url = $"{baseUrl}?client_id=com.dexef.Ecommerce" +
        //              $"&redirect_uri={Uri.EscapeDataString("https://Ecommercecustomerapi.azurewebsites.net/api/Auth/redirect")}" +
        //              $"&response_type={Uri.EscapeDataString(responseType)}" +
        //              $"&state={Uri.EscapeDataString("uijhknmkjljkljmffkjkkkm867")}" +
        //              $"&scope={Uri.EscapeDataString(scope)}" +
        //              $"&response_mode={Uri.EscapeDataString(responseMode)}";

        //    return url;
        //}
        [HttpGet("CheckEmailExist")]
        public async Task<IActionResult> CheckEmailExistAsync(string email)
        {
            var respose = await _authBLL.IsEmailExistAsync(new CheckEmailExistDto { Email = email });

            return Ok(respose);
        }

        [HttpGet("CheckMobileExist")]
        public async Task<IActionResult> CheckMobileExistAsync(string mobile)
        {
            var respose = await _authBLL.IsMobileExistAsync(new CheckMobileExistDto { Mobile = mobile });

            return Ok(respose);
        }

        [HttpPost("RegisterLandingPage")]
        public async Task<IActionResult> RegisterLandingPage(PreregisterLandingPageDto preregsiterLangingPageDto)
        {
            var respose = await _authBLL.PreregisterLandingPageAsync(preregsiterLangingPageDto);

            return Ok(respose);
        }

        [HttpPost("SendSmsVerification")]
        public async Task<IActionResult> SendSmsVerification(int mobileId, bool isWatsAPP)
        {
            var userLangs = Request.Headers[DXConstants.SupportedLanguage.RequestHeader].ToString();
            var response = await _authBLL.SendSmsVerificationAsync(mobileId, isWatsAPP, userLangs);

            return Ok(response);
        }

        [HttpPost("Preregister")]
        public async Task<IActionResult> PreregisterAsync(PreregisterDto preRegisterDto)
        {
            var respose = await _authBLL.PreregisterAsync(preRegisterDto);

            return Ok(respose);
        }

        [HttpPost("ResendMobileCode/{id}")]
        public async Task<IActionResult> ResendMobileCodeAsync(int id, bool isWhatsapp)
        {
            var response = await _authBLL.ResendMobileCodeAsync(new ResendMobileCodeDto(id, SmsTypeEnum.Verification, isWhatsapp));

            return Ok(response);
        }

        [HttpPost("ResendForgetPassowrdMobileCode/{id}")]
        public async Task<IActionResult> ResendForgetPassowrdMobileCode(int id, bool isWhatsapp)
        {
            var response = await _authBLL.ResendMobileCodeAsync(new ResendMobileCodeDto(id, SmsTypeEnum.ForgetPassword, isWhatsapp));

            return Ok(response);
        }

        [HttpPost("VerifyMobile")]
        public async Task<IActionResult> VerifyMobileAsync(VerifyMobileDto verifyMobileDto)
        {
            var response = await _authBLL.VerifyMobileAsync(verifyMobileDto);

            return Ok(response);
        }

        [HttpPost("VerifyForgetPasswordMobileCode")]
        public async Task<IActionResult> VerifyForgetPasswordMobileCodeAsync(ForgetPasswordVerifyMobileDto verifyMobileDto)
        {
            var response = await _authBLL.VerifyForgetPasswordMobileCodeAsync(verifyMobileDto);

            return Ok(response);
        }

        [HttpPost("ChangeMobile")]
        public async Task<IActionResult> ChangeMobileAsync(ChangeMobileDto newMobileDto)
        {
            var response = await _authBLL.ChangeMobileAsync(newMobileDto);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _authBLL.RegisterAsync(registerDto);

            return Ok(response);
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            var response = await _authBLL.VerifyEmailAsync(verifyEmailDto);

            return Ok(response);
        }

        [HttpPost("ResendVerificationEmailLink/{id}")]
        public async Task<IActionResult> ResendVerificationEmailLinkAsync(int id)
        {
            var response = await _authBLL.ResendVerificationEmailLinkAsync(id);

            return Ok(response);
        }

        [HttpPost("RegisterByGoogle")]
        public async Task<IActionResult> RegisterByGoogleAsync(OAuthRegisterDto googleRegisterDto)
        {
            var response = await _authBLL.RegisterByGoogleAsync(googleRegisterDto);

            return Ok(response);
        }
        //[HttpPost("RegisterByApple")]
        //public async Task<IActionResult> RegisterByAppleAsync(AppleRegisterDto appleRegisterDto)
        //{
        //    var response = await _authBLL.RegisterByAppleAsync(appleRegisterDto);

        //    return Ok(response);
        //}
        //[HttpPost("LoginByApple")]
        //public async Task<IActionResult> LoginByAppleAsync(AppleLoginDto loginDto)
        //{
        //    var response = await _authBLL.LoginByAppleAsync(loginDto);

        //    return Ok(response);
        //}

        [HttpPost("RegisterByApple")]
        public async Task<IActionResult> RegisterByAppleAsync(OAuthRegisterDto appleRegisterDto)
        {
            var response = await _authBLL.RegisterByAppleAsync(appleRegisterDto);

            return Ok(response);
        }
        [HttpPost("LoginByApple")]
        public async Task<IActionResult> LoginByAppleAsync(OAuthLoginDto loginDto)
        {
            var response = await _authBLL.LoginByAppleAsync(loginDto);

            return Ok(response);
        }


        [HttpPost("LoginByGoogle")]
        public async Task<IActionResult> LoginByGoogleAsync(OAuthLoginDto loginDto)
        {
            var response = await _authBLL.LoginByGoogleAsync(loginDto);

            return Ok(response);
        }

        [HttpPost("RegisterByFaceBook")]
        public async Task<IActionResult> RegisterByFaceBookAsync(OAuthRegisterDto loginDto)
        {
            var response = await _authBLL.RegisterByFaceBookAsync(loginDto);

            return Ok(response);
        }

        [HttpPost("LoginByFaceBook")]
        public async Task<IActionResult> LoginByFaceBookAsync(OAuthLoginDto loginDto)
        {
            var response = await _authBLL.LoginByFaceBookAsync(loginDto);

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var response = await _authBLL.LoginAsync(loginDto);

            return Ok(response);
        }
        [HttpPost("ValidateEmail")]
        public async Task<IActionResult> ValidateEmail(string userName)
        {
            var response = await _authBLL.ValidateEmailAsync(userName);

            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var response = await _authBLL.RefreshTokenAsync(refreshTokenDto, _tokenValidationParameters);

            if (response.IsSuccess)
                return Ok(response);
            else
                return Unauthorized();
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync(LogoutDto logoutDto)
        {
            var response = await _authBLL.LogoutAsync(logoutDto);

            return Ok(response);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            var response = await _authBLL.ForgetPasswordAsync(forgetPasswordDto);

            return Ok(response);
        }

        [HttpPost("ForgetPasswordByMobile")]
        public async Task<IActionResult> ForgetPasswordByMobileAsync(ForgetPasswordByMobileDto forgetPasswordDto)
        {
            var response = await _authBLL.ForgetPasswordByMobileAsync(forgetPasswordDto);

            return Ok(response);
        }

        [HttpPost("ResendForgetPasswordEmailLink/{id}")]
        public async Task<IActionResult> ResendForgetPasswordEmailLinkAsync(int id)
        {
            var response = await _authBLL.ResendForgetPasswordEmailLinkAsync(id);

            return Ok(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var response = await _authBLL.ChangePasswordAsync(changePasswordDto/*, _tokenValidationParameters*/);

            return Ok(response);
        }

        [HttpPost("ChangePasswordByMobileCode")]
        public async Task<IActionResult> ChangePasswordByMobileCodeAsync(ChangePasswordByMobileCodeDto changePasswordDto)
        {
            var response = await _authBLL.ChangePasswordByMobileCodeAsync(changePasswordDto);

            return Ok(response);
        }


    }
}
