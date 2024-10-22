using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class WishListApplication:EntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Application Application { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
