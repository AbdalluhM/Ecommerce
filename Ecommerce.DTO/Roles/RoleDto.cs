using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Roles
{
    public class RoleDto
    {

        public class CreateRoleInputDto
        {
            public string Name { get; set; }
            public bool IsActive { get; set; }
            [JsonIgnore]
            public int CreatedBy { get; set; }
            [JsonIgnore]
            public DateTime CreateDate { get; set; }
            public IEnumerable<RolePageActionDto> RolePageAction { get; set; }
        }
        public class UpdateRoleInputDto : CreateRoleInputDto
        {
            public int RoleId { get; set; }
            [JsonIgnore]
            public int ModifiedBy { get; set; }
            [JsonIgnore]
            public DateTime ModifiedDate { get; set; }

        }
        public class RolePageActionDto
        {
            //public int RoleId { get; set; }
            //public bool IsActive { get; set; }
            //public int PageActionId { get; set; }
            [JsonIgnore]
            public int CreatedBy { get; set; }
            [JsonIgnore]
            public int? ModifiedBy { get; set; }
            [JsonIgnore]
            public DateTime CreateDate { get; set; }
            [JsonIgnore]
            public DateTime ModifiedDate { get; set; }
            //public PageActionDto PageAction { get; set; }
            public int PageId { get; set; }
            //public PageDto Page { get; set; }
            public List<int> ActionIds { get; set; }
        }
        public class PageActionDto
        {
            public int Id { get; set; }
            public int PageId { get; set; }
            //public PageDto Page { get; set; }
            public List<int> ActionIds { get; set; }
        }
        public class PageDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class ActionDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }

        }
        public class GetRoleOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Status { get; set; }
            public List<GetPageActionOutputDto> PagesActions { get; set; } = new ();
            [JsonIgnore]
            public GetRolePageActionOutputDto RolePageActions { get; set; } = new();

        }
        public class GetPageActionOutputDto
        {
            public int PageId { get; set; }
            public List<int> ActionIds { get; set; }
        }
        public class GetRolePageActionOutputDto
        {
            public int PageId { get; set; }
            public List<int> ActionIds { get; set; }

        }

       
      
      
        
    }
}

