using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace Ecommerce.API
{
    public class PassThroughDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
