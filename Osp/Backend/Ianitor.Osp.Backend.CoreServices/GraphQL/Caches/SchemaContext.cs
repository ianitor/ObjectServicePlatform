using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.CoreServices.Services;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Common.Shared;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Caches
{
    /// <summary>
    /// The schema context allows to cache GraphQL Schemas based on a data source
    /// </summary>
    public class SchemaContext : ISchemaContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOspService _ospService;
        private readonly MemoryCache _cache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ospService">The OSP service</param>
        /// <param name="distributedWithPubSubCache"></param>
        public SchemaContext(IOspService ospService, IDistributedWithPubSubCache distributedWithPubSubCache)
        {
            _ospService = ospService;

            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 64
            });
            
            var sub = distributedWithPubSubCache.Subscribe<string>(CacheCommon.KeyTenantUpdate);
            sub.OnMessage(message =>
            {
                _cache.Remove(message.Message.MakeKey());
                return Task.CompletedTask;
            });

        }

        /// <summary>
        /// Invalidates a cached schema
        /// </summary>
        /// <param name="tenantId">The Id of tenant</param>
        public void Invalidate(string tenantId)
        {
            var key = tenantId.MakeKey();
            _cache.Remove(key);
        }

        /// <inheritdoc />
        public async Task<OspSchema> GetOrCreateAsync(ITenantContext tenantContext)
        {
            ArgumentValidation.Validate(nameof(tenantContext), tenantContext);
            
            var key = tenantContext.TenantId.MakeKey();
            
            Logger.Debug($"Looking up GraphQL schema for {tenantContext.TenantId}");
            
            if (!_cache.TryGetValue(key, out OspSchema schema))
            {
                try
                {
                    await _semaphore.WaitAsync();

                    var t = new Func<ICacheEntry, OspSchema>(entry =>
                    {
                        Logger.Debug($"Creating GraphQL schema for {tenantContext.TenantId}");
                        entry.SetSize(1);
                        entry.SlidingExpiration = TimeSpan.FromDays(1);

                        var graphTypesCache = new GraphTypesCache(tenantContext);
                        var ckEntities = tenantContext.CkCache.GetCkEntities().Where(x => !x.IsAbstract).ToList();
                        var rtEntitiesTypes = ckEntities.Select(ck => graphTypesCache.GetOrCreate(ck.CkId)).ToList();

                        var ospQuery = new OspQuery(graphTypesCache, rtEntitiesTypes);
                        var ospMutation = new OspMutation(ckEntities, graphTypesCache, tenantContext.CkCache);
                        var ospSubscriptions = new OspSubscriptions(rtEntitiesTypes);

                        graphTypesCache.Populate();

                        var createdSchema = new OspSchema(ospQuery, ospMutation, ospSubscriptions);
                       createdSchema.RegisterTypes(graphTypesCache.GetTypes());
                        
                        Logger.Debug($"GraphQL schema for {tenantContext.TenantId} completed");
                        return createdSchema;
                    });

                    schema = _cache.GetOrCreate(key, t);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return schema;
        }
    }
}