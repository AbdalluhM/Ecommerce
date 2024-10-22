using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;

using System.Collections.Generic;

namespace Ecommerce.DTO.Applications.ApplicationBase.Outputs
{
    public class AppDetailsDto : BaseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public FileStorageDto Logo { get; set; } = new();

        public string Description { get; set; }

        public bool HasLabel => Label != null ? true : false;

        public bool IsInWishlist { get; set; }

        public GetApplicationLabelOutputDto Label { get; set; } = new();

        public List<string> SlidersPath { get; set; } = new();

        public IEnumerable<GetTagOutputDto> Tags { get; set; } = new List<GetTagOutputDto>();

        public IEnumerable<RelatedAppDto> RelatedApps { get; set; } = new List<RelatedAppDto>();

        public RateDto Rate { get; set; } = new();
        public string countryCurrency { get; set; }

    }
}
