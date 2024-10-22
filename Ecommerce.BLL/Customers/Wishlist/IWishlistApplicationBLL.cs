using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Wishlist
{
    public interface IWishlistApplicationBLL
    {
        IResponse<GetWishlistApplicationOutputDto> Create(CreateWishlistApplicationInputDto inputDto);
        IResponse<bool> Delete(DeleteWishlistApplicationInputDto inputDto);
        Task<IResponse<List<GetWishlistApplicationOutputDto>>> GetAllAsync(WishlistApplicationSearchInputDto inputDto);
        Task<IResponse<PagedResultDto<GetWishlistApplicationOutputDto>>> GetPagedListAsync(CustomerFilteredResultRequestDto pagedDto);
    }
}
