using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class ForgetPasswordDtoValidator : DtoValidationAbstractBase<ForgetPasswordDto>
    {
        public ForgetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);
        }
    }
}
