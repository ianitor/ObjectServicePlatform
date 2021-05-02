using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Server.Transports.AspNetCore;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.CoreServices.Services;
using Microsoft.AspNetCore.Http;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    public class TenantUserContextBuilder : IUserContextBuilder
    {
        private readonly IOspService _ospService;

        public TenantUserContextBuilder(IOspService ospService)
        {
            _ospService = ospService;
        }

        public async Task<IDictionary<string, object>> BuildUserContext(HttpContext httpContext)
        {
            var tenantId = httpContext.GetTenantId();

            using var systemSession = await _ospService.SystemContext.StartSystemSessionAsync();
            systemSession.StartTransaction();

            var userContext = new GraphQLUserContext
            {
                User = httpContext.User
            };

            if (!string.IsNullOrWhiteSpace(tenantId) &&
                await _ospService.SystemContext.IsTenantExistingAsync(systemSession, tenantId))
            {
                userContext.TenantContext = await _ospService.SystemContext.CreateOrGetTenantContext(tenantId);
            }

            await systemSession.CommitTransactionAsync();
            return userContext;
        }
    }
}