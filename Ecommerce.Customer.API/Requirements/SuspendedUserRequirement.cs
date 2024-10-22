using Microsoft.AspNetCore.Authorization;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.Core.Enums.Customers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Requirements
{
    public class SuspendedUserRequirement : AuthorizationHandler<SuspendedUserRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuspendedUserRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type.Equals(TokenClaimTypeEnum.StatusId.ToString())))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var status = context.User.FindFirstValue(TokenClaimTypeEnum.StatusId.ToString());

            if (string.IsNullOrEmpty(status) || Convert.ToInt32(status) == (int)CustomerStatusEnum.Suspended)
                context.Fail();
            else
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
