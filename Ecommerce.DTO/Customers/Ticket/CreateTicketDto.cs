using Microsoft.AspNetCore.Http;
using Ecommerce.DTO.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Ticket
{
    public class CreateTicketDto
    {

        public int? CustomerId { get; set; }

        public int Topic { get; set; }

        public int? InvoiceId { get; set; }

        public string ProductNumber { get; set; }

        public Guid? SubjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string Program { get; set; }

        public string SenderName { get; set; }

        public bool HasAttachment { get; set; }
        
        public List<IFormFile> Attachment { get; set; } = new();
    }
}
