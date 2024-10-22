using Ecommerce.BLL.Responses;
using Ecommerce.DTO.WorkSpaces;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Ecommerce.BLL.WorkSpaces
{
    public interface IWorkSpaceBLL
    {
        Task<IResponse<List<DexefCountryDto>>> GetDexefCountry();
        Task<IResponse<List<DexefCurrencyDto>>> GetDexefCurrency();
        Task<IResponse<WorkspaceDetailsDto>> CreateAsync(CreateWorkSpaceDto inputDto);
        Task<IResponse<int>> UpdateAsync(UpdateWorkSpaceDto inputDto);
        Task<IResponse<int>> ExtendWorkSpaceAsync(int workSpaceId);
        Task<IResponse<IEnumerable<WorkspaceDto>>> GetWorkSpacesAsync(int customerId);
        Task<IResponse<WorkspaceDetailsDto>> GetWorkspaceDetailsAsync(int id);
        int GetCustomerCountryCurrencyId(int customerId);
        Task<IResponse<List<SimpleDatabaseDto>>> GetSimpleDatabaseAsync();
        Task<IResponse<int>> CreateSimpleDatabaseAsync(SimpleDatabaseDto inputDto);
        Task<IResponse<SimpleDatabaseDto>> GetSimpleDatabaseByIdAsync(int id);
        Task<IResponse<bool>> DeleteSimpleDatabase(int id);
        Task<IResponse<int>> UpdateSimpleDatabase(SimpleDatabaseDto inputDto);

    }
}
