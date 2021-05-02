using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using IdentityServer4.Models;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    public class ClientStore : IOspClientStore
    {
        private readonly ICachedCollection<OspClient> _clientCollection;
        private readonly IRepository _repository;

        public ClientStore(ISystemContext systemContext)
        {
            _repository = systemContext.OspSystemDatabase;
            _clientCollection = _repository.GetCollection<OspClient>();
        }

        public async Task CreateAsync(OspClient ospClient)
        {
            ArgumentValidation.Validate(nameof(ospClient), ospClient);
            
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            await _clientCollection.InsertAsync(session, ospClient);

            await session.CommitTransactionAsync();
        }

        public async Task DeleteAsync(string clientId)
        {
            ArgumentValidation.ValidateString(nameof(clientId), clientId);
            
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var client = await GetClientByClientId(session, clientId);
            if (client == null)
            {
                throw new EntityNotFoundException($"Client id '{clientId}' does not exist.");
            }

            await _clientCollection.DeleteOneAsync(session, client.Id);
            
            await session.CommitTransactionAsync();

        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            ArgumentValidation.ValidateString(nameof(clientId), clientId);
            
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var result = await _clientCollection.FindSingleOrDefaultAsync(session, x => x.ClientId == clientId);
            
            await session.CommitTransactionAsync();
            return result;
        }

        public async Task<IEnumerable<OspClient>> GetClients()
        {
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();
            
            var result = await _clientCollection.GetAsync(session);
            
            await session.CommitTransactionAsync();
            return result;
        }

        public async Task UpdateAsync(string clientId, OspClient client)
        {
            ArgumentValidation.ValidateString(nameof(clientId), clientId);
            ArgumentValidation.Validate(nameof(client), client);
            
            var session = await _repository.StartSessionAsync();
            session.StartTransaction();

            var dbClient = await GetClientByClientId(session, clientId);
            if (dbClient == null)
            {
                throw new EntityNotFoundException($"Client id '{clientId}' does not exist.");
            }

            await _clientCollection.ReplaceByIdAsync(session, dbClient.Id, client);
            
            await session.CommitTransactionAsync();
        }

        private async Task<OspClient> GetClientByClientId(IOspSession session, string clientId)
        {
            var client = await _clientCollection.FindSingleOrDefaultAsync(session, x => x.ClientId == clientId);
            return client;
        }
    }
}