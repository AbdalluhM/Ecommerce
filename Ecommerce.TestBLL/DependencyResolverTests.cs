using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using MyDexef.API;
using MyDexef.BLL.LookUps;
using MyDexef.Core.Infrastructure;

using NUnit.Framework;

using Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestFixture]
    public class DependencyResolverTests
    {
        private DependencyResolverHelper _serviceProvider;

        public DependencyResolverTests( )
        {

            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                  .UseServiceProviderFactory(new DefaultServiceProviderFactory(new ServiceProviderOptions()
                  {
                       
                      ValidateScopes = true,
                  }
                  ))
                .Build();
            _serviceProvider = new DependencyResolverHelper(webHost);
        }

        //[Test]
        //public void Service_Should_Get_Resolved( )
        //{

        //    //Act
        //    var YourService = _serviceProvider.GetService<ILookUpBLL>();

        //    //Assert
        //    Assert.IsNotNull(YourService);
        //}

        //[Test]
        //public void Should_Get_All_Countries( )
        //{

        //    // Arrange
        //    var myService = _serviceProvider.GetService<ILookUpBLL>();

        //    // Act
        //    var countries = myService.GetAllCountries();
        //    // Assert

        //    countries.ShouldNotBeNull();
        //    countries.Data.ShouldNotBeNull();
        //    countries.Data.Count.ShouldBeGreaterThan(0);

        //}
    }
}
