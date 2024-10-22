using Microsoft.AspNetCore.SignalR;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Notifications;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public class DexefHub : Hub, IDexefHub
    {
        private List<ConnectionInfoDto> _connections;
        private readonly IHubContext<DexefHub> _hubContext;
        private readonly IRepository<ExceptionLog> _exceptionLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DexefHub(IHubContext<DexefHub> hubContext, IRepository<ExceptionLog> exceptionLogRepository, IUnitOfWork unitOfWork)
        {
            _connections = new List<ConnectionInfoDto>();
            _hubContext = hubContext;
            _exceptionLogRepository = exceptionLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Register(ConnectionDto connectionDto)
        {
            var info = _connections.FirstOrDefault(c => c.UserId == connectionDto.UserId);

            if(info is null)
            {
                info = new ConnectionInfoDto
                {
                    UserId = connectionDto.UserId,
                    ConnectionId = Context.ConnectionId
                };

                _connections.Add(info);
            }

            var groupName = $"{connectionDto.UserType}-{connectionDto.UserId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(HubChannelEnum.NotificationChannel.ToString(), $"This connection: {Context.ConnectionId} has joined group: {groupName}.");
        }

        public async Task SendMessageToUsers(UserNotificationDto userNotificationDto)
        {
            try
            {
                var channel = userNotificationDto.Channel.ToString();

                var recievers = new List<string>();

                foreach (var reciever in userNotificationDto.Recievers)
                {
                    recievers.Add($"{userNotificationDto.RecieverType}-{reciever}");
                }

                await _hubContext.Clients.Groups(recievers).SendAsync(channel, userNotificationDto.Notification);
            }
            catch (Exception ex) 
            {
                //var connection = new SqlConnection("Data source=tcp:Ecommerce-db-server.database.windows.net,1433;initial catalog=EcommerceDB;Integrated security=False; User Id=dexefAdmin;Password=Admin#123;MultipleActiveResultSets=True;Connection Timeout=6000;");
                //var command = new SqlCommand($"INSERT INTO dbo.ExceptionLogs(Exception)VALUES('{nameof(SendMessageToUsers)}---{ex}')", connection);
                //connection.Open();
                //command.ExecuteNonQuery();
                //connection.Close();
                throw;

            }
        }
    }
}
