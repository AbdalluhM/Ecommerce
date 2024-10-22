using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Addons.AddonSliders.Inputs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Addons.AddonSilders
{
    public class NewAddonSliderDtoValidator : DtoValidationAbstractBase<NewAddonSliderDto>
    {
        public NewAddonSliderDtoValidator()
        {
            RuleFor(s => s.AddonId)
                .GreaterThan(0)
                .WithDXErrorCode(MessageCodes.Required);

        }
    }
}
