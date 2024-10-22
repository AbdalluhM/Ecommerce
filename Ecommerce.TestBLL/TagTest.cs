using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Tags;
using MyDexef.DTO;
using MyDexef.DTO.Paging;
using MyDexef.DTO.Tags;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class TagTest : UnitTestBase
    {
        private readonly ITagBLL _tagBLL;
        private const int _employeeId = 4067;
        private const string _name = "{\"default\":\"NewTag1\",\"ar\":\"تاج جديد1\"}";
        private const string _updateName = "{\"default\":\"NewTag2\",\"ar\":\"تاج جديد2\"}";
        public TagTest()
        {
            _tagBLL = serviceProvider.GetRequiredService<ITagBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public void GetTags_CheckFilterIfExisit_ReturnTags(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Tags =  _tagBLL.GetPagedTagList(filter);
            //Assert
            Tags.IsSuccess.ShouldBeTrue();
            Tags.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public void CreateTag_AddNewTagWithCorrectData_ReturnTag()
        {
            //Arrange
            var param = new CreateTagInputDto()
            {
                Name = _name,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tag =  _tagBLL.Create(param);
            //Assert
            Tag.IsSuccess.ShouldBeTrue();
            Tag.Data.ShouldNotBeNull();
            Tag.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public void ValidateFieldsIsRequired_AddNewTagWithoutCreatedBy_ReturnCannotCreateTagWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateTagInputDto()
            {
                Name = _name,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tag =  _tagBLL.Create(param);
            //Assert
            Tag.IsSuccess.ShouldBeFalse();
            Tag.Data.ShouldBeNull();
            Tag.Errors.Count.ShouldBeGreaterThan(0);
        }
        
        [TestMethod]
        public void ValidateNameIsUnique_AddNewTagWithNameExisitInDB_ReturnCanNotCreateTagAlreadyExisit()
        {
            //Arrange
            var param = new CreateTagInputDto()
            {
                Name = _name,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
            };
            //Action
            var Tag =  _tagBLL.Create(param);
            //Assert
            Tag.IsSuccess.ShouldBeFalse();
            Tag.Data.ShouldBeNull();
            Tag.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public void UpdateTag_UpdateTagWithCorrectData_ReturnTagUpdated()
        {
            //Arrange
            var param = new UpdateTagInputDto()
            {
                Name = _updateName,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                Id = 3117
            };
            //Action
            var Tag =  _tagBLL.Update(param);
            //Assert
            Tag.IsSuccess.ShouldBeTrue();
            Tag.Data.ShouldNotBeNull();
            Tag.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public void UpdateTag_UpdateTagWithNameAlreadyExisit_ReturnThisTagAlreadyExisit()
        {
            //Arrange
            var param = new UpdateTagInputDto()
            {
                Name = _updateName,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                Id = 3116
            };
            //Action
            var Tag =  _tagBLL.Update(param);
            //Assert
            Tag.IsSuccess.ShouldBeFalse();
            Tag.Data.ShouldBeNull();
            Tag.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteTagNotFound_TakeTagIdAndDeleteFromDb_ReturnBool()
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
            var IsDelted = _tagBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public void DeleteTag_DeleteTagExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = 3117,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _tagBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public void DeleteTagRelatedData_TakeTagIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 14,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = _tagBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
