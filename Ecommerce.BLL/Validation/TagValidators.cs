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

namespace Ecommerce.BLL.Validation
{
    internal class CreateTagDtoValidator : DtoValidationAbstractBase<CreateTagInputDto>
    {
        public CreateTagDtoValidator( )
        {

            //Validate Tag Name
            RuleFor(x => x.Name)
                .ValidJson();
            RuleFor(a => a.CreatedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class UpdateTagDtoValidator : DtoValidationAbstractBase<UpdateTagInputDto>
    {
        public UpdateTagDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(a => a.ModifiedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, ( ) =>
            {

                RuleFor(x => x.Name)
                .ValidJson();

            });

        }
    }
    internal class GetTagInputDtoValidator : DtoValidationAbstractBase<GetTagInputDto>
    {
        public GetTagInputDtoValidator( )
        {
            //Validate Id
            RuleFor(x => x.Id).NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }
    }

}
