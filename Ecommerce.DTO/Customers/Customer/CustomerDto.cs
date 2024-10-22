using Microsoft.AspNetCore.Http;

using Ecommerce.Core.Entities;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.DTO.Customers
{
    public class CustomerDto
    {
        #region Api
        public class UploadCustomerImageAptInputDto
        {
            public IFormFile File { get; set; }
        }

        #endregion

        #region DownloadCount
        public class UpdateVersionDownloadCountInputDto
        {
            public int VersionId { get; set; }
            [JsonIgnore]
            public int? CustomerId { get; set; }
            public string IpAddress { get; set; }

        }
        public class UpdateAddOnDownloadCountInputDto
        {
            public int AddOnId { get; set; }
            [JsonIgnore]
            public int? CustomerId { get; set; }
            public string IpAddress { get; set; }
        }
        #endregion

        #region UpdateCustomer



        public class UploadCustomerImageInputDto
        {
            [JsonIgnore]
            public int Id { get; set; }
            [JsonIgnore]
            //public FileDto File { get; set; }
            public SingleFilebaseDto File { get; set; }
        }
        public class UpdateCustomerInputDto
        {
            [JsonIgnore]
            public int Id { get; set; }
            public string Name { get; set; }
            public string PostalCode { get; set; }
            public string CompanyName { get; set; }
            public string TaxRegistrationNumber { get; set; }
            public int? IndustryId { get; set; }
            public int? CompanySizeId { get; set; }
            public string CompanyWebsite { get; set; }
            public string City { get; set; }
            [JsonIgnore]
            public DateTime ModifiedDate { get; set; }
            public DateTime? BirthDate { get; set; }
            public string FullAddress { get; set; }

        }
        #endregion

        #region CrudOperation
        public class GetCustomerInputDto
        {
            public int Id { get; set; }
        }

        public class DeleteCustomerInputDto
        {
            public int Id { get; set; }
        }
        #endregion

        #region CustomerHistoryRegistered
        public class UpdateCustomerStatusInputDto
        {
            public int Id { get; set; }
        }
        #endregion

        #region Contract
        public class InvoiceFilterDto : FilteredResultRequestDto
        {
            public int InvoiceStatusId { get; set; }
        }
        public class FilterInvoiceByCountry : InvoiceFilterDto
        {
            public int EmployeeId { get; set; }
        }
        public class FilterByEmployeeCountryInputDto : FilteredResultRequestDto
        {
            public int EmployeeId { get; set; }
        }
        public class UpdateCustomerByAdminInputDto
        {
            public int CustomerId { get; set; }
            public int CountryId { get; set; }
            public bool Active { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string FullAddress { get; set; }
        }
        #endregion

        #region Output
        #region CustomerRegistrationHistory
        public class GetCustomerRegistrationHistoryOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone => CustomerMobile?.Mobile ?? null;
            public string Email => CustomerEmail?.Email ?? null;
            public string Country { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string Status { get; set; }
            public int StatusId { get; set; }
            [JsonIgnore]
            public CustomerEmailDto CustomerEmail = new();
            [JsonIgnore]
            public CustomerMobileDto CustomerMobile = new();
            //public IEnumerable<VersionTitle> Versions => CustomerSubscriptions.SelectMany(x => x.Versions);
            public List<VersionTitle> Versions { get; set; }
            [JsonIgnore]
            public List<CustomerSubscriptionDto> CustomerSubscriptions { get; set; }
        }

        public class CustomerMobileDto
        {
            public string Mobile { get; set; }
        }
        public class CustomerEmailDto
        {
            public string Email { get; set; }
        }
        public class VersionTitle
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
        }
        #endregion
        #region CustomerOutput
        public class GetCustomerOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string PhoneCode { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
            public string CompanyName { get; set; }
            public string TaxRegistrationNumber { get; set; }
            public string CompanyWebsite { get; set; }
            public string City { get; set; }
            public string FullAddress { get; set; }
            public DateTime? BirthDate { get; set; }
            public DateTime LastPasswordUpdate { get; set; }
            public FileStorageDto Logo { get; set; }
            public IndustryOutputDto Industry { get; set; }
            public CompanySizeOutputDto CompanySize { get; set; }
            public IEnumerable<CardTokenDto> CustomerCards { get; set; }
        }
        public class GetSimplifiedCustomerOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }
        public class IndustryOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Isactive { get; set; }
        }
        public class CompanySizeOutputDto
        {
            public int Id { get; set; }
            public string Size { get; set; }
            public bool IsActive { get; set; }
        }
        #endregion
        #region CustomerImage
        public class GetCustomerImageOutputDto
        {
            public FileStorageDto Logo { get; set; }
        }
        #endregion
        #region Contract
        public class GetAllCountriesOutputDto
        {
            public int Id { get; set; }
            public string Country { get; set; }
            public string Currency { get; set; }
        }
        public class GetContractCustomerOutputDto
        {
            public int Id { get; set; }
            public string ContractId { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public string Currency { get; set; }
            public DateTime RegistrationDate { get; set; }
            //public string Status { get; set; }
            public bool IsActive { get; set; }
            public bool Invoiced { get; set; }
            /* public IEnumerable<VersionTitle> Versions { get; set; } *//*=> CustomerSubscriptions.SelectMany(x => x.Versions)*/
            public List<VersionTitle> Versions { get; set; }
            [JsonIgnore]
            public List<CustomerSubscriptionDto> CustomerSubscriptions { get; set; }
        }
        public class CustomerSubscriptionDto
        {
            public IEnumerable<VersionTitle> Versions { get; set; }
        }
        public class GetCustomerReferencesOutputDto
        {
            public int Id { get; set; }
            public string ContractId { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public int CountryId { get; set; }
            public string PostalCode { get; set; }
            public string CompanyName { get; set; }
            public string CurrencyName { get; set; }
            public string TaxRegistrationNumber { get; set; }
            public string CompanyWebsite { get; set; }
            public string City { get; set; }
            public string FullAddress { get; set; }
            public string Status { get; set; }
            public FileStorageDto Logo { get; set; }
            public string Industry { get; set; }
            public string CompanySize { get; set; }
            public bool IsActive { get; set; }
            public IEnumerable<GetCustomerProductOutputDto> CustomerApplications { get; set; }
        }
        public class CustomerApplicationDto
        {
            public int Id { get; set; }
            public int NumberOfLicenses { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public int NewRelease { get; set; } = 2;
            public string PriceLevel { get; set; }
            public FileStorageDto Logo { get; set; }
            public VersionPriceDetailsDto Price { get; set; }
        }
        public class CustomerReguesttDto
        {
            public int Id { get; set; }
            public string DeviceName { get; set; }
            public string Serial { get; set; }
            public DateTime ActivateOn { get; set; }
            public DateTime RenewalDate { get; set; }
            public string Status { get; set; }
        }
        public class GetCustomerActivitesOutputDto
        {
            public string CustomerName { get; set; }
            public PagedResultDto<CustomerActivitesDto> CustomerActivities { get; set; }
        }
        public class CustomerActivitesDto
        {
            public string Id { get; set; }
            public AuditActionTypeDto ActionType { get; set; }
            public string Entity { get; set; }
            public string Field { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public string Owner { get; set; }
            public DateTime CreateDate { get; set; }
        }
        #endregion

       

       
        #endregion

    }
}
