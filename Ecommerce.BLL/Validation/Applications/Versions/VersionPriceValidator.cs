using FluentValidation;

using Ecommerce.Core.Enums;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationVersions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.Versions
{
    internal class CreateVersionPricesInputDtoValidator : DtoValidationAbstractBase<CreateVersionPriceInputDto>
    {
        public CreateVersionPricesInputDtoValidator( )
        {
            RuleFor(x => x.VersionId)
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
    internal class UpdateVersionPricesInputDtoValidator : DtoValidationAbstractBase<UpdateVersionPriceInputDto>
    {
        public UpdateVersionPricesInputDtoValidator( )
        {
            RuleFor(x => x.Id)
           .NotEmpty().WithDXErrorCode(MessageCodes.Required)
           .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            When(x => x.VersionId > 0, ( ) =>
            {
                RuleFor(x => x.VersionId)
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
    internal class GetVersionPriceInputDtoValidator : DtoValidationAbstractBase<GetVersionPriceInputDto>
    {
        public GetVersionPriceInputDtoValidator( )
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    //internal class DeleteVersionPriceInputDtoValidator : DtoValidationAbstractBase<DeleteVersionPriceInputDto>
    //{
    //    public DeleteVersionPriceInputDtoValidator( )
    //    {
    //        RuleFor(x => x.Id)
    //            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
    //            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

    //    }

    // }
    internal class GetVersionPricesInputDtoValidator : DtoValidationAbstractBase<GetApplicationVersionPricesInputDto>
    {
        public GetVersionPricesInputDtoValidator( )
        {
            //RuleFor(x => x.VersionId)
            //    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //    .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            
        }

    }



}