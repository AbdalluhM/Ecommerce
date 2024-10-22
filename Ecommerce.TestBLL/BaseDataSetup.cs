using System.Threading.Tasks;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Employees;
using Microsoft.Extensions.DependencyInjection;
using MyDexef.DTO.Employees;
using MyDexef.DTO.Paging;
using MyDexef.DTO.Settings.Auth;
using MyDexef.Helper.Security;
using MyDexef.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using MyDexef.Core.Entities;
namespace MyDexef.TestBLL
{
    public class BaseDataSetup: UnitTestBase
    {
       private IRepository<Employee> _employeeRepository;

        public BaseDataSetup()
        {
            //
            _employeeRepository = serviceProvider.GetRequiredService<IRepository<Employee>>();
        }

        [TestMethod]
        public async Task Should1CreateSuperAdmin()
        {
            var output = await _employeeRepository.AddAsync(new Employee()
            {
                IsActive = true,
                UserName="admin",
                Password= "sha1:64000:18:f3kspDJV+GaraO3dp5P/iaDehI2ea6jE:g6Z3fpr45eOyHJ9i/Bm5B+xK",
                IsAdmin=true,
                IsDeleted=false
            });
            // Assert
            output.ShouldNotBeNull();
            output.Id.ShouldBeGreaterThan(0);
        }
    }
}
