using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Jobs.Jobs;
using Ianitor.Osp.Backend.JobServices.Jobs;
using Ianitor.Osp.Backend.Persistence;

namespace Ianitor.Osp.Backend.JobServices.Services
{
    /// <summary>
    /// Implements the service hook service, that creates hangfire jobs for each data source
    /// </summary>
    public class ServiceHookService : IServiceHookService
    {
        private readonly ISystemContext _systemContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemContext"></param>
        /// <param name="distributedCache"></param>
        public ServiceHookService(ISystemContext systemContext, IDistributedWithPubSubCache distributedCache)
        {
            _systemContext = systemContext;
            var sub = distributedCache.Subscribe<string>(CacheCommon.KeyTenantUpdate);
            sub.OnMessage(async message => { await SyncDataSourceAndCreateJobsAsync(); });
        }

        /// <summary>
        /// Removes all jobs and creates new jobs for existing data sources
        /// </summary>
        /// <returns></returns>
        public async Task SyncDataSourceAndCreateJobsAsync()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            using var systemSession = await _systemContext.StartSystemSessionAsync();
            systemSession.StartTransaction();

            // Clean old jobs
            var result = await _systemContext.GetTenantsAsync(systemSession);
            foreach (var ospDataSource in result.List)
            {
                RecurringJob.AddOrUpdate<ServiceHookJob>($"ServiceHook_{ospDataSource.TenantId}",
                    job => job.Run(ospDataSource.TenantId, JobCancellationToken.Null), "*/15 * * * *");

                RecurringJob.AddOrUpdate<AttributeValueAggregatorJob>($"AttributeValueAggregate_{ospDataSource.TenantId}",
                    job => job.Run(ospDataSource.TenantId, JobCancellationToken.Null), Cron.Daily);
                RecurringJob.AddOrUpdate<EMailSenderJob>($"Notification_EMail_Sender_{ospDataSource.TenantId}",
                    job => job.SendEMail(ospDataSource.TenantId, JobCancellationToken.Null), Cron.Minutely);
            }

            await systemSession.CommitTransactionAsync();
        }
    }
}