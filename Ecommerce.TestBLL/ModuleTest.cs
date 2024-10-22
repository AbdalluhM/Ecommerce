using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Modules;
using MyDexef.DTO;
using MyDexef.DTO.Modules;
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
    public class ModuleTest : UnitTestBase
    {
        private readonly IModuleBLL _moduleBLL;
        private const int _employeeId = 4067;
        private const string _filePath = "Admin\\Module";
        private const string _name = "{\"default\":\"NewModule1\",\"ar\":\"موديل جديد1\"}";
        private const string _updateName = "{\"default\":\"NewModel2\",\"ar\":\"موديل جديد2\"}";
        private const string _Title = "{\"default\":\"ModelTitle1\",\"ar\":\"عنوان الموديل\"}";
        private const string _updateTitle = "{\"default\":\"ModelTitle2\",\"ar\":\"عنوان الموديل\"}";
        private const string _shortDesc = "{\"default\":\"shortDescription\",\"ar\":\"وصف صغير\"}";
        private const string _longDesc = "{\"default\":\"longDescription\",\"ar\":\"وصف كبير\"}";
        private const string _mainPageUrl = "www.ModuleUrl.com";
        public ModuleTest()
        {
            _moduleBLL = serviceProvider.GetRequiredService<IModuleBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetModules_CheckFilterIfExisit_ReturnModules(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Modules = await _moduleBLL.GetPagedListAsync(filter);
            //Assert
            Modules.IsSuccess.ShouldBeTrue();
            Modules.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateModule_AddNewModuleWithCorrectData_ReturnModule()
        {
            //Arrange
            var param = new CreateModuleInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
            };
            //Action
            var Module = await _moduleBLL.CreateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeTrue();
            Module.Data.ShouldNotBeNull();
            Module.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewModuleWithoutCreatedBy_ReturnCannotCreateModuleWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateModuleInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
            };
            //Action
            var Module = await _moduleBLL.CreateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeFalse();
            Module.Data.ShouldBeNull();
            Module.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewModuleWithoutImage_ReturnCannotCreateModuleWithoutImage()
        {
            //Arrange
            var param = new CreateModuleInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = null,
            };
            //Action
            var Module = await _moduleBLL.CreateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeFalse();
            Module.Data.ShouldBeNull();
            Module.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateNameIsUnique_AddNewModuleWithNameExisitInDB_ReturnCanNotCreateModuleAlreadyExisit()
        {
            //Arrange
            var param = new CreateModuleInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
            };
            //Action
            var Module = await _moduleBLL.CreateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeFalse();
            Module.Data.ShouldBeNull();
            Module.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateModule_UpdateModuleWithCorrectData_ReturnModuleUpdated()
        {
            //Arrange
            var param = new UpdateModuleInputDto()
            {
                Name = _name,
                Title = _Title,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
                Id = 6
            };
            //Action
            var Module = await _moduleBLL.UpdateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeTrue();
            Module.Data.ShouldNotBeNull();
            Module.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task UpdateModule_UpdateModuleWithNameAlreadyExisit_ReturnThisModuleAlreadyExisit()
        {
            //Arrange
            var param = new UpdateModuleInputDto()
            {
                Name = _updateName,
                Title = _updateTitle,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                File = GetFormFile(_filePath),
                Id = 7
            };
            //Action
            var Module = await _moduleBLL.UpdateAsync(param);
            //Assert
            Module.IsSuccess.ShouldBeFalse();
            Module.Data.ShouldBeNull();
            Module.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteModuleNotFound_TakeModuleIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = await _moduleBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteModule_DeleteModuleExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 7,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _moduleBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteModuleRelatedData_TakeModuleIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 6,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _moduleBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
