using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public class IdentityProviderStore : IOspIdentityProviderStore
    {
        private readonly ICachedCollection<OspIdentityProvider> _providerCollection;
        private readonly IRepository _repository;

        public IdentityProviderStore(ISystemContext systemContext)
        {
            _repository = systemContext.OspSystemDatabase;

            _providerCollection = _repository.GetCollection<OspIdentityProvider>();
        }

        public async Task<OspIdentityProvider> GetAsync(string id)
        {
            ArgumentValidation.ValidateString(nameof(id), id);
            
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var result = await _providerCollection.DocumentAsync(session, id);

            await session.CommitTransactionAsync();
            return result;
        }

        public async Task<IEnumerable<OspIdentityProvider>> GetAllAsync()
        {
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();
            
            var result =  await _providerCollection.GetAsync(session);
            
            await session.CommitTransactionAsync();
            return result;
        }

        public async Task StoreAsync(OspIdentityProvider identityProvider)
        {
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();
            
            var persistentProvider =  await GetAsync(identityProvider.Alias);
            if (persistentProvider == null)
            {
                await _providerCollection.InsertAsync(session, identityProvider);
            }
            else
            {
                await _providerCollection.ReplaceByIdAsync(session, identityProvider.Id, identityProvider);
            }
            
            await session.CommitTransactionAsync();
        }

        public async Task RemoveAsync(string id)
        {
            ArgumentValidation.ValidateString(nameof(id), id);

            var session = await _repository.StartSessionAsync();
            session.StartTransaction();
            
            await _providerCollection.DeleteOneAsync(session, id);
            
            await session.CommitTransactionAsync();
        }
    }
}