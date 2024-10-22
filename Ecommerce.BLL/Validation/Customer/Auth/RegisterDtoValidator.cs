using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class RegisterDtoValidator : DtoValidationAbstractBase<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.CompanyName)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.IndustryId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.CompanySizeId)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Email)
                .NotNull().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail);

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
