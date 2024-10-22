using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.CustomerProduct
{
    public class NewAddOnLicenseDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }

        public int VersionSubscriptionId { get; set; }

        public int LicenseId { get; set; }

        public int AddOnSubscriptionId { get; set; }
    }
}
