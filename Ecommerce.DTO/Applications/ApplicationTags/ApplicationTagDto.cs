using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.ApplicationTags
{
    public class ApplicationTagDto : BaseDto
    {
        public int ApplicationId { get; set; }
        public int TagId { get; set; }
    }

    #region Input 

    public class CreateApplicationTagInputDto : ApplicationTagDto
    {

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateApplicationTagInputDto: CreateApplicationTagInputDto
    {

        public int Id { get; set; }
        public bool IsFeatured { get; set; }


    }
    public class GetApplicationTagInputDto : BaseDto
    {
        public int Id { get; set; }

    }
  
    #endregion


    #region Output 
    public class GetApplicationTagOutputDto : ApplicationTagDto
    {
        //represents Id
        public int Id { get; set; }
        public bool IsFeatured { get; set; }
        public string Name { get; set; }
    }
   
    #endregion
}



