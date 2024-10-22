using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Connection:EntityBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
