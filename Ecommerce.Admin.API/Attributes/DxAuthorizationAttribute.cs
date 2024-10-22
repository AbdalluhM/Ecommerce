using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.Repositroy.Base;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DxAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter// IAuthorizationFilter
    {
        private readonly int[] _pageIds;
        private readonly int[] _actionIds;
        public IRepository<Role> _roleRepository;
        public IEmployeeBLL _employeeBLL { get; set; }
        public DxAuthorizeAttribute(PagesEnum page, params ActionsEnum[] actions) : this(new PagesEnum[] { page }, actions) { }

        public DxAuthorizeAttribute(PagesEnum[] pages, params ActionsEnum[] actions)
        {
            _pageIds = Array.ConvertAll(pages, value => (int)value);
            _actionIds = Array.ConvertAll(actions, value => (int)value);
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext actionContext)
        {
            #region unAuthorizedResponse 
            var unAuthorizedResponse = new JsonResult(new AuthorizationResponse
            {
                IsSuccess = false,
                Data = false,
                Errors = new System.Collections.Generic.List<TErrorField>
                {
                     new TErrorField
                     {
                           Code = ((int)MessageCodes.UnAuthorizedAccess).ToString(),
                           Message =MessageCodes.UnAuthorizedAccess.GetDescription()
                     }
                }
            });

            unAuthorizedResponse.StatusCode = (int)HttpStatusCode.OK;

            #endregion

            var customerClaim = actionContext.HttpContext.User;
            // redirect to access denied page
            if (!customerClaim.Identity.IsAuthenticated)
            {
                actionContext.Result = unAuthorizedResponse;
                return;
            }
            _employeeBLL = (EmployeeBLL)actionContext.HttpContext.RequestServices.GetService(typeof(IEmployeeBLL));
            _roleRepository = (BaseRepository<Role>)actionContext.HttpContext.RequestServices.GetService(typeof(IRepository<Role>));
            var employeeId = Convert.ToInt32(customerClaim.FindFirstValue(TokenClaimTypeEnum.Id.ToString()));

            //var userName = currentIdentity.Name;

            //// step 1 : retrieve employee 
            var employee = await _employeeBLL.GetByIdAsync(employeeId);
            if (employee == null)
            {
                actionContext.Result = unAuthorizedResponse;
                return;
            }

            if (employee.IsAdmin.HasValue && employee.IsAdmin.Value)
                return;



            //// step 2 : retrieve employee permissions
            var role = _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Employees.Contains(employee)).FirstOrDefault();
            var employeePermissions = role?.RolePageActions?.Select(x => x.PageAction).ToList();
            //// step 3 : match employee permission(s) against class/method's required pages / actions
            var permission = employeePermissions?.FirstOrDefault(x => _pageIds.Contains(x.PageId)
            && _actionIds.Contains(x.ActionId));
            //_actions.Contains((ActionsEnum)Enum.Parse(typeof(ActionsEnum), x.ActionId + "")));
            //// step 4 : continue/redirect to access denied page or return unAuthroized Response
            if (permission == null && _actionIds.Contains((int)ActionsEnum.DEFAULT))
            {
                ////add code for default permissions
                ////case getmyemployeeinfodetail
                //foreach (var page in _pageIds)
                //{
                switch (_pageIds[0]) //TODO:Refactor this
                {
                    case (int)PagesEnum.MyUsers:
                        string empIdGetParameter = actionContext.HttpContext.Request.Query["id"];
                        if (empIdGetParameter != null && employeeId.ToString() == empIdGetParameter)
                        {
                            return;
                        }
                        break;
                    case (int)PagesEnum.Notifications:
                        return;
                    default:
                        break;
                }
                //}
            }
            else if (permission == null)
            {
                actionContext.Result = unAuthorizedResponse;
                return;
            }


            return;
        }

        //actionContext.Result = unAuthorizedResponse;

    }

}



