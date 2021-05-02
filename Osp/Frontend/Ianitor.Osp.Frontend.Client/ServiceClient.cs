using System;
using System.Net;
using Ianitor.Common.Shared;
using RestSharp;

namespace Ianitor.Osp.Frontend.Client
{
    public abstract class ServiceClient
    {
        private RestClient _client;

        protected RestClient Client
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

        protected ServiceClient(ServiceClientOptions options, IServiceClientAccessToken accessToken)
        {
            ArgumentValidation.Validate(nameof(options), options);
            ArgumentValidation.Validate(nameof(accessToken), accessToken);

            Options = options;
            AccessToken = accessToken;
        }
        
        protected ServiceClient(ServiceClientOptions options)
        {
            ArgumentValidation.Validate(nameof(options), options);

            Options = options;
        }

        public void Initialize()
        {
            ServiceUri = BuildServiceUri();
            _client = new RestClient(ServiceUri);

            if (AccessToken != null)
            {
                AccessToken.AccessTokenUpdated += (sender, args) =>
                    UpdateAccessToken(AccessToken.AccessToken);
                UpdateAccessToken(AccessToken.AccessToken);
            }
        }

        private void UpdateAccessToken(string accessToken)
        {
            Client.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"bearer {accessToken}",
                ParameterType.HttpHeader));
        }

        protected abstract Uri BuildServiceUri();

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public ServiceClientOptions Options { get; }
        public IServiceClientAccessToken AccessToken { get; }

        public Uri ServiceUri { get; private set; }

        protected static void ValidateResponse(IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedServiceAccessException(response.ErrorException);
                }
                
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    throw new ServiceClientException(response.ErrorMessage, response.ErrorException);
                }

                throw new ServiceClientResultException(response.Content, response.StatusCode);
            }
        }
    }
}