using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class OAuthLoginDtoValidator : DtoValidationAbstractBase<OAuthLoginDto>
    {
        public OAuthLoginDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
