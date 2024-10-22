using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Apps;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Apps
{
    public interface IAppsBLL
    {
        #region Basic CRUD & Internal Business

        Task<IResponse<BrowseAppsOutputDto>> GetByIdAsync(int ApplicationId );

        Task<IResponse<BrowseAppsOutputDto>> GetAllAsync( string searchTerm = null );
        Task<IResponse<BrowseAppsOutputDto>> FilterApps(FilterAppsInputDto inputDto);
        Task<IResponse<GetPriceRangeOutputDto>> GetPriceRangeAsync(int? ApplicationId = null );


        #endregion
    }
}
