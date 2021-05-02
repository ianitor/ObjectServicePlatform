using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.CkModelEntities;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public class EntityNotificationRepository : INotificationRepository
    {
        private readonly ISystemContext _systemContext;

        public EntityNotificationRepository(ISystemContext systemContext)
        {
            _systemContext = systemContext;
        }

        public async Task AddShortMessageAsync(string tenantId, string toPhoneNumber, string message)
        {
            await AddShortMessageAsync(tenantId, toPhoneNumber, message, null);
        }

        public async Task AddEMailMessageAsync(string tenantId, string emailAddress, string subject,
            string htmlMessage)
        {
            await AddEMailMessageAsync(tenantId, emailAddress, subject, htmlMessage, null);
        }

        public async Task AddShortMessageAsync(string tenantId, string toPhoneNumber, string message, RtEntityId? associatedRtId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateString(nameof(toPhoneNumber), toPhoneNumber);
            ArgumentValidation.ValidateString(nameof(message), message);

            try
            {
                var notificationMessage = new NotificationMessageDto
                {
                    SendStatus = SendStatusDto.Pending,
                    BodyText = message,
                    RecipientAddress = toPhoneNumber,
                    NotificationType = NotificationTypesDto.Sms,
                    LastTryDateTime = DateTime.MinValue
                };

                await AddMessageAsync(tenantId, notificationMessage, associatedRtId);
            }
            catch (Exception e)
            {
                throw new NotificationSendFailedException("Message send failed.", e);
            }
        }

        public async Task AddEMailMessageAsync(string tenantId, string emailAddress, string subject,
            string htmlMessage, RtEntityId? associatedRtId)
        {
            
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateString(nameof(emailAddress), emailAddress);
            ArgumentValidation.ValidateString(nameof(subject), subject);
            ArgumentValidation.ValidateString(nameof(htmlMessage), htmlMessage);

            try
            {
                var notificationMessage = new NotificationMessageDto
                {
                    SendStatus = SendStatusDto.Pending,
                    SubjectText = subject,
                    BodyText = htmlMessage,
                    RecipientAddress = emailAddress,
                    NotificationType = NotificationTypesDto.EMail,
                    LastTryDateTime = DateTime.MinValue
                };

                await AddMessageAsync(tenantId, notificationMessage, associatedRtId);
            }
            catch (Exception e)
            {
                throw new NotificationSendFailedException("Message send failed.", e);
            }
        }

        public async Task<PagedResult<NotificationMessageDto>> GetPendingMessagesAsync(string tenantId,
            NotificationTypesDto notificationType, int? take = null)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            
            var tenantContext = await _systemContext.CreateOrGetTenantContext(tenantId);
            var session = await tenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var result = await tenantContext.Repository.GetRtEntitiesByTypeAsync<RtSystemNotificationMessage>(session, 
                new DataQueryOperation
                {
                    FieldFilters = new[]
                    {
                        new FieldFilter(nameof(RtSystemNotificationMessage.SendStatus), FieldFilterOperator.Equals, SendStatusDto.Pending),
                        new FieldFilter(nameof(RtSystemNotificationMessage.LastTryDateTime), FieldFilterOperator.LessEqualThan, DateTime.UtcNow.AddMinutes(-5)),
                        new FieldFilter(nameof(RtSystemNotificationMessage.NotificationType), FieldFilterOperator.Equals, notificationType),
                    }
                });

            await session.CommitTransactionAsync();

            return new PagedResult<NotificationMessageDto>(result.Result.Select(CreateNotificationMessage), 0, take,
                result.TotalCount);
        }

        public async Task<IEnumerable<NotificationMessageDto>> StoreNotificationMessages(string tenantId,
            IEnumerable<NotificationMessageDto> notificationMessages)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.Validate(nameof(notificationMessages), notificationMessages);


            var tenantContext = await _systemContext.CreateOrGetTenantContext(tenantId);
            var session = await tenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var entityUpdateInfos = await Task.WhenAll(notificationMessages.Select(async dto =>
                new EntityUpdateInfo(await PrepareUpdateRtEntityAsync(session, dto, tenantContext),
                    EntityModOptions.Update)));

            await tenantContext.Repository.ApplyChanges(session, entityUpdateInfos);

            await session.CommitTransactionAsync();

            return entityUpdateInfos.Select(x=> CreateNotificationMessage((RtSystemNotificationMessage)x.RtEntity));
        }


        private async Task AddMessageAsync(string tenantId, NotificationMessageDto notificationMessageDto, RtEntityId? targetRtId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.Validate(nameof(notificationMessageDto), notificationMessageDto);


            var tenantContext = await _systemContext.CreateOrGetTenantContext(tenantId);
            var session = await tenantContext.Repository.StartSessionAsync();
            session.StartTransaction();
            
            var rtEntity = CreateRtEntity(notificationMessageDto, tenantContext);
            
            var associationUpdateInfos = new List<AssociationUpdateInfo>();
            if (targetRtId != null)
            {
                associationUpdateInfos.Add(new AssociationUpdateInfo(rtEntity.ToRtEntityId(), targetRtId.Value, Constants.RelatedRoleId, AssociationModOptionsDto.Create)); 
            }

            await tenantContext.Repository.ApplyChanges(session, new[]
            {
                new EntityUpdateInfo(rtEntity, EntityModOptions.Create)
            }, associationUpdateInfos);
            
            await session.CommitTransactionAsync();
        }

        private static RtSystemNotificationMessage CreateRtEntity(NotificationMessageDto notificationMessageDto,
            ITenantContext tenantContext)
        {
            var rtEntity = tenantContext.Repository.CreateTransientRtEntity<RtSystemNotificationMessage>();

            ApplyDtoData(notificationMessageDto, rtEntity);

            return rtEntity;
        }

        private static void ApplyDtoData(NotificationMessageDto notificationMessageDto, RtSystemNotificationMessage rtEntity)
        {
            rtEntity.SubjectText = notificationMessageDto.SubjectText;
            rtEntity.BodyText = notificationMessageDto.BodyText;
            rtEntity.RecipientAddress = notificationMessageDto.RecipientAddress;
            rtEntity.SentDateTime = notificationMessageDto.SentDateTime;
            rtEntity.LastTryDateTime = notificationMessageDto.LastTryDateTime;
            rtEntity.ErrorText = notificationMessageDto.ErrorText;
            rtEntity.SendStatus = notificationMessageDto.SendStatus == null
                ? SendStatus.Pending
                : (SendStatus) notificationMessageDto.SendStatus;
            rtEntity.NotificationType = notificationMessageDto.NotificationType == null
                ? NotificationTypes.EMail
                : (NotificationTypes) notificationMessageDto.NotificationType;
        }

        private static async Task<RtSystemNotificationMessage> PrepareUpdateRtEntityAsync(IOspSession session, NotificationMessageDto notificationMessageDto,
            ITenantContext tenantContext)
        {
            var rtEntity = await tenantContext.Repository.GetRtEntityAsync<RtSystemNotificationMessage>(session, notificationMessageDto.ToRtEntityId());

            ApplyDtoData(notificationMessageDto, rtEntity);

            return rtEntity;
        }

        private static NotificationMessageDto CreateNotificationMessage(RtSystemNotificationMessage rtEntity)
        {
            return new NotificationMessageDto
            {
                RtId = rtEntity.RtId.ToOspObjectId(),
                CkId = rtEntity.CkId,
                SubjectText = rtEntity.SubjectText,
                BodyText = rtEntity.BodyText,
                RecipientAddress = rtEntity.RecipientAddress,
                SentDateTime = rtEntity.SentDateTime,
                LastTryDateTime = rtEntity.LastTryDateTime,
                SendStatus = (SendStatusDto?) rtEntity.SendStatus,
                NotificationType = (NotificationTypesDto?) rtEntity.NotificationType,
                ErrorText = rtEntity.ErrorText
            };
        }
    }
}