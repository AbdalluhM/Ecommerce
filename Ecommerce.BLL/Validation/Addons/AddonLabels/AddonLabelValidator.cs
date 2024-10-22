using FluentValidation;

using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Tags;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Addons.AddonLabels
{
    internal class AddAddonLabelValidator : DtoValidationAbstractBase<CreateAddonLabelInputDto>
    {
        public AddAddonLabelValidator()
        {
            RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
             RuleFor(x => x.Name)
                .ValidJson(x => x.Default.Length <= 15 && x.Ar.Length <= 15,
                  MessageCodes.LengthValidationError, nameof(AddOnLabel.Name))
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            RuleFor(x => x.Color).NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.CreatedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class UpdateAddonLabelDtoValidator : DtoValidationAbstractBase<UpdateAddonLabelInputDto>
    {
        public UpdateAddonLabelDtoValidator( )
        {

            RuleFor(x => x.AddOnId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => !string.IsNullOrEmpty(x.Color), ( ) =>
            {
                RuleFor(x => x.Color)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            });
            When(x => !string.IsNullOrEmpty(x.Name), ( ) =>
            {
                RuleFor(x => x.Name)
               //.ValidJson2<CreateTagDto, string, JsonLanguageModel>(x => x.Default.Length <= 15 && x.Ar.Length <= 15)
                .ValidJson(x => x.Default.Length <= 15 && x.Ar.Length <= 15,
                  MessageCodes.LengthValidationError,  nameof(AddOnLabel.Name))
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            });

            RuleFor(x => x.ModifiedBy)
               .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class GetAddOnLabelInputDtoValidator : DtoValidationAbstractBase<GetAddOnLabelInputDto>
    {
        public GetAddOnLabelInputDtoValidator( )
        {
            RuleFor(x => x.AddOnId)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
  



}