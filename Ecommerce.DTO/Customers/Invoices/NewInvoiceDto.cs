using Ecommerce.Core.Entities;

using System;
using System.Collections.Generic;

namespace Ecommerce.DTO.Customers.Invoices
{
    public class NewInvoiceDto
    {
        public int VersionSubscriptionId { get; set; }

        public int? AddonSubscriptionId { get; set; }

        public int InvoiceTypeId { get; set; }

        public int PaymentMethodId { get; set; }
        public int CountryId { get; set; }

        public Invoice OldInvoice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Save invoice after create, Default: True.
        /// </summary>
        public bool IsCommit { get; set; } = true;
    }
}
