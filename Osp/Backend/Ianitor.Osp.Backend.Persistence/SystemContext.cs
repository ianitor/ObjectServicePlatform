using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.Commands;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Persistence
{
    // ReSharper disable once UnusedMember.Global
    public class SystemContext : ISystemContext
    {
        private readonly IRepositoryClient _repositoryClient;

        private readonly IDistributedWithPubSubCache _distributedWithPubSubCache;
        private readonly OspSystemConfiguration _systemConfiguration;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<string, ICkCache> _ckCaches;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly ICachedCollection<SystemEntities.OspTenant> _tenantCollection;
        private readonly ICachedCollection<OspConfiguration> _configurationCollection;

        public SystemContext(IOptions<OspSystemConfiguration> systemConfiguration,
            IDistributedWithPubSubCache distributedWithPubSubCache)
        {
            ArgumentValidation.Validate(nameof(systemConfiguration), systemConfiguration);
            ArgumentValidation.Validate(nameof(distributedWithPubSubCache), distributedWithPubSubCache);

            _systemConfiguration = systemConfiguration.Value;
            _distributedWithPubSubCache = distributedWithPubSubCache;

            var sharedSettings = new MongoConnectionOptions
            {
                MongoDbHost = _systemConfiguration.DatabaseHost,
                MongoDbUsername = _systemConfiguration.AdminUser,
                MongoDbPassword = _systemConfiguration.AdminUserPassword,
                AuthenticationSource = _systemConfiguration.AuthenticationDatabaseName
            };

            _ckCaches = new ConcurrentDictionary<string, ICkCache>();

            _repositoryClient = new MongoRepositoryClient(sharedSettings);
            OspSystemDatabase = _repositoryClient.GetRepository(_systemConfiguration.SystemDatabaseName);

            _tenantCollection = OspSystemDatabase.GetCollection<SystemEntities.OspTenant>();
            _configurationCollection = OspSystemDatabase.GetCollection<OspConfiguration>();

            var sub = _distributedWithPubSubCache.Subscribe<string>(CacheCommon.KeyTenantUpdate);
            sub.OnMessage(message =>
            {
                RemoveCkCache(message.Message);
                return Task.CompletedTask;
            });
        }

        #region Transaction handling

        public async Task<IOspSession> StartSystemSessionAsync()
        {
            var systemSession = await OspSystemDatabase.StartSessionAsync();
            return systemSession;
        }

        #endregion Transaction handling

        #region System database handling

        public IRepository OspSystemDatabase { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task CreateSystemDatabaseAsync()
        {
            if (await IsSystemDatabaseExistingAsync())
            {
                throw new DatabaseException("System database already exists.");
            }

            try
            {
                await _repositoryClient.CreateRepositoryAsync(_systemConfiguration.SystemDatabaseName);

                using var systemSession = await OspSystemDatabase.StartSessionAsync();
                systemSession.StartTransaction();
                
                await CreateSystemSchemaAsync(systemSession);

                UnloadAllCaches();

                await systemSession.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _repositoryClient.DropRepositoryAsync(_systemConfiguration.SystemDatabaseName);
                throw;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public async Task ClearSystemDatabaseAsync()
        {
            if (!await IsSystemDatabaseExistingAsync())
            {
                throw new DatabaseException("System database does not exist.");
            }

            await CreateSystemDatabaseAsync();
        }

        public async Task UpdateSystemSchemaAsync(IOspSession systemSession)
        {
            if (!await IsSystemDatabaseExistingAsync())
            {
                throw new DatabaseException("System database does not exist.");
            }

            var version = await GetConfigurationAsync(systemSession, Constants.SystemSchemaVersion, 0);

            if (version < Constants.SystemSchemaVersionValue)
            {
                await CreateSystemSchemaAsync(systemSession);
            }
        }

        private async Task CreateSystemSchemaAsync(IOspSession systemSession)
        {
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<SystemEntities.OspTenant>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspConfiguration>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspUser>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspRole>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspPermission>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspPermissionRole>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspClient>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspApiResource>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspApiScope>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspIdentityResource>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspPersistedGrant>();
            await OspSystemDatabase.CreateCollectionIfNotExistsAsync<OspIdentityProvider>();

            await SetConfigAsync(systemSession, Constants.SystemSchemaVersion, Constants.SystemSchemaVersionValue);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task DropSystemDatabaseAsync()
        {
            if (!await IsSystemDatabaseExistingAsync())
            {
                throw new DatabaseException("System database does not exist.");
            }

            await _repositoryClient.DropRepositoryAsync(_systemConfiguration.SystemDatabaseName);

            UnloadAllCaches();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<bool> IsSystemDatabaseExistingAsync()
        {
            return await IsDatabaseAlreadyExistingAsync(_systemConfiguration.SystemDatabaseName);
        }

        #endregion System database handling


        #region TenantId Context Handling

        public async Task<ITenantContext> CreateOrGetTenantContext(string tenantId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            using var systemSession = await StartSystemSessionAsync();
            systemSession.StartTransaction();

            var result = await CreateOrGetTenantContextInternal(systemSession, tenantId);

            await systemSession.CommitTransactionAsync();
            return result;
        }

        private async Task<ITenantContextInternal> CreateOrGetTenantContextInternal(IOspSession systemSession,
            string tenantId)
        {
            if (TryGetCkCache(tenantId, out var ckCache))
            {
                return await CreateTenantContextAsync(systemSession, ckCache);
            }

            try
            {
                await _semaphoreSlim.WaitAsync();

                if (TryGetCkCache(tenantId, out ckCache))
                {
                    return await CreateTenantContextAsync(systemSession, ckCache);
                }

                var databaseContext = await CreateDatabaseContextByTenantAsync(systemSession, tenantId);
                ckCache = new CkCache(tenantId);
                await ckCache.Initialize(databaseContext);

                var key = tenantId.MakeKey();
                _ckCaches[key] = ckCache;
                return await CreateTenantContextAsync(systemSession, ckCache);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<ITenantContextInternal> CreateTenantContextAsync(IOspSession systemSession,
            ICkCache ckCache)
        {
            var databaseContext = await CreateDatabaseContextByTenantAsync(systemSession, ckCache.TenantId);
            var tenantRepository = new TenantRepository(ckCache, databaseContext);
            return new TenantContext(ckCache.TenantId, tenantRepository, ckCache);
        }

        public bool TryGetCkCache(string tenantId, out ICkCache ckCache)
        {
            var key = tenantId.MakeKey();

            if (_ckCaches.TryGetValue(key, out ckCache))
            {
                if (ckCache != null && !ckCache.IsDisposed)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion TenantId Context Handling

        #region User Data Source handling

        private async Task<SystemEntities.OspTenant> GetOspDatabaseFromTenantAsync(IOspSession systemSession, string tenantId)
        {
            return await _tenantCollection.DocumentAsync(systemSession, tenantId.MakeKey());
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<bool> IsTenantExistingAsync(IOspSession systemSession, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var ospDatabase = await GetOspDatabaseFromTenantAsync(systemSession, tenantId);
            return ospDatabase != null;
        }

        private async Task<bool> IsDatabaseAlreadyExistingAsync(string databaseName)
        {
            return await _repositoryClient.IsRepositoryExistingAsync(databaseName);
        }

        public async Task<PagedResult<OspTenant>> GetTenantsAsync(IOspSession systemSession, int? skip = null,
            int? take = null)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);

            var result = await _tenantCollection.GetAsync(systemSession, skip, take);
            var totalCount = await _tenantCollection.GetTotalCountAsync(systemSession);
            return new PagedResult<OspTenant>(result.Select(d => new OspTenant(d.TenantId, d.DatabaseName)),
                skip, take, totalCount);
        }

        public async Task<OspTenant> GetTenantAsync(IOspSession systemSession, string tenantId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.Validate(nameof(systemSession), systemSession);

            var ospTenant = await _tenantCollection.DocumentAsync(systemSession, tenantId.MakeKey());
            if (ospTenant == null)
            {
                throw new TenantException($"Tenant '{tenantId}' not found.");
            }

            return new OspTenant(ospTenant.TenantId, ospTenant.DatabaseName);
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task CreateTenantAsync(IOspSession systemSession, string databaseName, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(databaseName), databaseName);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            var normalizedDatabaseName = databaseName.ToLower();

            var normalizedTenantId = databaseName.MakeKey();
            if (await IsTenantExistingAsync(systemSession, normalizedTenantId))
            {
                throw new TenantException($"Tenant '{normalizedTenantId}' already exists.");
            }

            if (await IsDatabaseAlreadyExistingAsync(normalizedDatabaseName))
            {
                throw new DatabaseException($"Database '{normalizedDatabaseName}' already exists.");
            }

            await _repositoryClient.CreateRepositoryAsync(normalizedDatabaseName);
            await _repositoryClient.CreateUser(systemSession, _systemConfiguration.AuthenticationDatabaseName,
                normalizedDatabaseName, string.Format(_systemConfiguration.DatabaseUser, normalizedDatabaseName),
                _systemConfiguration.DatabaseUserPassword);

            var repository = _repositoryClient.GetRepository(normalizedDatabaseName);
            await repository.CreateCollectionIfNotExistsAsync<CkAttribute>();
            await repository.CreateCollectionIfNotExistsAsync<CkEntity>();
            await repository.CreateCollectionIfNotExistsAsync<CkEntityAssociation>();
            await repository.CreateCollectionIfNotExistsAsync<CkEntityInheritance>();
            await repository.CreateCollectionIfNotExistsAsync<RtAssociation>();


            var ospTenant = new SystemEntities.OspTenant
            {
                TenantId = normalizedTenantId,
                DatabaseName = normalizedDatabaseName
            };

            await _tenantCollection.InsertAsync(systemSession, ospTenant);
            await RestoreTenantSystemCkModelAsync(systemSession, ospTenant);
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task AttachTenantAsync(IOspSession systemSession, string databaseName, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(databaseName), databaseName);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            if (await IsTenantExistingAsync(systemSession, tenantId))
            {
                throw new TenantException($"Tenant '{tenantId}' already exists.");
            }

            if (!await IsDatabaseAlreadyExistingAsync(databaseName))
            {
                throw new DatabaseException($"Database '{databaseName}' does not exist.");
            }

            var ospTenant = new SystemEntities.OspTenant
            {
                TenantId = tenantId,
                DatabaseName = databaseName
            };

            await _tenantCollection.InsertAsync(systemSession, ospTenant);
        }

        public async Task DetachTenantAsync(IOspSession systemSession, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            var ospTenant = await GetOspDatabaseFromTenantAsync(systemSession, tenantId);
            if (ospTenant == null)
            {
                throw new TenantException($"Tenant '{tenantId}' does not exists.");
            }

            await _tenantCollection.DeleteOneAsync(systemSession, ospTenant.TenantId);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task ClearTenantAsync(IOspSession systemSession, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var ospTenant = await GetOspDatabaseFromTenantAsync(systemSession, tenantId);
            if (ospTenant == null)
            {
                throw new TenantException($"Tenant '{tenantId}' does not exist.");
            }

            await DropTenantAsync(systemSession, tenantId);
            await CreateTenantAsync(systemSession, ospTenant.DatabaseName, tenantId);
            await UnloadTenantCachesAsync(tenantId);
        }

        private async Task<IDatabaseContext> CreateDatabaseContextByTenantAsync(IOspSession systemSession,
            string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var ospTenant = await GetOspDatabaseFromTenantAsync(systemSession, tenantId);
            if (ospTenant == null)
            {
                throw new TenantException($"Tenant '{tenantId}' does not exist.");
            }

            return new DatabaseContext(_systemConfiguration.DatabaseHost, ospTenant.DatabaseName,
                string.Format(_systemConfiguration.DatabaseUser, ospTenant.DatabaseName),
                _systemConfiguration.DatabaseUserPassword, _systemConfiguration.AuthenticationDatabaseName);
        }

        private async Task RestoreTenantSystemCkModelAsync(IOspSession systemSession, SystemEntities.OspTenant ospTenant)
        {
            var ckModelFilePath = Path.Combine(Helper.AssemblyDirectory, "CKModel.json");
            Logger.Info("Importing construction kit model '{0}'", ckModelFilePath);
            await ImportCkModelAsync(systemSession, ospTenant.TenantId, ScopeIds.System, ckModelFilePath, null);
            Logger.Info("Construction kit model imported.");
        }

// ReSharper disable once MemberCanBePrivate.Global
        public async Task DropTenantAsync(IOspSession systemSession, string tenantId)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var ospTenant = await GetOspDatabaseFromTenantAsync(systemSession, tenantId);
            if (ospTenant == null)
            {
                throw new TenantException($"Tenant '{tenantId}' does not exist.");
            }

            await UnloadTenantCachesAsync(tenantId);
            await _repositoryClient.DropRepositoryAsync(ospTenant.DatabaseName);
            await _tenantCollection.DeleteOneAsync(systemSession, ospTenant.TenantId);
        }

        #endregion Tenant handling

        #region Model handling

// ReSharper disable once UnusedMember.Global
        public async Task ImportCkModelAsTextAsync(IOspSession systemSession, string tenantId, ScopeIds scopeId,
            string jsonText)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateString(nameof(jsonText), jsonText);

            var databaseContext = await CreateDatabaseContextByTenantAsync(systemSession, tenantId);
            using var session = await databaseContext.StartSessionAsync();
            session.StartTransaction();

            var importer = new ImportCkModel(databaseContext);
            await importer.ImportText(session, jsonText, scopeId);

            await session.CommitTransactionAsync();

            await UnloadTenantCachesAsync(tenantId);
        }

        public async Task ImportCkModelAsync(IOspSession systemSession, string tenantId, ScopeIds scopeId,
            string filePath, CancellationToken? cancellationToken)
        {
            ArgumentValidation.Validate(nameof(systemSession), systemSession);
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateExistingFile(nameof(filePath), filePath);

            var databaseContext = await CreateDatabaseContextByTenantAsync(systemSession, tenantId);
            using var session = await databaseContext.StartSessionAsync();
            session.StartTransaction();

            var importer = new ImportCkModel(databaseContext);
            await importer.Import(session, filePath, scopeId, cancellationToken);

            await session.CommitTransactionAsync();

            await UnloadTenantCachesAsync(tenantId);
        }

        #endregion Model handling

        #region Configuration

        public async Task<TValueType> GetConfigurationAsync<TValueType>(IOspSession systemSession, string key,
            TValueType defaultValue) where
            TValueType
            : struct

        {
            ArgumentValidation.ValidateString(nameof(key), key);
            return (TValueType) Convert.ChangeType(await GetConfigAsync(systemSession, key, defaultValue),
                typeof(TValueType));
        }

        public async Task<string> GetConfigurationAsync(IOspSession systemSession, string key, string defaultValue)
        {
            ArgumentValidation.ValidateString(nameof(key), key);
            return (string) await GetConfigAsync(systemSession, key, defaultValue);
        }


        public async Task SetConfigurationAsync<TValueType>(IOspSession systemSession, string key, TValueType value)
            where TValueType : struct

        {
            ArgumentValidation.ValidateString(nameof(key), key);
            await SetConfigAsync(systemSession, key, value);
        }

        public async Task SetConfigurationAsync(IOspSession systemSession, string key, string value)
        {
            ArgumentValidation.ValidateString(nameof(key), key);
            await SetConfigAsync(systemSession, key, value);
        }


        private async Task<object> GetConfigAsync(IOspSession systemSession, string key, object defaultValue)
        {
            var document = await _configurationCollection.DocumentAsync(systemSession, key);
            if (document == null)
            {
                return defaultValue;
            }

            return document.Value;
        }

        public async Task SetConfigAsync(IOspSession systemSession, string key, object value)
        {
            var document = await _configurationCollection.DocumentAsync(systemSession, key);
            if (document == null)
            {
                document = new OspConfiguration {Key = key, Value = value};
                await _configurationCollection.InsertAsync(systemSession, document);
            }

            else
            {
                document.Value = value;
                await _configurationCollection.ReplaceByIdAsync(systemSession, key, document);
            }
        }

        #endregion Configuration

        #region Private Methods

        private async Task UnloadTenantCachesAsync(string tenantId)
        {
            await _distributedWithPubSubCache.PublishAsync(CacheCommon.KeyTenantUpdate, tenantId);
            RemoveCkCache(tenantId);
        }

        private void RemoveCkCache(string tenantId)
        {
            if (_ckCaches.TryRemove(tenantId, out ICkCache ckCache))
            {
                ckCache.Dispose();
            }
        }

        private void UnloadAllCaches()
        {
            _ckCaches.Clear();
        }

        #endregion Private Methods
    }
}