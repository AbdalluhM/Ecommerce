using System;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class ChangeLicenseDto : LicenseBaseDto
    {
        public string OldSerialNumber { get; set; }

        public string Serial { get; set; }

        public string DeviceName { get; set; }

        public DateTime ActivateOn { get; set; }

        public DateTime RenewalDate { get; set; }

        public DateTime ChangedDate { get; set; }
    }
}
