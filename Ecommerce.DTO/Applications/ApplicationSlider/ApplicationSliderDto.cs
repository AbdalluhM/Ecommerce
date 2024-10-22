using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.ApplicationSlider
{
    public class ApplicationSliderDto : BaseDto
    {

    }
    #region API  
    public class CreateApplicationSliderAPIInputDto : ApplicationSliderDto
    {
        public int ApplicationId { get; set; }
        public bool IsActive { get; set; }
        public IFormFile File { get; set; }

    }
    public class UpdateApplicationSliderAPIInputDto : CreateApplicationSliderAPIInputDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Input 

    public class CreateApplicationSliderInputDto : ApplicationSliderDto
    {
        public bool IsActive { get; set; }
        public int ApplicationId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public int MediaId { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }

    }
    public class UpdateApplicationSliderInputDto : CreateApplicationSliderInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }

    }
    public class GetApplicationSliderInputDto : BaseDto
    {
        public int Id { get; set; }

    }



    #endregion


    #region Output 
    public class GetApplicationSliderOutputDto : ApplicationSliderDto
    {

        public int Id { get; set; }
        [JsonIgnore]
        public FileStorageBlobDto Image { get; set; }
        public string ImagePath => Image.FullPath;

        public string Name { get; set; }

        public decimal Size { get; set; }
    }

    #endregion
}
