using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Features
{
    public interface IFeatureBLL
    {
        public Task<IResponse<GetFeatureOutputDto>> CreateAsync( CreateFeatureInputDto inputDto/*, FileDto fileDto*/ );
        public Task<IResponse<GetFeatureOutputDto>> UpdateAsync( UpdateFeatureInputDto inputDto/*, FileDto fileDto*/ );

        public Task<IResponse<bool>> DeleteAsync( DeleteTrackedEntityInputDto inputDto );
        public Task<IResponse<List<GetFeatureOutputDto>>> GetAllAsync( );
        public Task<IResponse<GetFeatureOutputDto>> GetByIdAsync( GetFeatureInputDto inputDto );
        public Task<IResponse<PagedResultDto<GetFeatureOutputDto>>> GetPagedListAsync( FilteredResultRequestDto pagedDto );
    }
}
