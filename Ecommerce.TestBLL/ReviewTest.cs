using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Customers.Reviews.Admins;
using MyDexef.Core.Enums.Reviews;
using MyDexef.DTO.Customers.Review.Admins;
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
    public class ReviewTest : UnitTestBase
    {
        private readonly IAdminReviewBLL _adminReviewBLL;
        private const int _employeeId = 4067;
        private const int _statusId = (int)ReviewStatusEnum.Confirmed;
        public ReviewTest()
        {
            _adminReviewBLL = serviceProvider.GetRequiredService<IAdminReviewBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetReviews_CheckFilterIfExisit_ReturnReviews(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Reviews =await _adminReviewBLL.GetPagedListAsync(filter);
            //Assert
            Reviews.IsSuccess.ShouldBeTrue();
            Reviews.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Submit
        [TestMethod]
        public async Task SubmitReview_AddNewReviewWithCorrectData_ReturnReview()
        {
            //Arrange
            var param = new SubmitCustomerReviewInputDto()
            {
                Id = 4028,
                StatusId =(int) ReviewStatusEnum.Confirmed,
                ApprovedBy = _employeeId,
                ApprovedDate = DateTime.UtcNow,
            };
            //Action
            var Review =await _adminReviewBLL.SubmitAsync(param);
            //Assert
            Review.IsSuccess.ShouldBeTrue();
            Review.Data.ShouldNotBeNull();
            Review.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #endregion


        #region Delete
        [TestMethod]
        public void DeleteReviewNotFound_TakeReviewIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            var param = new DeleteCustomerReviewInputDto()
            {
                //id country not found
                Id = 1000000000,
            };
            //Arrange
            var IsDelted = _adminReviewBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public void DeleteReview_DeleteReviewExisitInDb_ReturnBoolean()
        {
            //Assert
            var param = new DeleteCustomerReviewInputDto()
            {
                Id = 5278,
            };
            //Arrange
            var IsDelted = _adminReviewBLL.Delete(param);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        #endregion
    }
}
