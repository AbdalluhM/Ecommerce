using FluentValidation;
using Ecommerce.DTO.Customers;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer
{
    public class RegisterLandingPageInputDtoValidator : DtoValidationAbstractBase<PreregisterLandingPageDto>
    {
        public RegisterLandingPageInputDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);

            RuleFor(x => x.SourceId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Password)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
                .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
                .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
                .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);

            RuleFor(x => x.CountryCode)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Mobile)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
}
