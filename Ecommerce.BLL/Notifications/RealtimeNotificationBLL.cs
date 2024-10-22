using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.DTO.Settings.RealtimeNotifications;
using Ecommerce.Repositroy.Base;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public class RealtimeNotificationBLL : IRealtimeNotificationBLL
    {
        private readonly IDexefHub _dexefHub;
        private readonly RealtimeNotificationSetting _realtimeNotificationSetting;
        private readonly IRepository<ExceptionLog> _exceptionLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RealtimeNotificationBLL(IDexefHub dexefHub,
                                       IOptions<RealtimeNotificationSetting> realtimeNotificationSetting,
                                       IRepository<ExceptionLog> exceptionLogRepository,
                                       IUnitOfWork unitOfWork,
                                       IWebHostEnvironment webHostEnvironment)
        {
            _dexefHub = dexefHub;
            _realtimeNotificationSetting = realtimeNotificationSetting.Value;
            _exceptionLogRepository = exceptionLogRepository;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

            // Todo-Sully: check why urls in appsettings not working and remove these links.
            if (_webHostEnvironment.IsDevelopment())
            {
                _realtimeNotificationSetting.AdminServerUrl = "http://192.168.1.7:9001";
            }
            else if (_webHostEnvironment.IsStaging())
            {
                _realtimeNotificationSetting.AdminServerUrl = "http://Ecommerceapi.azurewebsites.net";
            }
            else if (_webHostEnvironment.IsProduction())
            {
                _realtimeNotificationSetting.AdminServerUrl = "https://Ecommerceapilive.azurewebsites.net";
            }
        }

        public async Task SendMessageToUsersAsync(ProjectTypeEnum projectTypeEnum, UserNotificationDto userNotificationDto)
        {
            switch (projectTypeEnum)
            {
                case ProjectTypeEnum.Customer:
                    await SendCustomerNotificationAsync(userNotificationDto);
                    break;
                case ProjectTypeEnum.Admin:
                    await SendAdminNotificationAsync(userNotificationDto);
                    break;
                default:
                    throw new ArgumentException("Invalid project type.");
            }
        }

        private async Task SendCustomerNotificationAsync(UserNotificationDto userNotificationDto)
        {
            try
            {
                var url = $"{_realtimeNotificationSetting.AdminServerUrl}/{_realtimeNotificationSetting.Hub}"; // Path.Combine(_realtimeNotificationSetting.AdminServerUrl, _realtimeNotificationSetting.Hub);

                var connection = new HubConnectionBuilder()
                  .WithUrl(url, c => {
                      c.SkipNegotiation = true;
                      c.Transports = HttpTransportType.WebSockets;
                  })
                  .Build();


                await connection.StartAsync();

                await connection.InvokeAsync("SendMessageToUsers", userNotificationDto);
            }
            catch (Exception ex)
            {
                //var connection = new SqlConnection("Data source=tcp:Ecommerce-db-server.database.windows.net,1433;initial catalog=EcommerceDB;Integrated security=False; User Id=dexefAdmin;Password=Admin#123;MultipleActiveResultSets=True;Connection Timeout=6000;");
                //var command = new SqlCommand($"INSERT INTO dbo.ExceptionLogs(Exception)VALUES('{nameof(SendCustomerNotificationAsync)}---{ex}')", connection);
                //connection.Open();
                //command.ExecuteNonQuery();
                //connection.Close();
                throw;
            }
        }

        private async Task SendAdminNotificationAsync(UserNotificationDto userNotificationDto)
        {
            await _dexefHub.SendMessageToUsers(userNotificationDto);
        }
    }
}
