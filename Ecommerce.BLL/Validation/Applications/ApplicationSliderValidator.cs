using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Applications.ApplicationSlider;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications
{

    internal class CreateApplicationSliderInputDtoValidator : DtoValidationAbstractBase<CreateApplicationSliderInputDto>
    {
        public CreateApplicationSliderInputDtoValidator( )
        {


            RuleFor(s => s.ApplicationId)
                 .NotEmpty()
                .WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());


        }
    }
    internal class UpdateApplicationSliderInputDtoValidator : DtoValidationAbstractBase<UpdateApplicationSliderInputDto>
    {
        public UpdateApplicationSliderInputDtoValidator( )


        {

            RuleFor(s => s.ApplicationId)
                .NotEmpty()
               .WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.MediaId > 0, ( ) =>
            {
                RuleFor(a => a.MediaId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());

            });
            RuleFor(a => a.ModifiedBy)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

          


        }

    }
    internal class GetApplicationSliderInputDtoValidator : DtoValidationAbstractBase<GetApplicationSliderInputDto>
    {
        public GetApplicationSliderInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
 
}
