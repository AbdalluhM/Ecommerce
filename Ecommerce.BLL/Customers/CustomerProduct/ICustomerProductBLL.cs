using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.CustomerProduct;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Customers.CustomerProduct
{
    public interface ICustomerProductBLL
    {

        #region CustomerProduct
        Task<IResponse<GetCustomerProductOutputDto>> GetCustomerProductById(CustomerProductInputDto inputDto);
        Task<IResponse<List<GetCustomerProductOutputDto>>> GetAllCustomerProducts(int customerId);
        Task<IResponse<List<GetCustomerProductOutputDto>>> GetCustomerSubscriptions(int customerId);
        Task<IResponse<List<GetCustomerApplicationVersionsDto>>> GetCustomerApplicationVersions(int customerId, int appId);
        //Task<IResponse<GetCustomerApplicationVersionsAndAddOnsDto>> GetCustomerApplicationVersionsAndAddOns(int customerId, int appId);
        Task<IResponse<NearestVersionRenewDto>> GetFirstRenewDateOfAllVersionsOfUser(int customerId);
        Task<IResponse<List<CustomerProductLookupDto>>> GetCustomerProductByAppIdLookup(int customerId, int appId);
        Task<IResponse<List<CustomerProductLookupDto>>> GetAllCustomerProductsLookupAsync(int customerId, int? appId = null);
        Task<IResponse<List<CustomerProductWorkspacesDto>>> GetAllCustomerProductsWorkspacesLookupAsync(int customerId);
        Task<IResponse<List<CustomerProductLookupDto>>> GetAllCustomerProductsLookupAsync(int customerId, int deviceTypeId);
        Task<IResponse<List<CustomerApplicationLookupDto>>> GetCustomerApplicationsLookupAsync(int customerId);
        int GetVersionSubscriptionUsedDevice(int versionSubscriptionId);


        #endregion

        #region MangeLicenses
        Task<IResponse<GetCustomerSubscriptionOutputDto>> GetCustomerSubscriptionById(CutomerSubscriptionInputDto inputDto);
        Task<IResponse<bool>> UpdateCustomerSubscription(UpdateCutomerSubscriptionInputDto inputDto);
        Task<IResponse<bool>> RefundSubscriptionRequest(RefundRequestInputDto inputDto);
        #endregion

        #region CustomerProductAddOn
        Task<IResponse<List<GetAllCustomerProductAddonOutputDto>>> GetCustomerProductAddOns(CustomerProductInputDto inputDto);

        Task<IResponse<ProductAddonDetailsDto>> GetProductAddonDetailsAsync(int addonSubscriptionId);

        Task<IResponse<bool>> CancelAddonSubscriptionAsync(CancelAddonSubscriptionDto cancelAddonSubscription);
        #endregion

        #region CustomerProductReleases
        Task<IResponse<List<GetReleasesOutputDto>>> GetCustomerProductReleases(CustomerProductInputDto inputDto);
        Task<IResponse<bool>> DownloadRelease(VersionReleaseInputDto inputDto);
        #endregion

    }
}
