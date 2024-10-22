using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Account:EntityBase
    {
        public Account()
        {
            Contract1 = new HashSet<Contract1>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StatusId { get; set; }

        public virtual Country Country { get; set; }
        public virtual AccountStatu Status { get; set; }
        public virtual ICollection<Contract1> Contract1 { get; set; }
    }
}
