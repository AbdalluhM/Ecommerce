using Microsoft.AspNetCore.Http;

using Ecommerce.DTO.Files;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Employees
{
    public class EmployeeDto :BaseDto
    {
          
    }
    #region Input 
   
    public class LoginModelInputDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
    #region Login
    public class LoginModelOutputDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public int RoleId { get; set; }
        public string Role { get; set; }
   
    }
    public class ChangePasswordInputDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    #endregion
    public class APICreateEmployeeInputDto : EmployeeDto
    {
        public IFormFile File { get; set; }

        public int EmployeeTypeId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int? CreatedBy { get; set; }
        public List<int> EmployeeCountries { get; set; }
        [JsonIgnore]
        public List<UpdateAssignEmployeeToCountryInputDto> EmployeeCountriesList { get; set; }

    }
    public class CreateEmployeeInputDto : EmployeeDto
    {
        [JsonIgnore]
        public FileDto File { get; set; }
        //public int EmployeeTypeId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int? CreatedBy { get; set; }
        public List<int> EmployeeCountries { get; set; }
        [JsonIgnore]
        public List<UpdateAssignEmployeeToCountryInputDto> EmployeeCountriesList  { get; set; }
        public string Mobile { get; set; }

        public int? RoleId { get; set; }
        public bool? IsAdminForOtherCountries { get; set; }

    }
    public class APIUpdateEmployeeInputDto : BaseDto
    {
        public int Id { get; set; }

        public IFormFile File { get; set; }

        public string Name { get; set; }
        public string Mobile { get; set; }


    }

    public class UpdateEmployeeInputDto : CreateEmployeeInputDto
    {

        public int Id { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }



    }
    public class GetEmployeeInputDto : EmployeeDto
    {
        public int Id { get; set; }

    }
    public class DeleteEmployeeInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }

    public class LoginEmployeeInputDto : EmployeeDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
    #endregion
    #region Output

    public class GetEmployeeOutputDto : EmployeeDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public string Token { get; set; }
        public List<GetAssignedEmployeeToCountryOutputDto> EmployeeCountries { get; set; }
        public string Mobile { get; set; }
        public FileStorageDto Image { get; set; }
        public bool? IsAdmin { get; set; }
        public bool IsAdminForOtherCountries { get; set; }
    }
    #endregion

    //#region Input 

    //public class CreateEmployeeInputDto : EmployeeDto
    //{
    //    public int EmployeeTypeId { get; set; }
    //    public string Name { get; set; }
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //    public bool? IsActive { get; set; }
    //    public  DateTime CreateDate { get; set; } 
    //    public int? CreatedBy { get; set; }
    //    public List<GetSelectAssignedCurrencyToCountryOutputDto> CountryCurrencies { get; set; }

    //}

    //public class UpdateEmployeeInputDto : CreateEmployeeInputDto
    //{
    //    public int Id { get; set; }
    //    public bool IsDeleted { get; set; }
    //    public  DateTime? ModifiedDate { get; set; }
    //    public int? ModifiedBy { get; set; }


    //}

    //public class GetEmployeeInputDto : EmployeeDto
    //{
    //    public int Id { get; set; }

    //}

    //public class LoginEmployeeInputDto : EmployeeDto
    //{
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    public string Password { get; set; }

    //}
    //#endregion
    //#region Output

    //public class GetEmployeeOutputDto : EmployeeDto
    //{
    //    public int Id { get; set; }
    //    public int EmployeeTypeId { get; set; }
    //    public string Name { get; set; }
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    //public bool? IsActive { get; set; }
    //    //public bool IsDeleted { get; set; }
    //    //public List<GetSelectAssignedCurrencyToCountryOutputDto> CountryCurrencies { get; set; }
    //}
    //#endregion
}
