using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Lookups
{

    public class CurrencyDto : BaseDto
    {
    }

    #region Input 

    public class CreateCurrencyInputDto : CurrencyDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Code { get; set; }

        public int MultiplyFactor { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCurrencyInputDto : CreateCurrencyInputDto
    {
        public int Id { get; set; }



    }

    public class GetCurrencyInputDto : CurrencyDto
    {
        public int Id { get; set; }

    }


    #endregion
    #region Output

    public class GetCurrencyOutputDto : CurrencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int MultiplyFactor { get; set; }
    }
    #endregion

}
