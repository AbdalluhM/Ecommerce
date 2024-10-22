using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Roles;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO.Paging;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.DTO.Roles.RoleDto;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : BaseAdminController
    {
        private readonly IRoleBLL _RoleBLL;
        public RoleController(IRoleBLL RoleBLL, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _RoleBLL = RoleBLL;
        }
        [HttpGet("GetPagedList")]
        [DxAuthorize(PagesEnum.Roles, ActionsEnum.READ)]
        public async Task< IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var output =await _RoleBLL.GetAllRolesPagedList(inputDto);
            return Ok(output);
        }
        [HttpGet("GetAllLists")]
        public async Task<IActionResult> GetAllRoles()
        {
            var output = await _RoleBLL.GetAllRoleAsync();
            return Ok(output);
        }
        [HttpGet("GetById")]
        [DxAuthorize(PagesEnum.Roles, ActionsEnum.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var output = await _RoleBLL.GetRoleByIdAsync(id);
            return Ok(output);
        }
        [HttpPost("Create")]
        [DxAuthorize(PagesEnum.Roles, ActionsEnum.CREATE)]
        public async Task<IActionResult> Create(CreateRoleInputDto inputDto)
        {
            inputDto.CreatedBy = CurrentEmployeeId;
            inputDto.CreateDate = System.DateTime.UtcNow; ;
            inputDto.RolePageAction.ToList().ForEach(x => {
                x.CreatedBy = CurrentEmployeeId;
                x.CreateDate = System.DateTime.UtcNow;
            });
            var output = await _RoleBLL.CreateRoleAsync(inputDto);
            return Ok(output);
        }
        [HttpPost("Update")]
        [DxAuthorize(PagesEnum.Roles, ActionsEnum.UPDATE)]
        public async Task<IActionResult> Update(UpdateRoleInputDto inputDto)
        {
            inputDto.ModifiedBy = CurrentEmployeeId;
            inputDto.ModifiedDate = System.DateTime.UtcNow;
            inputDto.CreatedBy = CurrentEmployeeId;
            inputDto.RolePageAction.ToList().ForEach(x => {
                x.CreatedBy = CurrentEmployeeId;
                x.CreateDate = System.DateTime.UtcNow;
                x.ModifiedBy = CurrentEmployeeId;
                x.ModifiedDate = System.DateTime.UtcNow;
            });
            var output = await _RoleBLL.UpdateRoleAsync(inputDto);

           
            
            return Ok(output);
        }
        [HttpPost("Delete")]
        [DxAuthorize(PagesEnum.Roles, ActionsEnum.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var output = await _RoleBLL.DeleteRoleAsync(id);
            //TODO: Notifiy to all members

            return Ok(output);
        }

        [HttpGet("GetLookUpPages")]
        public async Task<IActionResult> GetLookUpPages()
        {
            var output = await _RoleBLL.GetLookUpPagesAsync();
            return Ok(output);
        }
        [HttpGet("GetLookUpActions")]
        public async Task<IActionResult> GetLookUpActions()
        {
            var output = await _RoleBLL.GetLookUpActionsAsync();
            return Ok(output);
        }
    }
}
