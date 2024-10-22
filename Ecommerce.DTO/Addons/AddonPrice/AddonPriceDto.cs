using Ecommerce.DTO.Paging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Addons.AddonPrice
{
    public class AddOnPriceDto : BaseDto
    {
        public int AddOnId { get; set; }
        public int CountryCurrencyId { get; set; }
        public int PriceLevelId { get; set; }
    }

    #region Input
    public class CreateAddOnPriceInputDto : AddOnPriceDto
    {
        public decimal YearlyPrice { get; set; }
        public decimal YearlyPrecentageDiscount { get; set; }
        public decimal YearlyNetPrice
        {
            get
            {
                return YearlyPrice - (YearlyPrice * YearlyPrecentageDiscount / 100);
            }
            set { }
        }
        public decimal MonthlyPrice { get; set; }
        public decimal MonthlyPrecentageDiscount { get; set; }
        public decimal MonthlyNetPrice
        {
            get
            {
                return MonthlyPrice - (MonthlyPrice * MonthlyPrecentageDiscount / 100);
            }
            set { }
        }
        public decimal ForeverPrice { get; set; }
        public decimal ForeverPrecentageDiscount { get; set; }
        public decimal ForeverNetPrice
        {
            get
            {
                return ForeverPrice - (ForeverPrice * ForeverPrecentageDiscount / 100);
            }
            set { }
        }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }


    }

    public class UpdateAddOnPriceInputDto : CreateAddOnPriceInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteAddOnPriceInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetAddOnPriceInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    public class GetAddOnPricesInputDto : BaseDto
    {
        public int AddOnId { get; set; }

    }


    #endregion
    #region Output
    public class GetAddOnPriceOutputDto : UpdateAddOnPriceInputDto
    {
        public string CountryName { get; set; }
        public string CurrencyShortCode { get; set; }
        public string PriceLevelName { get; set; }
        public string NumberOfLicenses { get; set; }
        public int MissingPricesCount { get; set; } = 0;

    }

    #endregion

    #region Comparer
    public class AddOnPricesEqualityComparer : IEqualityComparer<GetAddOnPriceOutputDto>
    {

        public bool Equals( GetAddOnPriceOutputDto x, GetAddOnPriceOutputDto y )
        {
            if (/*x.AddOnId == y.AddOnId &&*/ x.CountryCurrencyId == y.CountryCurrencyId & x.PriceLevelId == y.PriceLevelId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode( GetAddOnPriceOutputDto x )
        {
            int hCode = /*x.AddOnId ^*/ x.PriceLevelId ^ x.CountryCurrencyId;
            return hCode.GetHashCode();
        }
    }

    #endregion
}
