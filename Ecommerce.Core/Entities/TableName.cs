using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TableName:EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
