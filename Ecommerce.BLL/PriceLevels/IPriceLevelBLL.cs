using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Ecommerce.BLL.PriceLevels
{
    public interface IPriceLevelBLL
    {
        IResponse<bool> AddPriceLevel(NewPriceLevelDto newPriceLevel);

        IResponse<bool> UpdatePriceLevel(UpdatePriceLevelDto updatePriceLevelDto);

        IResponse<bool> DeletePriceLevel( DeleteTrackedEntityInputDto inputDto);
        IResponse<List<RetrievePriceLevelDto>> GetAllList( );

        IResponse<PagedResultDto<RetrievePriceLevelDto>> GetAllPriceLevelsPagedList( FilteredResultRequestDto filterDto);
        Task<IResponse<RetrievePriceLevelDto>>  GetByIdAsync( GetPriceLevelInputDto inputDto );
    }
}
