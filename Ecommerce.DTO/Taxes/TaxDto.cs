using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Taxes
{
    public class TaxDto : BaseDto
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public decimal Percentage { get; set; }
        public bool PriceIncludeTax { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    #region Input 
   
    public class CreateTaxInputDto : TaxDto
    {
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
    public class UpdateTaxInputDto : CreateTaxInputDto
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetTaxInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    public class GetCountryDefaultTaxInputDto : BaseDto
    {
        public int CountryId { get; set; }

    }
    public class GetCountryDefaultTaxByCustomerIdInputDto : BaseDto
    {
        public int CustomerId { get; set; }

    }

    public class GetTaxActivitesOutputDto
    {
        public string TaxName { get; set; }
        public int TaxId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }

        public PagedResultDto<TaxActivitesDto> TaxActivities { get; set; }
    }
    public class TaxActivitesDto
    {
        public string Id { get; set; }
        public AuditActionTypeDto ActionType { get; set; }
        public string Entity { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Owner { get; set; }
        [JsonIgnore]
        public int AuditLogId { get; set; }
        //[JsonIgnore]
        //public string ModifiedById { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    #endregion


    #region Output 
    public class GetTaxOutputDto : TaxDto
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }

        public string CountryName { get; set; }

        public string CreatedBy { get; set; }

    }

    #endregion
}
