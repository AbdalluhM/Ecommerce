using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Enums;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonPrice;

using System;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Addons.AddonBase
{
    
    internal class CreateAddOnInputDtoValidator : DtoValidationAbstractBase<CreateAddOnInputDto>
    {
        public CreateAddOnInputDtoValidator( )
        {
           

            RuleFor(a => a.CreatedBy)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(a => a.Name)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);


            RuleFor(a => a.Title)
                 .ValidJson()
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);


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
                 .MaximumLength(1000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);


            RuleFor(a => a.LongDescription)
                 .ValidJson(isHtml: true)
                 .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

            RuleFor(x => x.File).NotNull().WithDXErrorCode(MessageCodes.Required).SetValidator(new FileDtoValidator());


        }
    }
    internal class UpdateAddOnInputDtoValidator : DtoValidationAbstractBase<UpdateAddOnInputDto>
    {
        public UpdateAddOnInputDtoValidator( )
        {
            When(x => x.LogoId > 0, ( ) =>
            {
                RuleFor(a => a.LogoId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });

            RuleFor(a => a.ModifiedBy)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            When(x => x.Name != null, ( ) =>
              {
                  RuleFor(a => a.Name)
                  .ValidJson().MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,  500);

              });
            When(x => x.Title != null, ( ) =>
            {
                RuleFor(a => a.Title)
                .ValidJson().MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,  500);

            });
            When(x => x.MainPageUrl != null, ( ) =>
            {
                RuleFor(a => a.MainPageUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,  500);
            });
            When(x => x.DownloadUrl != null, () =>
            {
                RuleFor(a => a.DownloadUrl)
                 .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength,  2000);
            });
            When(x => x.ShortDescription != null, ( ) =>
            {
                RuleFor(a => a.ShortDescription)
                    .ValidJson(isHtml: true)
                    .MaximumLength(1000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);
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
    internal class GetAddOnInputDtoValidator : DtoValidationAbstractBase<GetAddOnInputDto>
    {
        public GetAddOnInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .NotEmpty().GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    internal class DeleteAddOnInputDtoValidator : DtoValidationAbstractBase<DeleteAddOnInputDto>
    {
        public DeleteAddOnInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

        }

    }
    //public class AddonValidator : DtoValidationAbstractBase<NewAddonDto>
    //{
    //    public AddonValidator( )
    //    {
    //        RuleFor(a => a.Name)
    //            .ValidJson()
    //            .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength,  500);

    //        RuleFor(a => a.Title)
    //             .ValidJson()
    //             .MaximumLength(500).WithDXErrorCode(MessageCodes.InvalidMaxLength, 500);

    //        RuleFor(a => a.MainPageUrl)


    //            .Matches(Regex.HttpUrl).WithDXErrorCode(MessageCodes.InvalidHttpsUrl)
    //            .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000)
    //            .NotEmpty().WithDXErrorCode(MessageCodes.Required);

    //        RuleFor(a => a.ShortDescription)
    //             .ValidJson(isHtml:true)
    //             .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);

    //        RuleFor(a => a.LongDescription)
    //             .ValidJson(isHtml: true)
    //             .MaximumLength(4000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 4000);


    //    }
    //}

}
