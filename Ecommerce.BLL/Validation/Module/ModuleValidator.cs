using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Modules;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Modules
{
   
    internal class CreateModuleInputDtoValidator : DtoValidationAbstractBase<CreateModuleInputDto>
    {
        public CreateModuleInputDtoValidator( )
        {
            //RuleFor(a => a.ImageId)
            //     .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //RuleFor(a => a.CreatedBy)
            //     .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.Name)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);


            RuleFor(a => a.Title)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,500);


            RuleFor(a => a.MainPageUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength,  2000);


            RuleFor(a => a.ShortDescription)
                 .ValidJson(isHtml: true)
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);


            RuleFor(a => a.LongDescription)
                 .ValidJson(isHtml: true)
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength,  4000);

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());
        }
    }
    internal class UpdateModuleInputDtoValidator : DtoValidationAbstractBase<UpdateModuleInputDto>
    {
        public UpdateModuleInputDtoValidator( )
        {
            When(x => x.ImageId > 0, ( ) =>
            {
                RuleFor(a => a.ImageId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());


            });
            RuleFor(a => a.ModifiedBy)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, ( ) =>
            {
                RuleFor(a => a.Name)
                .ValidJson().MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);

            });
            When(x => x.Title != null, ( ) =>
            {
                RuleFor(a => a.Title)
                .ValidJson().MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,500);

            });
            When(x => x.MainPageUrl != null, ( ) =>
            {
                RuleFor(a => a.MainPageUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);
            });
            When(x => x.ShortDescription != null, ( ) =>
            {
                RuleFor(a => a.ShortDescription)
                    .ValidJson(isHtml: true)
                    .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength,4000);
            });
            When(x => x.LongDescription != null, ( ) =>
            {
                RuleFor(a => a.LongDescription)
                     .ValidJson(isHtml: true)
                     .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            });
            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File).SetValidator(new FileDtoValidator());

            });



        }

    }
    internal class GetModuleInputDtoValidator : DtoValidationAbstractBase<GetModuleInputDto>
    {
        public GetModuleInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class DeleteModuleInputDtoValidator : DtoValidationAbstractBase<DeleteModuleInputDto>
    {
        public DeleteModuleInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
}
