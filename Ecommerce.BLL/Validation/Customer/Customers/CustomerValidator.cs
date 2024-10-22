using FluentValidation;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Employees;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.BLL.Validation.Customer.Customers
{
    public class CustomerValidator
    {
        internal class UpdateCustomerInputDtoValidator : DtoValidationAbstractBase<UpdateCustomerInputDto>
        {
            public UpdateCustomerInputDtoValidator()
            {

              

                    RuleFor(x => x.CompanySizeId)
                       //.NotEmpty(MessageCodes.Required)
                       .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero)
                       .When(x=>x.CompanySizeId != null);



                    RuleFor(x => x.IndustryId)
                     //.NotEmpty(MessageCodes.Required)
                     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero)
                     .When(x=>x.IndustryId != null);

             


                When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
                {
                    RuleFor(a => a.Name)
                     .NotEmpty(MessageCodes.Required)
                     .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

                });


                When(x =>  !string.IsNullOrWhiteSpace(x.CompanyName), () =>
                {
                    RuleFor(a => a.CompanyName)
                      .NotEmpty(MessageCodes.Required)
                      .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

                });


                When(x => !string.IsNullOrWhiteSpace(x.CompanyWebsite), () =>
                {
                    RuleFor(a => a.CompanyWebsite)
                      .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                      .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

                });



            }

        }
        internal class ChangePasswordInputDtoValidator : DtoValidationAbstractBase<ChangePasswordInputDto>
        {
            public ChangePasswordInputDtoValidator( )
            {


                //Validate New Password

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                    .Length(PasswordValidations.MinLength, PasswordValidations.MaxLength).WithDXErrorCode(MessageCodes.InbetweenValue, PasswordValidations.MinLength, PasswordValidations.MaxLength)
                     .Matches(PasswordValidations.RegexContainsOneNumberOrMore).WithDXErrorCode(MessageCodes.MissingPasswordDigits)
                    .Matches(PasswordValidations.RegexContainsAlbhabeticCharacter).WithDXErrorCode(MessageCodes.MissingPasswordAlphabetic)
                    .Matches(PasswordValidations.RegexContainsSpecialCharacter).WithDXErrorCode(MessageCodes.MissingPasswordSpecialCharacters);

                When(x => x.NewPassword != null, ( ) =>
                {
                    RuleFor(x => x.ConfirmNewPassword)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .Equal(x => x.NewPassword).WithDXErrorCode(MessageCodes.MismatchNewConfirmPassword);
                });
                When(x => x.NewPassword != null, ( ) =>
                {
                    RuleFor(x => x.OldPassword)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .NotEqual(x => x.NewPassword).WithDXErrorCode(MessageCodes.NewPasswordAlreadyDefined);
                });
            }
        }
        internal class UploadCustomerImageInputDtoValidator : DtoValidationAbstractBase<UploadCustomerImageInputDto>
        {
            public UploadCustomerImageInputDtoValidator()
            {
                 RuleFor(x => x.File.FileDto)
                    .NotEmpty(MessageCodes.Required)
                    .SetValidator(new FileDtoValidator());

            }

        }
        internal class GetCustomerInputDtoValidator : DtoValidationAbstractBase<GetCustomerInputDto>
        {
            public GetCustomerInputDtoValidator()
            {
                RuleFor(x => x.Id)
                     .NotEmpty(MessageCodes.Required)
                     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            }

        }        
        internal class DeleteCustomerInputDtoValidator : DtoValidationAbstractBase<DeleteCustomerInputDto>
        {
            public DeleteCustomerInputDtoValidator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }
        internal class UpdateCustomerStatusInputDtoValidator : DtoValidationAbstractBase<UpdateCustomerStatusInputDto>
        {
            public UpdateCustomerStatusInputDtoValidator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }
        internal class UpdateCustomerByAdminInputDtoValidator : DtoValidationAbstractBase<UpdateCustomerByAdminInputDto>
        {
            public UpdateCustomerByAdminInputDtoValidator()
            {
                RuleFor(x => x.CustomerId)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }
        internal class UpdateVersionDownloadCountInputDtoValidator : DtoValidationAbstractBase<UpdateVersionDownloadCountInputDto>
        {
            public UpdateVersionDownloadCountInputDtoValidator()
            {
                RuleFor(x => x.VersionId)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(a => a.IpAddress)
                      .NotEmpty(MessageCodes.Required)
                      .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

            }

        }
        internal class UpdateAddOnDownloadCountInputDtoValidator : DtoValidationAbstractBase<UpdateAddOnDownloadCountInputDto>
        {
            public UpdateAddOnDownloadCountInputDtoValidator()
            {
                RuleFor(x => x.AddOnId)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(a => a.IpAddress)
                      .NotEmpty(MessageCodes.Required)
                      .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

            }

        }

    }
}
