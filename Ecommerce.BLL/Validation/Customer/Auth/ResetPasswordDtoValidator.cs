using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class ResetPasswordDtoValidator : DtoValidationAbstractBase<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.CurrentPassword)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Password)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
                .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
                .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
                .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);
        }
    }
}
