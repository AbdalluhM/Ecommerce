using Ecommerce.BLL.Responses;
using Ecommerce.DTO.ContactUs;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ContactUs
{
    public interface IContactUsBLL
    {
        Task<IResponse<bool>> SendRequestAsync(NewContactRequestDto request);
    }
}
