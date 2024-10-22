using Ecommerce.Core.Enums.Chat;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Chats
{
    public interface IChatHub
    {
        //Task SendMessage(MessageDto inputDto);
        //Task OnConnectedAsync();
        Task SendMessage(MessageResultDto input, ChatRecieverTypeEnum userType);
        Task Register(ConnectionChatDto connectionDto);

    }
}
