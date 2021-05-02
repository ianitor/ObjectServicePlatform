using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Ianitor.Osp.Frontend.Client.System
{
    public class CoreServicesClient : ServiceClient, ICoreServicesClient
    {
        public CoreServicesClient(IOptions<CoreServiceClientOptions> coreServiceClientOptions,
            ICoreServiceClientAccessToken coreAccessToken)
            : this(coreServiceClientOptions.Value, coreAccessToken)
        {
        }

        public CoreServicesClient(CoreServiceClientOptions coreServiceClientOptions,
            ICoreServiceClientAccessToken coreAccessToken)
            : base(coreServiceClientOptions, coreAccessToken)
        {
        }

        protected override Uri BuildServiceUri()
        {
            if (string.IsNullOrWhiteSpace(Options.EndpointUri))
            {
                throw new ServiceConfigurationMissingException($"Core services URI is missing.");
            }

            return new Uri(Options.EndpointUri).Append("system").Append("v1");
        }

        public async Task<JobDto> GetImportJobStatus(string id)
        {
            ArgumentValidation.ValidateString(nameof(id), id);

            var request = new RestRequest("jobs", Method.GET);
            request.AddQueryParameter("id", id);

            IRestResponse<JobDto> response = await Client.ExecuteAsync<JobDto>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task<string> ImportCkModel(string tenantId, ScopeIdsDto scopeId, string ckModelFilePath)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateExistingFile(nameof(ckModelFilePath), ckModelFilePath);

            var request = new RestRequest("models/ImportCk", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);
            request.AddQueryParameter("scopeId", ((int)scopeId).ToString());

            if (Path.GetExtension(ckModelFilePath)?.ToLower() == ".zip")
            {
                request.AddFile("file", ckModelFilePath, contentType: "application/zip");
            }
            else if (Path.GetExtension(ckModelFilePath)?.ToLower() == ".json")
            {
                request.AddFile("file", ckModelFilePath, contentType: "application/json");
            }
            else
            {
                throw new ServiceClientException($"'{ckModelFilePath}' is not a supported file.");
            }

            IRestResponse<string> response = await Client.ExecuteAsync<string>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task<string> ImportRtModel(string tenantId, string rtModelFilePath)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateExistingFile(nameof(rtModelFilePath), rtModelFilePath);

            var request = new RestRequest("models/ImportRt", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);

            if (Path.GetExtension(rtModelFilePath)?.ToLower() == ".zip")
            {
                request.AddFile("file", rtModelFilePath, contentType: "application/zip");
            }
            else if (Path.GetExtension(rtModelFilePath)?.ToLower() == ".json")
            {
                request.AddFile("file", rtModelFilePath, contentType: "application/json");
            }
            else
            {
                throw new ServiceClientException($"'{rtModelFilePath}' is not a supported file.");
            }

            IRestResponse<string> response = await Client.ExecuteAsync<string>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task<string> ExportRtModel(string tenantId, OspObjectId queryId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.Validate(nameof(queryId), queryId);

            var request = new RestRequest("models/ExportRt", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);
            request.AddJsonBody(new ExportModelRequestDto {QueryId = queryId});

            IRestResponse<string> response = await Client.ExecuteAsync<string>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task CleanTenant(string tenantId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var request = new RestRequest("tenants/clear", Method.PUT);
            request.AddQueryParameter("tenantId", tenantId);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task ClearTenantCache(string tenantId)
        {
            var request = new RestRequest("tenants/clearCache", Method.PUT);
            request.AddQueryParameter("tenantId", tenantId);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }


        public async Task<IEnumerable<TenantDto>> GetTenants()
        {
            var request = new RestRequest("tenants", Method.GET);

            IRestResponse<List<TenantDto>> response = await Client.ExecuteAsync<List<TenantDto>>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task CreateTenant(string tenantId, string databaseName)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateString(nameof(databaseName), databaseName);

            var request = new RestRequest("tenants", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);
            request.AddQueryParameter("databaseName", databaseName);

            IRestResponse response = await Client.ExecutePostAsync(request);
            ValidateResponse(response);
        }

        public async Task AttachTenant(string tenantId, string databaseName)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);
            ArgumentValidation.ValidateString(nameof(databaseName), databaseName);

            var request = new RestRequest("tenants/attach", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);
            request.AddQueryParameter("databaseName", databaseName);

            IRestResponse response = await Client.ExecutePostAsync(request);
            ValidateResponse(response);
        }

        public async Task DetachTenant(string tenantId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var request = new RestRequest("dataSources/detach", Method.POST);
            request.AddQueryParameter("tenantId", tenantId);

            IRestResponse response = await Client.ExecutePostAsync(request);
            ValidateResponse(response);
        }

        public async Task DeleteTenant(string tenantId)
        {
            ArgumentValidation.ValidateString(nameof(tenantId), tenantId);

            var request = new RestRequest("tenants", Method.DELETE);
            request.AddQueryParameter("tenantId", tenantId);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }
    }
}