using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using IdentityServer4.Models;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public class ResourceStore : IOspResourceStore
    {
        private readonly ICachedCollection<OspApiResource> _apiResourceCollection;
        private readonly ICachedCollection<OspIdentityResource> _identityResourceCollection;
        private readonly ICachedCollection<OspApiScope> _apiScopeCollection;
        private readonly IRepository _repository;

        public ResourceStore(ISystemContext systemContext)
        {
            _repository = systemContext.OspSystemDatabase;

            _apiResourceCollection = _repository.GetCollection<OspApiResource>();
            _identityResourceCollection = _repository.GetCollection<OspIdentityResource>();
            _apiScopeCollection = _repository.GetCollection<OspApiScope>();
        }

        public async Task CreateApiResourceAsync(OspApiResource apiResource)
        {
            ArgumentValidation.Validate(nameof(apiResource), apiResource);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _apiResourceCollection.InsertAsync(session, apiResource);

                await session.CommitTransactionAsync();
            }
        }

        public async Task CreateIdentityResourceAsync(OspIdentityResource identityResource)
        {
            ArgumentValidation.Validate(nameof(identityResource), identityResource);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _identityResourceCollection.InsertAsync(session, identityResource);

                await session.CommitTransactionAsync();
            }
        }

        public async Task CreateApiScopeAsync(OspApiScope apiScope)
        {
            ArgumentValidation.Validate(nameof(apiScope), apiScope);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _apiScopeCollection.InsertAsync(session, apiScope);

                await session.CommitTransactionAsync();
            }
        }

        public async Task<OspApiResource> TryCreateApiResourceAsync(ApiResource apiResource)
        {
            var res = await GetApiResourceByNameAsync(apiResource.Name);
            if (res == null)
            {
                res = new OspApiResource
                {
                    Description = apiResource.Description,
                    Name = apiResource.Name,
                    Enabled = apiResource.Enabled,
                    DisplayName = apiResource.DisplayName,
                    ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
                    Properties = new Dictionary<string, string>(apiResource.Properties),
                    UserClaims = new List<string>(apiResource.UserClaims),
                    Scopes = apiResource.Scopes
                };

                await CreateApiResourceAsync(res);
            }

            return res;
        }


        public async Task<OspIdentityResource> TryCreateIdentityResourceAsync(IdentityResource identityResource)
        {
            var res = await GetIdentityResourceByNameAsync(identityResource.Name);
            if (res == null)
            {
                res = new OspIdentityResource
                {
                    Description = identityResource.Description,
                    Name = identityResource.Name,
                    Enabled = identityResource.Enabled,
                    DisplayName = identityResource.DisplayName,
                    Emphasize = identityResource.Emphasize,
                    Required = identityResource.Required,
                    ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                    Properties = new Dictionary<string, string>(identityResource.Properties),
                    UserClaims = new List<string>(identityResource.UserClaims)
                };

                await CreateIdentityResourceAsync(res);
            }

            return res;
        }

        public async Task<OspApiScope> TryCreateApiScopeAsync(ApiScope apiScope)
        {
            var res = (await FindApiScopesByNameAsync(new[] {apiScope.Name})).ToArray();
            if (!res.Any())
            {
                var dbApiScope = new OspApiScope
                {
                    Description = apiScope.Description,
                    Name = apiScope.Name,
                    Enabled = apiScope.Enabled,
                    DisplayName = apiScope.DisplayName,
                    Emphasize = apiScope.Emphasize,
                    Required = apiScope.Required,
                    ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                    Properties = new Dictionary<string, string>(apiScope.Properties),
                    UserClaims = new List<string>(apiScope.UserClaims)
                };

                await CreateApiScopeAsync(dbApiScope);

                return dbApiScope;
            }

            return (OspApiScope) res.First();
        }


        public async Task DeleteApiResourceAsync(string resourceId)
        {
            ArgumentValidation.ValidateString(nameof(resourceId), resourceId);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _apiResourceCollection.DeleteOneAsync(session, resourceId);
            }
        }

        public async Task DeleteIdentityResourceAsync(string resourceId)
        {
            ArgumentValidation.ValidateString(nameof(resourceId), resourceId);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _identityResourceCollection.DeleteOneAsync(session, resourceId);
            }
        }
        
        public async Task DeleteApiScopeAsync(string resourceId)
        {
            ArgumentValidation.ValidateString(nameof(resourceId), resourceId);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                await _apiScopeCollection.DeleteOneAsync(session, resourceId);
            }
        }

        public async Task<OspApiResource> GetApiResourceByNameAsync(string apiResourceName)
        {
            ArgumentValidation.ValidateString(nameof(apiResourceName), apiResourceName);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result =
                    await _apiResourceCollection.FindSingleOrDefaultAsync(session, x => x.Name == apiResourceName);

                await session.CommitTransactionAsync();

                return result;
            }
        }

        public async Task<OspIdentityResource> GetIdentityResourceByNameAsync(string identityResourceName)
        {
            ArgumentValidation.ValidateString(nameof(identityResourceName), identityResourceName);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result =
                    await _identityResourceCollection.FindSingleOrDefaultAsync(session,
                        x => x.Name == identityResourceName);

                await session.CommitTransactionAsync();

                return result;
            }
        }


        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            ArgumentValidation.Validate(nameof(scopeNames), scopeNames);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result = await _identityResourceCollection.FindManyAsync(session, x => scopeNames.Contains(x.Name));

                await session.CommitTransactionAsync();

                return result;
            }
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            ArgumentValidation.Validate(nameof(scopeNames), scopeNames);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result = await _apiScopeCollection.FindManyAsync(session, x => scopeNames.Contains(x.Name));

                await session.CommitTransactionAsync();

                return result;
            }
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            ArgumentValidation.Validate(nameof(scopeNames), scopeNames);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result =
                    await _apiResourceCollection.FindManyAsync(session, api => api.Scopes.Any(x=> scopeNames.Contains(x)));

                await session.CommitTransactionAsync();

                return result;
            }
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            ArgumentValidation.Validate(nameof(apiResourceNames), apiResourceNames);

            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var result =
                    await _apiResourceCollection.FindManyAsync(session, x => apiResourceNames.Contains(x.Name));

                await session.CommitTransactionAsync();

                return result;
            }
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            using (var session = await _repository.StartSessionAsync())
            {
                session.StartTransaction();

                var identityResources = await _identityResourceCollection.GetAsync(session);
                var apiResources = await _apiResourceCollection.GetAsync(session);
                var apiScopes = await _apiScopeCollection.GetAsync(session);

                await session.CommitTransactionAsync();

                return new Resources(identityResources, apiResources, apiScopes);
            }
        }
    }
}