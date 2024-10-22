namespace Ecommerce.DTO.Settings.Crm
{
    public class CrmSetting
    {
        public string BaseUrl { get; set; }

        public Endpoint Endpoint { get; set; } = new();
    }
}
