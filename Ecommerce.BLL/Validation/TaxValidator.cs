using Ecommerce.DTO.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Newtonsoft.Json;
using Ecommerce.Core.Helpers.JsonLanguages;
using static Ecommerce.BLL.DXConstants;
using Ecommerce.DTO.Taxes;

namespace Ecommerce.BLL.Validation
{
    internal class CreateTaxDtoValidator : DtoValidationAbstractBase<CreateTaxInputDto>
    {
        public CreateTaxDtoValidator( )
        {

            //Validate Tax Name
            RuleFor(x => x.Name)
                .ValidJson();
            RuleFor(a => a.CreatedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class UpdateTaxDtoValidator : DtoValidationAbstractBase<UpdateTaxInputDto>
    {
        public UpdateTaxDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(a => a.ModifiedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, ( ) =>
            {
                RuleFor(x => x.Name)
                .ValidJson();

            });

        }
    }
    internal class GetTaxInputDtoValidator : DtoValidationAbstractBase<GetTaxInputDto>
    {
        public GetTaxInputDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id).NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }

}
