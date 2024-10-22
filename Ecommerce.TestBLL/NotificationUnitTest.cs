using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using MyDexef.Core.Enums.Notifications;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MyDexef.TestBLL
{
    [TestClass]
    public class NotificationUnitTest : UnitTestBase
    {
        private int adminId= 4066;
        private int customerId = 1201;
        private int supportInvoiceId = 6140;
        private INotificationDataBLL myService;
        public NotificationUnitTest()
        {
             myService =serviceProvider.GetRequiredService<INotificationDataBLL>();
        }
        //assume admin and customer has notifications and suppInv related to customer
       

        [TestMethod]
        public async Task  Should1CreateNotification()
        { 
            var output =await myService.CreateAsync(new DTO.Notifications.GetNotificationForCreateDto()
            {
                CustomerId = customerId,
                IsAdminSide = false,
                IsCreatedBySystem = false,
                NotificationActionTypeId = (int)NotificationActionTypeEnum.TechnicalSupport,
                AdminId = adminId,
                InvoiceId = supportInvoiceId
            });

            // Assert
            output.ShouldNotBeNull();
            output.Data.ShouldBeGreaterThanOrEqualTo(0);

            var output2 = await myService.CreateAsync(new DTO.Notifications.GetNotificationForCreateDto()
            {
                CustomerId = customerId,
                IsAdminSide = true,
                IsCreatedBySystem = false,
                NotificationActionTypeId = (int)NotificationActionTypeEnum.TechnicalSupport,
                InvoiceId = supportInvoiceId
            });

            // Assert
            output2.ShouldNotBeNull();
            output2.Data.ShouldBeGreaterThanOrEqualTo(0);
        }

        [TestMethod]
        public void Should2GetCountOfUnRead()
        {
            var res = myService.GetCountOfUnRead(
                new DTO.Notifications.NotificationFilterParameterDto()
                {
                    IsAdmin = true,
                    AdminId = adminId,
                }
                );

            res.ShouldNotBeNull();
            var res2 = myService.GetCountOfUnRead(
                new DTO.Notifications.NotificationFilterParameterDto()
                {
                    IsAdmin = false,
                    AdminId = customerId,
                }
                );
            res2.ShouldNotBeNull();
        }


        [TestMethod]
        public async Task Should3GetAllNotifications()
        { 
            var res =await myService.GetPagedListAsync(new DTO.Notifications.NotificationFilteredResultRequestDto
            (){
                AdminId= adminId,
                IsAdmin=true    
            });
            // Assert

            res.ShouldNotBeNull();
            res.Data.ShouldNotBeNull();
            res.Data.Items.Count.ShouldBeGreaterThan(0);
            var res2 = await myService.GetPagedListAsync(new DTO.Notifications.NotificationFilteredResultRequestDto
            ()
            {
                CustomerId = customerId,
                IsAdmin = false
            });
            // Assert
            res2.ShouldNotBeNull();
            res2.Data.ShouldNotBeNull();
            res2.Data.Items.Count.ShouldBeGreaterThan(0);
        }


        [TestMethod]
        public async Task Should4MarkOneNotificationToBeRead()
        { 
            var resList = await myService.GetPagedListAsync(
                new DTO.Notifications.NotificationFilteredResultRequestDto()
            {
                AdminId = adminId,
                IsAdmin = true
            });
            var notificationId = resList.Data.Items.Where(x => x.IsRead == false).FirstOrDefault().Id;
            var res =  myService.MarkOneNotificationToBeRead(new DTO.Notifications.NotificationFilterParameterDto
            ()
            {
                AdminId = adminId,
                IsAdmin = true,
                NotificationId= notificationId

            });
            // Assert

            res.ShouldNotBeNull();
            res.Data.ShouldBeTrue();

            var resList2 = await myService.GetPagedListAsync(
                new DTO.Notifications.NotificationFilteredResultRequestDto()
                {
                    CustomerId = customerId,
                    IsAdmin = false,
                });
            var notificationId2 = resList2.Data.Items.Where(x => x.IsRead == false).FirstOrDefault().Id;

            var res2 = myService.MarkOneNotificationToBeRead(new DTO.Notifications.NotificationFilterParameterDto
              ()
            {
                CustomerId = customerId,
                IsAdmin = false,
                NotificationId = notificationId2

            });
            // Assert

            res2.ShouldNotBeNull();
            res2.Data.ShouldBeTrue();
        }

        [TestMethod]
        public async Task Should5MarkOneNotificationToBeHide()
        { 
            var resList = await myService.GetPagedListAsync(
                new DTO.Notifications.NotificationFilteredResultRequestDto()
                {
                    AdminId = adminId,
                    IsAdmin = true
                });
            var notificationId = resList.Data.Items.Where(x => x.IsHide == false).FirstOrDefault().Id;
            var res = myService.MarkOneNotificationToBeHide(new DTO.Notifications.NotificationFilterParameterDto
            ()
            {
                AdminId = adminId,
                IsAdmin = true,
                NotificationId = notificationId

            });
            // Assert

            res.ShouldNotBeNull();
            res.Data.ShouldBeTrue();

            var resList2 = await myService.GetPagedListAsync(
                new DTO.Notifications.NotificationFilteredResultRequestDto()
                {
                    CustomerId = customerId,
                    IsAdmin = false,
                });
            var notificationId2 = resList2.Data.Items.Where(x => x.IsHide == false).FirstOrDefault().Id;

            var res2 = myService.MarkOneNotificationToBeHide(new DTO.Notifications.NotificationFilterParameterDto
              ()
            {
                CustomerId = customerId,
                IsAdmin = false,
                NotificationId = notificationId2

            });
            // Assert

            res2.ShouldNotBeNull();
            res2.Data.ShouldBeTrue();
        }


        [TestMethod]
        public  void Should6MarkAllNotificationsToBeRead()
        { 
            var res = myService.MarkAllNotificationsToBeRead(new DTO.Notifications.NotificationFilterParameterDto
            ()
            {
                AdminId = adminId,
                IsAdmin = true,

            });
            // Assert

            res.ShouldNotBeNull();
            res.Data.ShouldBeTrue();

            var res2 = myService.MarkAllNotificationsToBeRead(new DTO.Notifications.NotificationFilterParameterDto
              ()
            {
                CustomerId = customerId,
                IsAdmin = false,
            });
            // Assert

            res2.ShouldNotBeNull();
            res2.Data.ShouldBeTrue();
        }


        [TestMethod]
        public async Task Should7DeleteRelatedNotificationAsync()
        { 
            var res = await myService.DeleteRelatedNotificationAsync(new DTO.Notifications.DeleteNotificationDto
            ()
            {
                InvoiceId= supportInvoiceId

            });
            // Assert

            res.ShouldNotBeNull();
            res.Data.ShouldBeTrue();
        }
    }
}
