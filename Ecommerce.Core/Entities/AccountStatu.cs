using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AccountStatu:EntityBase
    {
        public AccountStatu()
        {
            Accounts = new HashSet<Account>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
