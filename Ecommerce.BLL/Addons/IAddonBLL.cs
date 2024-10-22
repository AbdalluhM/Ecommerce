using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO;
using Ecommerce.DTO.Addons;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonSliders.Inputs;
using Ecommerce.DTO.Addons.AddonSliders.Outputs;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Addons
{
    public interface IAddOnBLL
    {
        #region AddOn
        //IResponse<bool> CreateAddon( FileDto fileDto, NewAddonDto newAddonDto, int currentUserId );
        Task<IResponse<GetAddOnOutputDto>> CreateAsync(CreateAddOnInputDto inputDto);
        Task<IResponse<GetAddOnOutputDto>> UpdateAsync(UpdateAddOnInputDto inputDto);
        Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto);
        Task<IResponse<GetAddOnOutputDto>> GetByIdAsync(GetAddOnInputDto inputDto);
        AddOnForBuyNowOutputDto GetAddOnDataForBuyNow(int addOnId, int priceLevelId, int versionId = 0);
        Task<IResponse<List<GetAddOnOutputDto>>> GetAllAsync();
        Task<IResponse<PagedResultDto<GetAddOnOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);
        Task<IResponse<AddonDetailsDto>> GetAddonDetailsAsync(int addonId, int? currentUserId = null);
        DateTime GetVersionNextRenewDate(VersionSubscription versionSubscription);
        int GetMissingPriceCount(int addonId, IEnumerable<int> countryCurrencyIds);
        #endregion

        #region AddonTag

        IResponse<bool> AddAddonTag(CreateAddonTagInputDto inputDto);
        IResponse<bool> AssignFeaturedToAddonTag(int Id);
        IResponse<bool> DeleteAddonTag(DeleteEntityInputDto inputDto);
        Task<IResponse<GetAddonTagOutputDto>> GetAddOnTagByIdAsync(GetAddOnTagInputDto getAddOnPriceInputDto);

        IResponse<List<GetAddonTagOutputDto>> GetAllAddOnTagListByAddonId(int AddonId);
        IResponse<List<GetTagOutputDto>> GetAllActiveTagsNotAssignToAddonId(int AddonId);
        IResponse<GetAddonTagOutputDto> UpdateAddonTag(UpdateAddonTagDto inputDto);
        Task<IResponse<PagedResultDto<GetAddonTagOutputDto>>> GetAddOnTagPagedListAsync(AddOnFilteredPagedResult inputDto);

        #endregion

        #region AddonLabel       
        IResponse<GetAddonLabelOutputDto> CreateAddonLabel(CreateAddonLabelInputDto inputDto);
        IResponse<GetAddonLabelOutputDto> UpdateAddonLabel(UpdateAddonLabelInputDto inputDto);
        IResponse<bool> DeleteAddonLabel(DeleteTrackedEntityInputDto Id);
        Task<IResponse<GetAddonLabelOutputDto>> GetAddOnLabelByAddOnIdAsync(GetAddOnLabelInputDto inputDto);
        IResponse<List<GetAddonLabelOutputDto>> GetAllAddOnLabelListByAddonId(GetAddOnLabelInputDto AddonId);

        Task<IResponse<PagedResultDto<GetAddonLabelOutputDto>>> GetAddOnLabelPagedListAsync(AddOnFilteredPagedResult inputDto);
        #endregion

        #region AddonPrices

        Task<IResponse<GetAddOnPriceOutputDto>> CreateAddOnPriceAsync(CreateAddOnPriceInputDto inputDto);

        Task<IResponse<GetAddOnPriceOutputDto>> UpdateAddOnPriceAsync(UpdateAddOnPriceInputDto inputDto);

        Task<IResponse<bool>> DeleteAddOnPriceAsync(DeleteTrackedEntityInputDto inputDto);


        Task<IResponse<List<GetAddOnPriceOutputDto>>> GetAllExistingAddOnPricesAsync(GetAddOnPricesInputDto inpuDto);

        Task<IResponse<List<GetAddOnPriceOutputDto>>> GetAllMissingAddOnPricesAsync(GetAddOnPricesInputDto inputDto);

        Task<IResponse<GetAddOnPriceOutputDto>> GetAddOnPriceByIdAsync(GetAddOnPriceInputDto inputDto);

        Task<IResponse<PagedResultDto<GetAddOnPriceOutputDto>>> GetAllExistingAddOnPricePagedListAsync(AddOnFilteredPagedResult pagedDto, int currentEmployeeId);

        Task<IResponse<PagedResultDto<GetAddOnPriceOutputDto>>> GetAllMissingAddOnPricesPagedListAsync(AddOnFilteredPagedResult pagedDto, int currentEmployeeId);

        AddonPriceDetailsDto GetMinimumAddonPrice(int addonId, int? countryId = null);
        VersionPriceAllDetailsDto GetMinimumAddonPrice_(int addonId, int? countryId = null);
        public Task<IResponse<AddOnPriceData>> GetAddonPriceByPriceLevel(int addOnId, int? countryId = null, int? priceLevelId = null);

        #endregion

        #region Addon Sliders.
        Task<IResponse<bool>> AddAddoneSlider(FileDto fileDto, NewAddonSliderDto newAddonSliderDto, int currentUserId);

        IResponse<PagedResultDto<RetrieveAddonSliderDto>> GetAllAddonSlidersPagedList(AddOnFilteredPagedResult pagedDto);

        IResponse<bool> DeleteAddonSlider(DeleteTrackedEntityInputDto inputDto);
        #endregion

        Task<DownloadPriceDto> GetMimumPriceWithCurrency(AddOn addOn, int? currencyId);
        //Task<DownloadPriceDto> GetMimumPriceWithCurrency(AddOn addOn, int? currencyId);
    }
}