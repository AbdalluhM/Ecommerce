using System.Collections.Generic;
using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using System.Threading.Tasks;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.Core.Entities;

namespace Ecommerce.BLL.Chats
{
    public interface IChatBLL
    {
        Task<IResponse<PagedResultDto<MessageResultDto>>> GetPagedListAsync(FilterChatDto pagedDto);
        Task<IResponse<bool>> SendMessage(MessageDto inputDto);
        Task<IResponse<bool>> MarkMessageToBeReceived(int id);
        Task<IResponse<bool>> MarkMessageToBeRead(int ticketId,bool isCustomer);
        Task<IResponse<int>> GetCountOfUnRead(int currentId, bool isCustomer);
        Task<IResponse<string>> DownLoadChat( int ticketId);

    }
}