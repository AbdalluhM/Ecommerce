using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Notifications;
using Ecommerce.Repositroy.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Employees;
using Ecommerce.Core.Enums.Notifications;
using System.Data.SqlClient;

namespace Ecommerce.BLL.Notifications
{
    public class NotificationDataBLL : BaseBLL, INotificationDataBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Notification> _notificationRepository;
        IRepository<EmployeeCountry> _employeeCountryRepository;
        IRepository<NotificationAction> _notificationActionRepository;
        IEmployeeBLL _employeeBLL;
        private readonly IRealtimeNotificationBLL _realtimeNotificationBLL;
        #endregion

        #region Constructor
        public NotificationDataBLL(IMapper mapper,
                                   IUnitOfWork unitOfWork,
                                   IRepository<Notification> notificationRepository,
                                   IRepository<EmployeeCountry> employeeCountryRepository,
                                   IRepository<NotificationAction> notificationActionRepository,
                                   IEmployeeBLL employeeBLL,
                                   IRealtimeNotificationBLL realtimeNotificationBLL)
            : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _employeeCountryRepository = employeeCountryRepository;
            _notificationActionRepository = notificationActionRepository;
            _employeeBLL = employeeBLL;
            _realtimeNotificationBLL = realtimeNotificationBLL;
        }


        #endregion

        #region Bascic operations
        /// <summary>
        /// Get All Notifications by filter
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetNotificationOutputDto>>> GetPagedListAsync(NotificationFilteredResultRequestDto inputDto)
        {
            //var lisOfemployeeCountriesIds = _employeeCountryRepository.Where(x=>x.EmployeeId== inputDto.AdminId).Select(x=>x.CountryCurrency.CountryId).ToList();

            var lisOfemployeeCountriesIds = await _employeeBLL.GetEmployeeCountries(inputDto.AdminId.GetValueOrDefault());
            var output = new Response<PagedResultDto<GetNotificationOutputDto>>();
            var notificationResult = GetPagedList<GetNotificationOutputDto, Notification, DateTime>(
               pagedDto: inputDto,
               repository: _notificationRepository,
               orderExpression: x => x.CreateDate,
               sortDirection: inputDto.SortingDirection,
               /*sortDirection: nameof(SortingDirection.DESC)*/
               searchExpression: x => ((inputDto.IsAdmin && x.NotificationAction.IsAdminSide && lisOfemployeeCountriesIds.Contains(x.Customer.CountryId))
               || (!inputDto.IsAdmin && !x.NotificationAction.IsAdminSide
               && x.CustomerId == inputDto.CustomerId))
                 && (string.IsNullOrEmpty(inputDto.SearchTerm)
                  || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                  && (x.Customer.Name.Contains(inputDto.SearchTerm)
                  || (x.InvoiceId != null && x.InvoiceId.ToString() == inputDto.SearchTerm)
                  || (x.LicenceId != null && x.Licence.Serial == inputDto.SearchTerm)
                  || (x.Customer.Contract.Id.ToString() == inputDto.SearchTerm)
                  || (x.TicketRefrences != null && x.TicketRefrences.Contains(inputDto.SearchTerm))
                  || x.NotificationAction.NotificationActionType.Name.Contains(inputDto.SearchTerm)
                  )))
                  && (!x.IsHide),
               disableFilter: true
               );

            return output.CreateResponse(notificationResult);
        }

        public async Task<IResponse<int>> GetCountOfUnRead(NotificationFilterParameterDto notificationFilterParameterDto)
        {
            var lisOfemployeeCountriesIds = await _employeeBLL.GetEmployeeCountries(notificationFilterParameterDto.AdminId.GetValueOrDefault());
            var output = new Response<int>();
            var countOfUnread = _notificationRepository.Where(x => (notificationFilterParameterDto.IsAdmin
            && !x.IsRead
            && x.NotificationAction.IsAdminSide && lisOfemployeeCountriesIds.Contains(x.Customer.CountryId))
            || (!notificationFilterParameterDto.IsAdmin
            && !x.IsRead
            && !x.NotificationAction.IsAdminSide
            && x.CustomerId == notificationFilterParameterDto.CustomerId)).Count();
            return output.CreateResponse(countOfUnread);
        }


        public IResponse<bool> MarkAllNotificationsToBeRead(NotificationFilterParameterDto notificationFilterParameterDto)
        {
            var output = new Response<bool>();
            var notificationsList = _notificationRepository.Where(x => (notificationFilterParameterDto.IsAdmin
            && x.NotificationAction.IsAdminSide)
            || (!notificationFilterParameterDto.IsAdmin
            && !x.NotificationAction.IsAdminSide
            && x.CustomerId == notificationFilterParameterDto.CustomerId)).ToList();
            if (notificationsList.Count > 0)
            {
                notificationsList.ForEach(itemNotification =>
                    {
                        if (notificationFilterParameterDto.IsAdmin)
                            itemNotification.ReadByEmployeeId = notificationFilterParameterDto.AdminId;
                        itemNotification.ReadAt = DateTime.UtcNow;
                        itemNotification.IsRead = true;
                        _notificationRepository.Update(itemNotification);
                    });
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            return output.CreateResponse(false);
        }


        public IResponse<bool> MarkOneNotificationToBeRead(NotificationFilterParameterDto notificationFilterParameterDto)
        {
            var output = new Response<bool>();
            var notificationToBeRead = _notificationRepository.Where(x => (notificationFilterParameterDto.IsAdmin
           && x.NotificationAction.IsAdminSide
            && x.Id == notificationFilterParameterDto.NotificationId)
            || (!notificationFilterParameterDto.IsAdmin
           && !x.NotificationAction.IsAdminSide
            && x.CustomerId == notificationFilterParameterDto.CustomerId
            && x.Id == notificationFilterParameterDto.NotificationId)).FirstOrDefault();
            if (notificationToBeRead != null)
            {
                if (notificationFilterParameterDto.IsAdmin)
                    notificationToBeRead.ReadByEmployeeId = notificationFilterParameterDto.AdminId;
                notificationToBeRead.ReadAt = DateTime.UtcNow;
                notificationToBeRead.IsRead = true;
                _notificationRepository.Update(notificationToBeRead);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            return output.CreateResponse(false);
        }

        public IResponse<bool> MarkOneNotificationToBeHide(NotificationFilterParameterDto notificationFilterParameterDto)
        {
            var output = new Response<bool>();
            var notificationToBeHide = _notificationRepository.Where(x => (notificationFilterParameterDto.IsAdmin
            && x.NotificationAction.IsAdminSide
            && x.Id == notificationFilterParameterDto.NotificationId)
            || (!notificationFilterParameterDto.IsAdmin
           && !x.NotificationAction.IsAdminSide
            && x.CustomerId == notificationFilterParameterDto.CustomerId
            && x.Id == notificationFilterParameterDto.NotificationId)).FirstOrDefault();
            if (notificationToBeHide != null)
            {
                if (notificationFilterParameterDto.IsAdmin)
                {
                    notificationToBeHide.HiddenByEmployeeId = notificationFilterParameterDto.AdminId;
                    notificationToBeHide.ReadByEmployeeId = notificationFilterParameterDto.AdminId;
                }
                notificationToBeHide.HiddenAt = DateTime.UtcNow;
                notificationToBeHide.ReadAt = DateTime.UtcNow;
                notificationToBeHide.IsHide = true;
                notificationToBeHide.IsRead = true;
                _notificationRepository.Update(notificationToBeHide);
                _unitOfWork.Commit();

                return output.CreateResponse(true);
            }
            return output.CreateResponse(false);
        }

        public async Task<IResponse<int>> CreateAsync(GetNotificationForCreateDto inputDto)
        {
            var output = new Response<int>();

            try
            {
                var notificationActionTypeItem = await _notificationActionRepository.GetAsync(x => x.NotificationActionTypeId == inputDto.NotificationActionTypeId
                                                                                                && x.IsCreatedBySystem == inputDto.IsCreatedBySystem
                                                                                                && x.IsAdminSide == inputDto.IsAdminSide
                                                                                                && (inputDto.NotificationActionSubTypeId == null
                                                                                                || (inputDto.NotificationActionSubTypeId != null
                                                                                                && x.NotificationActionSubTypeId == inputDto.NotificationActionSubTypeId)));

                var notificationItem = new Notification();

                if (notificationActionTypeItem is not null)
                    notificationItem.NotificationActionId = notificationActionTypeItem.Id;

                notificationItem.CustomerId = inputDto.CustomerId;
                notificationItem.CreatedByEmployeeId = inputDto.AdminId;
                notificationItem.IsRead = false;
                notificationItem.IsHide = false;
                notificationItem.CreateDate = DateTime.UtcNow;
                notificationItem.InvoiceId = inputDto.InvoiceId;
                notificationItem.LicenceId = inputDto.LicenceId;
                notificationItem.TicketRefrences = inputDto.TicketRefrences;
                notificationItem.TicketId = inputDto.TicketId;
                notificationItem.RefundRequestId = inputDto.RefundRequestId;

                var notificationDto = new GetNotificationOutputDto();

                if (inputDto.SaveInDB)
                {
                    var entity = await _notificationRepository.AddAsync(notificationItem);

                    await _unitOfWork.CommitAsync();

                    await _notificationRepository.RefreshEntityReferencesAsync(entity);

                    notificationDto = _mapper.Map<GetNotificationOutputDto>(entity);
                }

                // message sent from Customer server to Admin server..
                if (inputDto.ProjectTypeEnum == ProjectTypeEnum.Customer)
                {
                    var empIds = await _employeeBLL.GetEmployeesByCustomerIdAsync(inputDto.CustomerId.Value);

                    await _realtimeNotificationBLL.SendMessageToUsersAsync(inputDto.ProjectTypeEnum, new UserNotificationDto
                    {
                        RecieverType = NotificationRecieverTypeEnum.Employee,
                        Recievers = empIds,
                        Channel = inputDto.HubChannel,
                        Notification = inputDto.SaveInDB ? notificationDto : null
                    });

                }
                else // message sent from Admin server to Customer server.
                {
                    var customerIds = new List<int> { notificationItem.CustomerId.Value };

                    await _realtimeNotificationBLL.SendMessageToUsersAsync(inputDto.ProjectTypeEnum, new UserNotificationDto
                    {
                        RecieverType = NotificationRecieverTypeEnum.Customer,
                        Recievers = customerIds,
                        Channel = inputDto.HubChannel,
                        Notification = inputDto.SaveInDB ? notificationDto : null
                    });
                }

                return output.CreateResponse(notificationItem.Id);
            }
            catch (Exception ex)
            {
                //var connection = new SqlConnection("Data source=tcp:Ecommerce-db-server.database.windows.net,1433;initial catalog=EcommerceDB;Integrated security=False; User Id=dexefAdmin;Password=Admin#123;MultipleActiveResultSets=True;Connection Timeout=6000;");
                //var command = new SqlCommand($"INSERT INTO dbo.ExceptionLogs(Exception)VALUES('{nameof(CreateAsync)}---{ex}')", connection);
                //connection.Open();
                //command.ExecuteNonQuery();
                //connection.Close();

                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<bool>> DeleteRelatedNotificationAsync(DeleteNotificationDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                var notifications = await _notificationRepository
                    .Where(x => (inputDto.LicenceId != null && x.LicenceId == inputDto.LicenceId)
                    || (inputDto.InvoiceId != null && x.InvoiceId == inputDto.InvoiceId)
                    || (inputDto.CustomerSubscriptionId != null &&
                    (x.Invoice.CustomerSubscriptionId == inputDto.CustomerSubscriptionId
                    || x.Licence.CustomerSubscriptionId == inputDto.CustomerSubscriptionId))
                    || (inputDto.RefundRequestId != null && x.RefundRequestId == inputDto.RefundRequestId)
                    ).ToListAsync();
                if (notifications != null)
                {
                    _notificationRepository.DeleteRange(notifications);
                    _unitOfWork.Commit();
                    return output.CreateResponse(true);
                }
                return output.CreateResponse(false);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }


        #region Comment

        // no comments

        #endregion
        #endregion
    }
}
