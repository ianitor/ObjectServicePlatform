using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Ianitor.Osp.Frontend.Client.System
{
    public class IdentityServicesClient : ServiceClient, IIdentityServicesClient
    {
        public IdentityServicesClient(IOptions<IdentityServiceClientOptions> identityServiceClientOptions, IIdentityServiceClientAccessToken identityAccessToken)
            : this(identityServiceClientOptions.Value, identityAccessToken)
        {
        }
        
        public IdentityServicesClient(IdentityServiceClientOptions identityServiceClientOptions, IIdentityServiceClientAccessToken identityAccessToken)
        : base(identityServiceClientOptions, identityAccessToken)
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
        
        public async Task<IEnumerable<IdentityProviderDto>> GetIdentityProviders()
        {
            var request = new RestRequest("identityProviders", Method.GET);

            IRestResponse<IdentityProvidersResult> response = await Client.ExecuteAsync<IdentityProvidersResult>(request);
            ValidateResponse(response);

            return response.Data.IdentityProviders;
        }

        public async Task<IdentityProviderDto> GetIdentityProvider(string id)
        {
            ArgumentValidation.ValidateString(nameof(id),id);

            var request = new RestRequest("identityProviders/{id}", Method.GET);
            request.AddUrlSegment("id", id);

            IRestResponse<IdentityProviderDto> response = await Client.ExecuteAsync<IdentityProviderDto>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task CreateIdentityProvider(IdentityProviderDto identityProvider)
        {
            ArgumentValidation.Validate(nameof(identityProvider), identityProvider);
            
            var request = new RestRequest("identityProviders", Method.POST);
            request.AddJsonBody(identityProvider);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task UpdateIdentityProvider(string id, IdentityProviderDto identityProvider)
        {
            ArgumentValidation.ValidateString(nameof(id),id);
            ArgumentValidation.Validate(nameof(identityProvider), identityProvider);

            var request = new RestRequest("identityProviders/{id}", Method.PUT);
            request.AddUrlSegment("id", id);
            request.AddJsonBody(identityProvider);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task DeleteIdentityProvider(string id)
        {
            ArgumentValidation.ValidateString(nameof(id),id);

            var request = new RestRequest("identityProviders", Method.DELETE);
            request.AddQueryParameter("id", id);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }
        

        public async Task<IEnumerable<ClientDto>> GetClients()
        {
            var request = new RestRequest("clients", Method.GET);

            IRestResponse<List<ClientDto>> response = await Client.ExecuteAsync<List<ClientDto>>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task<ClientDto> GetClient(string clientId)
        {
            ArgumentValidation.ValidateString(nameof(clientId),clientId);

            var request = new RestRequest("clients/{id}", Method.GET);
            request.AddUrlSegment("id", clientId);

            IRestResponse<ClientDto> response = await Client.ExecuteAsync<ClientDto>(request);
            ValidateResponse(response);

            return response.Data;
        }

        public async Task CreateClient(ClientDto client)
        {
            ArgumentValidation.Validate(nameof(client), client);
            
            var request = new RestRequest("clients", Method.POST);
            request.AddJsonBody(client);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task UpdateClient(string clientId, ClientDto client)
        {
            ArgumentValidation.ValidateString(nameof(clientId),clientId);
            ArgumentValidation.Validate(nameof(client), client);

            var request = new RestRequest("clients/{id}", Method.PUT);
            request.AddUrlSegment("id", clientId);
            request.AddJsonBody(client);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task DeleteClient(string clientId)
        {
            ArgumentValidation.ValidateString(nameof(clientId),clientId);

            var request = new RestRequest("clients/{id}", Method.DELETE);
            request.AddUrlSegment("id", clientId);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }
        
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var request = new RestRequest("identities", Method.GET);
            
            IRestResponse<List<UserDto>> response = await Client.ExecuteAsync<List<UserDto>>(request);
            ValidateResponse(response);

            return response.Data;
        }
        
        public async Task CreateUser(UserDto userDto)
        {
            ArgumentValidation.Validate(nameof(userDto), userDto);

            var request = new RestRequest("identities", Method.POST);
            request.AddJsonBody(userDto);

            IRestResponse response = await Client.ExecutePostAsync(request);
            ValidateResponse(response);
        }
        
        public async Task UpdateUser(string userName, UserDto userDto)
        {
            ArgumentValidation.ValidateString(nameof(userName),userName);
            ArgumentValidation.Validate(nameof(userDto), userDto);

            var request = new RestRequest("identities/{userName}", Method.PUT);
            request.AddUrlSegment("userName", userName);
            request.AddJsonBody(userDto);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }
        
        public async Task DeleteUser(string userName)
        {
            ArgumentValidation.ValidateString(nameof(userName),userName);

            var request = new RestRequest("identities", Method.DELETE);
            request.AddQueryParameter("userName", userName);

            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task ResetPassword(string userName, string password)
        {
            ArgumentValidation.ValidateString(nameof(userName),userName);
            ArgumentValidation.ValidateString(nameof(password),password);

            var request = new RestRequest("identities/resetPassword", Method.POST);
            request.AddQueryParameter("userName", userName);
            request.AddQueryParameter("password", password);
            
            IRestResponse response = await Client.ExecuteAsync(request);
            ValidateResponse(response);
        }

        public async Task<RoleDto> GetRoleByName(string roleName)
        {
            ArgumentValidation.ValidateString(nameof(roleName),roleName);

            var request = new RestRequest("roles/names/{roleName}", Method.GET);
            request.AddUrlSegment("roleName", roleName);

            IRestResponse<RoleDto> response = await Client.ExecuteAsync<RoleDto>(request);
            ValidateResponse(response);

            return response.Data;
        }

    }
}