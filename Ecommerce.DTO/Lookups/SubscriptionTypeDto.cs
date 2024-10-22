using System.Collections.Generic;

namespace Ecommerce.DTO.Lookups
{
    public class SubscriptionTypeDto : BaseDto
    {
    }

    #region Input 

    public class CreateSubscriptionTypeInputDto : SubscriptionTypeDto
    {
        public string Name { get; set; }
    }

    public class UpdateSubscriptionTypeInputDto : CreateSubscriptionTypeInputDto
    {
        public int Id { get; set; }
     


    }

    public class GetSubscriptionTypeInputDto  : SubscriptionTypeDto
    {
        public int Id { get; set; }

    }


 
    #endregion
    #region Output

    public class GetSubscriptionTypeOutputDto : SubscriptionTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }


    }
    #endregion
}
