using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.License;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class NewLicenseLogDto
    {
        public License License { get; set; } = new();

        public LicenseActionTypeEnum ActionTypeId { get; set; }

        public LicenseStatusEnum? OldStatusId { get; set; }

        public LicenseStatusEnum? NewStatusId { get; set; }

        public bool IsCreatedByAdmin { get; set; }

        public int CreatedBy { get; set; }
    }
}
