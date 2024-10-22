using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ExceptionLog:EntityBase
    {
        public int Id { get; set; }
        public string Exception { get; set; }
    }
}
