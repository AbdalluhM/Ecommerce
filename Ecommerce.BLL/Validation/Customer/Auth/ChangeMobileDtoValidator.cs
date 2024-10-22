using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    internal class ChangeMobileDtoValidator : DtoValidationAbstractBase<ChangeMobileDto>
    {
        public ChangeMobileDtoValidator()
        {
            RuleFor(x => x.MobileId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.NewMobile)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }

    
}
