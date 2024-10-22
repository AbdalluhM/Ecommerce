using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ContactUsHelpOption:EntityBase
    {
        public int Id { get; set; }
        public string HelpName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
