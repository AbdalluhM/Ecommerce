using FluentAssertions.Common;

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
    public class AdminLookupBLLTest : AdminTestBase
    {

        private readonly ILookUpBLL _lookupBLL;
        public AdminLookupBLLTest( )
        {
            _lookupBLL = Resolve<ILookUpBLL>(); 
        }

        [TestMethod]
        public void Should_Get_All_Countries( )
        {
            // Arrange
            // Act
            var countries = _lookupBLL.GetAllCountries();
            // Assert

            countries.ShouldNotBeNull();
            countries.Data.ShouldNotBeNull();
            countries.Data.Count.ShouldBeGreaterThan(0);


        }

        [TestMethod]
        public void Should_Get_All_Currencies( )
        {
            // Arrange
            // Act
            var countries = _lookupBLL.GetAllCurrencies();
            // Assert

            countries.ShouldNotBeNull();
            countries.Data.ShouldNotBeNull();
            countries.Data.Count.ShouldBeGreaterThan(0);


        }
    }
}
