using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Contracts.Invoices;
using MyDexef.BLL.Countries;
using MyDexef.DTO;
using MyDexef.DTO.Lookups;
using MyDexef.DTO.Paging;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class CountryTest :UnitTestBase
    {
        private readonly ICountryBLL _countryBLL;
        private const int _employeeId = 4067;
        private const int _countryId = 1;
        private const int _currencyId = 2;
        public CountryTest()
        {
            _countryBLL = serviceProvider.GetRequiredService<ICountryBLL>();
        }



        #region Get
        //get all country
        [TestMethod]
        [DataRow("")]
        [DataRow("eg")]
        public async Task GetCountryCurrency_CheckFilterIfExisit_ReturnCountryCurrency(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var countryCurrency = await _countryBLL.GetPagedListAsync(filter);
            //Assert
            countryCurrency.IsSuccess.ShouldBeTrue();
            countryCurrency.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #endregion

        #region Create
        //Best case to create country
        [TestMethod]
        public async Task CreateCountryCurrency_AssignCurrencyToCountry_ReturnCountryCurrency()
        {
            //Arrange
            var param = new AssignCurrencyToCountryInputDto
            {
                CountryId = _countryId,
                CurrencyId = _currencyId,
                DefaultForOther = true,
                IsActive = true,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.CreateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeTrue();
            countryCurrency.Data.ShouldNotBeNull();
            countryCurrency.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        //Input Validation
        [TestMethod]
        [DataRow(0, _currencyId, _employeeId)]
        [DataRow(_countryId, 0, _employeeId)]
        [DataRow(_countryId, _currencyId, 0)]
        public async Task ValidateFieldsIsRequired_AssignCurrencyToCountry_ReturnFieldsIsRequired(int countryId, int currencyId, int createdBy)
        {
            //Arrange
            var param = new AssignCurrencyToCountryInputDto
            {
                CountryId = countryId,
                CurrencyId = currencyId,
                CreatedBy = createdBy,
                DefaultForOther = true,
                IsActive = true,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.CreateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }


        //create country already exisit
        [TestMethod]
        public async Task CreateCountryCurrencyExisit_AssignCurrencyToCountryAlreadyExisit_ReturnAlreadyExisit()
        {
            //Arrange
            var param = new AssignCurrencyToCountryInputDto
            {
                CountryId = _countryId,
                CurrencyId = _currencyId,
                DefaultForOther = true,
                IsActive = true,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.CreateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }

        //create country not found 
        [TestMethod]
        [DataRow(1000, _currencyId)]
        [DataRow(_countryId, 2000)]
        public async Task CheckCountryOrCurrencyExisitInDb_AssignCurrencyToCountryAlreadyExisit_ReturnNotFound(int countryId, int currencyId)
        {
            //Arrange
            var param = new AssignCurrencyToCountryInputDto
            {
                CountryId = countryId,
                CurrencyId = currencyId,
                DefaultForOther = true,
                IsActive = true,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.CreateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update

        [TestMethod]
        public async Task UpdateCountryCurrency_AssignCountrytoAnotherCurrency_ReturnCountryCurrency()
        {
            //Arrange
            var param = new UpdateAssignCurrencyToCountryInputDto
            {
                Id = 5070,
                CountryId = 8,
                CurrencyId = 2,
                DefaultForOther = true,
                IsActive = true,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.UpdateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeTrue();
            countryCurrency.Data.ShouldNotBeNull();
            countryCurrency.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        //Update country already exisit
        [TestMethod]
        public async Task UpdateCountryCurrencyExisit_AssignCurrencyToCountryAlreadyExisit_ReturnAlreadyExisit()
        {
            //Arrange
            var param = new UpdateAssignCurrencyToCountryInputDto
            {
                Id = 5068,
                CountryId = 66,
                CurrencyId = 1,
                DefaultForOther = true,
                IsActive = true,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.UpdateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }
        //create country not found 
        [TestMethod]
        [DataRow(1000, 1)]
        [DataRow(2, 2000)]
        public async Task CheckInUpdateCountryOrCurrencyExisitInDb_AssignCurrencyToCountryAlreadyExisit_ReturnNotFound(int countryId, int currencyId)
        {
            //Arrange
            var param = new UpdateAssignCurrencyToCountryInputDto
            {
                Id = 5068,
                CountryId = countryId,
                CurrencyId = currencyId,
                DefaultForOther = true,
                IsActive = true,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.UpdateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }
        //Input Validation
        [TestMethod]
        [DataRow(0, 1, 4067)]
        [DataRow(3, 0, 4067)]
        [DataRow(3, 1, 0)]
        public async Task ValidateFieldsIsRequiredInUpdate_AssignCurrencyToCountry_ReturnFieldsIsRequired(int countryId, int currencyId, int createdBy)
        {
            //Arrange
            var param = new UpdateAssignCurrencyToCountryInputDto
            {
                Id = 5068,
                CountryId = countryId,
                CurrencyId = currencyId,
                DefaultForOther = true,
                IsActive = true,
                ModifiedBy = createdBy,
                ModifiedDate = DateTime.UtcNow,
            };
            //Action
            var countryCurrency = await _countryBLL.UpdateAsync(param);
            //Assert
            countryCurrency.IsSuccess.ShouldBeFalse();
            countryCurrency.Data.ShouldBeNull();
            countryCurrency.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteCountryCurrencyNotFound_TakeCountryCurrencyIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country not found
                Id = 1000000000,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _countryBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteCountryCurrency_TakeCountryCurrencyIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 5072,
                ModifiedBy=_employeeId,
                ModifiedDate= DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _countryBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteCountryCurrencyRelatedData_TakeCountryCurrencyIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 5065,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _countryBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
