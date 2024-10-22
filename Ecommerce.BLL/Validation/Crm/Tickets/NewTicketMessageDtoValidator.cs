using FluentValidation;
using Ecommerce.DTO.Customers.Crm.Tickets;

namespace Ecommerce.BLL.Validation.Crm.Tickets
{
    public class NewTicketMessageDtoValidator : DtoValidationAbstractBase<NewTicketMessageDto>
    {
        public NewTicketMessageDtoValidator()
        {
            RuleFor(m => m.SenderName)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
            
            RuleFor(m => m.Message)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            When(t => t.HasAttachment, () =>
            {
                RuleFor(t => t.Attachment.FileName)
                    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
            });

            When(t => t.HasAttachment, () =>
            {
                RuleFor(t => t.Attachment.FileBase64)
                    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
            });

            When(t => t.HasAttachment, () =>
            {
                RuleFor(t => t.Attachment.MimeType)
                    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
            });
        }
    }
}
