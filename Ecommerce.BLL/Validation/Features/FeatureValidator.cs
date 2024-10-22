using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Features;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Features
{
    internal class CreateFeatureInputDtoValidator : DtoValidationAbstractBase<CreateFeatureInputDto>
    {
        public CreateFeatureInputDtoValidator( )
        {
            RuleFor(x => x.Name)
                .ValidJson()
                .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);
            RuleFor(x => x.Description)
                .ValidJson(isHtml: true)
                .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            //RuleFor(x => x.File)
            //    .NotEmpty().WithDXErrorCode(MessageCodes.Required, string.Format(MessageCodes.Required.GetDescription(), nameof(IFileDto.File)));

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);
        }

    }
    internal class UpdateFeatureInputDtoValidator : DtoValidationAbstractBase<UpdateFeatureInputDto>
    {
        public UpdateFeatureInputDtoValidator( )
        {
            RuleFor(x => x.Id)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            When(x => x.Name != null , ( ) =>
            {
                RuleFor(x => x.Name)
               .ValidJson()
               .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);
            });
            When(x => x.Description != null, ( ) =>
            {
                RuleFor(x => x.Description)
                .ValidJson(isHtml:true)
                .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);
            });

            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File).SetValidator(new FileDtoValidator());

            });

            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.Required);
        }

    }
    internal class GetFeatureInputDtoValidator : DtoValidationAbstractBase<GetFeatureInputDto>
    {
        public GetFeatureInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class DeleteFeatureInputDtoValidator : DtoValidationAbstractBase<DeleteFeatureInputDto>
    {
        public DeleteFeatureInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }

    }
    



}