using System;

namespace Ecommerce.DTO.Customers.Review.Admins
{
    public class GetCustomerReviewOutputDto
    {
        public int Id { get; set; }
        public DateTime RateDate { get; set; }
        public decimal Rate { get; set; }
        public string Review { get; set; }
        public DateTime CreateDate { get; set; }
        public string ApplicationName { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
    }
}
