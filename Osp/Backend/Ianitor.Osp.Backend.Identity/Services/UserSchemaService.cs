using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Internationalization;
using Ianitor.Osp.Common.Shared;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Identity.Services
{
    internal class UserSchemaService : IUserSchemaService
    {
        private readonly ISystemContext _systemContext;
        private readonly UserManager<OspUser> _userManager;
        private readonly RoleManager<OspRole> _roleManager;
        private readonly IOspClientStore _clientStore;
        private readonly IOspResourceStore _resourceStore;
        private readonly IOspIdentityProviderStore _ospIdentityProviderStore;
        private readonly OspIdentityOptions _ospIdentityOptions;

        public UserSchemaService(ISystemContext systemContext, UserManager<OspUser> userManager,
            RoleManager<OspRole> roleManager, IOspClientStore clientStore, IOspResourceStore resourceStore,
            IOspIdentityProviderStore ospIdentityProviderStore, IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            _systemContext = systemContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _ospIdentityProviderStore = ospIdentityProviderStore;
            _ospIdentityOptions = ospIdentityOptions.Value;
        }

        public async Task SetupAsync()
        {
            using var session = await _systemContext.StartSystemSessionAsync();
            session.StartTransaction();

            var version =
                await _systemContext.GetConfigurationAsync(session, IdentityServiceConstants.IdentitySchemaVersionKey,
                    0);
            if (version < IdentityServiceConstants.IdentitySchemaVersionValue)
            {
                await CreateClients();
                await CreateUsersAndRoles();

                await CreateApiScopes();
                await CreateApiResources();
                await CreateIdentityResources();
                await CreateIdentityProvider();

                await _systemContext.SetConfigurationAsync(session, IdentityServiceConstants.IdentitySchemaVersionKey,
                    IdentityServiceConstants.IdentitySchemaVersionValue);
            }

            await session.CommitTransactionAsync();
        }

        private async Task CreateIdentityProvider()
        {
            var googleProvider = await _ospIdentityProviderStore.GetAsync(CommonConstants.GoogleIdentityProvider);
            if (googleProvider == null)
            {
                googleProvider = new GoogleIdentityProvider
                {
                    IsEnabled = false,
                    ClientId = "392724150963-34b8f10j23nm1rg31vi64lrb07o3aaga.apps.googleusercontent.com",
                    ClientSecret = "i0MW0fbgOiwazab4msWeBnnl",
                    Type = IdentityProviderTypes.Google,
                    Alias = CommonConstants.GoogleIdentityProvider,
                };

                await _ospIdentityProviderStore.StoreAsync(googleProvider);
            }

            var microsoftProvider = await _ospIdentityProviderStore.GetAsync(CommonConstants.MicrosoftIdentityProvider);
            if (microsoftProvider == null)
            {
                microsoftProvider = new MicrosoftIdentityProvider
                {
                    IsEnabled = false,
                    ClientId = "9697862a-d54b-429a-8526-8e0693c9ecba",
                    ClientSecret = "z8H3]C/:VQ=bJE3jCXLP4F@L-/NwoI@J",
                    Type = IdentityProviderTypes.Microsoft,
                    Alias = CommonConstants.MicrosoftIdentityProvider,
                };

                await _ospIdentityProviderStore.StoreAsync(microsoftProvider);
            }
        }

        private async Task CreateApiScopes()
        {
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.IdentityApiFullAccess,
                CommonConstants.IdentityApiFullAccessDisplayName));
            await _resourceStore.TryCreateApiScopeAsync(new ApiScope(CommonConstants.IdentityApiReadOnly,
                CommonConstants.IdentityApiReadOnlyDisplayName));
        }

        private async Task CreateApiResources()
        {
            await _resourceStore.TryCreateApiResourceAsync(new OspApiResource
            {
                Name = CommonConstants.IdentityApi,
                DisplayName = CommonConstants.IdentityApiDisplayName,
                Description = CommonConstants.IdentityApiDescription,
                Enabled = true,
                Scopes = new List<string>
                {
                    CommonConstants.IdentityApiFullAccess,
                    CommonConstants.IdentityApiReadOnly
                }
            });
        }

        private async Task CreateIdentityResources()
        {
            await _resourceStore.TryCreateIdentityResourceAsync(new IdentityResources.OpenId());
            await _resourceStore.TryCreateIdentityResourceAsync(new IdentityResources.Profile());
            await _resourceStore.TryCreateIdentityResourceAsync(new IdentityResources.Email());
            await _resourceStore.TryCreateIdentityResourceAsync(new IdentityResource
            {
                Name = JwtClaimTypes.Role,
                DisplayName = Texts.Backend_Identity_UserSchema_Roles_DisplayName,
                Description = Texts.Backend_Identity_UserSchema_Roles_Description,
                UserClaims = new List<string> {JwtClaimTypes.Role}
            });
        }

        private async Task CreateUsersAndRoles()
        {
            var adminRole = await _roleManager.FindByNameAsync(CommonConstants.AdministratorsRole);
            if (adminRole == null)
            {
                adminRole = new OspRole
                {
                    Name = CommonConstants.AdministratorsRole,
                    Claims = new List<IdentityRoleClaim<string>>()
                };
                await _roleManager.CreateAsync(adminRole);
            }

            var userRole = await _roleManager.FindByNameAsync(CommonConstants.UsersRole);
            if (userRole == null)
            {
                userRole = new OspRole
                {
                    Name = CommonConstants.UsersRole,
                    Claims = new List<IdentityRoleClaim<string>>()
                };
                await _roleManager.CreateAsync(userRole);
            }
        }

        private async Task CreateClients()
        {
            var ospToolClient = await _clientStore.FindClientByIdAsync(CommonConstants.OspToolClientId);
            if (ospToolClient == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.OspToolClientId,

                    // no interactive user, use the clientId/secret for authentication
                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.DeviceCode},

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(CommonConstants.OspToolClientSecret.Sha256())
                    },

                    AllowOfflineAccess = true,

                    // scopes that client has access to
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.Role,
                        CommonConstants.SystemApiFullAccess,
                        CommonConstants.IdentityApiFullAccess,
                        CommonConstants.JobApiFullAccess
                    }
                };

                await _clientStore.CreateAsync(appClient);
            }

            var ospIdentityServiceSwaggerClient =
                await _clientStore.FindClientByIdAsync(CommonConstants.IdentityServicesSwaggerClientId);
            if (ospIdentityServiceSwaggerClient == null)
            {
                var appClient = new OspClient
                {
                    ClientId = CommonConstants.IdentityServicesSwaggerClientId,

                    ClientName = Texts.Backend_IdentityServices_UserSchema_Swagger_DisplayName,
                    ClientUri = _ospIdentityOptions.AuthorityUrl,

                    AllowedGrantTypes = new[] {OidcConstants.GrantTypes.AuthorizationCode},

                    RequirePkce = true,
                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris =
                    {
                        _ospIdentityOptions.AuthorityUrl.EnsureEndsWith("/swagger/oauth2-redirect.html")
                    },

                    PostLogoutRedirectUris = {_ospIdentityOptions.AuthorityUrl.EnsureEndsWith("/")},
                    AllowedCorsOrigins = {_ospIdentityOptions.AuthorityUrl.TrimEnd('/')},
                    AllowedScopes =
                    {
                        CommonConstants.Scopes.OpenId,
                        CommonConstants.Scopes.Profile,
                        CommonConstants.Scopes.Email,
                        JwtClaimTypes.Role,
                        CommonConstants.IdentityApiFullAccess,
                        CommonConstants.IdentityApiReadOnly
                    }
                };
                await _clientStore.CreateAsync(appClient);
            }
        }
    }
}