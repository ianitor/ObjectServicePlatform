using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Frontend.Client.System
{
    public interface ICoreServicesClient : IServiceClient
    {
        Task<JobDto> GetImportJobStatus(string id);
        Task<string> ImportCkModel(string tenantId, ScopeIdsDto scopeId, string ckModelFilePath);
        Task<string> ImportRtModel(string tenantId, string rtModelFilePath);
        Task<string> ExportRtModel(string tenantId, OspObjectId queryId);
        Task CleanTenant(string tenantId);
        Task ClearTenantCache(string tenantId);
        Task<IEnumerable<TenantDto>> GetTenants();
        Task CreateTenant(string tenantId, string databaseName);
        Task AttachTenant(string tenantId, string databaseName);
        Task DetachTenant(string tenantId);
        Task DeleteTenant(string tenantId);
    }
}