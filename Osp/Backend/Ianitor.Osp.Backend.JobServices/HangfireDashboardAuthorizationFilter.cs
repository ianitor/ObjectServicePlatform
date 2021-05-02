using Hangfire.Dashboard;

namespace Ianitor.Osp.Backend.JobServices
{
    internal class HangfireDashboardAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}