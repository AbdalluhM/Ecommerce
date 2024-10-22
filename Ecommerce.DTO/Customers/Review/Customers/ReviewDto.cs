
using System;

namespace Ecommerce.DTO.Customers.Review.Customers
{
    public class ReviewDto
    {
        public string CustomerName { get; set; }

        public DateTime RateDate { get; set; }

        public decimal Rate { get; set; }

        public string Review { get; set; }
    }
}
