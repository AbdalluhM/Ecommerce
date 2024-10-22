using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using System.Text.Json.Serialization;
using System;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Lookups;

namespace Ecommerce.DTO.Application
{
    public class ApplicationDto : BaseDto
    {
       
    }
    #region API
    public class CreateApplicationAPIInputDto : ApplicationDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public int SubscriptionTypeId { get; set; }
        public int? DeviceTypeId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
     
        public IFormFile File { get; set; }
    }
    public class UpdateApplicationAPIInputDto : CreateApplicationAPIInputDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Input

    public class CreateApplicationInputDto : ApplicationDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public int ImageId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public int? DeviceTypeId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }



    }
    public class UpdateApplicationInputDto : CreateApplicationInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteApplicationInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetApplicationInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    #endregion

    #region Output
    public class GetApplicationOutputDto : ApplicationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public int SubscriptionTypeId { get; set; }

        public string ShortDescription { get; set; }
        public bool IsActive { get; set; }

        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }

        //Extra attributes
        public int VersionsCount { get; set; }
        public int ModulesCount { get; set; }
        public int MissingPricesCount { get; set; }
        //public string ApplicationName { get; set; }
        //public string FeatureName { get; set; }
        //public string VersionName { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetApplicationLabelOutputDto Label { get; set; }



    }
    public class GetApplicationDropDownOutputDto : ApplicationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public GetSubscriptionTypeOutputDto SubscriptionType { get; set; }

    }

    #endregion

}
