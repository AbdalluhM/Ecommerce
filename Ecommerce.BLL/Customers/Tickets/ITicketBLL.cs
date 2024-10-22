using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Tickets
{
    public interface ITicketBLL
    {
        Task<IResponse<RetrieveNewTicketDto>> CreateTicketAsync(CreateTicketDto newTicket  , List<SingleFilebaseDto>? files);
        Task<IResponse<List<GetTicketFilterDateDto>>> GetTickets(FilteredResultRequestDto pagedDto  , int customerId);
        Task<IResponse<bool>> CancelTicket(int id);
        Task<IResponse<bool>> CloseTicket(int id);
    }
}
