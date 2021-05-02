using System.Collections.Concurrent;
using System.Linq;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Types;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Caches
{
    /// <summary>
    /// Implements the graph type cache
    /// </summary>
    internal class GraphTypesCache : IGraphTypesCache
    {
        private readonly ITenantContext _tenantContext;
        private readonly ConcurrentDictionary<string, RtEntityDtoType> _types;
        private readonly ConcurrentDictionary<string, RtEntityDtoInputType> _inputTypes;
        private readonly ConcurrentDictionary<IGraphType, DynamicConnectionType> _connectionTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tenantContext"></param>
        public GraphTypesCache(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
            _types = new ConcurrentDictionary<string, RtEntityDtoType>();
            _inputTypes = new ConcurrentDictionary<string, RtEntityDtoInputType>();
            _connectionTypes = new ConcurrentDictionary<IGraphType, DynamicConnectionType>();
        }

        /// <inheritdoc />
        public RtEntityDtoType GetOrCreate(string ckId)
        {
            return _types.GetOrAdd(ckId, s =>
            {
                var rtEntityType = new RtEntityDtoType(ckId);
                return rtEntityType;
            });
        }

        /// <inheritdoc />
        public RtEntityDtoInputType GetOrCreateInput(string ckId)
        {
            return _inputTypes.GetOrAdd(ckId, s =>
            {
                var rtEntityDtoInputType = new RtEntityDtoInputType(ckId);
                return rtEntityDtoInputType;
            });
        }

        /// <inheritdoc />
        public DynamicConnectionType GetOrCreateConnection(IGraphType graphType, string prefixName)
        {
            return _connectionTypes.GetOrAdd(graphType, s =>
            {
                var edgeType = new DynamicEdgeType(
                    $"{prefixName}{CommonConstants.GraphQlEdgeSuffix}",
                    $"An edge in a connection from an object to another object of type `{graphType.Name}`.", graphType);

                return new DynamicConnectionType
                (
                    $"{prefixName}{CommonConstants.GraphQlConnectionSuffix}",
                    $"A connection to `{prefixName}`.",
                    graphType, edgeType
                );
            });
        }

        /// <inheritdoc />
        public IGraphType[] GetTypes()
        {
            // ReSharper disable once CoVariantArrayConversion
            return _types.Values.ToArray();
        }

        public void Populate()
        {
            foreach (var rtEntityDtoType in _types.Values)
            {
                var entityCacheItem = _tenantContext.CkCache.GetEntityCacheItem(rtEntityDtoType.CkId);
                rtEntityDtoType.Populate(this, entityCacheItem);
            }

            foreach (var rtEntityDtoInputType in _inputTypes.Values)
            {
                var entityCacheItem = _tenantContext.CkCache.GetEntityCacheItem(rtEntityDtoInputType.CkId);
                rtEntityDtoInputType.Populate(entityCacheItem);
            }
        }
    }
}