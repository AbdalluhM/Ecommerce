using Ecommerce.DTO.Customers.Invoices.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.CustomerSubscriptions
{
    public interface ICustomerSubscriptionBLL
    {
        GetSubscriptionStatusDto GetSubscriptionStatus(int customerSubscriptionId);
    }
}
