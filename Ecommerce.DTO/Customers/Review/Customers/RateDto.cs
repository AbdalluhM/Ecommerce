using System.Collections.Generic;

namespace Ecommerce.DTO.Customers.Review.Customers
{
    public class RateDto
    {
        public decimal AverageRate { get; set; }

        public int Count { get; set; }

        public IEnumerable<ReviewStarDto> Stars { get; set; } = new List<ReviewStarDto>();
    }
}