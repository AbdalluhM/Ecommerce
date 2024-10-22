using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Modules.ModuleSlider
{
    public class ModuleSliderDto : BaseDto
    {
       
    }
    #region API  
    public class CreateModuleSliderAPIInputDto : ModuleSliderDto
    {
        public int ModuleId { get; set; }
        public bool IsActive { get; set; }
        public IFormFile File { get; set; }

    }
    public class UpdateModuleSliderAPIInputDto : CreateModuleSliderAPIInputDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Input 

    public class CreateModuleSliderInputDto : ModuleSliderDto
    {
        public bool IsActive { get; set; }
        public int ModuleId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public int MediaId { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }

    }
    public class UpdateModuleSliderInputDto : CreateModuleSliderInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }

    }
    public class GetModuleSliderInputDto : BaseDto
    {
        public int Id { get; set; }

    }



    #endregion


    #region Output 
    public class GetModuleSliderOutputDto : ModuleSliderDto
    {
        public int Id { get; set; }

        public FileStorageDto Image { get; set; }
        public string ImagePath => Image.FullPath;

        public string Name { get; set; }

        public decimal Size { get; set; }
    }

    #endregion
}
