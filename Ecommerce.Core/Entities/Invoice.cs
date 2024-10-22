using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Invoice:EntityBase
    {
        public Invoice()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Serial { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int InvoiceStatusId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        public string TaxReg { get; set; }
        public string Address { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentInfo { get; set; }
        public string Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateDate { get; set; }
        public string PaymentResponse { get; set; }
        public string CancelReason { get; set; }
        public string InvoiceTitle { get; set; }
        public string PaymentInfoSearch { get; set; }
        public int? CurrencyId { get; set; }
        public int? PaidBy { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual CustomerSubscription CustomerSubscription { get; set; }
        public virtual InvoiceStatu InvoiceStatus { get; set; }
        public virtual InvoiceType InvoiceType { get; set; }
        public virtual Employee PaidByNavigation { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual RefundRequest RefundRequest { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
