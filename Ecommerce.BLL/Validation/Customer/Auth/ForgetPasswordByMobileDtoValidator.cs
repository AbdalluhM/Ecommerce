using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class ForgetPasswordByMobileDtoValidator : DtoValidationAbstractBase<ForgetPasswordByMobileDto>
    {
        public ForgetPasswordByMobileDtoValidator()
        {
            RuleFor(x => x.Username)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            When(x => x.Username.Contains("@"), () =>
            {
                RuleFor(x => x.Username)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                    .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);
            });
        }
    }
}
