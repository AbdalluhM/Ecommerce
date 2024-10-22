using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class LicenseRequestFilterDto : FilteredResultRequestDto
    {
        public int? CustomerSubscriptionId { get; set; }
    }
}
