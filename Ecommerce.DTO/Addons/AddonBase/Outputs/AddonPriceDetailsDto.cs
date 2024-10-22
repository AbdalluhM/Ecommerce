namespace Ecommerce.DTO.Addons.AddonBase.Outputs
{
    public class AddonPriceDetailsDto
    {
        public int PaymentCountryId { get; set; }         
        public decimal PriceBeforeDiscount { get; set; }

        public decimal DiscountPercentage { get; set; } 

        public decimal NetPrice { get; set; }

        public string CurrencySymbol { get; set; }

        public string Discrimination { get; set; } = "Monthly";

    }
}
