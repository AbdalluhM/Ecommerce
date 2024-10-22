using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Taxes;
using MyDexef.DTO;
using MyDexef.DTO.Paging;
using MyDexef.DTO.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class TaxTest : UnitTestBase
    {
        private readonly ITaxBLL _taxBLL;
        private const int _employeeId = 4067;
        private const int _countryId = 66;
        private const int _percentage = 20;
        private const string _name = "{\"default\":\"NewTax5\",\"ar\":\"5ضريبة جديدة\"}";
        private const string _name2 = "{\"default\":\"NewTax4\",\"ar\":\"4ضريبة جديدة\"}";
        private const string _updateName = "{\"default\":\"NewTax2\",\"ar\":\"ضريبة جديد2\"}";
        private const string _exisitName = "{\"default\":\"VAT14\",\"ar\":\"ضريبة 14\"}";
        public TaxTest()
        {
            _taxBLL = serviceProvider.GetRequiredService<ITaxBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public void GetTaxs_CheckFilterIfExisit_ReturnTaxs(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Taxs = _taxBLL.GetPagedTaxList(filter);
            //Assert
            Taxs.IsSuccess.ShouldBeTrue();
            Taxs.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        [DataRow(true , _name)]
        [DataRow(false ,_name2)]
        public void CreateTax_AddNewTaxWithCorrectData_ReturnTax(bool include , string name)
        {
            //Arrange
            var param = new CreateTaxInputDto()
            {
                Name = name,
                CountryId = _countryId,
                IsDefault = true,
                Percentage = _percentage,
                PriceIncludeTax = include,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tax = _taxBLL.Create(param);
            //Assert
            Tax.IsSuccess.ShouldBeTrue();
            Tax.Data.ShouldNotBeNull();
            Tax.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        [DataRow("", _countryId , _percentage ,_employeeId)]
        [DataRow(_name, 0 , _percentage ,_employeeId)]
        [DataRow(_name, _countryId , 0 ,_employeeId)]
        [DataRow(_name, _countryId , _percentage ,0)]
        public void ValidateFieldsIsRequired_AddNewTaxWithoutCreatedBy_ReturnCannotCreateTaxWithoutCreatedBy
            (string name , int countryId , int percentage , int employeeId)
        {
            //Arrange
            var param = new CreateTaxInputDto()
            {
                Name = name,
                CountryId = countryId,
                IsDefault = true,
                Percentage = percentage,
                PriceIncludeTax = true,
                CreatedBy = employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tax = _taxBLL.Create(param);
            //Assert
            Tax.IsSuccess.ShouldBeFalse();
            Tax.Data.ShouldBeNull();
            Tax.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void ValidateNameIsUnique_AddNewTaxWithNameExisitInDB_ReturnCanNotCreateTaxAlreadyExisit()
        {
            //Arrange
            var param = new CreateTaxInputDto()
            {
                Name = _exisitName,
                CountryId = _countryId,
                IsDefault = true,
                Percentage = _percentage,
                PriceIncludeTax = true,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tax = _taxBLL.Create(param);
            //Assert
            Tax.IsSuccess.ShouldBeFalse();
            Tax.Data.ShouldBeNull();
            Tax.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public void UpdateTax_UpdateTaxWithCorrectData_ReturnTaxUpdated()
        {
            //Arrange
            var param = new UpdateTaxInputDto()
            {
                Name = _updateName,
                CountryId = _countryId,
                IsDefault = true,
                Percentage = _percentage,
                PriceIncludeTax = true,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                Id = 1009
            };
            //Action
            var Tax = _taxBLL.Update(param);
            //Assert
            Tax.IsSuccess.ShouldBeTrue();
            Tax.Data.ShouldNotBeNull();
            Tax.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public void UpdateTax_UpdateTaxWithNameAlreadyExisit_ReturnThisTaxAlreadyExisit()
        {
            //Arrange
            var param = new UpdateTaxInputDto()
            {

                Name = _exisitName,
                CountryId = _countryId,
                IsDefault = true,
                Percentage = _percentage,
                PriceIncludeTax = true,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                Id = 1008
            };
            //Action
            var Tax = _taxBLL.Update(param);
            //Assert
            Tax.IsSuccess.ShouldBeFalse();
            Tax.Data.ShouldBeNull();
            Tax.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteTaxNotFound_TakeTaxIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = _taxBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public void DeleteTax_DeleteTaxExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 1008,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _taxBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public void DeleteTaxRelatedData_TakeTaxIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 1010,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _taxBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
