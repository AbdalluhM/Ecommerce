using FluentValidation;

using Ecommerce.DTO.Lookups;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation
{ 
    # region AssignCurrencyToCountry
internal class AssignCurrencyToCountryInputDtoValidator : DtoValidationAbstractBase<AssignCurrencyToCountryInputDto>
    {
        public AssignCurrencyToCountryInputDtoValidator( )
        {
            //Validate Country
            RuleFor(x => x.CountryId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            //Validate Currency
            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            //Validate Employee
            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }
    }

    internal class UpdateAssignCurrencyToCountryInputDtoValidator : DtoValidationAbstractBase<UpdateAssignCurrencyToCountryInputDto>
    {
        public UpdateAssignCurrencyToCountryInputDtoValidator( )
        {           
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //Validate Country
            RuleFor(x => x.CountryId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //Validate Currency
            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //Validate Employee
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }
    }
    internal class GetAssignedCurrencyToCountryInputDtoValidator : DtoValidationAbstractBase<GetAssignedCurrencyToCountryInputDto>
    {
        public GetAssignedCurrencyToCountryInputDtoValidator( )
        {
            //Validate Country Currency Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);



        }
    }
    #endregion
    #region Country
    #endregion
}
