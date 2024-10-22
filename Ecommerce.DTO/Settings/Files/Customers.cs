namespace Ecommerce.DTO.Settings.Files
{
    public class Customers
    {
        public ProfileData ProfileData { get; set; }

        public AdminFileBase License { get; set; }
        public AdminFileBase Ticket { get; set; }

    }

    public class ProfileData
    {
        public string Base { get; set; }
        public string Path { get; set; }
    }
}
