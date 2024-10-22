using System;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseRequestDto : LicenseBaseDto
    {
        public int RequestId { get; set; }

        public int? InvoiceId { get; set; }

        public string OldDevice { get; set; }

        public string DeviceName { get; set; }

        public string OldSerial { get; set; }

        public string Serial { get; set; }

        public string Reason { get; set; }
        public string AddonVersionName { get; set; }

        public DateTime CreateDate { get; set; }

        public int RequestTypeId { get; set; }

        public string RequestType { get; set; }
    }
}
