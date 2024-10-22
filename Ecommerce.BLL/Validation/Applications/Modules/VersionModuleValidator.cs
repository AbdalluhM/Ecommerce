using FluentValidation;

using Ecommerce.DTO.Applications.VersionModules;

using System.Linq;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.Modules
{

    internal class CreateVersionModuleInputDtoValidator : DtoValidationAbstractBase<CreateVersionModuleInputDto>
    {
        public CreateVersionModuleInputDtoValidator( )
        {


            RuleFor(a => a.ModuleId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.CreatedBy)
           .NotEmpty().WithDXErrorCode(MessageCodes.Required)
           .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //Validate Versions
            RuleFor(x => x.Versions)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .Must(x => x.Count() > 0).WithDXErrorCode(MessageCodes.InvalidItemsSelect);
            RuleFor(x => x.Versions)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required).
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.ModuleId > 0 && c.CreatedBy > 0)).WithDXErrorCode(MessageCodes.Required);


        }
    }
    internal class UpdateVersionModuleInputDtoValidator : DtoValidationAbstractBase<UpdateVersionModuleInputDto>
    {
        public UpdateVersionModuleInputDtoValidator( )
        {

            //RuleFor(a => a.ApplicationId)
            //  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.ModuleId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.ModifiedBy)
           .NotEmpty().WithDXErrorCode(MessageCodes.Required)
           .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //Validate Versions
            RuleFor(x => x.Versions)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .Must(x => x.Count() > 0).WithDXErrorCode(MessageCodes.InvalidItemsSelect);
            RuleFor(x => x.Versions)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required).
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.ModuleId > 0 /*&& c.ModifiedBy > 0*/)).WithDXErrorCode(MessageCodes.Required);


        }

    }
    internal class GetVersionModuleInputDtoValidator : DtoValidationAbstractBase<GetVersionModuleInputDto>
    {
        public GetVersionModuleInputDtoValidator( )
        {
            RuleFor(x => x.ModuleId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ApplicationId)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

    internal class DeleteVersionModuleInputDtoValidator : DtoValidationAbstractBase<DeleteApplicatinModuleInputDto>
    {
        public DeleteVersionModuleInputDtoValidator( )
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
           RuleFor(x => x.ModuleId)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }

    }
    internal class GetUnAsignedModuleInputDtoValidator : DtoValidationAbstractBase<GetApplicationByIdInputDto>
    {
        public GetUnAsignedModuleInputDtoValidator()
        {
            RuleFor(x => x.ApplicationId)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

}
