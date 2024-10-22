namespace Ecommerce.DTO.Settings.Files
{
    public class Admin
    {
        public string ContainerName { get; set; }
        public Addon Addon { get; set; }

        public AdminFileBase Feature { get; set; }

        public Modules Module { get; set; }

        public Applications Application { get; set; }

        public AdminFileBase AdminProfile { get; set; }

        public AdminFileBase License { get; set; }
    }
}
