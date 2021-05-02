using System;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.NotificationMessages
{
    internal class CompletePendingNotifications : ServiceClientOspCommand<ITenantClient>
    {
        private readonly INotificationRepository _notificationRepository;
        private IArgument _type;


        public CompletePendingNotifications(IOptions<OspToolOptions> options, INotificationRepository notificationRepository, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("CompletePendingNotifications", "Sets the sent date time for pending notification message", options, tenantClient, authenticationService)
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
            Logger.Info(
                $"Completing notification messages at '{Options.Value.CoreServiceUrl}' for tenant '{Options.Value.TenantId}'");

            var type = CommandArgumentValue.GetArgumentScalarValue<NotificationTypesDto>(_type);

            var result = await _notificationRepository.GetPendingMessagesAsync(Options.Value.TenantId, type);
            foreach (var notificationMessageDto in result.List)
            {
                notificationMessageDto.SentDateTime = DateTime.UtcNow;
                notificationMessageDto.SendStatus = SendStatusDto.Sent;
            }

            await _notificationRepository.StoreNotificationMessages(Options.Value.TenantId, result.List);

            Logger.Info($"Notficiation message completed.");
        }
    }
}