using FluentValidation;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Applications.ApplicationVersions;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Applications.Versions
{

    internal class CreateVersionInputDtoValidator : DtoValidationAbstractBase<CreateVersionInputDto>
    {
        public CreateVersionInputDtoValidator()
        {


            RuleFor(a => a.Name)
                 .ValidJson()
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);


            RuleFor(a => a.Title)
                 .ValidJson()
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);


            RuleFor(a => a.MainPageUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

            RuleFor(a => a.DownloadUrl)
                   .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);


            RuleFor(a => a.ShortDescription)
                 .ValidJson(isHtml: true)
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);


            RuleFor(a => a.LongDescription)
                 .ValidJson(isHtml: true)
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            //RuleFor(a => a.IsHighlightedVersion)
            //.NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(a => a.ApplicationId)
            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.ReleaseNumber)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required)
             .MaximumLength(50).WithDXErrorCode(MessageCodes.InvalidMaxLength,50);

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());
        }
    }
    internal class UpdateVersionInputDtoValidator : DtoValidationAbstractBase<UpdateVersionInputDto>
    {
        public UpdateVersionInputDtoValidator()
        {
            //When(x => x.ImageId > 0, () =>
            //{
            //    RuleFor(a => a.ImageId)
            //     .NotEmpty().WithDXErrorCode(MessageCodes.Required)
            //     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            //    RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());

            //});

            RuleFor(a => a.ModifiedBy)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, () =>
            {
                RuleFor(a => a.Name)
                .ValidJson().MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            });
            When(x => x.Title != null, () =>
            {
                RuleFor(a => a.Title)
                .ValidJson().MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            });
            When(x => x.MainPageUrl != null, () =>
            {

                RuleFor(a => a.MainPageUrl)
                     .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                     .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                     .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

            });
            When(x => !string.IsNullOrWhiteSpace(x.DownloadUrl), () =>
            {

                //RuleFor(a => a.DownloadUrl)
                //       .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                //       .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                //       .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

                RuleFor(a => a.ReleaseNumber)
                        .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                        //.MinimumLength(2).WithDXErrorCode(MessageCodes.InvalidMinLength, 2)
                        //.MaximumLength(50).WithDXErrorCode(MessageCodes.InvalidMaxLength, 50)
                        .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);
            });
            When(x => !string.IsNullOrWhiteSpace(x.ReleaseNumber), () =>
           {

               RuleFor(a => a.DownloadUrl)
                      .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                      .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                      .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);

               //RuleFor(a => a.ReleaseNumber)
               //        .NotEmpty().WithDXErrorCode(MessageCodes.Required)
               //        //.MinimumLength(2).WithDXErrorCode(MessageCodes.InvalidMinLength, 2)
               //        //.MaximumLength(50).WithDXErrorCode(MessageCodes.InvalidMaxLength, 50)
               //        .Length(2, 50).WithDXErrorCode(MessageCodes.InbetweenValue, 2, 50);
           });
            When(x => x.ShortDescription != null, () =>
            {
                RuleFor(a => a.ShortDescription)
                    .ValidJson(isHtml: true)
                    .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);
            });
            When(x => x.LongDescription != null, () =>
            {
                RuleFor(a => a.LongDescription)
                     .ValidJson(isHtml: true)
                     .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            });

            When(x => x.ApplicationId > 0, () =>
            {
                RuleFor(a => a.ApplicationId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


            });

        }

    }
    internal class GetVersionInputDtoValidator : DtoValidationAbstractBase<GetVersionInputDto>
    {
        public GetVersionInputDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class DeleteVersionInputDtoValidator : DtoValidationAbstractBase<DeleteVersionInputDto>
    {
        public DeleteVersionInputDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
}
