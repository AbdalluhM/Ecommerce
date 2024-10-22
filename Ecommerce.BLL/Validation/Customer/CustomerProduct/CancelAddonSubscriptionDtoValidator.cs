using FluentValidation;
using Ecommerce.DTO.Customers.CustomerProduct;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.CustomerProduct
{
    public class CancelAddonSubscriptionDtoValidator : DtoValidationAbstractBase<CancelAddonSubscriptionDto>
    {
        public CancelAddonSubscriptionDtoValidator()
        {
            RuleFor(x => x.AddonSubscriptionId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.Reason)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)

                .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);
        }
    }
}
