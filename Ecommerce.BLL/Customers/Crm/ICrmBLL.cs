using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Crm;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Crm
{
    public interface ICrmBLL
    {
        Task<string> CreateLeadAsync(CrmNewLeadDto newLead);

        Task<string> QualifyLeadAsync(QualifyLeadDto qualifyLeadDto);

        Task<CrmRetrieveBaseDto> GetAccountByLeadIdAsync(Guid leadId);

        Task<CrmRetrieveBaseDto> GetAccountByIdAsync(Guid accountId);


        Task<IResponse<string>> GetSubjectsLookupAsync();
        Task<IResponse<List<GetSubjectsDto>>> GetSubjectsAsync();
        Task<IResponse<string>> GetCustomerTicketsAsync(TicketFilterDto ticketFilter, Guid customerId);
        Task<IResponse<TicketDto>> GetTicketInfoAsync(Guid ticketId);
        Task<IResponse<string>> GetTicketMessagesAsync(Guid ticketId, int pageNumber, int pageSize);
        Task<IResponse<string>> GetTicketLogsAsync(Guid ticketId);

        Task<IResponse<RetrieveNewTicketDto>> CreateTicketAsync(NewTicketNotificationDto newTicket);

        Task<IResponse<RetrieveNewTicketDto>> SendTicketMessageAsync(NewTicketMessageNotificationDto newMessage);
        Task<IResponse<bool>> CancelTicketAsync(TicketBaseDto ticket);
        Task<IResponse<bool>> CloseTicketAsync(CloseTicketDto ticket);
        Task<IResponse<bool>> ReopenTicketAsync(TicketBaseDto ticket);
        Task<IResponse<bool>> AssignAgnetToTicketAsync(AssignAgnetToTicket assignAgnetToTicket);
    }
}
