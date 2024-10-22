using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.CustomerSubscriptions
{
    public class CustomerSubscriptionBLL : BaseBLL, ICustomerSubscriptionBLL
    {
        #region Fields
        IMapper _mapper;
        IRepository<CustomerSubscription> _customerSubscriptionRepository;
        IRepository<Invoice> _invoiceRepository;
        #endregion
        public CustomerSubscriptionBLL(IMapper mapper, IRepository<CustomerSubscription> customerSubscriptionRepository, IRepository<Invoice> invoiceRepository) : base(mapper)
        {
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _invoiceRepository = invoiceRepository;
        }
        public GetSubscriptionStatusDto GetSubscriptionStatus(int customerSubscriptionId)
        {
            var output = new GetSubscriptionStatusDto();
           
            var invoice = _invoiceRepository.Where(x => x.CustomerSubscriptionId == customerSubscriptionId && 
                                                                     x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid 
                                                                     )
                                            .OrderByDescending(x => x.CreateDate).FirstOrDefault();
            
            if (invoice != null && invoice.EndDate > DateTime.UtcNow)
            {
                output.CustomerSubscriptionId = customerSubscriptionId;
                output.IsValid = true;
                output.EndDate = invoice.EndDate;
            }
            else
            {
                output.CustomerSubscriptionId = customerSubscriptionId;
                output.IsValid = false;
                output.EndDate = null;
            }

            return output;
        }
    }
}
