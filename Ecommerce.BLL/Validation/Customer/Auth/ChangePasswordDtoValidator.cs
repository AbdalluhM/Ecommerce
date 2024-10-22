using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class ChangePasswordDtoValidator : DtoValidationAbstractBase<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.Token)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Password)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
                .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
                .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
                .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);
        }
    }
}
