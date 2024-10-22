using FluentValidation;

using Ecommerce.Core.Entities;
using Ecommerce.DTO.Applications.ApplicationLabels;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.ApplicationLabels
{
    internal class CreateApplicationLabelValidator : DtoValidationAbstractBase<CreateApplicationLabelInputDto>
    {
        public CreateApplicationLabelValidator( )
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
             RuleFor(x => x.Name)
                .ValidJson(x => x.Default.Length <= 15 && x.Ar.Length <= 15,
                  MessageCodes.LengthValidationError, nameof(ApplicationLabel.Name))
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            RuleFor(x => x.Color).NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.CreatedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class UpdateApplicationLabelDtoValidator : DtoValidationAbstractBase<UpdateApplicationLabelInputDto>
    {
        public UpdateApplicationLabelDtoValidator( )
        {

            RuleFor(x => x.ApplicationId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => !string.IsNullOrEmpty(x.Color), ( ) =>
            {
                RuleFor(x => x.Color)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            });
            When(x => !string.IsNullOrEmpty(x.Name), ( ) =>
            {
                RuleFor(x => x.Name)
               //.ValidJson2<CreateTagDto, string, JsonLanguageModel>(x => x.Default.Length <= 15 && x.Ar.Length <= 15)
                .ValidJson(x => x.Default.Length <= 15 && x.Ar.Length <= 15,
                  MessageCodes.LengthValidationError,  nameof(ApplicationLabel.Name))
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            });

          
        }

    }
    internal class GetApplicationLabelInputDtoValidator : DtoValidationAbstractBase<GetApplicationLabelInputDto>
    {
        public GetApplicationLabelInputDtoValidator( )
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
  



}