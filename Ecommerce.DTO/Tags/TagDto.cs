using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Tags
{
    public class TagDto : BaseDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    #region Input 
   
    public class CreateTagInputDto : TagDto
    {
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateTagInputDto : CreateTagInputDto
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetTagInputDto : BaseDto
    {
        public int Id { get; set; }

    }




    #endregion


    #region Output 
    public class GetTagOutputDto : TagDto
    {
        public int Id { get; set; }
    }

    #endregion
}
