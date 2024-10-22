using System;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class VerificationLinkDto
    {
        public int EmailId { get; set; }

        public string Code { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}
