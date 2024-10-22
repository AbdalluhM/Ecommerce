using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.Review.Customers
{
    public class NewReviewDto : BaseReviewDto
    {

        [JsonIgnore]
        public int CustomerId { get; set; }
        public int ApplicationId { get; set; }
    }
}