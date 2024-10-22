using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO
{
    public abstract class BaseDto : IDto
    {

        //[JsonIgnore]
        //public string Lang { get; set; }
        //public IResponseDto Response { get; set; } = new ResponseDto();
    }

    public class GetEntityInputDto : BaseDto
    {
        public int Id { get; set; }
    }
    public class DeleteEntityInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    public class DeleteTrackedEntityInputDto : DeleteEntityInputDto
    {
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
}
