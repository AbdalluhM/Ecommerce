using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Enums.Notifications
{
    public enum RequestCancelationTypeEnum
    {
        Addon = 1,
        App = 2,
    }

    public enum SuscribtionExpiredTypeEnum
    {
        Today = 3,
        WithinNDays = 4,
    }
    public enum NotificationActionTypeEnum
    {
        TicketReply=1,
        KeyGenerated = 2,
        SuccessPayment = 3,
        ProductSubscribtion = 4,
        TechnicalSupport = 5,
        EndSubscription = 6,
        AcceptCancelation = 7,
        RejectCancelation = 8,
        RefundInvoice = 9,
        SubscribtionRenewal = 10,
        SubscribtionExpired = 11,
        RequestKey = 12,
        CreateTicket =13,
        RequestCancelation = 14
    }

    public enum NotificationActioPageEnum
    {
       TicketDetail=1,
       censesRequests=2,
       PaidInvoices=3,
       idInvoices=4,
       fundedInvoice=5,
       pMange=6,  
       pDetail=7
    }

    
}
