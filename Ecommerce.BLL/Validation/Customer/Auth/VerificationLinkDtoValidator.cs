using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class VerificationLinkDtoValidator : DtoValidationAbstractBase<VerificationLinkDto>
    {
        public VerificationLinkDtoValidator()
        {
            RuleFor(x => x.EmailId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Code)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
