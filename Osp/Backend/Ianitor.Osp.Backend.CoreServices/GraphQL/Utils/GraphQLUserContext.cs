using System.Collections.Generic;
using System.Security.Claims;
using Ianitor.Osp.Backend.Persistence;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Utils
{
    public class GraphQLUserContext : Dictionary<string, object>
    {
        public ClaimsPrincipal User { get; set; }
        
        public ITenantContext TenantContext { get; set; }
    }
}