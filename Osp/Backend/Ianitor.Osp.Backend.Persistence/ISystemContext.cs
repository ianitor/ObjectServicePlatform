using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.Persistence
{
    public interface ISystemContext
    {
        Task<IOspSession> StartSystemSessionAsync();
        IRepository OspSystemDatabase { get; }

        Task CreateSystemDatabaseAsync();
        Task UpdateSystemSchemaAsync(IOspSession systemSession);
        Task ClearSystemDatabaseAsync();
        Task DropSystemDatabaseAsync();
        Task<bool> IsSystemDatabaseExistingAsync();
        Task<bool> IsTenantExistingAsync(IOspSession systemSession, string tenantId);

        Task<PagedResult<OspTenant>> GetTenantsAsync(IOspSession systemSession, int? skip = null, int? take = null);
        Task<OspTenant> GetTenantAsync(IOspSession systemSession, string tenantId);
        
        Task CreateTenantAsync(IOspSession systemSession, string databaseName, string tenantId);
        Task AttachTenantAsync(IOspSession systemSession, string databaseName, string tenantId);
        Task DetachTenantAsync(IOspSession systemSession, string tenantId);
        Task ClearTenantAsync(IOspSession systemSession, string tenantId);
        Task DropTenantAsync(IOspSession systemSession, string tenantId);
        Task ImportCkModelAsTextAsync(IOspSession systemSession, string tenantId, ScopeIds scopeId, string jsonText);

        Task ImportCkModelAsync(IOspSession systemSession, string tenantId, ScopeIds scopeId, string filePath, CancellationToken? cancellationToken);

        Task<TValueType> GetConfigurationAsync<TValueType>(IOspSession systemSession, string key, TValueType defaultValue) where TValueType : struct;
        Task<string> GetConfigurationAsync(IOspSession systemSession, string key, string defaultValue);
        Task SetConfigurationAsync<TValueType>(IOspSession systemSession, string key, TValueType value) where TValueType : struct;
        Task SetConfigurationAsync(IOspSession systemSession, string key, string value);

        Task<ITenantContext> CreateOrGetTenantContext(string tenantId);

        bool TryGetCkCache(string tenantId, out ICkCache ckCache);
    }
}