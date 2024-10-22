using FluentValidation;
using Ecommerce.DTO.Applications.VersionAddOns;
using System.Linq;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.AddOns
{

    internal class CreateVersionAddOnInputDtoValidator : DtoValidationAbstractBase<CreateVersionAddOnInputDto>
    {
        public CreateVersionAddOnInputDtoValidator( )
        {


            RuleFor(a => a.AddOnId)
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
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.AddOnId > 0 && c.CreatedBy > 0)).WithDXErrorCode(MessageCodes.Required);


        }
    }
    internal class UpdateVersionAddOnInputDtoValidator : DtoValidationAbstractBase<UpdateVersionAddOnInputDto>
    {
        public UpdateVersionAddOnInputDtoValidator( )
        {

            //RuleFor(a => a.ApplicationId)
            //  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.AddOnId)
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
                Must(x => x.TrueForAll(c => c.VersionId > 0 && c.AddOnId > 0 /*&& c.ModifiedBy > 0*/)).WithDXErrorCode(MessageCodes.Required);


        }

    }
    internal class GetVersionAddOnInputDtoValidator : DtoValidationAbstractBase<GetVersionAddOnInputDto>
    {
        public GetVersionAddOnInputDtoValidator( )
        {
            RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ApplicationId)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

    internal class DeleteVersionAddOnInputDtoValidator : DtoValidationAbstractBase<DeleteApplicatinAddOnInputDto>
    {
        public DeleteVersionAddOnInputDtoValidator( )
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
           RuleFor(x => x.AddOnId)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }

    }
        internal class GetUnAsignedVersionInputDtoValidator : DtoValidationAbstractBase<GetApplicationByIdInputDto>
        {
            public GetUnAsignedVersionInputDtoValidator()
            {
                RuleFor(x => x.ApplicationId)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }

}
