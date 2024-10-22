using System.Threading.Tasks;
using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Paging;

namespace Ecommerce.BLL.Customers.Reviews.Customers
{
    public interface ICustomerReviewBLL
    {
        Task<IResponse<int>> MakeNewReviewAsync(NewReviewDto newReviewDto);

        Task<IResponse<int>> UpdateReviewAsync(EditReviewDto editReviewDto);

        Task<RateDto> GetRateAsync(int applicationId, bool includeStarPercentage = false);

        Task<IResponse<CustomerReviewDto>> GetCustomerReviewAsync(int applicationId, int customerId);

        IResponse<PagedResultDto<ReviewDto>> GetApplicationReviewsPagedList(FilteredResultRequestDto filterDto, int appId, int customerId);
    }
}