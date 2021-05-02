using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.MongoDb
{
    /// <summary>
    /// MongoDB CRUD operations implementation.
    /// </summary>
    public class MongoRepository : IRepositoryInternal
    {
        private readonly Dictionary<Type, string> _collectionNameMapping = new Dictionary<Type, string>();

        private readonly IMongoDatabase _database;

        public MongoRepository(IMongoDatabase mongoDatabase)
        {
            _database = mongoDatabase;
        }
        
        public async Task<IOspSession> StartSessionAsync()
        {
            var session = await _database.Client.StartSessionAsync();
            return new OspSession(session);
        }

        public async Task CreateCollectionIfNotExistsAsync<TCollection>(string suffix = null) where TCollection : class, new()
        {
            if (!await CollectionExistsAsync<TCollection>(suffix))
            {
                var name = GetCollectionName<TCollection>(suffix);
                await _database.CreateCollectionAsync(name);
            }
        }


        private async Task<bool> CollectionExistsAsync<T>(string suffix = null) where T : class, new()
        {
            var name = GetCollectionName<T>(suffix);

            var filter = new BsonDocument("name", name);
            //filter by collection name
            var collections = await _database.ListCollectionsAsync(new ListCollectionsOptions {Filter = filter});
            //check for existence
            return await collections.AnyAsync();
        }

        public string GetCollectionName<T>(string suffix = null) where T : class, new()
        {
            if (!_collectionNameMapping.TryGetValue(typeof(T), out var name))
            {
                var collectionNameAttribute =
                    (CollectionNameAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute));
                if (collectionNameAttribute == null)
                {
                    name = typeof(T).Name;
                }
                else
                {
                    name = collectionNameAttribute.CollectionName;
                }

                _collectionNameMapping.Add(typeof(T), name);
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                return name + "_" + suffix;
            }

            return name;
        }

        public ICachedCollection<T> GetCollection<T>(string suffix = null) where T : class, new()
        {
            var name = GetCollectionName<T>(suffix);

            return new CachedCollection<T>(_database.GetCollection<T>(name));
        }
    }
}