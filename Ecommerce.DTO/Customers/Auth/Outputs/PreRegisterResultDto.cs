using Ecommerce.Core.Enums.Customers;
using System;

namespace Ecommerce.DTO.Customers.Auth.Outputs
{
    public class PreregisterResultDto
    {
        public int Id { get; set; }

        public int MobileId { get; set; }

        public int EmailId { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? BlockTillDate { get; set; }

        public CustomerStatusEnum Status { get; set; }
    }
}
