using System;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class ExpiredLicenseDto : LicenseBaseDto
    {
        public string DeviceName { get; set; }

        public string Serial { get; set; }

        public DateTime ActivateOn { get; set; }

        public DateTime RenewalDate { get; set; }
    }
}
