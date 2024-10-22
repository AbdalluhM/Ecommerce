using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Settings.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.WishlistApplication
{
    public class WishlistApplicationDto
    {

    }
    public class CreateWishlistApplicationInputDto : WishlistApplicationDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        public int ApplicationId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class DeleteWishlistApplicationInputDto : WishlistApplicationDto
    {

        public int ApplicationId { get; set; }
        [JsonIgnore]
        public int CustomerId { get; set; }

    }

    public class WishlistApplicationSearchInputDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        public string SearchItems { get; set; }
    }
    public class WishlistApplicationSearchApiInputDto
    {
        public string SearchItems { get; set; }
    }
    #region Output
    public class GetWishlistApplicationOutputDto : WishlistApplicationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime CreateDate { get; set; }
        public GetApplicationWishlistOutputDto Application { get; set; }
    }
   
    public class GetApplicationWishlistOutputDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public FileStorageDto Logo { get; set; }
        public bool HasLabel =>Label != null ? true :false;

        public int SubscribtionTypeId { get; set; }
        public string FeaturedTag { get; set; }

        public GetApplicationLabelOutputDto Label { get; set; }

        //public decimal MinimumPrice { get; set; } = 0;
        public VersionPriceDetailsDto Price { get; set; }

        public RateDto Rate { get; set; }


    }
    #endregion
}
