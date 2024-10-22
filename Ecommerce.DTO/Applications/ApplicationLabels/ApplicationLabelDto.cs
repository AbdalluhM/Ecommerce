using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.ApplicationLabels
{
    public class ApplicationLabelDto : BaseDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    #region Input 

    public class CreateApplicationLabelInputDto : ApplicationLabelDto
    {
        public int ApplicationId { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateApplicationLabelInputDto: CreateApplicationLabelInputDto
    {

        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }

    }
    public class GetApplicationLabelInputDto : BaseDto
    {
        public int ApplicationId { get; set; }

    }
  
    #endregion


    #region Output 
    public class GetApplicationLabelOutputDto : ApplicationLabelDto
    {
        //represents Id
        public int ApplicationId { get; set; }
    }
   
    #endregion
}



