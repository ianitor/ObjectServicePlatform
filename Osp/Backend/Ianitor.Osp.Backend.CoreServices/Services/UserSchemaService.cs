using Ianitor.Osp.Backend.Persistence;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection.Options;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Shared;
using IdentityModel;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.CoreServices.Services
{
    internal class UserSchemaService : IUserSchemaService
    {
        private readonly ISystemContext _systemContext;
        private readonly IOspResourceStore _resourceStore;
        private readonly IOspClientStore _clientStore;
        private readonly OspCoreServicesOptions _ospCoreServicesOptions;

        public UserSchemaService(IOspService ospService, IOspResourceStore resourceStore, IOspClientStore clientStore,
            OspCoreServicesOptions ospCoreServicesOptions)
        {
            _systemContext = ospService.SystemContext;
            _resourceStore = resourceStore;
            _clientStore = clientStore;
            _ospCoreServicesOptions = ospCoreServicesOptions;
        }

        public async Task SetupAsync()
        {
            using var session = await _systemContext.StartSystemSessionAsync();
            session.StartTransaction();

            var version =
                await _systemContext.GetConfigurationAsync(session, CoreServiceConstants.CoreServiceSchemaVersionKey,
                    0);
            if (version < CoreServiceConstants.CoreServiceSchemaVersionValue)
            {
                await CreateApiScopes();
                await CreateApiResources();
                await CreateClients();

                await _systemContext.SetConfigurationAsync(session, CoreServiceConstants.CoreServiceSchemaVersionKey,
                    CoreServiceConstants.CoreServiceSchemaVersionValue);
            }

            await session.CommitTransactionAsync();
        }

        private async Task CreateApiScopes()
        {
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.SystemApiFullAccess,
                CommonConstants.SystemApiFullAccessDisplayName));
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.SystemApiReadOnly,
                CommonConstants.SystemApiReadOnlyDisplayName));
        }

        private async Task CreateApiResources()
        {
            await _resourceStore.TryCreateApiResourceAsync(
                new ApiResource(CommonConstants.SystemApi, CommonConstants.SystemApiDisplayName)
                {
                    Description = CommonConstants.SystemApiDescription,
                    Enabled = true,
                    Scopes = new List<string>
                    {
                        CommonConstants.SystemApiFullAccess,
                        CommonConstants.SystemApiReadOnly
                    }
                });
        }

        private async Task CreateClients()
        {
            var ospDashboard = await _clientStore.FindClientByIdAsync(CommonConstants.OspDashboardClientId);
            if (ospDashboard == null)
            {
                var dashboardClient = new OspClient
                {
                    ClientId = CommonConstants.OspDashboardClientId,

                    ClientName = Texts.Backend_CoreServices_UserSchema_Dashboard_DisplayName,
                    ClientUri = _ospCoreServicesOptions.PublicDashboardUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.AuthorizationCode},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        _ospCoreServicesOptions.PublicDashboardUrl.EnsureEndsWith("/")
                    },

                    PostLogoutRedirectUris = {_ospCoreServicesOptions.PublicDashboardUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospCoreServicesOptions.PublicDashboardUrl.TrimEnd('/')},
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        CommonConstants.Scopes.OpenId,
                        CommonConstants.Scopes.Profile,
                        CommonConstants.Scopes.Email,
                        JwtClaimTypes.Role,
                        CommonConstants.SystemApiFullAccess,
                        CommonConstants.IdentityApiFullAccess,
                        CommonConstants.JobApiFullAccess
                    }
                };
                await _clientStore.CreateAsync(dashboardClient);
            }

            var ospCoreServices = await _clientStore.FindClientByIdAsync(CommonConstants.CoreServicesClientId);
            if (ospCoreServices == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.CoreServicesClientId,

                    ClientName = Texts.Backend_CoreServices_UserSchema_CoreServices_DisplayName,
                    ClientUri = _ospCoreServicesOptions.PublicUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.Implicit},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris =
                    {
                        _ospCoreServicesOptions.PublicUrl.EnsureEndsWith("/signin-oidc")
                    },

                    PostLogoutRedirectUris = {_ospCoreServicesOptions.PublicUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospCoreServicesOptions.PublicUrl.TrimEnd('/')},
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

            var ospCoreServiceSwaggerClient =
                await _clientStore.FindClientByIdAsync(CommonConstants.CoreServicesSwaggerClientId);
            if (ospCoreServiceSwaggerClient == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.CoreServicesSwaggerClientId,

                    ClientName = Texts.Backend_CoreServices_UserSchema_Swagger_DisplayName,
                    ClientUri = _ospCoreServicesOptions.PublicUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.AuthorizationCode},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris =
                    {
                        _ospCoreServicesOptions.PublicUrl.EnsureEndsWith("/swagger/oauth2-redirect.html")
                    },

                    PostLogoutRedirectUris = {_ospCoreServicesOptions.PublicUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospCoreServicesOptions.PublicUrl.TrimEnd('/')},
                    AllowedScopes =
                    {
                        CommonConstants.Scopes.OpenId,
                        CommonConstants.Scopes.Profile,
                        CommonConstants.Scopes.Email,
                        JwtClaimTypes.Role,
                        CommonConstants.SystemApiFullAccess,
                        CommonConstants.SystemApiReadOnly
                    }
                };
                await _clientStore.CreateAsync(appClient);
            }
        }
    }
}