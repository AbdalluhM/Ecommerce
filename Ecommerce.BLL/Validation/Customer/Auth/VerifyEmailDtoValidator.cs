using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class VerifyEmailDtoValidator : DtoValidationAbstractBase<VerifyEmailDto>
    {
        public VerifyEmailDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);     
        }
    }
}
