using FluentValidation;

using Ecommerce.Core.Enums;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonTags;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Addons.AddOnPrices
{
    internal class CreateAddOnPricesInputDtoValidator : DtoValidationAbstractBase<CreateAddOnPriceInputDto>
    {
        public CreateAddOnPricesInputDtoValidator( )
        {
            RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.PriceLevelId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.CountryCurrencyId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.CreatedBy)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ForeverPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.YearlyPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.MonthlyPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ForeverNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.YearlyNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.MonthlyNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
          
        }

    }
    internal class UpdateAddOnPricesInputDtoValidator : DtoValidationAbstractBase<UpdateAddOnPriceInputDto>
    {
        public UpdateAddOnPricesInputDtoValidator( )
        {
            RuleFor(x => x.Id)
           .NotEmpty().WithDXErrorCode(MessageCodes.Required)
           .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            When(x => x.AddOnId > 0, ( ) =>
            {
                RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });
            When(x => x.PriceLevelId > 0, ( ) =>
            {
                RuleFor(x => x.PriceLevelId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });
            When(x => x.CountryCurrencyId > 0, ( ) =>
            {
                RuleFor(x => x.CountryCurrencyId)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                    .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });

            RuleFor(a => a.ModifiedBy)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ForeverPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.YearlyPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.MonthlyPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ForeverNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.YearlyNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.MonthlyNetPrice).GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }

    }
    internal class GetAddOnPriceInputDtoValidator : DtoValidationAbstractBase<GetAddOnPriceInputDto>
    {
        public GetAddOnPriceInputDtoValidator( )
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    //internal class DeleteAddOnPriceInputDtoValidator : DtoValidationAbstractBase<DeleteAddOnPriceInputDto>
    //{
    //    public DeleteAddOnPriceInputDtoValidator( )
    //    {
    //        RuleFor(x => x.Id)
    //            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
    //            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

    //    }

    // }
    internal class GetAddOnPricesInputDtoValidator : DtoValidationAbstractBase<GetAddOnPricesInputDto>
    {
        public GetAddOnPricesInputDtoValidator( )
        {
            RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }



}