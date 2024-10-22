using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Features
{
    public class FeatureDto : BaseDto
    {
       
    }
    #region API
    public class CreateFeatureAPIInputDto : FeatureDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public IFormFile File { get; set; }
    }
    public class UpdateFeatureAPIInputDto : CreateFeatureAPIInputDto
    {
        public int Id { get; set; }
    }
    #endregion
    #region Input
    public class CreateFeatureInputDto : FeatureDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public int LogoId { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }
    }

    public class UpdateFeatureInputDto : CreateFeatureInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteFeatureInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; } 
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetFeatureInputDto : BaseDto
    {
        public int Id { get; set; }
  
    }

    
    #endregion
    #region Output
    public class GetFeatureOutputDto : FeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public FileStorageDto Logo { get; set; }
    }
    public class GetFeatureDropDownOutputDto : FeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }       
    }

    #endregion


}
