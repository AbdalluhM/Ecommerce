namespace Ecommerce.DTO.Customers.Crm.Tickets
{
    public class TicketAgnetDto
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public MessageAttachmentDto Image { get; set; } = new();
    }
}
