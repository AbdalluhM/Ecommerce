namespace Ecommerce.DTO.Settings.Auth
{
    public class RefreshTokenSetting
    {
        public int TokenLength { get; set; }

        public int RefreshTokenExpiryInMonths { get; set; }
    }
}
