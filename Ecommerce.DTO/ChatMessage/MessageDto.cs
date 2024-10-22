using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.ChatMessage
{
    public class MessageDto
    {

        public string? Message { get; set; }
        public int TicketId { get; set; }
        public bool HasAttachment => Files != null && Files.Any();

        [CanBeNull]
        public List<IFormFile> Files { get; set; }

        public bool IsCustomer { get; set; }
        public  ChatRecieverTypeEnum UserType { get; set; }
        public  int CustomerId { get; set; }

    }
    public class MessageInputDto
    {
        public string? Message { get; set; }
        public int TicketId { get; set; }
        public bool HasAttachment => Files != null && Files.Any();

        [CanBeNull]
        public List<IFormFile> Files { get; set; }
    }


    
    public class MessageResultDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int TicketId { get; set; }
        public string Name { get; set; }
        public bool HasAttachment => MediaUrl != null && MediaUrl.Any();

        [CanBeNull] 
        public List<MediaDto> MediaUrl { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ReadTime { get; set; }

    }

    public class MediaDto
    {
        public string Url { get; set; }  
        public string Extention { get; set; }  
        public string FileType { get; set; }  

    }
    public class FilterChatDto : FilteredResultRequestDto
    {
        public int TicketId { get; set; }


    }
    public class ConnectionChatDto
    {
        public int UserId { get; set; }
        public ChatRecieverTypeEnum UserType { get; set; }
    }
}
