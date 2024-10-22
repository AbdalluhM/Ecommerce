using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.DTO;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using Ecommerce.DTO.Taxes;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Taxes
{
    public interface ITaxBLL
    {
        IResponse<GetTaxOutputDto> Create(CreateTaxInputDto inputDto);
        IResponse<GetTaxOutputDto> Update( UpdateTaxInputDto inputDto );
        IResponse<bool> Delete( DeleteTrackedEntityInputDto inputDto);
        Task<IResponse<GetTaxOutputDto>> GetByIdAsync( GetTaxInputDto inputDto );
        GetTaxOutputDto GetDefaultTaxForCustomer(GetCountryDefaultTaxInputDto inputDto);
        GetTaxOutputDto GetDefaultTaxForCustomer(GetCountryDefaultTaxByCustomerIdInputDto inputDto);

        Tax GetDefaultTaxForCountry( int countryId );
        Task<IResponse<PagedResultDto<TaxActivitesDto>>> GetTaxActivitesPagedListAsync( LogFilterPagedResultDto pagedDto );
        IResponse<List<GetTaxOutputDto>> GetAllList();

        IResponse<PagedResultDto<GetTaxOutputDto>> GetPagedTaxList(FilteredResultRequestDto pagedDto);
    }
}