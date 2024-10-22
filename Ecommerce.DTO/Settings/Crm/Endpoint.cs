namespace Ecommerce.DTO.Settings.Crm
{
    public class Endpoint
    {
        public string CreateLead { get; set; }

        public string GetAccountByLeadId { get; set; }

        public string QualifyLead { get; set; }

        public string GetAccountById { get; set; }

        public string GetCustomerTickets { get; set; }

        public string GetTicketInfo { get; set; }

        public string GetTicketMessages { get; set; }

        public string GetTicketLogs { get; set; }

        public string GetSubjectsLookup { get; set; }

        public string CreateTicket { get; set; }

        public string SendTicketMessage { get; set; }

        public string CancelTicket { get; set; }

        public string CloseTicket { get; set; }

        public string ReopenTicket { get; set; }

        public string AssignAgnetToTicket { get; set; }
    }
}
