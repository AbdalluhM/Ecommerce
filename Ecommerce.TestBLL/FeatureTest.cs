using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Features;
using MyDexef.Core.Entities;
using MyDexef.DTO;
using MyDexef.DTO.Features;
using MyDexef.DTO.Files;
using MyDexef.DTO.Paging;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace MyDexef.TestBLL
{

    [TestClass]
    public class FeatureTest : UnitTestBase
    {
        private readonly IFeatureBLL _featureBLL;
        private const int _employeeId = 4067;
        private const string _filePath = "Admin\\Feature";
        private const string _name = "{\"default\":\"NewFeature1\",\"ar\":\"فتشير جديد1\"}";
        private const string _updateName = "{\"default\":\"NewFeature2\",\"ar\":\"فتشير جديد2\"}";
        private const string _description = "{\"default\":\"FeatureDesc1\",\"ar\":\"وصف الفيتشر1\"}";
        private const string _updateDescription = "{\"default\":\"FeatureDesc2\",\"ar\":\"وصف الفيتشر2\"}";
        public FeatureTest()
        {
            _featureBLL = serviceProvider.GetRequiredService<IFeatureBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetFatures_CheckFilterIfExisit_ReturnFeatures(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var features =await _featureBLL.GetPagedListAsync(filter);
            //Assert
            features.IsSuccess.ShouldBeTrue();
            features.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateFeature_AddNewFeatureWithCorrectData_ReturnFeature()
        {
            //Arrange
            var param = new CreateFeatureInputDto()
            {
                Name = _name,
                Description = _description,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
            };
            //Action
            var feature = await _featureBLL.CreateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeTrue();
            feature.Data.ShouldNotBeNull();
            feature.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewFeatureWithoutCreatedBy_ReturnCannotCreateFeatureWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateFeatureInputDto()
            {
                Name = _name,
                Description = _description,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
            };
            //Action
            var feature = await _featureBLL.CreateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeFalse();
            feature.Data.ShouldBeNull();
            feature.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewFeatureWithoutImage_ReturnCannotCreateFeatureWithoutImage()
        {
            //Arrange
            var param = new CreateFeatureInputDto()
            {
                Name = _name,
                Description = _description,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                File = null,
            };
            //Action
            var feature = await _featureBLL.CreateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeFalse();
            feature.Data.ShouldBeNull();
            feature.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateNameIsUnique_AddNewFeatureWithNameExisitInDB_ReturnCanNotCreateFeatureAlreadyExisit()
        {
            //Arrange
            var param = new CreateFeatureInputDto()
            {
                Name = _name,
                Description = _description,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
            };
            //Action
            var feature = await _featureBLL.CreateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeFalse();
            feature.Data.ShouldBeNull();
            feature.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateFeature_UpdateFeatureWithCorrectData_ReturnFeatureUpdated()
        {
            //Arrange
            var param = new UpdateFeatureInputDto()
            {
                Name = _updateName,
                Description = _updateDescription,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
                Id = 5077
            };
            //Action
            var feature = await _featureBLL.UpdateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeTrue();
            feature.Data.ShouldNotBeNull();
            feature.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task UpdateFeature_UpdateFeatureWithNameAlreadyExisit_ReturnThisFeatureAlreadyExisit()
        {
            //Arrange
            var param = new UpdateFeatureInputDto()
            {
                Name = _updateName,
                Description = _updateDescription,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
                Id = 5076
            };
            //Action
            var feature = await _featureBLL.UpdateAsync(param);
            //Assert
            feature.IsSuccess.ShouldBeFalse();
            feature.Data.ShouldBeNull();
            feature.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteFeatureNotFound_TakeFeatureIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = await _featureBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteFeature_DeleteFeatureExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 5077,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _featureBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteFeatureRelatedData_TakeFeatureIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 5062,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _featureBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion


    }
}
