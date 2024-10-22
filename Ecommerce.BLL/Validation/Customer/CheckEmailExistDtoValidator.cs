using FluentValidation;
using Ecommerce.DTO.Customers;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer
{
    public class CheckEmailExistDtoValidator : DtoValidationAbstractBase<CheckEmailExistDto>
    {
        public CheckEmailExistDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);
        }
    }
}
