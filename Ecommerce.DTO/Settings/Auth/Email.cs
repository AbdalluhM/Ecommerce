using System;

namespace Ecommerce.DTO.Settings.Auth
{
    public class Email
    {
        public string ClientBaseUrl { get; set; }

        public TimeSpan CodeExpiryTime { get; set; }

        public TimeSpan BlockSendingTime { get; set; }

        public int SendCountLimit { get; set; }

        public VerifyEmail VerifyEmail { get; set; } = new();

        public ForgetPassword ForgetPassword { get; set; } = new();

        public VerifyAlternativeEmail VerifyAlternativeEmail { get; set; } = new();
    }
}
