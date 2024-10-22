namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class OAuthResponseDto
    {
        public OAuthDataDto Google { get; set; }
        public OAuthDataDto Facebook { get; set; }
        public OAuthDataDto Apple { get; set; }
    }
}
