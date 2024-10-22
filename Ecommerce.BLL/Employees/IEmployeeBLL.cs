using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Employees
{
    public interface IEmployeeBLL
    {


        #region API's 
        Task<IResponse<LoginModelOutputDto>> LoginAsync( LoginModelInputDto inputDto );

        Task<IResponse<bool>> LogOutAsync( );

        #endregion
        #region Basic CRUD & Internal Business
        Task<IResponse<GetEmployeeOutputDto>> CreateAsync( CreateEmployeeInputDto inputDto );
        Task<IResponse<bool>> ChangePasswordAsync( ChangePasswordInputDto inputDto );
        Task<IResponse<GetEmployeeOutputDto>> UpdateAysnc( UpdateEmployeeInputDto inputDto );
        Task<IResponse<GetEmployeeOutputDto>> UpdateCountryByAdminAysnc( UpdateEmployeeInputDto inputDto );

        Task<IResponse<bool>> DeleteAsync( DeleteTrackedEntityInputDto inputDto );
        Task<IResponse<GetEmployeeOutputDto>> GetByIdAsync( GetEmployeeInputDto inputDto );
        Task<Employee> GetByIdAsync( int id );
        Task<IResponse<List<GetEmployeeOutputDto>>> GetAllListAsync( );
        Task<List<int>> GetEmployeesByCustomerIdAsync(int customerId);
        Task<IResponse<PagedResultDto<GetEmployeeOutputDto>>> GetPagedListAsync( FilteredResultRequestDto pagedDto );
        Task<List<int>> GetEmployeeCountries( int employeeId );

        IEnumerable<int> GetEmployeeCountryCurrencies(int employeeId);
        #endregion

    }
}
