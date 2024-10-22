using Ecommerce.DTO.Taxes;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Lookups
{
    #region Input 

    public class AssignCurrencyToCountryInputDto : BaseDto
    {
        public int CountryId { get; set; }
        public int CurrencyId { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultForOther { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]

        public int CreatedBy { get; set; }
    }

    public class UpdateAssignCurrencyToCountryInputDto : AssignCurrencyToCountryInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }


    }

    public class GetAssignedCurrencyToCountryInputDto : BaseDto
    {
        public int Id { get; set; }

    }


    #endregion
    #region Output

    public class GetAssignedCurrencyToCountryOutputDto
    {
        public int Id { get; set; }
        public int CountryId => Country?.Id ?? 0;// { get; set; }

        public string CountryName => Country?.Name;// { get; set; }
        public int CurrencyId => Currency?.Id ?? 0;//{ get; set; }
        public string CurrencyName => Currency?.Name ;//{ get; set; }
        public string CurrencySymbol => Currency?.Symbol;//{ get; set; }
        [JsonIgnore]
        public GetCountryOutputDto Country { get; set; }
        [JsonIgnore]
        public GetCurrencyOutputDto Currency { get; set; }

        public bool IsActive { get; set; }
        public bool DefaultForOther { get; set; }

        public List<GetTaxOutputDto> Taxes { get; set; }


    }

    #endregion
}