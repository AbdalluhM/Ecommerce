using FluentValidation;

using Ecommerce.DTO.Lookups.PriceLevels.Inputs;

using System;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.PriceLevels
{
    public class UpdatePriceLevelValidator : DtoValidationAbstractBase<UpdatePriceLevelDto>
    {
        public UpdatePriceLevelValidator( )
        {
            RuleFor(pl => pl.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, ( ) =>
              {
                  RuleFor(pl => pl.Name)
                  .ValidJson();
              });
           
                RuleFor(pl => pl.NumberOfLicenses)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.ModifiedBy)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }
    }

    internal class GetPriceLevelInputDtoValidator : DtoValidationAbstractBase<GetPriceLevelInputDto>
    {
        public GetPriceLevelInputDtoValidator( )
        {
            RuleFor(pl => pl.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }
    }
}
