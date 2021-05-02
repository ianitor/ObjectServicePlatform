using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using IdentityModel.Jwk;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.NotificationMessages
{
    internal class ResetNotificationsInError : ServiceClientOspCommand<ITenantClient>
    {
        private IArgument _type;
        private readonly ITenantClient _tenantClient;
        private readonly INotificationRepository _notificationRepository;


        public ResetNotificationsInError(IOptions<OspToolOptions> options, ITenantClient tenantClient,
            INotificationRepository notificationRepository, IAuthenticationService authenticationService)
            : base("ResetNotificationsInError", "Sets notifications in error to Pending.", options, tenantClient,
                authenticationService)
        {
            _tenantClient = tenantClient;
            _notificationRepository = notificationRepository;
        }

        protected override void AddArguments()
        {
            _type = CommandArgumentValue.AddArgument("t", "type",
                new[] {"Type of notification message, available is 'email' or 'sms'"}, true,
                1);
        }


        public override async Task Execute()
        {
            Logger.Info(
                $"Resetting notification messages at '{Options.Value.CoreServiceUrl}' for tenant '{Options.Value.TenantId}'");

            var type = CommandArgumentValue.GetArgumentScalarValue<NotificationTypesDto>(_type);

            var filterList = new List<FieldFilterDto>
            {
                new FieldFilterDto
                {
                    AttributeName = nameof(NotificationMessageDto.SendStatus),
                    Operator = FieldFilterOperatorDto.Equals,
                    ComparisonValue = SendStatusDto.Error
                },
                new FieldFilterDto
                {
                    AttributeName = nameof(NotificationMessageDto.NotificationType),
                    Operator = FieldFilterOperatorDto.Equals,
                    ComparisonValue = type
                }
            };

            var getQuery = new GraphQLRequest
            {
                Query = GraphQl.GetNotifications,
                Variables = new {fieldFilters = filterList.ToArray()}
            };

            var getResult = await _tenantClient.SendQueryAsync<NotificationMessageDto>(getQuery);
            if (!getResult.Items.Any())
            {
                Logger.Info($"No notifications in error has been returned.");
                return;
            }
            Logger.Info($"{getResult.Items.Count()} notification messages in error has been returned.");

            foreach (var notificationMessageDto in getResult.Items)
            {
                notificationMessageDto.LastTryDateTime = DateTime.UtcNow.AddMinutes(-10);
                notificationMessageDto.SendStatus = SendStatusDto.Pending;
            }

            await _notificationRepository.StoreNotificationMessages(Options.Value.TenantId, getResult.Items);

            Logger.Info($"Reset notficiation messages completed.");
        }
    }
}