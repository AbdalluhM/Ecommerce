using FluentValidation;

using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Addons.AddonTags
{
   internal class CreateAddonTagInputDtoValidator : DtoValidationAbstractBase<CreateAddonTagInputDto>
    {
        public CreateAddonTagInputDtoValidator()
        {
            RuleFor(x => x.AddonId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.TagId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class UpdateAddonTagDtoValidator : DtoValidationAbstractBase<UpdateAddonTagDto>
    {
        public UpdateAddonTagDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.AddonId)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required)
             .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.TagId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.AddonId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


        }

    }
    internal class GetAddOnTagInputDtoValidator : DtoValidationAbstractBase<GetAddOnTagInputDto>
    {
        public GetAddOnTagInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }

}