using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class RefreshTokenDtoValidator : DtoValidationAbstractBase<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.RefreshToken)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
