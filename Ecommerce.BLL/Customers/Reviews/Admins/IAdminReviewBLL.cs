using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Review;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Reviews.Admins
{
    public interface IAdminReviewBLL
    {
        Task<IResponse<GetCustomerReviewOutputDto>> SubmitAsync(SubmitCustomerReviewInputDto inputDto);
        IResponse<bool> Delete(DeleteCustomerReviewInputDto inputDto);
        Task<IResponse<PagedResultDto<GetCustomerReviewOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto);
    }
}
