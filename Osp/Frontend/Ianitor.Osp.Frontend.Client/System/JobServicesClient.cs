using System;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Ianitor.Osp.Frontend.Client.System
{
    public class JobServicesClient : ServiceClient, IJobServicesClient
    {
        public JobServicesClient(IOptions<JobServiceClientOptions> jobServiceClientOptions, IJobServiceClientAccessToken jobAccessToken)
            : this(jobServiceClientOptions.Value, jobAccessToken)
        {
        }
        
        public JobServicesClient(JobServiceClientOptions jobServiceClientOptions, IJobServiceClientAccessToken jobAccessToken)
        : base(jobServiceClientOptions, jobAccessToken)
        {
        }
        
        protected override Uri BuildServiceUri()
        {
            if (string.IsNullOrWhiteSpace(Options.EndpointUri))
            {
                throw new ServiceConfigurationMissingException($"Job services URI is missing.");
            }
            
            return new Uri(Options.EndpointUri).Append("system").Append("v1");
        }

        public async Task<JobDto> GetImportJobStatus(string id)
        {
            ArgumentValidation.ValidateString(nameof(id),id);

            var request = new RestRequest("jobs", Method.GET);
            request.AddQueryParameter("id", id);

            IRestResponse<JobDto> response = await Client.ExecuteAsync<JobDto>(request);
            ValidateResponse(response);

            return response.Data;
        }
        
        /// <summary>
        /// Downloads the job result as binary file
        /// </summary>
        /// <param name="id">Job id</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadExportRtResultAsync(string id)
        {
            ArgumentValidation.ValidateString(nameof(id),id);

            var request = new RestRequest("jobs/download", Method.GET);
            request.AddQueryParameter("id", id);

            var response = await Client.ExecuteAsync(request);
            ValidateResponse(response);

            return response.RawBytes;
        }
    }
}