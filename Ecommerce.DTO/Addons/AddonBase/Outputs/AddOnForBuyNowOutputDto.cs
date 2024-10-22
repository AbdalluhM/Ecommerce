using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Settings.Files;

using System.Collections.Generic;

namespace Ecommerce.DTO.Addons.AddonBase.Outputs
{
    public class AddOnForBuyNowOutputDto
    {
        public int VersionId { get; set; }
        public int AddOnId { get; set; }
        public string AddOnName { get; set; }
        public string AddOnTitle { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }

        public FileStorageDto AddOnImage { get; set; }
        //public List<AddOnPriceAllDetailsDto> AddOnPrices { get; set; } = new();

        public AddOnPriceAllDetailsDto AddOnPrice { get; set; } = new();

        public string Discriminator { get; set; }


    }

}
