using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.Repositroy.Base;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace Ecommerce.BLL.Chats
{
    public class ChatHub : Hub, IChatHub
    {
        private IRepository<Ticket> _ticketRepository;
        private IRepository<Employee> _employeeRepository;
        IMapper _mapper;
        private IRepository<ChatMessage> _chatMessageRepository;
        private static readonly string AdminGroupName = "Admins";
        IHubContext<ChatHub> _hubContext;
        public ChatHub(IHubContext<ChatHub> hubContext, IRepository<Employee> employeeRepository, IRepository<Ticket> ticketRepository,
        IMapper mapper, IRepository<ExceptionLog> exceptionLogRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<CustomerEmail> customerEmailRepository, IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _ticketRepository =ticketRepository;
            _employeeRepository =employeeRepository;
            _mapper = mapper;
            _chatMessageRepository =chatMessageRepository;
        }


        public async Task Register(ConnectionChatDto connectionDto)
        {

            if (connectionDto.UserType== ChatRecieverTypeEnum.Customer) {

                var groupName = GetCustomerGroupName(connectionDto.UserId.ToString());
                await _hubContext.Groups.AddToGroupAsync(Context.ConnectionId, groupName);
               //  messages = _chatMessageRepository.Where(e => e.IsCustomer && e.ReceivedTime == null).ToList();
              //   messagesResult = _mapper.Map<List<MessageResultDto>>(messages);
                

            }
            else
            {
                await _hubContext.Groups.AddToGroupAsync(Context.ConnectionId, AdminGroupName);
                
            }

            await SendMessageNotReceived(connectionDto.UserType, connectionDto.UserId);

        }



        //public override async Task OnConnectedAsync()
        // {


        //    //currentUserId = int.Parse(Context.GetHttpContext().User.FindFirstValue(TokenClaimTypeEnum.Id.ToString()));
        //    //currentEmail = Context.GetHttpContext().User.FindFirstValue(ClaimTypes.Email);

        //    var email = await _customerEmailRepository.GetAsync(e => e.Email == currentEmail);

        //    if (email is null)
        //    {
        //        // admin
        //        await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroupName);

        //    }
        //    else
        //    {
        //        // customer
        //        var groupName = GetCustomerGroupName(currentUserId.ToString());
        //        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //    }

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{

        //    await base.OnDisconnectedAsync(exception);
        //}

        //public async Task SendMessage(MessageDto inputDto)
        //{
        //    try
        //    {


        //        var Message = _mapper.Map<ChatMessage>(inputDto);
        //        Message.SenderId =
        //             int.Parse(Context.GetHttpContext().User.FindFirstValue(TokenClaimTypeEnum.Id.ToString()));
        //        var result = await _chatMessageRepository.AddAsync(Message);
        //        await _unitOfWork.CommitAsync();

        //        var messageResult = _mapper.Map<MessageResultDto>(Message);

        //        currentEmail = Context.GetHttpContext().User.FindFirstValue(ClaimTypes.Email);
        //        currentUserId =
        //            int.Parse(Context.GetHttpContext().User.FindFirstValue(TokenClaimTypeEnum.Id.ToString()));

        //        var email =await _customerEmailRepository.GetAsync(e => e.Email == currentEmail);
        //        if (email is null)
        //        {
        //            //admin send to customer

        //           var customerId =(await _ticketRepository.GetAsync(e => e.Id == messageResult.TicketId))?.CustomerId;
        //            var groupName = GetCustomerGroupName(customerId.ToString());

        //            await Clients.Group(groupName).SendAsync("ReceiveMessage", messageResult);
        //        }
        //        else
        //        {
        //            // customer send to admin 
        //            await Clients.Group(AdminGroupName).SendAsync("ReceiveMessage", messageResult);

        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        _exceptionLogRepository.Add(new ExceptionLog()
        //        {
        //            Exception = e.Message,
        //        });
        //    }
        //}

        public async Task SendMessage(MessageResultDto input ,ChatRecieverTypeEnum userType)
        {
            var ticket = _ticketRepository.GetById(input.TicketId);
            var groupName = GetCustomerGroupName(ticket.CustomerId.ToString());
            if (userType == ChatRecieverTypeEnum.Customer )
            {
                // customer send to admin 
                await _hubContext.Clients.Group(AdminGroupName).SendAsync("ReceiveMessage", input, "Receiver");
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", input,"Sender");

            }
            else
            {
                //admin send to customer

                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", input, "Receiver");
                await _hubContext.Clients.Group(AdminGroupName).SendAsync("ReceiveMessage", input, "Sender");
            }
        }

        private string GetCustomerGroupName(string customerId) => $"Customer-{customerId}";

        private async Task SendMessageNotReceived(ChatRecieverTypeEnum userType , int id)
        {
            List<ChatMessage> messages;
            List<MessageResultDto> messagesResult;
            if (userType == ChatRecieverTypeEnum.Customer)
            {
                 messages = _chatMessageRepository.Where(e => !e.IsCustomer && e.ReceivedTime == null &&e.Ticket.CustomerId==id).ToList();
                 messagesResult = _mapper.Map<List<MessageResultDto>>(messages);
                foreach (var message in messagesResult)
                {
                    await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message, "Receiver");
                }
            }
            else
            {
                 messages = _chatMessageRepository.Where(e => e.IsCustomer && e.ReceivedTime == null).ToList();
                 messagesResult = _mapper.Map<List<MessageResultDto>>(messages);
                 foreach (var message in messagesResult)
                 {
                     await _hubContext.Clients.Group(AdminGroupName).SendAsync("ReceiveMessage", message, "Receiver");
                 }
            }

           


        }
    }
}
