using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.DTO.Roles.RoleDto;

namespace Ecommerce.BLL.Roles
{
    public interface IRoleBLL
    {
        Task<IResponse<GetRoleOutputDto>> CreateRoleAsync(CreateRoleInputDto inputDto);
        Task<IResponse<GetRoleOutputDto>> UpdateRoleAsync(UpdateRoleInputDto inputDto);
        Task<IResponse<GetRoleOutputDto>> GetRoleByIdAsync(int id);
        Task<IResponse<PagedResultDto<GetRoleOutputDto>>> GetAllRolesPagedList(FilteredResultRequestDto pagedDto);
        Task<IResponse<List<GetRoleOutputDto>>> GetAllRoleAsync();
        Task<IResponse<bool>> DeleteRoleAsync(int Id);

        #region LookUp
        Task<IResponse<List<PageDto>>> GetLookUpPagesAsync();
        Task<IResponse<List<ActionDto>>> GetLookUpActionsAsync();
        #endregion

    }
}
