using Ecommerce.DTO.Settings.Files;
using System;

namespace Ecommerce.DTO.Customers.CustomerProduct
{
    public class ProductAddonDetailsDto
    {
        public int VersionSubscriptionId { get; set; }

        public int LicenseId { get; set; }

        public string AddonTitle { get; set; }

        public DateTime PurchasedDate { get; set; }

        public DateTime RenewalDate { get; set; }

        public int LicenseStatusId { get; set; }

        public FileStorageDto File { get; set; } = new();

        public bool CanCancelSubscription { get; set; }
    }
}
