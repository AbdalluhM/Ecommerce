using Ecommerce.Core.Entities;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Helpers
{
    public interface ITwilioBLL
    {
        Task SendWhatsAppVerificationCodeAsync(CustomerMobile mobile, string code, string lang = SupportedLanguage.EN);
    }
}
