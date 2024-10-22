using FluentValidation;

using Ecommerce.DTO.Applications.VersionFeatures;

using System.Linq;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.Features
{

    internal class CreateVersionFeatureInputDtoValidator : DtoValidationAbstractBase<CreateVersionFeatureInputDto>
    {
        public CreateVersionFeatureInputDtoValidator( )
        {


            RuleFor(a => a.FeatureId)
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
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.FeatureId > 0 && c.CreatedBy > 0)).WithDXErrorCode(MessageCodes.Required);


        }
    }
    internal class UpdateVersionFeatureInputDtoValidator : DtoValidationAbstractBase<UpdateVersionFeatureInputDto>
    {
        public UpdateVersionFeatureInputDtoValidator( )
        {

            //RuleFor(a => a.ApplicationId)
            //  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.FeatureId)
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
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.FeatureId > 0 /*&& c.ModifiedBy > 0*/)).WithDXErrorCode(MessageCodes.Required);


        }

    }
    internal class GetVersionFeatureInputDtoValidator : DtoValidationAbstractBase<GetVersionFeatureInputDto>
    {
        public GetVersionFeatureInputDtoValidator( )
        {
            RuleFor(x => x.FeatureId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ApplicationId)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

    internal class DeleteVersionFeatureInputDtoValidator : DtoValidationAbstractBase<DeleteApplicatinFeatureInputDto>
    {
        public DeleteVersionFeatureInputDtoValidator( )
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
           RuleFor(x => x.FeatureId)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }

    }
    internal class GetUnAsignedFeatureInputDtoValidator : DtoValidationAbstractBase<GetApplicationByIdInputDto>
    {
        public GetUnAsignedFeatureInputDtoValidator()
        {
            RuleFor(x => x.ApplicationId)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

}
