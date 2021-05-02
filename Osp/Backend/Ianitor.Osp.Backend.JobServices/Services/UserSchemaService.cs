using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Internationalization;
using Ianitor.Osp.Common.Shared;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.JobServices.Services
{
    internal class UserSchemaService : IUserSchemaService
    {
        private readonly ISystemContext _systemContext;
        private readonly IOspResourceStore _resourceStore;
        private readonly IOspClientStore _clientStore;
        private readonly OspJobServicesOptions _ospJobServicesOptions;

        public UserSchemaService(ISystemContext systemContext, IOspResourceStore resourceStore,
            IOspClientStore clientStore,
            IOptions<OspJobServicesOptions> ospJobServicesOptions)
        {
            _systemContext = systemContext;
            _resourceStore = resourceStore;
            _clientStore = clientStore;
            _ospJobServicesOptions = ospJobServicesOptions.Value;
        }

        public async Task SetupAsync()
        {
            using var session = await _systemContext.StartSystemSessionAsync();
            session.StartTransaction();

            var version =
                await _systemContext.GetConfigurationAsync(session, JobServiceConstants.JobServiceSchemaVersionKey, 0);
            if (version < JobServiceConstants.JobServiceSchemaVersionValue)
            {
                await CreateApiScopes();
                await CreateApiResources();
                await CreateClients();

                await _systemContext.SetConfigurationAsync(session, JobServiceConstants.JobServiceSchemaVersionKey,
                    JobServiceConstants.JobServiceSchemaVersionValue);
            }

            await session.CommitTransactionAsync();
        }
        
        private async Task CreateApiScopes()
        {
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.JobApiFullAccess,
                CommonConstants.JobApiFullAccessDisplayName));
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.JobApiReadOnly,
                CommonConstants.JobApiReadOnlyDisplayName));
        }

        private async Task CreateApiResources()
        {
            await _resourceStore.TryCreateApiResourceAsync(new ApiResource
            {
                Name = CommonConstants.JobApi,
                DisplayName = CommonConstants.JobApiDisplayName,
                Description = CommonConstants.JobApiDescription,
                Enabled = true,
                Scopes = new List<string>
                {
                    CommonConstants.JobApiFullAccess,
                    CommonConstants.JobApiReadOnly
                }
            });
        }

        private async Task CreateClients()
        {
            var ospJobServices = await _clientStore.FindClientByIdAsync(CommonConstants.JobServicesClientId);
            if (ospJobServices == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.JobServicesClientId,

                    ClientName = Texts.Backend_JobServices_UserSchema_JobServices_DisplayName,
                    ClientUri = _ospJobServicesOptions.PublicUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.Implicit},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris =
                    {
                        _ospJobServicesOptions.PublicUrl.EnsureEndsWith("/") + "signin-oidc"
                    },

                    PostLogoutRedirectUris = {_ospJobServicesOptions.PublicUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospJobServicesOptions.PublicUrl.TrimEnd('/')},
                    AllowedScopes =
                    {
                        CommonConstants.Scopes.OpenId,
                        CommonConstants.Scopes.Profile,
                        CommonConstants.Scopes.Email,
                        JwtClaimTypes.Role
                    }
                };
                await _clientStore.CreateAsync(appClient);
            }

            var ospJobServiceSwaggerClient =
                await _clientStore.FindClientByIdAsync(CommonConstants.JobServicesSwaggerClientId);
            if (ospJobServiceSwaggerClient == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.JobServicesSwaggerClientId,

                    ClientName = Texts.Backend_JobServices_UserSchema_Swagger_DisplayName,
                    ClientUri = _ospJobServicesOptions.PublicUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.AuthorizationCode},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris =
                    {
                        _ospJobServicesOptions.PublicUrl.EnsureEndsWith("/swagger/oauth2-redirect.html")
                    },

                    PostLogoutRedirectUris = {_ospJobServicesOptions.PublicUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospJobServicesOptions.PublicUrl.TrimEnd('/')},
                    AllowedScopes =
                    {
                        CommonConstants.Scopes.OpenId,
                        CommonConstants.Scopes.Profile,
                        CommonConstants.Scopes.Email,
                        JwtClaimTypes.Role,
                        CommonConstants.JobApiFullAccess,
                        CommonConstants.JobApiReadOnly
                    }
                };
                await _clientStore.CreateAsync(appClient);
            }
        }
    }
}