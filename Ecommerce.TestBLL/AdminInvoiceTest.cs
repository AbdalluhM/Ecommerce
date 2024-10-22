using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Contracts.Invoices;
using MyDexef.Core.Enums.Invoices;
using MyDexef.DTO.Customers.Invoices.Outputs;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyDexef.DTO.Customers.CustomerDto;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class AdminInvoiceTest : UnitTestBase
    {
        private readonly IAdminInvoiceBLL _adminInvoiceBLL;
        private int _employeeId = 4067;
        private int _customerId = 1201;
        private int _versionId  =  5037;
        private int _priceLevelId = 3063;
        private int _discriminator =  3 ;
        private int _versionReleaseId = 7071;
        private string _invoiceTitle = "SsupportInvoice";
        private decimal _price = 1500;
        private int _customerSubscriptionId = 6298;
        private int _versionSubscriptionId = 5917;
        private string _notes = "dasdsfdgdfgh";

    public AdminInvoiceTest()
        {
            _adminInvoiceBLL = serviceProvider.GetRequiredService<IAdminInvoiceBLL>();
        }
        [TestMethod]
        [DataRow((int)InvoiceStatusEnum.Paid)]
        [DataRow((int)InvoiceStatusEnum.Draft)]
        [DataRow((int)InvoiceStatusEnum.Unpaid)]
        [DataRow((int)InvoiceStatusEnum.Cancelled)]
        public async Task GetInvoices_TakeInvoiceStatusId_ReturnInvoicesByStatusId(int value)
        {
            //Arrange
            var filter = new FilterInvoiceByCountry
            {
                EmployeeId = _employeeId,
                InvoiceStatusId = value
            };
            //Action
            var invoices = await _adminInvoiceBLL.GetAllInvoicesPagedlist(filter);
            //Assert
            invoices.IsSuccess.ShouldBeTrue();
            invoices.Errors.Count.ShouldBeLessThanOrEqualTo(0);
           
        }
        [TestMethod]
        public async Task CreateSupportInvoice_TakeCustomerAndApplicatonOrAddOnDatA_ReturnDraftInvoice()
        {
            //Arrange
            var paymentDetails = new PaymentDetailsInputDto
            {
                VersionId = _versionId,
                VersionSubscriptionId = _versionSubscriptionId,
                IsAdminInvoice = true,
                IsInvoiceSupportAdmin = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(100),
                CustomerId =_customerId,
                Notes = _notes , 
                PriceLevelId = _priceLevelId,
                VersionReleaseId = _versionReleaseId,
                CustomerSubscriptionId = _customerSubscriptionId,
                Discriminator = _discriminator,
                InvoiceTitle = _invoiceTitle,
                Price=_price
            };
            //Action
            var invoice =await _adminInvoiceBLL.CreateSupportInvoice(paymentDetails ,_customerId);
            //Assert
            invoice.Data.ShouldNotBeNull();
        }
    }
}
