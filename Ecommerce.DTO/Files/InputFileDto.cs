using Ecommerce.Core.Enums.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Files
{
    public class InputFileDto
    {
        public TableNameEnum TableName { get; set; }
        public int EntityId { get; set; }
    }
}
