using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.LookUps;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class LookUpTest : UnitTestBase
    {
        private readonly ILookUpBLL _lookUpBLL;
        public LookUpTest()
        {
            _lookUpBLL = serviceProvider.GetRequiredService<ILookUpBLL>();
        }
        [TestMethod]
        public void  GetCountries_AllCountriesOnSystem_ReturnCountries()
        {
            //Arrange

            //Actions
            var countries =  _lookUpBLL.GetAllCountries();
            //Assert
            countries.IsSuccess.ShouldBeTrue();
            countries.Data.ShouldNotBeNull();
            countries.Errors.Count.ShouldBeLessThanOrEqualTo(0);
            countries.Data.Count.ShouldBeGreaterThan(0);

        }
        [TestMethod]
        public void GetCurrencies_AllCurrenciesOnSystem_ReturnCurrencies()
        {
            //Arrange

            //Actions
            var currencies = _lookUpBLL.GetAllCurrencies();
            //Assert
            currencies.IsSuccess.ShouldBeTrue();
            currencies.Data.ShouldNotBeNull();
            currencies.Errors.Count.ShouldBeLessThanOrEqualTo(0);
            currencies.Data.Count.ShouldBeGreaterThan(0);

        }
        [TestMethod]
        public async Task GetDashboardCounts_CalculateCountofAllPagesOnSystem_ReturnDashBoardCounts()
        {
            //Arrange

            //Actions
            var dashboardCount =await _lookUpBLL.GetAllDashboardCounts();
            //Assert
            dashboardCount.IsSuccess.ShouldBeTrue();
            dashboardCount.Data.ShouldNotBeNull();
            dashboardCount.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task GetSubscriptionType_VersionSubscriptionType_ReturnSubscriptionType()
        {
            //Arrange

            //Actions
            var subscriptionType = await _lookUpBLL.GetAllSubscriptionTypesAsync();
            //Assert
            subscriptionType.IsSuccess.ShouldBeTrue();
            subscriptionType.Data.ShouldNotBeNull();
            subscriptionType.Errors.Count.ShouldBeLessThanOrEqualTo(0);
            subscriptionType.Data.Count.ShouldBeGreaterThan(0);
        }
    }
}
