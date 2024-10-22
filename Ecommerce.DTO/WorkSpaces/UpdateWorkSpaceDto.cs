

using Microsoft.AspNetCore.Http;
using Ecommerce.DTO.Files;
using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.WorkSpaces
{
    public class UpdateWorkSpaceDto : CreateWorkSpaceDto
    {
        public int Id { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
