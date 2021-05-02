using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Ianitor.Osp.Backend.CoreServices.Services;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware
{
    internal class OspMessageListener: IOperationMessageListener
    {
        private readonly IOspService _ospService;
        private readonly string _tenantId;

        public OspMessageListener(IOspService ospService, string tenantId)
        {
            _ospService = ospService;
            _tenantId = tenantId;
        }

        /// <inheritdoc />
        public async Task BeforeHandleAsync(MessageHandlingContext context)
        {
            context.Properties[Statics.TenantContext] = await _ospService.SystemContext.CreateOrGetTenantContext(_tenantId);
        }

        /// <inheritdoc />
        public Task HandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AfterHandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }
    }
}