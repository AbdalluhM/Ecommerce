using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using System.Text.Json.Serialization;
using System;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Settings.Files;
using System.Collections.Generic;
using Ecommerce.DTO.Addons.AddonLabels;
using System.Linq;
using System.Security.Cryptography;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Addons.AddonPrice;

namespace Ecommerce.DTO.Addons.AddonBase.Inputs
{
    public class NewAddonDto
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public bool IsActive { get; set; }

        public IFormFile File { get; set; }
    }
    #region API
    public class CreateAddOnAPIInputDto : AddOnDto
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public IFormFile File { get; set; }
    }
    public class UpdateAddOnAPIInputDto : CreateAddOnAPIInputDto
    {
        public int Id { get; set; }
    }
    #endregion
    #region Input
    public class AddOnDto : BaseDto
    {

    }
    public class CreateAddOnInputDto : AddOnDto
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }


        public bool IsActive { get; set; }

        [JsonIgnore]
        public int LogoId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }



    }
    public class UpdateAddOnInputDto : CreateAddOnInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteAddOnInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetAddOnInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Output
    public class GetAddOnOutputDto : AddOnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ShortDescription { get; set; }
        public bool IsActive { get; set; }

        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }

        public bool HasLabel => Label != null ? true : false;
        public GetAddonLabelOutputDto Label { get; set; }
        public int MissingPricesCount { get; set; } = 0;


    }

    public class AddOnMinPriceDto
    {
        public decimal MinMonthlyPrice { get; set; }
        public decimal MinYearylyPrice { get; set; }
        public decimal MinForEverPrice { get; set; }
        public decimal MinPrice => Math.Min(MinMonthlyPrice, Math.Min(MinYearylyPrice, MinForEverPrice));
        public string Discrimination { get; set; }
    }


    public class GetAddOnDataOutputDto : AddOnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int VersionId { get; set; }
        public FileStorageDto Logo { get; set; }
        public AddOnPriceDto MinAddOnPrice { get; set; }
        public List<AddOnPriceDto> AddOnPrices { get; set; }

    }
    #endregion

}
