using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;

using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Customers
{
    public interface ICustomerBLL
    {
        Task<IResponse<PagedResultDto<GetCustomerRegistrationHistoryOutputDto>>> GetCustomerHistoryPagedListAsync(FilterByEmployeeCountryInputDto pagedDto);
        Task<IResponse<bool>> MakeCustomerVerified(UpdateCustomerStatusInputDto inputDto);
        IResponse<bool> Delete(DeleteCustomerInputDto inputDto);
        Task<GetCustomerOutputDto> CreateAsync(PreregisterDto inputDto);
        Task<IResponse<GetCustomerOutputDto>> UpdateAysnc(UpdateCustomerInputDto inputDto);
        IResponse<List<GetCustomerRegistrationHistoryOutputDto>> GetAllCustomerRegisterHistory();
        Task<IResponse<GetCustomerImageOutputDto>> UploadImage(UploadCustomerImageInputDto inputDto);
        //FileDto GetFile(IFormFile file, FilePathEnum filePathEnum);
        Task<IResponse<GetCustomerOutputDto>> GetByIdAsync(GetCustomerInputDto inputDto);
        Task<IResponse<bool>> UpdateVersionDownloadCount(UpdateVersionDownloadCountInputDto inputDto);
        Task<IResponse<bool>> UpdateAddOnDownloadCount(UpdateAddOnDownloadCountInputDto inputDto);
        Task<IResponse<GetCustomerReferencesOutputDto>> GetCustomerByDetails(int customerId);
        Task<IResponse<PagedResultDto<CustomerReguesttDto>>> GetCustomerRequestByProductPagedList(LicencesFilterInputDto pagedDto);
        Task<IResponse<PagedResultDto<GetContractCustomerOutputDto>>> GetContractustomerPagedListAsync(FilterByEmployeeCountryInputDto pagedDto);
        IResponse<bool> UpdateCustomerByAdmin(UpdateCustomerByAdminInputDto inputDto);
        Task<IResponse<PagedResultDto<CustomerActivitesDto>>> GetCustomerActivitesPagedListAsync(LogFilterPagedResultDto pagedDto);
        IResponse<List<GetContractCustomerOutputDto>> GetAlltCustomersInContract(int employeeId);

        Task<IResponse<List<GetAllCountriesOutputDto>>> GetAllCountriesAsync( );

        Task<IResponse<int>> CreateCustomerAsync(NewCustomerByAdminDto newCustomerDto, int currentEmployeeId);
        Task<IResponse<bool>> DeleteCustomerAsync(int customerId);

        #region TestValidateCustomer
        Task<IResponse<bool>> TestValidateMobile(int customerMobileId);
        Task<IResponse<bool>> TestValidateEmail(int customerEmailId);
        Task<IResponse<bool>> TestDeleteCustomer(int customerId);

        Task<IResponse<bool>> DeleteCustomerByEmailOrPhonrAsync(string userName);

        #endregion
    }
}
