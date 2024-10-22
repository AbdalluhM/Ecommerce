using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Workspace:EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ConnectionString { get; set; }
        public int CustomerId { get; set; }
        public int DexefCountryId { get; set; }
        public int? VersionSubscriptionId { get; set; }
        public int DexefCurrencyId { get; set; }
        public int StatusId { get; set; }
        public int? ImageId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsCloud { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Descreption { get; set; }
        public string ServerIp { get; set; }
        public string DatabaseName { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual CurrencyTable DexefCountry { get; set; }
        public virtual CurrencyTable DexefCurrency { get; set; }
        public virtual FileStorage Image { get; set; }
        public virtual WorkSpaceStatu Status { get; set; }
        public virtual VersionSubscription VersionSubscription { get; set; }
    }
}
