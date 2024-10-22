using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class EmployeeType:EntityBase
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
