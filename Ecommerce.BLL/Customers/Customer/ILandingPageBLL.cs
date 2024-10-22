using MyDexef.BLL.Responses;
using MyDexef.DTO.Customers;

namespace MyDexef.BLL.Customers
{
    public interface ILandingPageBLL
    {

        IResponse<RegisterLandingPageOutputDto> RegisterLandingPage(RegisterLandingPageInputDto inputDto);
    }
}
