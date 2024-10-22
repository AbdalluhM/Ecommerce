using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Paging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.DownloadCenter
{
    public interface IDownloadCenterBLL
    {


        #region Basic CRUD & Internal Business

        Task<IResponse<GetDownloadCenterApplicationsOutputDto>> GetByIdAsync(int ApplicationId);

        Task<IResponse<List<GetDownloadCenterApplicationsOutputDto>>> GetAllAsync();
        Task<IResponse<List<GetApplicationTagOutputDto>>> GetApplicationTagsAsync();
        Task<IResponse<List<GetAddonTagOutputDto>>> GetAddOnTagsAsync();
        Task<IResponse<PagedResultDto<GetDownloadCenterApplicationsOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);
        Task<IResponse<DownloadCenterResultDto>> GetDownloadCenterAsync(string searchTerm, int countryId, int? applicationTagId = null, int? addOnTagId = null);

        #endregion

    }
}
