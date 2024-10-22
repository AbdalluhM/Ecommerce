using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RefreshToken:EntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Jti { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
