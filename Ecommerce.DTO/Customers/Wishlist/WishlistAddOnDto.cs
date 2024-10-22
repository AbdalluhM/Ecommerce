using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Wishlist
{
    public class WishlistAddOnDto
    {

    }
    public class CreateWishlistAddOnInputDto : WishlistAddOnDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        public int AddOnId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class DeleteWishlistAddOnInputDto : WishlistAddOnDto
    {
        public int AddOnId { get; set; }
        [JsonIgnore]
        public int CustomerId { get; set; }

    }
    

    public class CustomerFilteredResultRequestDto : FilteredResultRequestDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
    }
    public class WishlistAddOnSearchInputDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        public string SearchItems { get; set; }
    }
    public class WishlistAddOnSearchApIInputDto
    {
        public string SearchItems { get; set; }
    }
    #region Output
    public class GetWihlistAddOnOutputDto : WishlistAddOnDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddOnId { get; set; }
        public DateTime CreateDate { get; set; }
        public GetAddOnWishlistOutputDto AddOn { get; set; }
    }
    public class GetAddOnWishlistOutputDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public FileStorageDto Logo { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetAddonLabelOutputDto Label { get; set; }
        //public decimal MinimumPrice { get; set; } = 0;
        public AddonPriceDetailsDto Price { get; set; }

    }

    #endregion

}
