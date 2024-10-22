using FluentValidation;
using Ecommerce.DTO.Customers;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer
{
    public class CheckMobileExistDtoValidator : DtoValidationAbstractBase<CheckMobileExistDto>
    {
        public CheckMobileExistDtoValidator()
        {
            RuleFor(x => x.Mobile)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
