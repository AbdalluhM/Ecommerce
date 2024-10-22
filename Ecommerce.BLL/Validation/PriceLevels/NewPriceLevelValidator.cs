using FluentValidation;
using Ecommerce.Core.Entities;

using Ecommerce.Core.Enums;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.Localization;

using System;
using System.ComponentModel;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.PriceLevels
{
 
    public class NewPriceLevelValidator : DtoValidationAbstractBase<NewPriceLevelDto>
    {
        public NewPriceLevelValidator()
        {
            RuleFor(pl => pl.Name)
                 .ValidJson();
                


            RuleFor(pl => pl.NumberOfLicenses)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)/*.OverridePropertyName("Admin.PriceLevel.NumberOfLicenses")*/
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.CreatedBy)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }
    }
}
