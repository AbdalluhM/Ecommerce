using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerReviewStatu:EntityBase
    {
        public CustomerReviewStatu()
        {
            CustomerReviews = new HashSet<CustomerReview>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<CustomerReview> CustomerReviews { get; set; }
    }
}
