using AutoMapper;
using Dexef.Payment.FawryAPI;
using HandlebarsDotNet;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Contracts.Licenses;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Payments;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Enums.WorkSpaces;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Notifications;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerDto;
using IO = System.IO;

namespace Ecommerce.BLL.Customers.Invoices.Job
{
    public class InvoiceJobBLL : IInvoiceJobBLL
    {
        private readonly IMapper _mapper;
        private readonly IInvoiceBLL _invoiceBLL;
        private readonly ILicensesBLL _licensesBLL;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Workspace> _workspaceRepository;
        private readonly IRepository<CustomerSubscription> _customerSubscriptionRepository;
        private readonly IPaymentSetupBLL _paymentSetupBLL;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IInvoiceHelperBLL _invoiceHelper;
        private readonly IPaymentBLL _paymentBLL;

        public InvoiceJobBLL(IMapper mapper,
                             IInvoiceBLL invoiceBLL,
                             ILicensesBLL licensesBLL,
                             IUnitOfWork unitOfWork,
                             IRepository<Invoice> invoiceRepository,
                             IPaymentSetupBLL paymentSetupBLL,
                             INotificationDataBLL notificationDataBLL,
                             IInvoiceHelperBLL invoiceHelper,
                             IPaymentBLL paymentBLL,
                             IRepository<CustomerSubscription> customerSubscriptionRepository,
                             IRepository<Workspace> workspaceRepository)
        {
            _mapper = mapper;
            _invoiceBLL = invoiceBLL;
            _licensesBLL = licensesBLL;
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
            _paymentSetupBLL = paymentSetupBLL;
            _notificationDataBLL = notificationDataBLL;
            _invoiceHelper = invoiceHelper;
            _paymentBLL = paymentBLL;
            _customerSubscriptionRepository = customerSubscriptionRepository;
            _workspaceRepository = workspaceRepository;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)] // 30 minutes timeout
        public async Task RenewInvoicesAutoAsync()
        {
            try
            {
                var subInvoices = _invoiceRepository.GetAll()
                                   .ToList()
                                   .GroupBy(o => o.CustomerSubscriptionId,
                                            o => o,
                                            (sub, invoices) => new CustomerSubInvoiceTestDto
                                            {
                                                SubId = sub,
                                                Invoices = invoices
                                                .OrderByDescending(i => i.Id)
                                                .Take(1)
                                                .ToList()
                                            });

                // 2. loop over invoices:
                var newInvoices = new List<Invoice>();

                if (subInvoices.Any())
                {
                    foreach (var expiredInvoice in subInvoices)
                    {
                        var invoice = expiredInvoice.Invoices.FirstOrDefault();

                        if (!(invoice.EndDate <= DateTime.UtcNow && (
                               invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid
                               || invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)))
                            continue;

                        if (expiredInvoice.SubId == 1864)
                        {

                        }

                        var versionSubId = default(int);
                        var addonSubId = default(int);

                        if (invoice.CustomerSubscription.IsAddOn)
                        {
                            var addonSub = invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault();

                            if (addonSub is null)
                                continue;

                            addonSubId = addonSub.Id;
                            versionSubId = addonSub.VersionSubscriptionId;
                        }
                        else
                        {
                            var versionSub = invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault();

                            if (versionSub is null)
                                continue;

                            versionSubId = versionSub.Id;
                        }

                        var paymentMethod = await _paymentBLL.GetCustomerDefaultPaymentMethodAsnyc(invoice.CustomerSubscription.CustomerId);

                        try
                        {

                            var newInvoice = await _invoiceBLL.CreateInvoiceAsync(new NewInvoiceDto
                            {
                                VersionSubscriptionId = versionSubId,
                                AddonSubscriptionId = addonSubId,
                                InvoiceTypeId = invoice.InvoiceTypeId,
                                PaymentMethodId = paymentMethod.Id,
                                CountryId = invoice.CustomerSubscription.Customer.CountryId,
                                OldInvoice = invoice,
                                StartDate = /*DateTime.UtcNow*/invoice.EndDate.AddDays(1),
                                EndDate = invoice.EndDate.AddDays(invoice.CustomerSubscription.RenewEvery + 1),
                                IsCommit = false
                            });

                            // set license status to be expired.
                            await _licensesBLL.MakeLicensesExpiredAsync(invoice.CustomerSubscription.Licenses.ToList(), isCommit: false);

                            await _unitOfWork.CommitAsync();

                            // if subscription is auto bill, pay invoice.
                            if (invoice.CustomerSubscription.AutoBill)
                            {
                                var paymentResult = await _paymentBLL.PayInvoiceAsync(new PaymentDto
                                {
                                    Invoice = newInvoice,
                                    CustomerInfo = _mapper.Map<GetCustomerOutputDto>(invoice.CustomerSubscription.Customer),
                                    CurrencyCode = invoice.CustomerSubscription.Customer.Country.CountryCurrency.Currency.Code
                                });

                                if (!paymentResult.IsSuccess)
                                    newInvoices.Add(newInvoice);
                            }
                            else
                                newInvoices.Add(newInvoice);

                            //push new notification db
                            var _notificationItem = new GetNotificationForCreateDto();
                            _notificationItem.CustomerId = newInvoice.CustomerSubscription.CustomerId;
                            _notificationItem.InvoiceId = invoice.Id;
                            _notificationItem.IsAdminSide = false;
                            _notificationItem.IsCreatedBySystem = true;
                            switch (newInvoice.InvoiceTypeId)
                            {
                                case (int)InvoiceTypes.Renewal:
                                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.SubscribtionRenewal;
                                    break;
                                case (int)InvoiceTypes.Support:
                                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.TechnicalSupport;
                                    break;
                                default:
                                    break;
                            }
                            await _notificationDataBLL.CreateAsync(_notificationItem);

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    //TODO:UnComment this after this
                    //TODO:return send mail condition after fixing CRM Old Accounts
                    //// 3. send email.
                    //_invoiceBLL.SendInvoicesEmails(newInvoices, hasAttachment: true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [DisableConcurrentExecution(timeoutInSeconds: 1800)] // 30 minutes timeout
        public async Task RenewOldInvoicesAutoAsync()
        {
            try
            {
                //var newsubInvoices = _customerSubscriptionRepository
                //    .Where(s => s.Invoices.OrderByDescending(o=>o.Id)
                //    .Any( v => v.EndDate < DateTime.Now && ( v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid || v.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid)) )
                // .Select(x => new CustomerSubInvoiceTestDto
                //{
                //    SubId = x.Id,
                //    Invoices = x.Invoices.OrderByDescending(o => o.Id).Take(1).ToList()
                //}).AsEnumerable();


                var subInvoices = _invoiceRepository.GetAll().Include(e => e.CustomerSubscription)
                                   .ToList()
                                   .GroupBy(o => o.CustomerSubscriptionId,
                                            o => o,
                                            (sub, invoices) => new CustomerSubInvoiceTestDto
                                            {
                                                SubId = sub,
                                                Invoices = invoices
                                                .OrderByDescending(i => i.Id)
                                                .Take(1)
                                                .ToList()
                                            });
                var ss = subInvoices.Where(x => x.Invoices.Any(i => i.EndDate < DateTime.UtcNow && (
                               i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid
                               || i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid))).ToList();
                // 2. loop over invoices:
                var newInvoices = new List<Invoice>();

                if (subInvoices.Any())
                {
                    foreach (var expiredInvoice in subInvoices)
                    {
                        //if (expiredInvoice.Invoices.Any(x => x.Id != 75 && x.CustomerSubscriptionId != 75))
                        //    continue;

                        var invoice = expiredInvoice.Invoices.FirstOrDefault();

                        if (!(invoice.EndDate <= DateTime.UtcNow && (
                               invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid
                               || invoice.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)))
                            continue;

                        var versionSubId = default(int);
                        var addonSubId = default(int);

                        if (invoice.CustomerSubscription.IsAddOn)
                        {
                            var addonSub = invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault();

                            if (addonSub is null)
                                continue;

                            addonSubId = addonSub.Id;
                            versionSubId = addonSub.VersionSubscriptionId;
                        }
                        else
                        {
                            var versionSub = invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault();

                            if (versionSub is null)
                                continue;

                            versionSubId = versionSub.Id;
                        }

                        var paymentMethod = await _paymentBLL.GetCustomerDefaultPaymentMethodAsnyc(invoice.CustomerSubscription.CustomerId);

                        try
                        {

                            var invoicePeriods = _invoiceHelper.GetInvoicePeriods(invoice.EndDate, invoice.CustomerSubscription.RenewEvery);
                            for (int i = 0; i < invoicePeriods.Count; i++)
                            {

                                var newInvoice = await _invoiceBLL.CreateInvoiceAsync(new NewInvoiceDto
                                {
                                    VersionSubscriptionId = versionSubId,
                                    AddonSubscriptionId = addonSubId,
                                    InvoiceTypeId = invoice.InvoiceTypeId,
                                    PaymentMethodId = paymentMethod.Id,
                                    CountryId = invoice.CustomerSubscription.Customer.CountryId,
                                    OldInvoice = invoice,
                                    StartDate = invoicePeriods[i].StartDate,
                                    EndDate = invoicePeriods[i].EndDate,
                                    IsCommit = false
                                });
                                // set license status to be expired.
                                await _licensesBLL.MakeLicensesExpiredAsync(invoice.CustomerSubscription.Licenses.ToList(), isCommit: false);
                                await _unitOfWork.CommitAsync();

                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //check invoices that will be expired within 3 days or today
        public async Task CheckInvoicesExpireationAutoAsync()
        {
            try
            {
                //get all invices that will expire after 3 days or in the same day
                var willExpireInvoices = _invoiceRepository.Where(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid
                                                                      && (x.EndDate.Date == DateTime.UtcNow.AddDays(3).Date || x.EndDate.Date == DateTime.UtcNow.Date)).ToList();
                foreach (var willExpireInvoice in willExpireInvoices)
                {
                    //push new notification db
                    var _notificationItem = new GetNotificationForCreateDto();
                    _notificationItem.CustomerId = willExpireInvoice.CustomerSubscription.CustomerId;
                    _notificationItem.IsAdminSide = false;
                    _notificationItem.IsCreatedBySystem = true;
                    _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.EndSubscription;
                    _notificationItem.InvoiceId = willExpireInvoice.Id;
                    //check if will ended today
                    if (willExpireInvoice.EndDate.Date == DateTime.UtcNow.Date)
                    {
                        _notificationItem.NotificationActionSubTypeId = (int)SuscribtionExpiredTypeEnum.Today;
                    }
                    else
                    {
                        //will end after three days
                        _notificationItem.NotificationActionSubTypeId = (int)SuscribtionExpiredTypeEnum.WithinNDays;
                    }
                    await _notificationDataBLL.CreateAsync(_notificationItem);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteOldGeneratedInoivcesPdf()
        {
            try
            {
                var basePath = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var tempPath = $@"{basePath}\Files\PDFs\Invoices\Temp";

                if (IO.Directory.Exists(tempPath))
                {
                    foreach (var file in IO.Directory.EnumerateFiles(tempPath, "*.pdf"))
                    {
                        try
                        {
                            var fileCreationDate = IO.File.GetCreationTimeUtc(file);

                            if (fileCreationDate < DateTime.UtcNow.AddDays(-1))
                                IO.File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateFawryReferenceInvoices()
        {
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);

            try
            {


                //get all Unpaid invoices, paymentmethod is Fawry 
                var unpaidInvoicesWithFawryRefereneces = _invoiceRepository.Where(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid && x.PaymentMethodId == paymentMethod.Id).OrderByDescending(x => x.Id).ToList();

                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                foreach (var invoice in unpaidInvoicesWithFawryRefereneces)
                {
                    if (invoice.PaymentInfo != null)
                    {

                        var paymentInfo = JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo);
                        var generatedReferenceNumber = string.Empty;
                        //update all fawry payments
                        if (paymentInfo != null && paymentInfo.IsSuccess && paymentInfo.Fawry != null/* && !paymentInfo.Fawary.IsFawryCard*/)
                        {
                            try
                            {
                                generatedReferenceNumber = paymentInfo.Fawry.GeneratedMerchantNumber;
                                if (!string.IsNullOrWhiteSpace(generatedReferenceNumber))
                                {
                                    var paymentStatus = await fawryClient.GetPaymentStatusAsync(generatedReferenceNumber);
                                    if (paymentStatus.IsSuccess && (paymentStatus.Status == (int)FawryPaymentStatus.Paid || paymentStatus.PaymentStatus == FawryPaymentStatus.Paid.ToString().ToUpper()))
                                    {
                                        await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoice.Id, (PaymentTypesEnum)paymentMethod.PaymentType.Id, (int)InvoiceStatusEnum.Paid, paymentStatus.Serialize(), true);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task UpdateFawryReferenceInvoice(int invoiceId)
        {
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);

            try
            {
                var invoice = _invoiceRepository.Where(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid && x.PaymentMethodId == paymentMethod.Id && x.Id == invoiceId).FirstOrDefault();

                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);

                if (invoice != null && invoice.PaymentInfo != null)
                {

                    var paymentInfo = JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo);
                    var generatedReferenceNumber = string.Empty;
                    //update all fawry payments
                    if (paymentInfo != null && paymentInfo.IsSuccess && paymentInfo.Fawry != null/* && !paymentInfo.Fawary.IsFawryCard*/)
                    {

                        generatedReferenceNumber = paymentInfo.Fawry.GeneratedMerchantNumber;
                        if (!string.IsNullOrWhiteSpace(generatedReferenceNumber))
                        {
                            var paymentStatus = await fawryClient.GetPaymentStatusAsync(generatedReferenceNumber);
                            if (paymentStatus.IsSuccess && (paymentStatus.Status == (int)FawryPaymentStatus.Paid || paymentStatus.PaymentStatus == FawryPaymentStatus.Paid.ToString().ToUpper()))
                            {
                                await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoice.Id, (PaymentTypesEnum)paymentMethod.PaymentType.Id, (int)InvoiceStatusEnum.Paid, paymentStatus.Serialize(), true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task BlockWorkSpaceAfterTrialPeriod()
        {
            var workSpaces = _workspaceRepository.GetAll().Where(w => w.StatusId == (int)WorkSpaceStatusEnum.Trial ||
                                                                                             w.StatusId == (int)WorkSpaceStatusEnum.Extended);
            string connectionString = $"Data source=185.44.64.217,1437;initial catalog=master;Integrated security=False; User Id=sa;Password=~!@Dexef321;MultipleActiveResultSets=True;Connection Timeout=6000;";
            foreach (var workSpace in workSpaces)
            {
                if (workSpace.ExpirationDate != null)
                {
                    bool isExpired = workSpace.ExpirationDate <= DateTime.UtcNow;
                    if (workSpace.StatusId == (int)WorkSpaceStatusEnum.Trial && isExpired)
                    {
                        //BlockDatabase 
                        using (var context = new RestoreDbContext(connectionString))
                        {
                            var query = $@"use master ALTER DATABASE {workSpace.DatabaseName} SET OFFLINE WITH ROLLBACK IMMEDIATE;";
                            context.Database.ExecuteSqlRaw(query);
                        }
                    }
                    if (workSpace.StatusId == (int)WorkSpaceStatusEnum.Extended && isExpired)
                    {
                        //DropDatabaseTrial
                        using (var context = new RestoreDbContext(connectionString))
                        {
                            var query = $@"use master ALTER DATABASE {workSpace.DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                            var query2 = $@"use master DROP DATABASE {workSpace.DatabaseName};";
                            context.Database.ExecuteSqlRaw(query);
                            context.Database.ExecuteSqlRaw(query2);
                            _workspaceRepository.Delete(workSpace);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }
            }
        }


    }

}
