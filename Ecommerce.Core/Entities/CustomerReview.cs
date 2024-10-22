using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerReview:EntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime RateDate { get; set; }
        public int ApplicationId { get; set; }
        public decimal Rate { get; set; }
        public string Review { get; set; }
        public int StatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual Application Application { get; set; }
        public virtual Employee ApprovedByNavigation { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual CustomerReviewStatu Status { get; set; }
    }
}
