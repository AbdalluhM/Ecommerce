using FluentValidation;
using FluentValidation.Results;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Enums.Files;
using Ecommerce.DTO.Files;
using Ecommerce.Localization;
using System;
using System.IO;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Files
{
    public class FileDtoValidator : DtoValidationAbstractBase<IFileDto>
    {
        private static ICustomStringLocalizer<ErrorMessage> _messageLocalizer;
        //TODO:Refactor fileDtoValidator to ImageValidator and FileValidator

        public FileDtoValidator()
        {
            RuleFor(f => f.File)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(f => f.FilePath)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(f => f.FileBaseDirectory)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            When(x => x.File != null, () =>
            {
                //validate file type
                RuleFor(f => f.File)
                   .Custom((file, context) =>
                   {
                       _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();

                       var extension = Path.GetExtension(file.FileName);
                       if (!Enum.TryParse<FileExtensionEnum>(extension.Substring(1), true, out var extensionType))
                       {
                           context.AddFailure(new ValidationFailure("[File]"/*context.DisplayName*/, //localize here
                           _messageLocalizer[MessageCodes.InvalidFileType.StringValue()])
                           {
                               ErrorCode = MessageCodes.InvalidFileType.StringValue()
                           });
                       }

                       //validate file content types
                       var slashIndex = file.ContentType.IndexOf('/');
                       var contentType = file.ContentType.Substring(0, slashIndex);
                       if (!Enum.TryParse<FileTypeEnum>(contentType, true, out var fileType))
                       {
                           context.AddFailure(new ValidationFailure("[File]"/*context.DisplayName*/, //localize here
                           _messageLocalizer[MessageCodes.InvalidFileContentType.StringValue()])
                           {
                               ErrorCode = MessageCodes.InvalidFileContentType.StringValue()
                           });
                       }

                       //validate file size
                       if (file.Length > 20 * 1024 * 1024) //2 mega bytes binary
                       {
                           context.AddFailure(new ValidationFailure("[File]"/*context.DisplayName*/, //localize here
                           _messageLocalizer[MessageCodes.InvalidFileSize.StringValue()])
                           {
                               ErrorCode = MessageCodes.InvalidFileSize.StringValue()
                           });
                       }
                   });
            });
        }
    }



    public class SingleFileBaseDtoValidator : DtoValidationAbstractBase<SingleFilebaseDto>
    {
        public SingleFileBaseDtoValidator(int megaSize, dynamic fileTypeEnum)
        {
            RuleFor(x => x.FileDto.File).NotEmpty();
            //validate file type
            RuleFor(x => x.FileDto.File).NotNull();
            When(x => x.FileDto.File != null, () =>
            {
                RuleFor(f => f.FileDto.File)
                   .Custom((file, context) =>
                   {
                       var contentType = file.ContentType.Split('/')[1]
                       .Replace("-", "").Replace(".", "").Replace("vndopenxmlformatsofficedocument", "");
                       var isSupportedType = Enum.TryParse(fileTypeEnum, contentType, true, out dynamic fileType);
                       if (!isSupportedType)
                           context.AddFailure(new ValidationFailure("[File]"/*context.DisplayName*/, //localize here
                            "Failed :Invalid File Type"));
                       //validate file size
                       if (file.Length > megaSize * 1024 * 1024) //2 mega bytes binary
                       {
                           context.AddFailure(new ValidationFailure("[File]"/*context.DisplayName*/, //localize here
                           "Failed :’File exceed limit. Max limit=" + megaSize + " MB"));
                       }
                   });
            });

        }
    }

}
