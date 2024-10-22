using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Enums;
using Ecommerce.DTO.Application;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications

{

    internal class CreateApplicationInputDtoValidator : DtoValidationAbstractBase<CreateApplicationInputDto>
    {
        public CreateApplicationInputDtoValidator( )
        {

            RuleFor(a => a.Name)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);


            RuleFor(a => a.Title)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);

            RuleFor(a => a.SubscriptionTypeId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.MainPageUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);


            RuleFor(a => a.ShortDescription)
                 .ValidJson(isHtml: true);


            RuleFor(a => a.LongDescription)
                 .ValidJson(isHtml: true)
                 ;

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());

        }
    }
    internal class UpdateApplicationInputDtoValidator : DtoValidationAbstractBase<UpdateApplicationInputDto>
    {
        public UpdateApplicationInputDtoValidator( )
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
                .ValidJson().MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);

            });

            RuleFor(a => a.SubscriptionTypeId)
             .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

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
                    .ValidJson(isHtml: true);
            });
            When(x => x.LongDescription != null, ( ) =>
            {
                RuleFor(a => a.LongDescription)
                     .ValidJson(isHtml: true)
                     ;

            });
            When(x => x.File != null, ( ) =>
            {
                RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());

            });


        }

    }
    internal class GetApplicationInputDtoValidator : DtoValidationAbstractBase<GetApplicationInputDto>
    {
        public GetApplicationInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class DeleteApplicationInputDtoValidator : DtoValidationAbstractBase<DeleteApplicationInputDto>
    {
        public DeleteApplicationInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
}
