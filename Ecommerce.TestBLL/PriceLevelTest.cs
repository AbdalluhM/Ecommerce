using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.PriceLevels;
using MyDexef.DTO;
using MyDexef.DTO.Lookups.PriceLevels.Inputs;
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
    public class PriceLevelTest : UnitTestBase
    {
        private readonly IPriceLevelBLL _priceLevel;
        private const int _employeeId = 4067;
        private const int _numberOfLicenses = 5;
        private const string _name = "{\"default\":\"NewPriceLevel1\",\"ar\":\"تاج جديد1\"}";
        private const string _updateName = "{\"default\":\"NewPriceLevel2\",\"ar\":\"تاج جديد2\"}";
        private const string _exisitName = "{\"default\":\"price 5\",\"ar\":\"سعر 5\"}";
        public PriceLevelTest()
        {
            _priceLevel = serviceProvider.GetRequiredService<IPriceLevelBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public void GetPriceLevels_CheckFilterIfExisit_ReturnPriceLevels(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var PriceLevels = _priceLevel.GetAllPriceLevelsPagedList(filter);
            //Assert
            PriceLevels.IsSuccess.ShouldBeTrue();
            PriceLevels.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public void CreatePriceLevel_AddNewPriceLevelWithCorrectData_ReturnPriceLevel()
        {
            //Arrange
            var param = new NewPriceLevelDto()
            {
                Name = _name,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                NumberOfLicenses = _numberOfLicenses,
                IsActive = true,
            };
            //Action
            var PriceLevel = _priceLevel.AddPriceLevel(param);
            //Assert
            PriceLevel.IsSuccess.ShouldBeTrue();
            PriceLevel.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        [DataRow("", _employeeId, _numberOfLicenses)]
        [DataRow(_name,0 , _numberOfLicenses)]
        [DataRow(_name ,_employeeId , 0)]
        public void ValidateFieldsIsRequired_AddNewPriceLevelWithoutCreatedBy_ReturnCannotCreatePriceLevelWithoutCreatedBy(string name, int employeeId , int num)
        {
            //Arrange
            var param = new NewPriceLevelDto()
            {
                Name = name,
                CreatedBy = employeeId,
                CreateDate = DateTime.UtcNow,
                NumberOfLicenses = num,
                IsActive = true,
            };
            //Action
            var PriceLevel = _priceLevel.AddPriceLevel(param);
            //Assert
            PriceLevel.IsSuccess.ShouldBeFalse();
            PriceLevel.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void ValidateNameIsUnique_AddNewPriceLevelWithNameExisitInDB_ReturnCanNotCreatePriceLevelAlreadyExisit()
        {
            //Arrange
            var param = new NewPriceLevelDto()
            {
                Name = _exisitName,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                NumberOfLicenses = _numberOfLicenses,
                IsActive = true,
            };
            //Action
            var PriceLevel = _priceLevel.AddPriceLevel(param);
            //Assert
            PriceLevel.IsSuccess.ShouldBeFalse();
            PriceLevel.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public void UpdatePriceLevel_UpdatePriceLevelWithCorrectData_ReturnPriceLevelUpdated()
        {
            //Arrange
            var param = new UpdatePriceLevelDto()
            {
                Name = _updateName,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                NumberOfLicenses = _numberOfLicenses,
                IsActive = true,
                Id = 3063
            };
            //Action
            var PriceLevel = _priceLevel.UpdatePriceLevel(param);
            //Assert
            PriceLevel.IsSuccess.ShouldBeTrue();
            PriceLevel.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public void UpdatePriceLevel_UpdatePriceLevelWithNameAlreadyExisit_ReturnThisPriceLevelAlreadyExisit()
        {
            //Arrange
            var param = new UpdatePriceLevelDto()
            {
                Name = _exisitName,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                Id = 3063
            };
            //Action
            var PriceLevel = _priceLevel.UpdatePriceLevel(param);
            //Assert
            PriceLevel.IsSuccess.ShouldBeFalse();
            PriceLevel.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeletePriceLevelNotFound_TakePriceLevelIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = _priceLevel.DeletePriceLevel(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public void DeletePriceLevel_DeletePriceLevelExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 3064,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _priceLevel.DeletePriceLevel(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public void DeletePriceLevelRelatedData_TakePriceLevelIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 3064,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _priceLevel.DeletePriceLevel(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
