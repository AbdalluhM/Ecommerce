using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Enums;
using Ecommerce.DTO.Employees;

using System.Linq;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation
{

    #region  Assign Employee To CountryCurrency
    internal class AssignEmployeeToCountryInputDtoValidator : DtoValidationAbstractBase<AssignEmployeeToCountryInputDto>
    {
        public AssignEmployeeToCountryInputDtoValidator( )
        {
            //Validate Employee
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required).GreaterThan(0)
                .WithDXErrorCode(MessageCodes.GreaterThanZero);
            //Validate Country Currency
            RuleFor(x => x.CountryCurrencyId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }
    }

    internal class UpdateAssignEmployeeToCountryInputDtoValidator : DtoValidationAbstractBase<UpdateAssignEmployeeToCountryInputDto>
    {
        public UpdateAssignEmployeeToCountryInputDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            //Validate Employee
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            //Validate Country Currency
            RuleFor(x => x.CountryCurrencyId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }
    }
    internal class GetAssignedEmployeeToCountryInputDtoValidator : DtoValidationAbstractBase<GetAssignedEmployeeToCountryInputDto>
    {
        public GetAssignedEmployeeToCountryInputDtoValidator( )
        {
            //Validate Employee Country Currency Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);



        }
    }
    #endregion
    #region Employee
    internal class CreateEmployeeInputDtoValidator : DtoValidationAbstractBase<CreateEmployeeInputDto>
    {
        public CreateEmployeeInputDtoValidator( )
        {
            //Validate Email
            RuleFor(x => x.Email)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail)
                .MaximumLength(250);

            //Validate UserName
            RuleFor(x => x.UserName)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .MaximumLength(150).WithDXErrorCode(MessageCodes.InvalidMaxLength,150);

            When(x => x.RoleId != null, () =>
            {
                RuleFor(x => x.RoleId)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero)
               ;
            });
            //Validate Name
            RuleFor(x => x.Name)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .MaximumLength(50).WithDXErrorCode(MessageCodes.InvalidMaxLength, 50);

            //Validate Password

            RuleFor(x => x.Password)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
                 .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
                .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
                .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);



            //Validate EmployeeTypeId
            //TODO:add check for EmployeeTypes after creating it


            //Validate EmployeeCountries
            When(x => x.IsAdminForOtherCountries == false, () =>
            {
                RuleFor(x => x.EmployeeCountries)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .Must(x => x.Count() > 0).WithDXErrorCode(MessageCodes.CountrySelected);
                RuleFor(x => x.EmployeeCountries)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required).
                    Must(x => x.TrueForAll(c => c > 0)).WithDXErrorCode(MessageCodes.Required);
              
            });

            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File)
                   .NotEmpty(MessageCodes.Required)
                   .SetValidator(new FileDtoValidator());
            });
        }
    }

    internal class UpdateEmployeeInputDtoValidator : DtoValidationAbstractBase<UpdateEmployeeInputDto>
    {
        public UpdateEmployeeInputDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.RoleId != null, () =>
            {
                RuleFor(x => x.RoleId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });
            //Validate Name
            When(x => x.Name != null, ( ) =>
            {
                RuleFor(x => x.Name)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .MaximumLength(50).WithDXErrorCode(MessageCodes.InvalidMaxLength, 50);
            });
            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File)
                   .NotEmpty(MessageCodes.Required)
                   .SetValidator(new FileDtoValidator());
            });

            When(x => !string.IsNullOrWhiteSpace(x.Mobile), ( ) =>
            {
                RuleFor(x => x.Mobile)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            });

            When(x => x.IsAdminForOtherCountries == false, () =>
            {
                RuleFor(x => x.EmployeeCountries)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .Must(x => x.Count() > 0).WithDXErrorCode(MessageCodes.CountrySelected);
                RuleFor(x => x.EmployeeCountries)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required).
                    Must(x => x.TrueForAll(c => c > 0)).WithDXErrorCode(MessageCodes.Required);
            });
            #region old
            ////Validate Email
            //When(x => x.Email != null, ( ) =>
            //{
            //    RuleFor(x => x.Email)
            //    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //    .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail)
            //    .MaximumLength(250).WithDXErrorCode(MessageCodes.InvalidMaxLength,  250);
            //});

            ////Validate Password
            //When(x => !string.IsNullOrEmpty(x.Password), ( ) =>
            //{

            //    RuleFor(x => x.Password)
            //        .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //        .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
            //           .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
            //        .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
            //        .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);
            //});


            //Validate EmployeeTypeId
            // RuleFor(x => x.EmployeeTypeId).NotEmpty().WithDXErrorCode(MessageCodes.Required).GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            ////Validate EmployeeCountries
            //RuleFor(x => x.EmployeeCountries)
            //   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //   .Must(x => x.Count() > 0).WithDXErrorCode(MessageCodes.InvalidItemsCount);
            //RuleFor(x => x.EmployeeCountries)
            //    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //    .Must(x => x.TrueForAll(c => c > 0)).WithDXErrorCode(MessageCodes.Required);
            #endregion

        }
    }

    internal class GetEmployeeInputDtoValidator : DtoValidationAbstractBase<GetEmployeeInputDto>
    {
        public GetEmployeeInputDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }
    }
    internal class LoginModelInputDtoValidator : DtoValidationAbstractBase<LoginModelInputDto>
    {
        public LoginModelInputDtoValidator( )
        {
            //Validate UserName
            RuleFor(x => x.UserName)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            //Validate Password
            RuleFor(x => x.Password)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }
    #endregion
    #region ValiationEnum
    public enum EmployeeValidation
    {
        Name,
        UserName,
        Email
    }
    #endregion
}
