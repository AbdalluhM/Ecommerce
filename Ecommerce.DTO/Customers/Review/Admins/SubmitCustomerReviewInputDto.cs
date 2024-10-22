using System;

namespace Ecommerce.DTO.Customers.Review.Admins
{
    public class SubmitCustomerReviewInputDto 
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
