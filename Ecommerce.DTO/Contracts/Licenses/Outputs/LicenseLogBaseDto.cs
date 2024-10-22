using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseLogBaseInfoDto : PagedResultDto<LicenseLogDto>
    {
        public string CustomerName { get; set; }

        public string Serial { get; set; }

        public string Product { get; set; }
    }
}
