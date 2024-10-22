using FluentValidation;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using System.Collections.Generic;

namespace Ecommerce.BLL.Validation.Crm.Tickets
{
    public class NewTicketDtoValidator : DtoValidationAbstractBase<NewTicketDto>
    {
        public NewTicketDtoValidator()
        {
            RuleFor(t => t.Title)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            RuleFor(t => t.Description)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            RuleFor(t => t.SenderName)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            When(t => t.Topic == (int)TicketTopicEnum.TechnicalSupport, () =>
            {
                RuleFor(t => t.ProductNumber)
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
            });

            When(t => t.Topic == (int)TicketTopicEnum.Billing, () =>
            {
                RuleFor(t => t.InvoiceId)
                    .GreaterThan(0).WithDXErrorCode(DXConstants.MessageCodes.GreaterThanZero);
            });
        }
    }




    ///new 
    public class NewTicketValidator : DtoValidationAbstractBase<CreateTicketDto>
    {
        public NewTicketValidator()
        {
            RuleFor(t => t.Title)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .MaximumLength(400).WithDXErrorCode(DXConstants.MessageCodes.InvalidMaxLength, 400);

            RuleFor(t => t.Description)
                .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            //RuleFor(t => t.SenderName)
            //    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
            //    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            When(t => t.Topic == (int)TicketTopicEnum.TechnicalSupport, () =>
            {
                //RuleFor(t => t.ProductNumber)
                //    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                //    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);


                RuleFor(t => t.SubjectId)
                    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);


            });

            When(t => t.Topic == (int)TicketTopicEnum.Billing, () =>
            {
                RuleFor(t => t.InvoiceId)
                    .GreaterThan(0).WithDXErrorCode(DXConstants.MessageCodes.GreaterThanZero);
            });

            When(t => t.HasAttachment == true, () =>
            {
                RuleFor(t => t.Attachment)
                    .NotNull().WithDXErrorCode(DXConstants.MessageCodes.Required)
                    .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required); 
            });


        }
    }
   

}
