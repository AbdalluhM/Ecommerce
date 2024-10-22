using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerSmsverification:EntityBase
    {
        public int Id { get; set; }
        public int CustomerMobileId { get; set; }
        public string VerificationCode { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int SmstypeId { get; set; }
        public bool IsVerifiedFromFrogetPassword { get; set; }

        public virtual CustomerMobile CustomerMobile { get; set; }
    }
}
