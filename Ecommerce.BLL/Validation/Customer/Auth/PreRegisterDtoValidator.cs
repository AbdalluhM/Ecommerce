using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class PreregisterDtoValidator : DtoValidationAbstractBase<PreregisterDto>
    {
        public PreregisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithDXErrorCode(DXConstants.MessageCodes.GreaterThanZero);
            
            RuleFor(x => x.Mobile)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
        }
    }
}
