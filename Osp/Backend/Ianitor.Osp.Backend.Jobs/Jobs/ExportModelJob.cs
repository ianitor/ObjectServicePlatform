using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Common.Shared;
using NLog;

namespace Ianitor.Osp.Backend.Jobs.Jobs
{
    /// <summary>
    /// HangFire Job that implements the export of CK and RT model files
    /// </summary>
    public class ExportModelJob
    {
        private readonly ISystemContext _systemContext;
        private readonly IDistributedWithPubSubCache _distributedCache;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemContext">System context object</param>
        /// <param name="distributedCache">Redis distributed cache for file caching</param>
        public ExportModelJob(ISystemContext systemContext, IDistributedWithPubSubCache distributedCache)
        {
            _systemContext = systemContext;
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Exports a runtime model
        /// </summary>
        /// <param name="tenantId">The corresponding tenant id</param>
        /// <param name="queryId">Id of query, whose data is exported</param>
        /// <param name="cancellationToken">An cancellation token to abort the job</param>
        /// <returns>The key the result file is stored.</returns>
        [DisplayName("Export Runtime Metadata to data source '{0}'")]
        public async Task<string> ExportRtAsync(string tenantId, string queryId,
            IJobCancellationToken cancellationToken)
        {
            try
            {
                Logger.Info($"Preparing output file for query '{queryId}' of data source '{tenantId}'");
                var tempFile = Path.GetTempFileName();
                var key = Guid.NewGuid().ToString();

                Logger.Info($"Starting export of file '{tempFile}'");

                var tenantContext = await _systemContext.CreateOrGetTenantContext(tenantId);
                using var session = await tenantContext.Repository.StartSessionAsync();
                session.StartTransaction();
                
                await tenantContext.ExportRtModelAsync(session, new OspObjectId(queryId), tempFile, cancellationToken.ShutdownToken);

                await session.CommitTransactionAsync();

                await CacheFileToRedis(key, tempFile);

                Logger.Info($"Export of file '{tempFile}' completed.");

                return key;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Export failed with error.");
                throw;
            }
        }

        private async Task CacheFileToRedis(string key, string tempFile)
        {
            using (StreamReader streamReader = new StreamReader(tempFile))
            {
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    await streamReader.BaseStream.PackFileToZipAsync("RtEntities.json", memoryStream);
                    await _distributedCache.Database.StringSetAsync(key + "value", memoryStream.ToArray());
                    await _distributedCache.Database.StringSetAsync(key + "contentType", "application/zip");
                    await _distributedCache.Database.KeyExpireAsync(key + "contentType", DateTime.Now.AddHours(1));
                    await _distributedCache.Database.KeyExpireAsync(key + "value", DateTime.Now.AddHours(1));
                }
            }
        }
    }
}