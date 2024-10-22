using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.HomePage
{
    public class HomePageDto
    {
        public class GetCustomerAppTagsOrHighlightedOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public FileStorageDto Logo { get; set; }
            public VersionPriceDetailsDto Price { get; set; }
        }
        public class FilterCustomerAppTagsInputDto : FilteredResultRequestDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            [JsonIgnore]
            public int CountryId { get; set; }
        }
        public class FilterCustomerAppTagsApiInputDto : FilteredResultRequestDto
        {
        
        }
    }
}
