using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using NLog;

namespace Ianitor.Osp.Backend.Jobs.Jobs
{
    /// <summary>
    /// HangFire job to aggregate attribute values for auto complete
    /// </summary>
    public class AttributeValueAggregatorJob
    {
        private readonly ISystemContext _systemContext;
        private readonly IDistributedWithPubSubCache _distributedCache;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemContext">System context object</param>
        /// <param name="distributedCache">Redis distributed cache for file caching</param>
        public AttributeValueAggregatorJob(ISystemContext systemContext, IDistributedWithPubSubCache distributedCache)
        {
            _systemContext = systemContext;
            _distributedCache = distributedCache;
        }
        
        /// <summary>
        /// Aggregates all aggregatable attributes 
        /// </summary>
        /// <param name="tenantId">The corresponding data source</param>
        /// <param name="cancellationToken">An cancellation token to abort the job</param>
        /// <returns></returns>
        [DisplayName("Aggregates all attributes of data source '{0}'")]
        public async Task Run(string tenantId, IJobCancellationToken cancellationToken)
        {
            try
            {
                Logger.Info($"Reading aggregatable attributes '{tenantId}'");

               var dataContext = await _systemContext.CreateOrGetTenantContext(tenantId);

                foreach (var entityCacheItem in dataContext.CkCache.GetCkEntities())
                {
                    cancellationToken?.ThrowIfCancellationRequested();
                    
                    foreach (var attributeCacheItem in entityCacheItem.Attributes)
                    {
                        if (!attributeCacheItem.Value.IsAutoCompleteEnabled)
                        {
                            continue;
                        }

                        cancellationToken?.ThrowIfCancellationRequested();

                        using var session = await dataContext.Repository.StartSessionAsync();
                        session.StartTransaction();

                        var autoCompleteTexts = await dataContext.Repository.ExtractAutoCompleteValuesAsync(session, entityCacheItem.CkId, 
                            attributeCacheItem.Value.AttributeName, attributeCacheItem.Value.AutoCompleteFilter, attributeCacheItem.Value.AutoCompleteLimit);

                        await dataContext.Repository.UpdateAutoCompleteTexts(session, entityCacheItem.CkId, attributeCacheItem.Value.AttributeName, autoCompleteTexts.Select(x=> x.Text));

                        await session.CommitTransactionAsync();
                    }
                }
                
                await _distributedCache.PublishAsync(CacheCommon.KeyTenantUpdate, tenantId);

                Logger.Info($"Aggregation of attribute values of data source '{tenantId}' completed.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Aggregation failed with error.");
                throw;
            }
        }
    }
}