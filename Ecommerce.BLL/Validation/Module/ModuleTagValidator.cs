using FluentValidation;

using Ecommerce.DTO.Modules.ModuleTags;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Moduless.ModuleTags
{
    internal class CreateModuleTagInputDtoValidator : DtoValidationAbstractBase<CreateModuleTagInputDto>
    {
        public CreateModuleTagInputDtoValidator()
        {
            RuleFor(x => x.ModuleId)
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
    internal class UpdateModuleTagDtoValidator : DtoValidationAbstractBase<UpdateModuleTagInputDto>
    {
        public UpdateModuleTagDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModuleId)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required)
             .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.TagId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ModuleId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }

    }
    internal class GetModuleTagInputDtoValidator : DtoValidationAbstractBase<GetModuleTagInputDto>
    {
        public GetModuleTagInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

}