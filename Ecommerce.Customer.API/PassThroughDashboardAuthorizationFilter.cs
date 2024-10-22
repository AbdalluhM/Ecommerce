using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Ecommerce.Customer.API
{
    public class PassThroughDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
