using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.DTO.Modules;
using Ecommerce.DTO;
using Ecommerce.DTO.Modules.ModuleTags;
using Ecommerce.DTO.Tags;
using Ecommerce.DTO.Modules.ModuleSlider;
using Ecommerce.DTO.Modules.ModuleBase.Outputs;

namespace Ecommerce.BLL.Modules
{
    public interface IModuleBLL
    {
        #region Module
        Task<IResponse<GetModuleOutputDto>> CreateAsync(CreateModuleInputDto inputDto);
        Task<IResponse<GetModuleOutputDto>> UpdateAsync(UpdateModuleInputDto inputDto);
        Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto);

        Task<IResponse<GetModuleOutputDto>> GetByIdAsync(GetModuleInputDto inputDto);

        Task<IResponse<List<GetModuleOutputDto>>> GetAllAsync();

        Task<IResponse<PagedResultDto<GetModuleOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);

        Task<IResponse<AppModulePopupDto>> GetAppModulePopupAsync(int moduleId);

         Task<IResponse<ModuleDetailsDto>> GetModuleDetailsAsync(int moduleId);
        #endregion

        #region Module Tag

        Task<IResponse<GetModuleTagOutputDto>> CreateModuleTagAsync(CreateModuleTagInputDto inputDto);
        Task<IResponse<bool>> AssignFeaturedToModuleTagAsync(int Id);
        Task<IResponse<bool>> DeleteModuleTagAsync(DeleteEntityInputDto inputDto);
        Task<IResponse<GetModuleTagOutputDto>> GetModuleTagByIdAsync(GetModuleTagInputDto getModulePriceInputDto);

        Task<IResponse<List<GetModuleTagOutputDto>>> GetAllModuleTagListByModuleIdAsync(int ModuleId);
        Task<IResponse<List<GetTagOutputDto>>> GetAllActiveTagsNotAssignToModuleIdAsync(int ModuleId);
        Task<IResponse<GetModuleTagOutputDto>> UpdateModuleTagAsync(UpdateModuleTagInputDto inputDto);
        Task<IResponse<PagedResultDto<GetModuleTagOutputDto>>> GetModuleTagPagedListAsync(ModuleFilteredPagedResult inputDto);

        #endregion

        #region Module Slider
        Task<IResponse<GetModuleSliderOutputDto>> CreateModuleSliderAsync(CreateModuleSliderInputDto inputDto);
        Task<IResponse<PagedResultDto<GetModuleSliderOutputDto>>> GetAllModuleSlidersPagedListAsync(ModuleFilteredPagedResult pagedDto);
        Task<IResponse<bool>> DeleteModuleSliderAsync(DeleteTrackedEntityInputDto inputDto);
        #endregion
    }
}