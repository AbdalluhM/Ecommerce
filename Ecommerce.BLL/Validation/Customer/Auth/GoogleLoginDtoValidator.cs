using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class GoogleLoginDtoValidator : DtoValidationAbstractBase<OAuthRegisterDto>
    {
        public GoogleLoginDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Mobile)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            
            RuleFor(x => x.CountryCode)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            
            RuleFor(x => x.SourceId)
                .IsInEnum().WithDXErrorCode(MessageCodes.InvalidSource);
        }
    }
}
