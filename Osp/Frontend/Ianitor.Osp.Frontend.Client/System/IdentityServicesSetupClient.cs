using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Ianitor.Osp.Frontend.Client.System
{
    public class IdentityServicesSetupClient : ServiceClient, IIdentityServicesSetupClient
    {
        public IdentityServicesSetupClient(IOptions<IdentityServiceClientOptions> identityServiceClientOptions)
            : this(identityServiceClientOptions.Value)
        {
        }
        
        public IdentityServicesSetupClient(IdentityServiceClientOptions identityServiceClientOptions)
        : base(identityServiceClientOptions)
        {
        }
        
        protected override Uri BuildServiceUri()
        {
            if (string.IsNullOrWhiteSpace(Options.EndpointUri))
            {
                throw new ServiceConfigurationMissingException($"Identity services URI is missing.");
            }
            
            return new Uri(Options.EndpointUri).Append("system").Append("v1");
        }

        public async Task AddAdminUser(AdminUserDto adminUserDto)
        {
            ArgumentValidation.Validate(nameof(adminUserDto), adminUserDto);
            
            var request = new RestRequest("setup", Method.POST);
            request.AddJsonBody(adminUserDto);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }
    }
}