using Ecommerce.DTO.Settings.Files;
using System;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseProductLookupDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UsedLicenseCount { get; set; }

        public int TotalLicenseCount { get; set; }

        public DateTime LastInvoiceStartDate { get; set; }

        public DateTime LastInvoiceEndDate { get; set; }

        public FileStorageDto Image { get; set; } = new();
    }
}
