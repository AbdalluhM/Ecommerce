using Ecommerce.DTO.Files;
using System;
using System.Collections.Generic;

namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class NewTicketDto
    {
        public Guid CustomerId { get; set; }

        public int Topic { get; set; }

        public int InvoiceId { get; set; }

        public string ProductNumber { get; set; }

        public Guid SubjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string SenderName { get; set; }

        public bool HasAttachment { get; set; }

        public MessageAttachmentDto Attachment { get; set; } = new();
    }

    public class NewTicketNotificationDto: NewTicketDto
    {
        public int? userId { get; set; }

        public int? AdminId { get; set; }

        public bool IsAdmin { get; set; }
    }




    //public class NewTicket
    //{
    //    public int CustomerId { get; set; }

    //    public int Topic { get; set; }

    //    public int InvoiceId { get; set; }

    //    public string ProductNumber { get; set; }

    //    public Guid SubjectId { get; set; }

    //    public string Title { get; set; }

    //    public string Description { get; set; }

    //    public string SenderName { get; set; }

    //    public bool HasAttachment { get; set; }

    //    public List<SingleFilebaseDto> Attachment { get; set; } = new();
    //}

    //public class NewTicketNotification : NewTicket
    //{
    //    public int? userId { get; set; }

    //    public int? AdminId { get; set; }

    //    public bool IsAdmin { get; set; }
    //}
}
