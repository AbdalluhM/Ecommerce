using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Modules.ModuleTags
{
    public class ModuleTagDto : BaseDto
    {
        public int ModuleId { get; set; }
        public int TagId { get; set; }
    }

    #region Input 

    public class CreateModuleTagInputDto : ModuleTagDto
    {

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateModuleTagInputDto: CreateModuleTagInputDto
    {

        public int Id { get; set; }
        public bool IsFeatured { get; set; }


    }
    public class GetModuleTagInputDto : BaseDto
    {
        public int Id { get; set; }

    }
  
    #endregion


    #region Output 
    public class GetModuleTagOutputDto : ModuleTagDto
    {       
        //represents Id
        public int Id { get; set; }
        public bool IsFeatured { get; set; }
        public string Name { get; set; }
    }
   
    #endregion
}



