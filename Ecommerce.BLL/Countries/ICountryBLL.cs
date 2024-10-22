using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Countries
{
    public interface ICountryBLL
    {
        #region Basic CRUD & Internal Business
        Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> CreateAsync( AssignCurrencyToCountryInputDto inputDto );
        Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> UpdateAsync( UpdateAssignCurrencyToCountryInputDto inputDto );
        Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto);

        Task<IResponse<GetAssignedCurrencyToCountryOutputDto>> GetByIdAsync(GetAssignedCurrencyToCountryInputDto inputDto);
        Task<IResponse<GetAssignedCurrencyToCountryOutputDto>>  GetByCountryIdOrDefaultAsync(int? countryId = null  );
        
        Task<IResponse<List<GetAssignedCurrencyToCountryOutputDto>>> GetAllListAsync();
        Task<IResponse<List<GetAssignedCurrencyToCountryOutputDto>>> GetAllByEmployeeIdAsync(int currentEmployeeId);

        Task<IResponse<PagedResultDto<GetAssignedCurrencyToCountryOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);

        string GetDefaultCurrencyCode( );
        #region Comment
        //IResponse<bool> DeleteAssignedCurrencyToCountry( GetAssignedCurrencyToCountryInputDto inputDto );
        //public IResponse<GetAssignedCurrencyToCountryOutputDto> AssignCurrencyToCountry( AssignCurrencyToCountryInputDto inputDto );
        //public IResponse<GetAssignedCurrencyToCountryOutputDto> UpdateAssignedCurrencyToCountry( UpdateAssignCurrencyToCountryInputDto inputDto );
        //IResponse<GetAssignedCurrencyToCountryOutputDto> GetAssignedCurrencyToCountryById( GetAssignedCurrencyToCountryInputDto inputDto );
        //IResponse<PagedResultDto<GetAssignedCurrencyToCountryOutputDto>> GetAssignedCurrencyToCountryPagedList( FilteredResultRequestDto pagedDto );

        #endregion
        #endregion
    }
}
