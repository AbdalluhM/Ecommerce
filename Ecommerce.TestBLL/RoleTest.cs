using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Roles;
using MyDexef.DTO.Paging;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyDexef.DTO.Roles.RoleDto;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class RoleTest : UnitTestBase
    {
        private readonly IRoleBLL _roleBLL;
        private const int _employeeId = 4067;
        private const string _name = "{\"default\":\"NewRole1\",\"ar\":\"دور جديد1\"}";
        private const string _updateName = "{\"default\":\"NewRole2\",\"ar\":\"دور جديد2\"}";
        private const string _exisitName = "{\"default\":\"employee\",\"ar\":\"موظف\"}";
        public List<RolePageActionDto> CreateRolePageActions => new List<RolePageActionDto>
        {
            new RolePageActionDto
            {
                CreatedBy = _employeeId,
                CreateDate = DateTime.Now,
                PageId = 1,
                ActionIds = new List<int>(){1,2,3,4}, 
            },
        };
        public List<RolePageActionDto> UpdateRolePageActions => new List<RolePageActionDto>
        {
            new RolePageActionDto
            {
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.Now,
                PageId = 2,
                ActionIds = new List<int>(){1,2},
            },
            new RolePageActionDto
            {
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.Now,
                PageId = 3,
                ActionIds = new List<int>(){1,2,3,4},
            },
        };
        public RoleTest()
        {
            _roleBLL = serviceProvider.GetRequiredService<IRoleBLL>();
        }
        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        public async Task GetRoles_CheckFilterIfExisit_ReturnRoles(string value)
        {
            //Arrange
            var filter = new FilteredResultRequestDto() { SearchTerm = value };
            //Action
            var Roles =await _roleBLL.GetAllRolesPagedList(filter);
            //Assert
            Roles.IsSuccess.ShouldBeTrue();
            Roles.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        #region Create
        [TestMethod]
        public async Task CreateRole_AddNewRoleWithCorrectData_ReturnRole()
        {
            //Arrange
            var param = new CreateRoleInputDto()
            {
                Name = _name,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                RolePageAction = CreateRolePageActions
            };
            //Action
            var Role =await _roleBLL.CreateRoleAsync(param);
            //Assert
            Role.IsSuccess.ShouldBeTrue();
            Role.Data.ShouldNotBeNull();
            Role.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }


        [TestMethod]
        public async Task ValidateFieldsIsRequired_AddNewRoleWithoutCreatedBy_ReturnCannotCreateRoleWithoutCreatedBy()
        {
            //Arrange
            var param = new CreateRoleInputDto()
            {
                Name = _name,
                CreatedBy = 0,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                RolePageAction = CreateRolePageActions
            };
            //Action
            var Role =await  _roleBLL.CreateRoleAsync(param);
            //Assert
            Role.IsSuccess.ShouldBeFalse();
            Role.Data.ShouldBeNull();
            Role.Errors.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public async Task ValidateNameIsUnique_AddNewRoleWithNameExisitInDB_ReturnCanNotCreateRoleAlreadyExisit()
        {
            //Arrange
            var param = new CreateRoleInputDto()
            {
                Name = _exisitName,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                RolePageAction = CreateRolePageActions
            };
            //Action
            var Role =await _roleBLL.CreateRoleAsync(param);
            //Assert
            Role.IsSuccess.ShouldBeFalse();
            Role.Data.ShouldBeNull();
            Role.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Update
        [TestMethod]
        public async Task UpdateRole_UpdateRoleWithCorrectData_ReturnRoleUpdated()
        {
            //Arrange
            var param = new UpdateRoleInputDto()
            {
                Name = _updateName,
                CreatedBy= _employeeId,
                CreateDate = DateTime.UtcNow,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                RolePageAction = UpdateRolePageActions,
                RoleId = 2063
            };
            //Action
            var Role =await _roleBLL.UpdateRoleAsync(param);
            //Assert
            Role.IsSuccess.ShouldBeTrue();
            Role.Data.ShouldNotBeNull();
            Role.Errors.Count.ShouldBeLessThanOrEqualTo(0);
        }
        [TestMethod]
        public async Task UpdateRole_UpdateRoleWithNameAlreadyExisit_ReturnThisRoleAlreadyExisit()
        {
            //Arrange
            var param = new UpdateRoleInputDto()
            {
                Name = _exisitName,
                CreatedBy = _employeeId,
                CreateDate = DateTime.UtcNow,
                ModifiedBy = _employeeId,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true,
                RolePageAction = UpdateRolePageActions,
                RoleId = 2063
            };
            //Action
            var Role =await _roleBLL.UpdateRoleAsync(param);
            //Assert
            Role.IsSuccess.ShouldBeFalse();
            Role.Data.ShouldBeNull();
            Role.Errors.Count.ShouldBeGreaterThan(0);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteRoleNotFound_TakeRoleIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            int RoleId = 100000000;
            //Arrange
            var IsDelted =await _roleBLL.DeleteRoleAsync(RoleId);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        [TestMethod]
        public async Task DeleteRole_DeleteRoleExisitInDb_ReturnBoolean()
        {
            //Assert
            int RoleId = 2063;
            //Arrange
            var IsDelted = await _roleBLL.DeleteRoleAsync(RoleId);
            //Assert
            IsDelted.IsSuccess.ShouldBeTrue();
        }
        [TestMethod]
        public async Task DeleteRoleRelatedData_TakeRoleIdAndDeleteFromDb_ReturnBool()
        {
            //Assert
            int RoleId = 2057; 
            //Arrange
            var IsDelted =await _roleBLL.DeleteRoleAsync(RoleId);
            //Assert
            IsDelted.IsSuccess.ShouldBeFalse();
        }
        #endregion
    }
}
