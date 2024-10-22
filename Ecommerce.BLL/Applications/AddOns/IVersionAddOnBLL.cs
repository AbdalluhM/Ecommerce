using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionAddOns;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Applications.AddOns
{
    public interface IVersionAddOnBLL
    {



        #region Basic CRUD & Internal Business
        Task<IResponse<GetVersionAddOnOutputDto>> CreateAsync( CreateVersionAddOnInputDto inputDto );
        Task<IResponse<List<GetUnAsignedAddOnOutputDto>>> GetAllUnAsignedAddOnAsync(GetApplicationByIdInputDto inputDto);
        Task<IResponse<GetVersionAddOnOutputDto>> UpdateAysnc( UpdateVersionAddOnInputDto inputDto );

        Task<IResponse<bool>> DeleteAsync( DeleteApplicatinAddOnInputDto inputDto );
        Task<IResponse<GetVersionAddOnOutputDto>> GetByIdAsync( GetVersionAddOnInputDto inputDto );

        Task<IResponse<List<GetVersionAddOnOutputDto>>> GetAllAsync( GetApplicationByIdInputDto inputDto);
        Task<IResponse<PagedResultDto<GetVersionAddOnOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto );

        #endregion

    }
}
