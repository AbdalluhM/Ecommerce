using FluentValidation;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.EmployeeTypes
{
    internal class CreateEmployeeTypeInputDtoValidator : DtoValidationAbstractBase<CreateEmployeeTypeInputDto>
    {
        public CreateEmployeeTypeInputDtoValidator()
        {

            RuleFor(a => a.Type)
                .ValidJson()
               .NotEmpty(MessageCodes.Required);

        }

    }

    internal class UpdateEmployeeTypeInputDtoValidator : DtoValidationAbstractBase<UpdateEmployeeTypeInputDto>
    {
        public UpdateEmployeeTypeInputDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty(MessageCodes.Required)
                .GreaterThan(0);
                
            RuleFor(a => a.Type)
                 .ValidJson()
                 .NotEmpty(MessageCodes.Required);
        }

    }

    internal class DeleteEmployeeTypeInputDtoValidator : DtoValidationAbstractBase<DeleteEntityInputDto>
    {
        public DeleteEmployeeTypeInputDtoValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty(MessageCodes.Required)
               .GreaterThan(0);

        }

    }
}
