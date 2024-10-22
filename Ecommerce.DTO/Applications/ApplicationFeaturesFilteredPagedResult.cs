using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Applications
{
    public class ApplicationFeaturesFilteredPagedResult : FilteredResultRequestDto
   {
        public int FeatureId { get; set; }
        public int ApplicationId { get; set; }

    }
}
