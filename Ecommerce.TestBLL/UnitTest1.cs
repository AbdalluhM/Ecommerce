using MyDexef.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.LookUps;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using MyDexef.API;
using MyDexef.BLL.Tags;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class UnitTest1 : UnitTestBase
    {

        [TestMethod]
        public void Should_Get_All_Countries( )
        { // Arrange
            var myService = serviceProvider.GetRequiredService<ILookUpBLL>();
            // Act
            var countries = myService.GetAllCountries();
            // Assert

            countries.ShouldNotBeNull();
            countries.Data.ShouldNotBeNull();
            countries.Data.Count.ShouldBeGreaterThan(0);


        }

        [TestMethod]
        public void Should_Get_Create_Tag( )
        { // Arrange
            var myService = serviceProvider.GetRequiredService<ITagBLL>();
            // Act
            var output = myService.Create(new DTO.Tags.CreateTagInputDto
            {
                Name = "{\"default\":\"Tag12\",\"ar\":\"Tag12\"}",
                CreatedBy = 4066,
                CreateDate = DateTime.UtcNow,
                IsActive = true
            });
            ;
            // Assert

            output.ShouldNotBeNull();
            output.Data.ShouldNotBeNull();
            output.Data.Name ="{\"default\":\"Tag1\",\"ar\":\"Tag1\"}";
            output.Data.Id.ShouldBeGreaterThan(0);


        }


        [TestMethod]

        public void MultiplyTest()
        {
            DemoUT bid = new DemoUT();
            var result = bid.Multiply(2, 2);
            Assert.AreEqual(4, result);
    
        }
    }
}
