using FluentValidation;

using Ecommerce.DTO.Applications.ApplicationTags;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.ApplicationTags
{
    internal class CreateApplicationTagInputDtoValidator : DtoValidationAbstractBase<CreateApplicationTagInputDto>
    {
        public CreateApplicationTagInputDtoValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.TagId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class UpdateApplicationTagDtoValidator : DtoValidationAbstractBase<UpdateApplicationTagInputDto>
    {
        public UpdateApplicationTagDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ApplicationId)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required)
             .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.TagId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }

    }
    internal class GetApplicationTagInputDtoValidator : DtoValidationAbstractBase<GetApplicationTagInputDto>
    {
        public GetApplicationTagInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

}