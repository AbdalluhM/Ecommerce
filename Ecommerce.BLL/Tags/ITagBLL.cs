using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Tags
{
    public interface ITagBLL
    {
        IResponse<GetTagOutputDto> Create(CreateTagInputDto inputDto);
        IResponse<GetTagOutputDto> Update( UpdateTagInputDto inputDto );
        IResponse<bool> Delete( DeleteTrackedEntityInputDto inputDto);
        Task<IResponse<GetTagOutputDto>> GetByIdAsync( GetTagInputDto inputDto );

        IResponse<List<GetTagOutputDto>> GetAllList();

        IResponse<PagedResultDto<GetTagOutputDto>> GetPagedTagList(FilteredResultRequestDto pagedDto);
    }
}