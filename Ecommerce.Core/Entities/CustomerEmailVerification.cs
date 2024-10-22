using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerEmailVerification:EntityBase
    {
        public int Id { get; set; }
        public int CustomerEmailId { get; set; }
        public string VerificationCode { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual CustomerEmail CustomerEmail { get; set; }
    }
}
