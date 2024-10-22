using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Paging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Applications.Features
{
    public interface IVersionFeatureBLL
    {



        #region Basic CRUD & Internal Business
        Task<IResponse<bool>> CreateAsync( CreateVersionFeatureInputDto inputDto );
        //Task<IResponse<GetEmployeeOutputDto>> CreateAsync( CreateEmployeeInputDto inputDto );

        Task<IResponse<GetVersionFeatureOutputDto>> UpdateAysnc( UpdateVersionFeatureInputDto inputDto );

        Task<IResponse<bool>> DeleteAsync( DeleteApplicatinFeatureInputDto inputDto );
        Task<IResponse<GetVersionFeatureOutputDto>> GetByIdAsync( GetVersionFeatureInputDto inputDto );

        Task<IResponse<List<GetVersionFeatureOutputDto>>> GetAllAsync( GetAllVersionFeatureInputDto inputDto);
        Task<IResponse<List<GetUnAsignedFeatureOutputDto>>> GetAllUnAsignedFeatureAsync(GetApplicationByIdInputDto inputDto);
        Task<IResponse<PagedResultDto<GetVersionFeatureOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto );

        #endregion

    }
}
