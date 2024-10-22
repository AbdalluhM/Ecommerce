using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MyDexef.BLL.Applications.Versions;
using MyDexef.DTO;
using MyDexef.DTO.Applications;
using MyDexef.DTO.Applications.ApplicationVersions;
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
    public class VersionTest : UnitTestBase
    {
        private readonly IVersionBLL _versionBLL;
        private const int _employeeId = 4067;
        private const int _applicationId = 4009;
        private const int _versionId = 5042;
        private const int _versionId2 = 5041;
        private const int _versionPriceId = 5069;
        private const int _foreverPrice = 5000;
        private const int _yearlyPrice = 1000;
        private const int _monthlyPrice = 200;
        private const int _priceLevel = 3065;
        private const int _discount = 5;
        private const int _countryCurrency = 5073;
        private const string _releaseNumber = "1.2.3";
        private const string _filePath = "Admin\\Application\\Version";
        private const string _name = "{\"default\":\"NewVersion1\",\"ar\":\"فيرجين جديد1\"}";
        private const string _updateName = "{\"default\":\"NewVersion2\",\"ar\":\"فيرجين جديد2\"}";
        private const string _Title = "{\"default\":\"VersionTitle1\",\"ar\":\"عنوان الفيرجين\"}";
        private const string _updateTitle = "{\"default\":\"VersionTitle2\",\"ar\":\"عنوان الفيرجين\"}";
        private const string _shortDesc = "{\"default\":\"shortDescription\",\"ar\":\"وصف صغير\"}";
        private const string _longDesc = "{\"default\":\"longDescription\",\"ar\":\"وصف كبير\"}";
        private const string _mainPageUrl = "www.VersionMainUrl.com";
        private const string _downloadUrl = "www.VersionDownUrl.com";
        public VersionTest( )
        {
            _versionBLL = serviceProvider.GetRequiredService<IVersionBLL>();
        }


        #region Version
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetVersions_CheckFilterIfExisit_ReturnVersions( string value )
        {
            //Arrange
            var filter = new ApplicationFilteredPagedResult() { SearchTerm = value };
            //Action
            var Versions = await _versionBLL.GetPagedListAsync(filter);
            //Assert
            Versions.IsSuccess.ShouldBeTrue();
            Versions.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #region Create
        [TestMethod]
        public async Task CreateVersion_AddNewVersionWithCorrectData_ReturnVersion( )
        {
            //Arrange
            var param = new CreateVersionInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = false,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                ApplicationId = _applicationId,
                DownloadUrl = _downloadUrl,
                IsHighlightedVersion = false,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                File = GetFormFile(_filePath),
            };
            //Action
            var Version = await _versionBLL.CreateAsync(param);
            //Assert
            Version.IsSuccess.ShouldBeTrue();
            Version.Data.ShouldNotBeNull();
            Version.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }

        [TestMethod]
        public async Task CreateVersionCheckedMaxActiveVersion_AddNewVersionActive_ReturnMaxActiveVersionMustBe3( )
        {
            //Arrange
            var param = new CreateVersionInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = false,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                ApplicationId = _applicationId,
                DownloadUrl = _downloadUrl,
                IsHighlightedVersion = true,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                File = GetFormFile(_filePath),
            };
            //Action
            var Version = await _versionBLL.CreateAsync(param);
            Version.IsSuccess.ShouldBeFalse();
            Version.Data.ShouldBeNull();
            Version.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task CreateVersionCheckedActiveButNotHighlighted_AddNewVersionActiveNotHighlighted_ReturnVersionMustBeHighlighted( )
        {
            //Arrange
            var param = new CreateVersionInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                ApplicationId = _applicationId,
                DownloadUrl = _downloadUrl,
                IsHighlightedVersion = false,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                File = GetFormFile(_filePath),
            };
            //Action
            var Version = await _versionBLL.CreateAsync(param);
            Version.IsSuccess.ShouldBeFalse();
            Version.Data.ShouldBeNull();
            Version.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        [DataRow("", _employeeId, _releaseNumber)]
        [DataRow(_name, 0, _releaseNumber)]
        [DataRow(_name, _employeeId, "")]
        public async Task ValidateFieldsIsRequired_AddNewVersionWithoutFieldsRequired_ReturnFieldsIsRequired( string name, int employeeId, string releaseNumber )
        {
            //Arrange
            var param = new CreateVersionInputDto()
            {
                Name = name,
                Title = _Title,
                CreatedBy = employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = false,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                ApplicationId = 1,
                DownloadUrl = _downloadUrl,
                IsHighlightedVersion = false,
                ProductCrmId = "",
                ReleaseNumber = releaseNumber,
                File = GetFormFile(_filePath),
            };
            //Action
            var Version = await _versionBLL.CreateAsync(param);
            //Assert
            Version.IsSuccess.ShouldBeFalse();
            Version.Data.ShouldBeNull();
            Version.Errors.Count.ShouldBeGreaterThan(0);
        }
        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewVersionWithoutImage_ReturnCannotCreateVersionWithoutImage( )
        {
            //Arrange
            var param = new CreateVersionInputDto()
            {
                Name = _name,
                Title = _Title,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                ApplicationId = 1,
                DownloadUrl = _downloadUrl,
                IsHighlightedVersion = true,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                File = GetFormFile(_filePath),
            };
            //Action
            var Version = await _versionBLL.CreateAsync(param);
            //Assert
            Version.IsSuccess.ShouldBeFalse();
            Version.Data.ShouldBeNull();
            Version.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateVersion_UpdateVersionWithCorrectData_ReturnVersionUpdated( )
        {
            //Arrange
            var param = new UpdateVersionInputDto()
            {
                Name = _updateName,
                Title = _updateTitle,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = false,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                IsHighlightedVersion = false,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                DownloadUrl = _downloadUrl,
                ApplicationId = _applicationId,
                File = GetFormFile(_filePath),
                Id = _versionId
            };
            //Action
            var Version = await _versionBLL.UpdateAsync(param);
            //Assert
            Version.IsSuccess.ShouldBeTrue();
            Version.Data.ShouldNotBeNull();
            Version.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task CheckMaxActiveVersionInUpdate_UpdateVersionMakeActive_ReturnMaxActiveVersionMustBe3( )
        {
            //Arrange
            var param = new UpdateVersionInputDto()
            {
                Name = _updateName,
                Title = _updateTitle,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                ShortDescription = _shortDesc,
                LongDescription = _longDesc,
                MainPageUrl = _mainPageUrl,
                IsHighlightedVersion = true,
                ProductCrmId = "",
                ReleaseNumber = _releaseNumber,
                DownloadUrl = _downloadUrl,
                ApplicationId = _applicationId,
                File = GetFormFile(_filePath),
                Id = _versionId
            };
            //Action
            var Version = await _versionBLL.UpdateAsync(param);
            //Assert
            Version.IsSuccess.ShouldBeFalse();
            Version.Data.ShouldBeNull();
            Version.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteVersionNotFound_TakeVersionIdAndDeleteFromDb_ReturnBool( )
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
            var IsDelted = await _versionBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteVersion_DeleteVersionExisitInDb_ReturnBoolean( )
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = _versionId,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _versionBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteVersionRelatedData_TakeVersionIdAndDeleteFromDb_ReturnBool( )
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                //id country related data
                Id = 5034,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _versionBLL.DeleteAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
        #endregion

        #region VersionPrice
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetVersionPrices_CheckFilterIfExisit_ReturnVersionPrices( string value )
        {
            //Arrange
            var filter = new ApplicationFilteredPagedResult() { SearchTerm = value };
            //Action
            var VersionPrices = await _versionBLL.GetAllExistingVersionPricePagedListAsync(filter, _employeeId);
            //Assert
            VersionPrices.IsSuccess.ShouldBeTrue();
            VersionPrices.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #region Create
        [TestMethod]
        public async Task CreateVersionPrice_AddNewVersionPriceForEverWithCorrectData_ReturnVersionPrice( )
        {
            //Arrange
            var param = new CreateVersionPriceInputDto()
            {
                CreatedBy = _employeeId,
                VersionId = _versionId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                CountryCurrencyId = _countryCurrency,
                ForeverPrice = _foreverPrice,
                ForeverPrecentageDiscount = _discount,
                ForeverNetPrice = _foreverPrice - _foreverPrice * _discount / 100,
                PriceLevelId = _priceLevel,
            };
            //Action
            var VersionPrice = await _versionBLL.CreateVersionPriceAsync(param);
            //Assert
            VersionPrice.IsSuccess.ShouldBeTrue();
            VersionPrice.Data.ShouldNotBeNull();
            VersionPrice.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task CreateVersionPrice_AddNewVersionPriceWithCorrectData_ReturnVersionPrice( )
        {
            //Arrange
            var param = new CreateVersionPriceInputDto()
            {
                CreatedBy = _employeeId,
                VersionId = _versionId2,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                CountryCurrencyId = _countryCurrency,
                MonthlyPrice = _monthlyPrice,
                MonthlyPrecentageDiscount = _discount,
                MonthlyNetPrice = _monthlyPrice - _monthlyPrice * _discount / 100,
                PriceLevelId = _priceLevel,
                YearlyPrice = _yearlyPrice,
                YearlyPrecentageDiscount = _discount,
                YearlyNetPrice = _yearlyPrice - _yearlyPrice * _discount / 100,
            };
            //Action
            var VersionPrice = await _versionBLL.CreateVersionPriceAsync(param);
            //Assert
            VersionPrice.IsSuccess.ShouldBeTrue();
            VersionPrice.Data.ShouldNotBeNull();
            VersionPrice.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task CreateVersionPrice_AddNewVersionPriceWithAlreadyExisit_ReturnVersionPriceAlreadyExisit( )
        {
            //Arrange
            var param = new CreateVersionPriceInputDto()
            {
                CreatedBy = _employeeId,
                VersionId = _versionId2,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                CountryCurrencyId = _countryCurrency,
                MonthlyPrice = _monthlyPrice,
                MonthlyPrecentageDiscount = _discount,
                MonthlyNetPrice = _monthlyPrice - _monthlyPrice * _discount / 100,
                PriceLevelId = _priceLevel,
                YearlyPrice = _yearlyPrice,
                YearlyPrecentageDiscount =  _discount,
                YearlyNetPrice = _yearlyPrice - _yearlyPrice * _discount / 100,
            };
            //Action
            var VersionPrice = await _versionBLL.CreateVersionPriceAsync(param);
            //Assert
            VersionPrice.IsSuccess.ShouldBeFalse();
            VersionPrice.Data.ShouldBeNull();
            VersionPrice.Errors.Count.ShouldBeGreaterThan(0);
        }


        [TestMethod]
        [DataRow(0, _employeeId, _countryCurrency)]
        [DataRow(_foreverPrice, 0, _countryCurrency)]
        [DataRow(_foreverPrice, _employeeId, 0)]
        public async Task ValidateFieldsIsRequired_AddNewVersionPriceWithoutFieldsRequired_ReturnFieldsIsRequired( int forEverPrice, int employeeId, int countryCurrency )
        {
            //Arrange
            var param = new CreateVersionPriceInputDto()
            {
                CreatedBy = employeeId,
                VersionId = _versionId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                CountryCurrencyId = countryCurrency,
                ForeverPrice = forEverPrice,
                ForeverPrecentageDiscount = _discount,
                ForeverNetPrice = forEverPrice - forEverPrice * _discount / 100,
                PriceLevelId = _priceLevel,
            };
            //Action
            var VersionPrice = await _versionBLL.CreateVersionPriceAsync(param);
            //Assert
            VersionPrice.IsSuccess.ShouldBeFalse();
            VersionPrice.Data.ShouldBeNull();
            VersionPrice.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateVersionPrice_UpdateVersionPriceWithCorrectData_ReturnVersionPriceUpdated( )
        {
            //Arrange
            var param = new UpdateVersionPriceInputDto()
            {
                VersionId = _versionId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                CountryCurrencyId = _countryCurrency,
                MonthlyPrice = _monthlyPrice,
                MonthlyPrecentageDiscount = _discount,
                MonthlyNetPrice = _monthlyPrice - _monthlyPrice * _discount / 100,
                PriceLevelId = _priceLevel,
                YearlyPrice = _yearlyPrice,
                YearlyPrecentageDiscount = _discount,
                YearlyNetPrice = _yearlyPrice - _yearlyPrice * _discount / 100,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                Id = _versionPriceId
            };
            //Action
            var VersionPrice = await _versionBLL.UpdateVersionPriceAsync(param);
            //Assert
            VersionPrice.IsSuccess.ShouldBeTrue();
            VersionPrice.Data.ShouldNotBeNull();
            VersionPrice.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteVersionPriceNotFound_TakeVersionPriceIdAndDeleteFromDb_ReturnBool( )
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
            var IsDelted = await _versionBLL.DeleteVersionPriceAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteVersionPrice_DeleteVersionPriceExisitInDb_ReturnBoolean( )
        {
            //Assert
            var param = new DeleteTrackedEntityInputDto()
            {
                Id = _versionPriceId,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
            };
            //Arrange
            var IsDelted = await _versionBLL.DeleteVersionPriceAsync(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        #endregion
        #endregion
    }
}
