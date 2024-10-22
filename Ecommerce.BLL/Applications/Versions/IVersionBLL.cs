using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.DTO;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;

namespace Ecommerce.BLL.Applications.Versions
{
    public interface IVersionBLL
    {
        #region Version
        Task<IResponse<GetVersionOutputDto>> CreateAsync(CreateVersionInputDto inputDto);
        Task<IResponse<GetVersionOutputDto>> UpdateAsync(UpdateVersionInputDto inputDto);
        Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto);

        Task<IResponse<GetVersionOutputDto>> GetByIdAsync(GetVersionInputDto inputDto);

        Task<IResponse<List<GetVersionOutputDto>>> GetAllAsync(int? applicationId = null);

        Task<IResponse<IEnumerable<BaseLookupDto>>> GetAppVersionsLookupAsync(int appId);

        Task<IResponse<PagedResultDto<GetVersionOutputDto>>> GetPagedListAsync(ApplicationFilteredPagedResult pagedDto);

        Task<IResponse<VersionDetailsDto>> GetVersionDetailsAsync(int appId, int countryId, int? versionId = null);

        Task<IResponse<List<GetVersionsByApplicationAndPricingOutputDto>>> GetAvailableVersionsByApplicationAndPricing(GetAvailableVersionsByApplicationAndPricingInputDto inputDto);
        Task<IResponse<List<GetVersionsByApplicationAndPricingOutputDto>>> GetAvailableVersionsByApplication(int appId, int countryId);
        VersionForBuyNowOutputDto GetVersionDataForBuyNow(PaymentDetailsInputDto inputDto);
        #endregion

        #region Version Releases
        VersionRelease GetCurrentVersionReleaseOrNull(int versionId, bool isFirstSubscription = true);
        VersionRelease GetVersionReleaseById(int versionReleaseId);

        #endregion

        #region Version Prices & Missing Prices

        Task<IResponse<GetVersionPriceOutputDto>> CreateVersionPriceAsync(CreateVersionPriceInputDto inputDto);

        Task<IResponse<GetVersionPriceOutputDto>> UpdateVersionPriceAsync(UpdateVersionPriceInputDto inputDto);

        Task<IResponse<bool>> DeleteVersionPriceAsync(DeleteTrackedEntityInputDto inputDto);

        Task<IResponse<List<GetVersionPriceOutputDto>>> GetAllExistingVersionPricesAsync(GetApplicationVersionPricesInputDto inpuDto);

        Task<IResponse<List<GetVersionPriceOutputDto>>> GetAllMissingVersionPricesAsync(GetApplicationVersionPricesInputDto inputDto);

        Task<IResponse<GetVersionPriceOutputDto>> GetVersionPriceByIdAsync(GetVersionPriceInputDto inputDto);

        Task<IResponse<PagedResultDto<GetVersionPriceOutputDto>>> GetAllExistingVersionPricePagedListAsync(ApplicationFilteredPagedResult pagedDto, int currentEmployeeId);
        Task<IResponse<PagedResultDto<GetVersionPriceOutputDto>>> GetAllMissingVersionPricesPagedListAsync(ApplicationFilteredPagedResult pagedDto, int currentEmployeeId);
        #endregion

        #region Totals
        int GetMissingPriceCount(int applicationId, IEnumerable<int> countryCurrencyIds);

        int GetApplicationVersionsCount(int applicationId);

        VersionPriceDetailsDto GetApplicationMinimumVersionPrice(int applicationId, int? countryCurrencyId = null, int? priceLevelId = null);
        VersionPriceAllDetailsDto GetApplicationMinimumVersionPrices(int applicationId, int? countryCurrencyId = null, int? priceLevelId = null);

        VersionPriceDetailsDto GetMinimumVersionPrice(int versionId, int? countryCurrencyId = null, int? priceLevelId = null);

        VersionPriceAllDetailsDto GetMinimumVersionAllPrices(int versionId, int? countryId = null, int? priceLevelId = null);

        Dictionary<SubscriptionTypeEnum, VersionPrice> GetApplicationMinimumVersionPriceHelper(int appId, int? countryId = null, int? priceLevelId = null);

        List<RetrieveVersionPrice> GetApplicationVersionPrices(int versionId, int? countryCurrencyId = null);
        #endregion
    }
}