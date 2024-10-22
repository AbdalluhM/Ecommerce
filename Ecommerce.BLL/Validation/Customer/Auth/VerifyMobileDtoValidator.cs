using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class VerifyMobileDtoValidator : DtoValidationAbstractBase<VerifyMobileDto>
    {
        public VerifyMobileDtoValidator()
        {
            RuleFor(x => x.MobileId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            
            RuleFor(x => x.Code)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
