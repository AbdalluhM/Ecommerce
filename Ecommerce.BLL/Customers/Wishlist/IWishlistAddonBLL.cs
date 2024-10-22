using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Wishlist
{
    public interface IWishlistAddonBLL
    {
        IResponse<GetWihlistAddOnOutputDto> Create(CreateWishlistAddOnInputDto inputDto);
        IResponse<bool> Delete(DeleteWishlistAddOnInputDto inputDto);
        Task<IResponse<List<GetWihlistAddOnOutputDto>>> GetAllAsync(WishlistAddOnSearchInputDto inputDto);
        Task<IResponse<PagedResultDto<GetWihlistAddOnOutputDto>>> GetPagedListAsync(CustomerFilteredResultRequestDto pagedDto);
    }
}
