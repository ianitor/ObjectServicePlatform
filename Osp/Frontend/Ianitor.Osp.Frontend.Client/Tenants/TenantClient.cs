using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Ianitor.Common.Shared;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Frontend.Client.Tenants
{
    public class TenantClient : ITenantClient
    {
        public IServiceClientAccessToken AccessToken { get; }
        public TenantClientOptions Options { get; }
        public Uri ServiceUri { get; private set; }
        
        private GraphQLHttpClient _client;

        // ReSharper disable once MemberCanBePrivate.Global
        protected GraphQLHttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    Initialize();
                }
                return _client;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSourceClientOptions">Options for data source client using DI</param>
        /// <param name="tenantClientAccessToken">Access Token for backend access</param>
        // ReSharper disable once UnusedMember.Global
        public TenantClient(IOptions<TenantClientOptions> dataSourceClientOptions,
            ITenantClientAccessToken tenantClientAccessToken)
            : this(dataSourceClientOptions.Value, tenantClientAccessToken)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tenantClientOptions">Options for data source client</param>
        /// <param name="tenantClientAccessToken">Access Token for backend access</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public TenantClient(TenantClientOptions tenantClientOptions,
            ITenantClientAccessToken tenantClientAccessToken)
        {
            AccessToken = tenantClientAccessToken;
            Options = tenantClientOptions;
            ArgumentValidation.Validate(nameof(tenantClientOptions), tenantClientOptions);
            ArgumentValidation.Validate(nameof(tenantClientAccessToken), tenantClientAccessToken);
        }

        public void Initialize()
        {
            if (string.IsNullOrWhiteSpace(Options.EndpointUri))
            {
                throw new ServiceConfigurationMissingException($"Core service URI is not configured.");
            }
                    
            if (string.IsNullOrWhiteSpace(Options.TenantId))
            {
                throw new ServiceConfigurationMissingException($"TenantId is not configured.");
            }

            ServiceUri = new Uri(Options.EndpointUri).Append("tenants/")
                .Append(Options.TenantId).Append("GraphQL");
            _client = new GraphQLHttpClient(ServiceUri, new NewtonsoftJsonSerializer());
                    
            AccessToken.AccessTokenUpdated += (sender, args) =>
                UpdateAccessToken(AccessToken.AccessToken);
            UpdateAccessToken(AccessToken.AccessToken);
        }

        private void UpdateAccessToken(string accessToken)
        {
            _client.HttpClient.DefaultRequestHeaders.Remove("Authorization");
            _client.HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
        }

        public async Task<QlItemsContainer<TDto>> SendQueryAsync<TDto>(GraphQLRequest query) where TDto : class
        {
            ArgumentValidation.Validate(nameof(query), query);

            try
            {
                var result = await Client.SendQueryAsync<QlQueryResponse<TDto>>(query);
                CheckResult(result);

                return result.Data.Connection;
            }
            catch (GraphQLHttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedServiceAccessException(e);
                }
                throw new ServiceClientException("Call to GraphQL source failed.", e);
            }
        }

        public async Task<TDto> SendMutationAsync<TDto>(GraphQLRequest query)
        {
            ArgumentValidation.Validate(nameof(query), query);

            try
            {
                var result = await Client.SendMutationAsync<QlMutationResponse<TDto>>(query);
                CheckResult(result);

                return result.Data.Result;
            }
            catch (GraphQLHttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedServiceAccessException(e);
                }
                throw new ServiceClientException("Call to GraphQL source failed.", e);
            }
        }

        private static void CheckResult<TResponse>(GraphQLResponse<TResponse> result) where TResponse : class
        {
            if (result.Errors != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("An error occurred in query definition:");
                foreach (var error in result.Errors)
                {
                    stringBuilder.AppendLine(error.Message);
                    stringBuilder.AppendLine("Location");
                    if (error.Path != null)
                    {
                        foreach (var path in error.Path)
                        {
                            stringBuilder.AppendLine(path.ToString());
                        }
                    }

                    if (error.Locations != null)
                    {
                        foreach (var location in error.Locations)
                        {
                            stringBuilder.AppendLine($"at {location.Line},{location.Column}:");
                        }
                    }

                    stringBuilder.AppendLine("----");
                }

                throw new QlQueryErrorException(stringBuilder.ToString());
            }
        }
    }
}