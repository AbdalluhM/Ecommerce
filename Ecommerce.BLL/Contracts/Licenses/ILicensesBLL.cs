using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO.Contracts.Licenses.Inputs;
using Ecommerce.DTO.Contracts.Licenses.Outputs;
using Ecommerce.DTO.Customers.CustomerProduct;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Contracts.Licenses
{
    public interface ILicensesBLL
    {
        Task<IResponse<List<LicenseCustomerLookupDto>>> GetCustomersLookupAsync(int currentEmployeeId);
        Task<IResponse<List<LicenseProductLookupDto>>> GetProductsLookupAsync(int customerId, int currentEmployeeId);
        Task<IResponse<List<GetReasonChangeDeviceOutputDto>>> GetReasonChangeDeviceLookupAsync();
        Task<IResponse<List<GetCustomerProductLicenseAddonOutputDto>>> GetVersionLicencesLookupAsync(CustomerProductInputDto inputDto);

        Task<IResponse<PagedResultDto<GetLicenseOutputDto>>> GetCustomerProductLicencesPagedListAsync(LicencesFilterInputDto pagedDto);
        Task<IResponse<PagedResultDto<GetLicenseOutputDto>>> GetCustomerProductsLicencesPagedListAsync(AllLicencesFilterInputDto pagedDto);
        Task<IResponse<PagedResultDto<GetRequestChangeDeviceOutputDto>>> GetCustomerRequestChangeDevice(LicencesFilterInputDto pagedDto);

        Task<IResponse<PagedResultDto<LicenseRequestDto>>> GetLicensesRequestsAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId);
        Task<IResponse<PagedResultDto<ActiveLicenseDto>>> GetActiveLicensesAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId);
        Task<IResponse<PagedResultDto<ExpiredLicenseDto>>> GetExpiredLicensesAsync(LicenseRequestFilterDto requestFilters, int currentEmployeeId);
        Task<IResponse<PagedResultDto<ChangeLicenseDto>>> GetChangedLicensesAsync(ChangeLicenseFilterDto requestFilters, int currentEmployeeId);

        Task<IResponse<LicenseLogBaseInfoDto>> GetLicenseLogAsync(LicenseLogFilterDto requestFilters);

        Task<IResponse<GetLicenseOutputDto>> CreateDeviceByCustomerAsync(CreateLicenseInputDto inputDto);
        Task<IResponse<bool>> CreateDeviceByAdminAsync(NewDeviceDto newLicense, IFileDto fileDto, int currentEmployeeId);
        Task<IResponse<bool>> CreateAddOnDeviceAsync(NewAddOnLicenseDto newAddonLicense);

        Task<IResponse<bool>> RenewVersionLicenseAsync(LicenseActionInputDto inputDto, bool isCommit = true);
        Task<IResponse<bool>> RenewAddOnLicenseAsync(AddOnLicenseActionInputDto inputDto);
        Task<List<IResponse<bool>>> RenewAllLicensesAsync(int customerSubscriptionId);

        Task<IResponse<bool>> UploadLicenseAsync(UploadLicenseDto uploadLicenseDto, IFileDto fileDto, int currentEmployeeId);

        Task<IResponse<RefundDto>> AcceptRefundRequestAsync(int requestId, int currentEmployeeId);
        Task<IResponse<bool>> AcceptRefundRequestByCashAsync(int requestId, int currentEmployeeId);
        Task<IResponse<bool>> RejectRefundRequestAsync(int requestId, int currentEmployeeId);

        Task<IResponse<GetRequestChangeDeviceOutputDto>> ChangeDeviceAsync(UpdateLicenseInputDto inputDto);
        Task<IResponse<bool>> RejectChangeDeviceRequestAsync(int requestId, int currentEmployeeId);

        Task<IResponse<bool>> ReactivateExpiredLicenseAsync(ReactivateLicenseDto reactivateLicenseDto, IFileDto fileDto, int currentEmployeeId);

        Task MakeLicensesExpiredAsync(List<License> licenses, bool isCommit = true);
    }
}
