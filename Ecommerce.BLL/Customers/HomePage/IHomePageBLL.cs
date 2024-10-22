using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.HomePage.HomePageDto;

namespace Ecommerce.BLL.Customers.HomePage
{
    public interface IHomePageBLL
    {
        Task<IResponse<PagedResultDto<GetCustomerAppTagsOrHighlightedOutputDto>>> GetCustomerAppTagsoRHighlighted(FilterCustomerAppTagsInputDto pagedDto);
    }
}
