namespace Ecommerce.DTO.Applications.ApplicationVersions
{
    public class VersionPriceDetailsDto
    {
        public int  PaymentCountryId { get; set; }             
        public decimal PriceBeforeDiscount { get; set; }

        public decimal DiscountPercentage { get; set; }

        public decimal NetPrice { get; set; }

        public string CurrencySymbol { get; set; }

        public virtual string Discrimination { get; set; } 

    }

    public class MonthlyPriceDetailsDto : VersionPriceDetailsDto
    {
        public override string Discrimination { get; set; } = "Monthly";

    }
    public class YearlyPriceDetailsDto : VersionPriceDetailsDto
    {
        public override string Discrimination { get; set; } = "Yearly";

    }
    public class ForeverPriceDetailsDto : VersionPriceDetailsDto
    {
        public override string Discrimination { get; set; } = "Forever";

    }


    public class VersionPriceAllDetailsDto
    {
        public bool IsDefault { get; set; }
        public MonthlyPriceDetailsDto Monthly { get; set; }
        public YearlyPriceDetailsDto Yearly { get; set; }
        public ForeverPriceDetailsDto Forever { get; set; }
    }
}
