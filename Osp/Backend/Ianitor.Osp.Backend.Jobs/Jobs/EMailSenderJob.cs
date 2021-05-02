using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Hangfire;
using Ianitor.Osp.Backend.Jobs.Services;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Common.Shared.Services;

namespace Ianitor.Osp.Backend.Jobs.Jobs
{
    /// <summary>
    /// Hangfire Job to send e-mails for notification messages
    /// </summary>
    public class EMailSenderJob
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEMailSender _eMailSender;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notificationRepository">The notification message repository</param>
        /// <param name="eMailSender">E-Mail sender for SMS</param>
        public EMailSenderJob(INotificationRepository notificationRepository, IEMailSender eMailSender)
        {
            _notificationRepository = notificationRepository;
            _eMailSender = eMailSender;
        }

        /// <summary>
        /// Exports a runtime model
        /// </summary>
        /// <param name="tenantId">The corresponding tenant id</param>
        /// <param name="cancellationToken">An cancellation token to abort the job</param>
        /// <returns>The key the result file is stored.</returns>
        [DisplayName("E-Mail Sender '{0}'")]
        [DisableConcurrentExecution(10 * 60)]
        public async Task SendEMail(string tenantId, IJobCancellationToken cancellationToken)
        {
            PagedResult<NotificationMessageDto> pagedResult;
            do
            {
                pagedResult = await _notificationRepository.GetPendingMessagesAsync(tenantId, NotificationTypesDto.EMail, 20);
                if (pagedResult.TotalCount == 0)
                {
                    break;
                }

                foreach (var notificationMessageDto in pagedResult.List)
                {
                    notificationMessageDto.LastTryDateTime = DateTime.UtcNow;
                }

                await _notificationRepository.StoreNotificationMessages(tenantId, pagedResult.List);

                cancellationToken?.ThrowIfCancellationRequested();

                foreach (var notificationMessageDto in pagedResult.List)
                {
                    try
                    {
                        await _eMailSender.SendEmailAsync(notificationMessageDto.RecipientAddress,
                            notificationMessageDto.SubjectText,
                            notificationMessageDto.BodyText);

                        notificationMessageDto.SentDateTime = DateTime.UtcNow;
                        notificationMessageDto.SendStatus = SendStatusDto.Sent;
                        notificationMessageDto.ErrorText = null;
                    }
                    catch (NotificationSendFailedException e)
                    {
                        notificationMessageDto.SentDateTime = DateTime.UtcNow;
                        notificationMessageDto.SendStatus = SendStatusDto.Error;
                        notificationMessageDto.ErrorText = e.GetDirectAndIndirectMessages();
                    }
                }

                await _notificationRepository.StoreNotificationMessages(tenantId, pagedResult.List);
                
            } while (pagedResult.TotalCount > 0);
        }
    }
}