using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Modules;
using Ecommerce.DTO.Modules.ModuleSlider;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Moduless
{
   
    internal class CreateModuleSliderInputDtoValidator : DtoValidationAbstractBase<CreateModuleSliderInputDto>
    {
        public CreateModuleSliderInputDtoValidator( )
        {


            RuleFor(s => s.ModuleId)
                 .NotEmpty()
                .WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());
        }
    }
    internal class UpdateModuleSliderInputDtoValidator : DtoValidationAbstractBase<UpdateModuleSliderInputDto>
    {
        public UpdateModuleSliderInputDtoValidator( )


        {

            RuleFor(s => s.ModuleId)
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


            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File).SetValidator(new FileDtoValidator());

            });

        }

    }
    internal class GetModuleSliderInputDtoValidator : DtoValidationAbstractBase<GetModuleSliderInputDto>
    {
        public GetModuleSliderInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
 
}
