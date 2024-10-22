using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Addons.AddonTags
{
   public class AddonTagDto : BaseDto
    {
        public int AddonId { get; set; }
        public int TagId { get; set; }
    }

    #region Input 

    public class CreateAddonTagInputDto : AddonTagDto
    {
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateAddonTagDto: AddonTagDto
    {

        public int Id { get; set; }
        public bool IsFeatured { get; set; }


    }
    public class GetAddOnTagInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    //public class AssignFeaturedAddonTagDto 
    //{
    //    public int Id { get; set; }
    //    public bool IsFeatured { get; set; }
    //}

    #endregion


    #region Output 
    public class GetAddonTagOutputDto : AddonTagDto
    {
        public int Id { get; set; }
        public bool IsFeatured { get; set; }
        public int AddonTagId { get; set; }
        public string Name { get; set; }
    }
    //public class SelectAddonTagDto 
    //{
    //    public int AddonTagId { get; set; }
    //    public int TagId { get; set; }
    //    public string Name { get; set; }
    //    public bool IsFeatured { get; set; }
    //}

    #endregion
}



