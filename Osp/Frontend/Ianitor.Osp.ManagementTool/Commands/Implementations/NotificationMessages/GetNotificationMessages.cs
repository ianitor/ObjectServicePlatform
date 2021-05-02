using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.NotificationMessages
{
    internal class GetNotificationMessages: ServiceClientOspCommand<ITenantClient>
    {
        private readonly INotificationRepository _notificationRepository;
        private IArgument _type;

        public GetNotificationMessages(IOptions<OspToolOptions> options, INotificationRepository notificationRepository, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("GetNotifications", "Gets all pending notification messages.", options, tenantClient, authenticationService)
        {
            _notificationRepository = notificationRepository;
        }

        protected override void AddArguments()
        {
            _type = CommandArgumentValue.AddArgument("t", "type", new[] {"Type of notification message, available is 'email' or 'sms'"}, true,
                1);
        }

        public override async Task Execute()
        {
            Logger.Info($"Getting pending notification messages from '{Options.Value.CoreServiceUrl}' for tenant '{Options.Value.TenantId}'");

            if (string.IsNullOrWhiteSpace(Options.Value.TenantId))
            {
                throw new ServiceConfigurationMissingException("Tenant is not configured.");
            }

            var type = CommandArgumentValue.GetArgumentScalarValue<NotificationTypesDto>(_type);
            
            var getResult = await _notificationRepository.GetPendingMessagesAsync(Options.Value.TenantId, type);
            if (!getResult.List.Any())
            {
                Logger.Info("No notification messages has been returned.");
                return;
            }
            
            Logger.Info(JsonConvert.SerializeObject(getResult.List, Formatting.Indented));
        }
    }
}