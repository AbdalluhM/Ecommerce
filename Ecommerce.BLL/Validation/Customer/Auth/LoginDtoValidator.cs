using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class LoginDtoValidator : DtoValidationAbstractBase<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
               // .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);

            RuleFor(x => x.Password)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
    public class LoginByMobileDtoValidator : DtoValidationAbstractBase<LoginMobileDto>
    {
        public LoginByMobileDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                //.EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);

            RuleFor(x => x.Password)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}


