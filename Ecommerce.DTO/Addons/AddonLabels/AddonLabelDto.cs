using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Addons.AddonLabels
{
    public class AddonLabelDto
    {
        public string Name { get; set; }
        public string Color { get; set; }        
    }

    #region Input 

    public class CreateAddonLabelInputDto : AddonLabelDto
    {
        public int AddOnId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateAddonLabelInputDto : CreateAddonLabelInputDto
    {
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetAddOnLabelInputDto : BaseDto
    {
        public int AddOnId { get; set; }

    }
 

    #endregion


    #region Output 
    public class GetAddonLabelOutputDto : AddonLabelDto
    {
        public int Id { get; set; }
        public int AddOnId { get; set; }
    }
   

    #endregion
}
