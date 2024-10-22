using Ecommerce.DTO.Taxes;

using System.Collections.Generic;

namespace Ecommerce.DTO.Lookups
{
    public class CountryDto : BaseDto
    {
    }

    #region Input 

    public class CreateCountryInputDto : CountryDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultForOther { get; set; }
        public List<GetCurrencyOutputDto> Currencies { get; set; }
    }

    public class UpdateCountryInputDto : CreateCountryInputDto
    {
        public int Id { get; set; }
     


    }

    public class GetCountryInputDto  : CountryDto
    {
        public int Id { get; set; }

    }


 
    #endregion
    #region Output

    public class GetCountryOutputDto : CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneCode { get; set; }
        public bool IsActive { get; set; }

        public List<GetTaxOutputDto> Taxes { get; set; }


    }
    #endregion
}
