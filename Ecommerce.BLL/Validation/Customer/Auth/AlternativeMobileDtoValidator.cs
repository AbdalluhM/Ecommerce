using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class AlternativeMobileDtoValidator : DtoValidationAbstractBase<AlternativeMobileDto>
    {
        public AlternativeMobileDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.Mobile)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
