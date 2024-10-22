using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Applications.ApplicationVersions
{
    public class VersionFilteredPagedResult : FilteredResultRequestDto
    {
        public int VersionId { get; set; }

    }
}
