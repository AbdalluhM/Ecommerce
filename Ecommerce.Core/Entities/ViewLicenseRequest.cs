using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewLicenseRequest:EntityBase
    {
        public int RequestId { get; set; }
        public int? LicenseId { get; set; }
        public int? InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string ContractSerial { get; set; }
        public int CountryId { get; set; }
        public bool IsAddOn { get; set; }
        public int? CustomerSubscriptionId { get; set; }
        public string VersionName { get; set; }
        public string AddonName { get; set; }
        public DateTime CreateDate { get; set; }
        public string OldDevice { get; set; }
        public string DeviceName { get; set; }
        public string OldSerial { get; set; }
        public string Serial { get; set; }
        public string Reason { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestType { get; set; }
        public string AddonVersionName { get; set; }
    }
}
