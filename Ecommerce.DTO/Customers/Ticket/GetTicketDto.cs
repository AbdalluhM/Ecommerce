using Azure;
using Microsoft.AspNetCore.Http;
using Ecommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Ticket
{
    public class GetTicketDto
    {
        public int Id { get; set; }
        public int Topic { get; set; }
        public string ProductNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Program { get; set; }
        public DateTime CreateDate { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        // public List<FileStorageDto> Files { get; set; } 


    }

    public class GetTicketFilterDateDto
    {
        public DateTime Date { get; set; }
        public List<GetTicketDto> Tickets { get; set; }
    }
    public class FileStorageDto
    {
        public Guid Name { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public decimal FileSize { get; set; }
        public int FileTypeId { get; set; }
        public int? EntityId { get; set; }
        // public int? NameId { get; set; }
    }
}
