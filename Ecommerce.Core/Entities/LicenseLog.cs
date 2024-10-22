using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicenseLog:EntityBase
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public int ActionTypeId { get; set; }
        public int? OldStatusId { get; set; }
        public int? NewStatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsCreatedByAdmin { get; set; }
        public int? CreatedByAdmin { get; set; }
        public int? CreatedByCustomer { get; set; }

        public virtual LicenseActionType ActionType { get; set; }
        public virtual Employee CreatedByAdminNavigation { get; set; }
        public virtual Customer CreatedByCustomerNavigation { get; set; }
        public virtual License License { get; set; }
        public virtual LicenseStatu NewStatus { get; set; }
        public virtual LicenseStatu OldStatus { get; set; }
    }
}
