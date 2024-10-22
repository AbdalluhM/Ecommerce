using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Lookups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.LookUps
{
    public interface ILookUpBLL
    { 
        #region Country
        IResponse<List<GetCountryOutputDto>> GetAllCountries( );
        Task<IResponse<List<GetCountryOutputDto>>> GetAllCountriesAsync( );
        #endregion

        #region Contact Us.
        IResponse<IEnumerable<ContactUsLookupDto>> GetAllContactUsHelpOptions();
        IResponse<IEnumerable<ContactUsLookupDto>> GetAllIndustries();
        IResponse<IEnumerable<ContactUsLookupDto>> GetAllCompanySize();
        IResponse<IEnumerable<DexefBranchDto>> GetAllDexefBranches();
        #endregion

        #region Currency
        IResponse<List<GetCurrencyOutputDto>> GetAllCurrencies( );
        Task<IResponse<List<GetCurrencyOutputDto>>> GetAllCurrenciesAsync( );

        #endregion

        #region EmployeeType
        IResponse<List<GetEmployeeTypeOutputDto>> GetAllEmployeeTypes( );

        Task<IResponse<List<GetEmployeeTypeOutputDto>>> GetAllEmployeeTypesAsync( );
        Task<IResponse<GetDashBoardOutputDto>> GetAllDashboardCounts( );

        #endregion

        #region SubscriptionType

        IResponse<List<GetSubscriptionTypeOutputDto>> GetAllSubscriptionTypes( );
        Task<IResponse<List<GetSubscriptionTypeOutputDto>>> GetAllSubscriptionTypesAsync( );
        #endregion
    }
}
