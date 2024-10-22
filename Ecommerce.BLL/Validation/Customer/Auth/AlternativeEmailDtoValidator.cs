using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class AlternativeEmailDtoValidator : DtoValidationAbstractBase<AlternativeEmailDto>
    {
        public AlternativeEmailDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.Email)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);
        }
    }
}
