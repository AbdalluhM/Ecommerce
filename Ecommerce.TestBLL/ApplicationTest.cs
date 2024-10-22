using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Applications;
using MyDexef.DTO;
using MyDexef.DTO.Application;
using MyDexef.DTO.Applications;
using MyDexef.DTO.Applications.ApplicationLabels;
using MyDexef.DTO.Applications.ApplicationSlider;
using MyDexef.DTO.Applications.ApplicationTags;
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
    public class ApplicationTest : UnitTestBase
    {
        private readonly IApplicationBLL _applicationBLL;
        private const int _employeeId = 4067;
        private const int _applicationId = 4009;
        private const int _tagId = 3121;
        private const int _applicationTagId = 6;
        private const int _updatedTagId = 17;
        private const int _applicationSliderId = 3033;
        private const string _color = "Red";
        private const string _filePath = "Admin\\Application";
        private const string _name = "{\"default\":\"NewApplication1\",\"ar\":\"ابلكيشن جديد1\"}";
        private const string _updateName = "{\"default\":\"NewApplication2\",\"ar\":\"ابلكيشن جديد2\"}";
        private const string _Title = "{\"default\":\"ApplicationTitle1\",\"ar\":\"عنوان الابلكيشن\"}";
        private const string _updateTitle = "{\"default\":\"ApplicationTitle2\",\"ar\":\"عنوان الابلكيشن\"}";
        private const string _shortDesc = "{\"default\":\"shortDescription\",\"ar\":\"وصف صغير\"}";
        private const string _longDesc = "{\"default\":\"longDescription\",\"ar\":\"وصف كبير\"}";
        private const string _mainPageUrl = "www.ApplicationUrl.com";
        private const string _applicationLabelName = "{\"default\":\"label\",\"ar\":\"ليبل1\"}";
        private const string _applicationLabelNameUpdated = "{\"default\":\"Updatedlabel\",\"ar\":\"ليبل محدث1\"}";
        public ApplicationTest()
        {
            _applicationBLL = serviceProvider.GetRequiredService<IApplicationBLL>();
        }


        #region ApplicationCrudOperation
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetApplications_CheckFilterIfExisit_ReturnApplications(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Applications = await _applicationBLL.GetPagedListAsync(filter);
            //Assert
            Applications.IsSuccess.ShouldBeTrue();
            Applications.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #region Create
        [TestMethod]
        public async Task CreateApplication_AddNewApplicationWithCorrectData_ReturnApplication()
        {
            //Arrange
            var param = new CreateApplicationInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                SubscriptionTypeId = 1,
                File = GetFormFile(_filePath),
            };
            //Action
            var Application = await _applicationBLL.CreateAsync(param);
            //Assert
            Application.IsSuccess.ShouldBeTrue();
            Application.Data.ShouldNotBeNull();
            Application.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        [DataRow("", _employeeId)]
        [DataRow(_name, 0)]
        public async Task ValidateFieldsIsRequired_AddNewApplicationWithoutFieldsRequired_ReturnFieldsIsRequired(string name, int employeeId)
        {
            //Arrange
            var param = new CreateApplicationInputDto()
            {
                Name = name,
                Title = _Title,
                CreatedBy = employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
            };
            //Action
            var Application = await _applicationBLL.CreateAsync(param);
            //Assert
            Application.IsSuccess.ShouldBeFalse();
            Application.Data.ShouldBeNull();
            Application.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewApplicationWithoutImage_ReturnCannotCreateApplicationWithoutImage()
        {
            //Arrange
            var param = new CreateApplicationInputDto()
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
            var Application = await _applicationBLL.CreateAsync(param);
            //Assert
            Application.IsSuccess.ShouldBeFalse();
            Application.Data.ShouldBeNull();
            Application.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateApplication_UpdateApplicationWithCorrectData_ReturnApplicationUpdated()
        {
            //Arrange
            var param = new UpdateApplicationInputDto()
            {
                Name = _updateName,
                Title = _updateTitle,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                File = GetFormFile(_filePath),
                SubscriptionTypeId = 2,
                Id = 4020
            };
            //Action
            var Application = await _applicationBLL.UpdateAsync(param);
            //Assert
            Application.IsSuccess.ShouldBeTrue();
            Application.Data.ShouldNotBeNull();
            Application.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteApplicationNotFound_TakeApplicationIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = await _applicationBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteApplication_DeleteApplicationExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 4019,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _applicationBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteApplicationRelatedData_TakeApplicationIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 4009,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _applicationBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
        #endregion

        #region ApplicationLabel
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetApplicationLabels_CheckFilterIfExisit_ReturnApplicationLabels(string value)
        {
            //Arrange
            var filter = new ApplicationFilteredPagedResult() { SearchTerm = value , ApplicationId = _applicationId};
            //Action
            var ApplicationLabels =await _applicationBLL.GetApplicationLabelPagedListAsync(filter);
            //Assert
            ApplicationLabels.IsSuccess.ShouldBeTrue();
            ApplicationLabels.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateApplicationLabel_AddNewApplicationLabelWithCorrectData_ReturnApplicationLabel()
        {
            //Arrange
            var param = new CreateApplicationLabelInputDto()
            {
                Name = _applicationLabelName,
                Color = _color,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationLabel =await _applicationBLL.CreateApplicationLabelAsync(param);
            //Assert
            ApplicationLabel.IsSuccess.ShouldBeTrue();
            ApplicationLabel.Data.ShouldNotBeNull();
            ApplicationLabel.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewApplicationLabelWithoutCreatedBy_ReturnCannotCreateApplicationLabelWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateApplicationLabelInputDto()
            {
                Name = _applicationLabelName,
                Color = _color,
                ApplicationId = _applicationId,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationLabel =await _applicationBLL.CreateApplicationLabelAsync(param);
            //Assert
            ApplicationLabel.IsSuccess.ShouldBeFalse();
            ApplicationLabel.Data.ShouldBeNull();
            ApplicationLabel.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public async Task ValidateNameIsUnique_AddNewApplicationLabelWithNameExisitInDB_ReturnCanNotCreateApplicationLabelAlreadyExisit()
        {
            //Arrange
            var param = new CreateApplicationLabelInputDto()
            {
                Name = _applicationLabelName,
                Color = _color,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationLabel =await _applicationBLL.CreateApplicationLabelAsync(param);
            //Assert
            ApplicationLabel.IsSuccess.ShouldBeFalse();
            ApplicationLabel.Data.ShouldBeNull();
            ApplicationLabel.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateApplicationLabel_UpdateApplicationLabelWithCorrectData_ReturnApplicationLabelUpdated()
        {
            //Arrange
            var param = new UpdateApplicationLabelInputDto()
            {
                Name = _applicationLabelNameUpdated,
                Color = _color,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                ApplicationId = _applicationId
            };
            //Action
            var ApplicationLabel =await _applicationBLL.UpdateApplicationLabelAsync(param);
            //Assert
            ApplicationLabel.IsSuccess.ShouldBeTrue();
            ApplicationLabel.Data.ShouldNotBeNull();
            ApplicationLabel.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteApplicationLabelNotFound_TakeApplicationLabelIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted =await _applicationBLL.DeleteApplicationLabelAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteApplicationLabel_DeleteApplicationLabelExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = _applicationId,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted =await _applicationBLL.DeleteApplicationLabelAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        #endregion
        #endregion

        #region AppplicationTag
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetApplicationTags_CheckFilterIfExisit_ReturnApplicationTags(string value)
        {
            //Arrange
            var filter = new ApplicationFilteredPagedResult() { SearchTerm = value, ApplicationId = _applicationId };
            //Action
            var ApplicationTags = await _applicationBLL.GetApplicationTagPagedListAsync(filter);
            //Assert
            ApplicationTags.IsSuccess.ShouldBeTrue();
            ApplicationTags.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateApplicationTag_AddNewApplicationTagWithCorrectData_ReturnApplicationTag()
        {
            //Arrange
            var param = new CreateApplicationTagInputDto()
            {
                TagId = _tagId,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationTag = await _applicationBLL.CreateApplicationTagAsync(param);
            //Assert
            ApplicationTag.IsSuccess.ShouldBeTrue();
            ApplicationTag.Data.ShouldNotBeNull();
            ApplicationTag.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewApplicationTagWithoutCreatedBy_ReturnCannotCreateApplicationTagWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateApplicationTagInputDto()
            {
                TagId = _tagId,
                ApplicationId = _applicationId,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationTag = await _applicationBLL.CreateApplicationTagAsync(param);
            //Assert
            ApplicationTag.IsSuccess.ShouldBeFalse();
            ApplicationTag.Data.ShouldBeNull();
            ApplicationTag.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public async Task ValidateIaAlreadyExisit_AddNewApplicationTagExisitInDB_ReturnCanNotCreateApplicationTagAlreadyExisit()
        {
            //Arrange
            var param = new CreateApplicationTagInputDto()
            {
                TagId = _tagId,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationTag = await _applicationBLL.CreateApplicationTagAsync(param);
            //Assert
            ApplicationTag.IsSuccess.ShouldBeFalse();
            ApplicationTag.Data.ShouldBeNull();
            ApplicationTag.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateApplicationTag_UpdateApplicationTagWithCorrectData_ReturnApplicationTagUpdated()
        {
            //Arrange
            var param = new UpdateApplicationTagInputDto()
            {
                Id = _applicationTagId,
                TagId = _updatedTagId,
                ApplicationId = _applicationId,
                IsFeatured = true
            };
            //Action
            var ApplicationTag = await _applicationBLL.UpdateApplicationTagAsync(param);
            //Assert
            ApplicationTag.IsSuccess.ShouldBeTrue();
            ApplicationTag.Data.ShouldNotBeNull();
            ApplicationTag.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteApplicationTagNotFound_TakeApplicationTagIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = await _applicationBLL.DeleteApplicationTagAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteApplicationTag_DeleteApplicationTagExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = _applicationTagId,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _applicationBLL.DeleteApplicationTagAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        #endregion
        #endregion

        #region AppplicationSider
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetApplicationTags_CheckFilterIfExisit_ReturnApplicationSliders(string value)
        {
            //Arrange
            var filter = new ApplicationFilteredPagedResult() { SearchTerm = value, ApplicationId = _applicationId };
            //Action
            var ApplicationSliders = await _applicationBLL.GetAllApplicationSlidersPagedListAsync(filter);
            //Assert
            ApplicationSliders.IsSuccess.ShouldBeTrue();
            ApplicationSliders.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateApplicationSlider_AddNewApplicationSliderWithCorrectData_ReturnApplicationSlider()
        {
            //Arrange
            var param = new CreateApplicationSliderInputDto()
            {
                File = GetFormFile($"{_filePath}\\Slider"),
                IsActive = true,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                 
            };
            //Action
            var ApplicationSlider = await _applicationBLL.CreateApplicationSliderAsync(param);
            //Assert
            ApplicationSlider.IsSuccess.ShouldBeTrue();
            ApplicationSlider.Data.ShouldNotBeNull();
            ApplicationSlider.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewApplicationSliderWithoutCreatedBy_ReturnCannotCreateApplicationSliderWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateApplicationSliderInputDto()
            {
                File = GetFormFile($"{_filePath}\\Slider"),
                IsActive = true,
                ApplicationId = _applicationId,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationSlider = await _applicationBLL.CreateApplicationSliderAsync(param);
            //Assert
            ApplicationSlider.IsSuccess.ShouldBeFalse();
            ApplicationSlider.Data.ShouldBeNull();
            ApplicationSlider.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public async Task ValidateFileIsExisit_AddNewApplicationSliderWithoutImage_ReturnCanNotCreateApplicationSliderWithoutFile()
        {
            //Arrange
            var param = new CreateApplicationSliderInputDto()
            {
                File = null,
                IsActive = true,
                ApplicationId = _applicationId,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
            };
            //Action
            var ApplicationSlider = await _applicationBLL.CreateApplicationSliderAsync(param);
            //Assert
            ApplicationSlider.IsSuccess.ShouldBeFalse();
            ApplicationSlider.Data.ShouldBeNull();
            ApplicationSlider.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        //[TestMethod]
        //public async Task UpdateApplicationSlider_UpdateApplicationSliderWithCorrectData_ReturnApplicationSliderUpdated()
        //{
        //    //Arrange
        //    var param = new UpdateApplicationSliderInputDto()
        //    {
        //        Id = _applicationSliderId,
        //        ApplicationId = _applicationId,
        //        ModifiedBy = _employeeId , 
        //        ModifiedDate = DateTime.UtcNow,
        //        IsActive = true,
        //        File = GetFormFile($"{_filePath}\\Slider")
        //    };
        //    //Action
        //    var ApplicationSlider = await _applicationBLL.UpdateApplicationSliderAsync(param);
        //    //Assert
        //    ApplicationSlider.IsSuccess.ShouldBeTrue();
        //    ApplicationSlider.Data.ShouldNotBeNull();
        //    ApplicationSlider.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        //}
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteApplicationSliderNotFound_TakeApplicationSliderIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = await _applicationBLL.DeleteApplicationSliderAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteApplicationSlider_DeleteApplicationSliderExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = _applicationSliderId,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _applicationBLL.DeleteApplicationSliderAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        #endregion
        #endregion
    }
}
