﻿using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class InvoiceType:EntityBase
    {
        public InvoiceType()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}