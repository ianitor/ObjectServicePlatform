using System;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.NotificationMessages
{
    internal class CreateNotification : ServiceClientOspCommand<ITenantClient>
    {
        private readonly INotificationRepository _notificationRepository;
        private IArgument _type;
        private IArgument _recipient;
        private IArgument _subject;
        private IArgument _body;
        private IArgument _associationCkId;
        private IArgument _associationRtId;


        public CreateNotification(IOptions<OspToolOptions> options, INotificationRepository notificationRepository, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("CreateNotification", "Create a new notification message", options, tenantClient, authenticationService)
        {
            _notificationRepository = notificationRepository;
        }

        protected override void AddArguments()
        {
            _type = CommandArgumentValue.AddArgument("t", "type",
                new[] {"Type of notification message, available is 'email' or 'sms'"}, true,
                1);

            _recipient = CommandArgumentValue.AddArgument("r", "recipient",
                new[] {"Address of recipient (for example e-mail address or phone number)'"}, true,
                1);

            _subject = CommandArgumentValue.AddArgument("s", "subject", new[] {"Subject of notification message."},
                false, 1);

            _body = CommandArgumentValue.AddArgument("b", "body", new[] {"Body of notification message"},
                true, 1);
            
            _associationCkId= CommandArgumentValue.AddArgument("ackid", "associationCkId", new[] {"Association construction kit ID of related entity"},
                false, 1);
            _associationRtId = CommandArgumentValue.AddArgument("artid", "associationRtId", new[] {"Association runtime ID of related entity"},
                false, 1);
        }


        public override async Task Execute()
        {
            var type = CommandArgumentValue.GetArgumentScalarValue<NotificationTypesDto>(_type);
            var recipient = CommandArgumentValue.GetArgumentScalarValue<string>(_recipient);
            var subject = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_subject);
            var body = CommandArgumentValue.GetArgumentScalarValue<string>(_body);
            var associationCkId = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_associationCkId);
            var associationRtId = CommandArgumentValue.GetArgumentScalarValueOrDefault<string>(_associationRtId);
            
            RtEntityId? rtEntityId = null;
            if (!string.IsNullOrWhiteSpace(associationCkId) && !string.IsNullOrWhiteSpace(associationRtId))
            {
                rtEntityId = new RtEntityId(associationCkId, OspObjectId.Parse(associationRtId));
            }

            Logger.Info(
                $"Creating notification messages at '{Options.Value.CoreServiceUrl}' for tenant '{Options.Value.TenantId}'");


            switch (type)
            {
                case NotificationTypesDto.Sms:
                    await _notificationRepository.AddShortMessageAsync(Options.Value.TenantId, recipient, body, rtEntityId);
                    break;
                case NotificationTypesDto.EMail:
                    await _notificationRepository.AddEMailMessageAsync(Options.Value.TenantId, recipient, subject, body, rtEntityId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            Logger.Info($"Notficiation message added.");
        }
    }
}