using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using System.Text.Json.Serialization;
using System;
using Ecommerce.DTO.Settings.Files;


namespace Ecommerce.DTO.Modules
{
    public class ModuleDto : BaseDto
    {
       
    }
    #region API
    public class CreateModuleAPIInputDto : ModuleDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
     
        public IFormFile File { get; set; }
    }
    public class UpdateModuleAPIInputDto : CreateModuleAPIInputDto
    {
        public int Id { get; set; }

    }
    #endregion
    #region Input

    public class CreateModuleInputDto : ModuleDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public int ImageId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }



    }
    public class UpdateModuleInputDto : CreateModuleInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteModuleInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetModuleInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Output
    public class GetModuleOutputDto : ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public bool IsActive { get; set; }

        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }


                          

    }

    #endregion

}
