using Microsoft.IdentityModel.Tokens;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using Ecommerce.DTO.Customers.Auth.Outputs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Auth
{
    public interface IAuthBLL
    {
        Task<IResponse<CustomerEmailExistDto>> IsEmailExistAsync(CheckEmailExistDto checkEmailExistDto);
        Task<IResponse<CustomerMobileExistDto>> IsMobileExistAsync(CheckMobileExistDto checkMobileExistDto);

        Task<IResponse<PreregisterResultDto>> PreregisterLandingPageAsync(PreregisterLandingPageDto preregisterLandingPageDto);
        Task<IResponse<PreregisterResultDto>> SendSmsVerificationAsync(int mobileId, bool isWatsApp, string lang);
        Task<IResponse<PreregisterResultDto>> PreregisterAsync(PreregisterDto preRegisterDto);
        Task<IResponse<PreregisterResultDto>> ResendMobileCodeAsync(ResendMobileCodeDto resendMobileCodeDto);
        Task<IResponse<PreregisterResultDto>> VerifyMobileAsync(VerifyMobileDto verifyMobileDto);
        Task<IResponse<bool>> VerifyForgetPasswordMobileCodeAsync(ForgetPasswordVerifyMobileDto verifyMobileDto);
        Task<IResponse<PreregisterResultDto>> ChangeMobileAsync(ChangeMobileDto newMobileDto);

        Task<IResponse<PreregisterResultDto>> RegisterAsync(RegisterDto registerDto);
        Task<IResponse<PreregisterResultDto>> VerifyEmailAsync(VerifyEmailDto verifyEmailDto);
        Task<IResponse<PreregisterResultDto>> ResendVerificationEmailLinkAsync(int emailId);

        Task<IResponse<PreregisterResultDto>> RegisterByGoogleAsync(OAuthRegisterDto loginDto);
        Task<IResponse<TokenResultDto>> LoginByGoogleAsync(OAuthLoginDto loginDto);

        Task<IResponse<PreregisterResultDto>> RegisterByAppleAsync(OAuthRegisterDto appleRegisterDto);
        Task<IResponse<TokenResultDto>> LoginByAppleAsync(OAuthLoginDto loginDto);

        //Task<IResponse<PreregisterResultDto>> RegisterByAppleAsync(AppleRegisterDto appleRegisterDto);
        //Task<IResponse<TokenResultDto>> LoginByAppleAsync(AppleLoginDto loginDto);
        Task<IResponse<PreregisterResultDto>> RegisterByFaceBookAsync(OAuthRegisterDto loginDto);
        Task<IResponse<TokenResultDto>> LoginByFaceBookAsync(OAuthLoginDto loginDto);

        Task<IResponse<TokenResultDto>> LoginAsync(LoginDto loginDto);
        Task<IResponse<ValidateEmailResultDto>> ValidateEmailAsync(string userName);
        Task<IResponse<TokenResultDto>> LoginByMobileAsync(LoginMobileDto loginDto);

        Task<IResponse<TokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, TokenValidationParameters tokenValidationParameters);
        Task<IResponse<bool>> LogoutAsync(LogoutDto logoutDto);

        Task<IResponse<PreregisterResultDto>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        Task<IResponse<PreregisterResultDto>> ForgetPasswordByMobileAsync(ForgetPasswordByMobileDto forgetPasswordDto);
        Task<IResponse<PreregisterResultDto>> ResendForgetPasswordEmailLinkAsync(int emailId);
        Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto/*, TokenValidationParameters tokenValidationParameters*/);
        Task<IResponse<bool>> ChangePasswordByMobileCodeAsync(ChangePasswordByMobileCodeDto changePasswordDto);


        Task<IResponse<IEnumerable<AlternativeEmailResultDto>>> GetEmailsAsnyc(int customerId);
        Task<IResponse<PreregisterResultDto>> VerifyAlternativeEmailAsync(AlternativeEmailDto alternativeEmailDto);
        Task<IResponse<bool>> MakeEmailPrimaryAsync(int emailId);

        Task<IResponse<IEnumerable<AlternativeMobileResultDto>>> GetMobilesAsync(int customerId);
        Task<IResponse<PreregisterResultDto>> VerifyAlternativeMobileAsync(AlternativeMobileDto alternativeMobileDto);
        Task<IResponse<bool>> MakeMobilePrimaryAsync(int mobileId);
        Task<IResponse<PreregisterResultDto>> UpdateMobileAsync(ChangeMobileDto newMobileDto, int customer);
        Task<IResponse<PreregisterResultDto>> VerifyUpdateMobile(VerifyUpdateMobileDto verifyMobileDto, int customer);

        Task<IResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// Create crm lead and assign leadId to customer with save.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<string> CreateCrmLeadAsync(int customerId);

        /// <summary>
        /// Create crm lead and assign leadId to customer without saving.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<string> CreateCrmLeadAsync(CustomerEmail email);

        Task<IResponse<bool>> ValidateEmailAndMobileAsync(PreregisterLandingPageDto landingPageDto);

    }
}
