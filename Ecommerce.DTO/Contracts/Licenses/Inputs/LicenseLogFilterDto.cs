using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class LicenseLogFilterDto : FilteredResultRequestDto
    {
        public int LicenseId { get; set; }
    }
}
