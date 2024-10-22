using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionModules;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Applications.Modules
{
    public interface IVersionModuleBLL
    {
        #region Basic CRUD & Internal Business
        Task<IResponse<GetVersionModuleOutputDto>> CreateAsync( CreateVersionModuleInputDto inputDto );
        Task<IResponse<List<GetUnAsignedModuleOutputDto>>> GetAllUnAsignedModuleAsync(GetApplicationByIdInputDto inputDto);
        Task<IResponse<GetVersionModuleOutputDto>> UpdateAysnc( UpdateVersionModuleInputDto inputDto );

        Task<IResponse<bool>> DeleteAsync( DeleteApplicatinModuleInputDto inputDto );
        Task<IResponse<GetVersionModuleOutputDto>> GetByIdAsync( GetVersionModuleInputDto inputDto );

        Task<IResponse<List<GetVersionModuleOutputDto>>> GetAllAsync( GetAllVersionModuleInputDto inputDto );
        Task<IResponse<PagedResultDto<GetVersionModuleOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto );

        #endregion
        #region Totals
        int GetVersionModulesCount( int applicationId );

        #endregion
    }
}
