using System;

namespace Ecommerce.DTO.Settings.Auth
{
    public class AuthSetting
    {
        public Jwt Jwt { get; set; } = new();

        public VerificationCode VerificationCode { get; set; } = new();

        public Sms Sms { get; set; } = new();

        public Email Email { get; set; } = new();

        public Crm Crm { get; set; } = new();
    }
}
