using System;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class NewTicketMessageDto
    {
        public Guid TicketId { get; set; }

        public string SenderName { get; set; }

        public string Message { get; set; }

        public bool HasAttachment { get; set; }

        public MessageAttachmentDto Attachment { get; set; } = new();
    }
    public class NewTicketMessageNotificationDto : NewTicketMessageDto
    {
        public int? userId { get; set; }

        public int? AdminId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
