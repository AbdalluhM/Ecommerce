using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.ApplicationBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationSlider;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Applications
{
    public interface IApplicationBLL
    {
        #region Application
        Task<IResponse<GetApplicationOutputDto>> CreateAsync(CreateApplicationInputDto inputDto);
        Task<IResponse<GetApplicationOutputDto>> UpdateAsync(UpdateApplicationInputDto inputDto);
        Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto);
        Task<IResponse<GetApplicationOutputDto>> GetByIdAsync(GetApplicationInputDto inputDto);
        Task<IResponse<List<GetApplicationOutputDto>>> GetAllAsync();
        Task<IResponse<PagedResultDto<GetApplicationOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);
        Task<IResponse<AppDetailsDto>> GetAppDetailsAsync(int appId, int countryId, int? currentUserId = null);
        Task<IResponse<GetAppDetailsPricingAndPlansOutputDto>> GetAppDetailsPricingAndPlans(GetAppDetailsPricingAndPlansInputDto inputDto);
        Task<IResponse<List<PriceLevelResultDto>>> GetPackagesAsync(int versionId, int subscriptionTypeId, int countryId);
        #endregion

        #region Application Label       
        Task<IResponse<GetApplicationLabelOutputDto>> CreateApplicationLabelAsync(CreateApplicationLabelInputDto inputDto);
        Task<IResponse<GetApplicationLabelOutputDto>> UpdateApplicationLabelAsync(UpdateApplicationLabelInputDto inputDto);
        Task<IResponse<bool>> DeleteApplicationLabelAsync(DeleteTrackedEntityInputDto inputDto);
        Task<IResponse<GetApplicationLabelOutputDto>> GetApplicationLabelByApplicationIdAsync(GetApplicationLabelInputDto inputDto);
        Task<IResponse<List<GetApplicationLabelOutputDto>>> GetAllApplicationLabelListByApplicationIdAsync(GetApplicationLabelInputDto ApplicationId);

        Task<IResponse<PagedResultDto<GetApplicationLabelOutputDto>>> GetApplicationLabelPagedListAsync(ApplicationFilteredPagedResult inputDto);
        #endregion

        #region Application Tag

        Task<IResponse<GetApplicationTagOutputDto>> CreateApplicationTagAsync(CreateApplicationTagInputDto inputDto);
        Task<IResponse<bool>> AssignFeaturedToApplicationTagAsync(int Id);
        Task<IResponse<bool>> DeleteApplicationTagAsync(DeleteEntityInputDto inputDto);
        Task<IResponse<GetApplicationTagOutputDto>> GetApplicationTagByIdAsync(GetApplicationTagInputDto getApplicationPriceInputDto);

        Task<IResponse<List<GetApplicationTagOutputDto>>> GetAllApplicationTagListByApplicationIdAsync(int ApplicationId);
        Task<IResponse<List<GetTagOutputDto>>> GetAllActiveTagsNotAssignToApplicationIdAsync(int ApplicationId);
        Task<IResponse<GetApplicationTagOutputDto>> UpdateApplicationTagAsync(UpdateApplicationTagInputDto inputDto);
        Task<IResponse<PagedResultDto<GetApplicationTagOutputDto>>> GetApplicationTagPagedListAsync(ApplicationFilteredPagedResult inputDto);

        #endregion

        #region Application Slider
        Task<IResponse<GetApplicationSliderOutputDto>> CreateApplicationSliderAsync(CreateApplicationSliderInputDto inputDto);
        Task<IResponse<PagedResultDto<GetApplicationSliderOutputDto>>> GetAllApplicationSlidersPagedListAsync(ApplicationFilteredPagedResult pagedDto);
        Task<IResponse<bool>> DeleteApplicationSliderAsync(DeleteTrackedEntityInputDto inputDto);
        #endregion

        #region DeviceType
        Task<IResponse<List<Ecommerce.DTO.Customers.DevicesType.GetDevicesTypeOutputDto>>> GetAllDevicesTypeAsync();
        #endregion

        Task<DownloadPriceDto> GetMimumPriceWithCurrencyId(Application application, int currencyId);
        //Task<DownloadPriceDto> GetMimumPriceWithCurrencyId(Application application);
    }
}