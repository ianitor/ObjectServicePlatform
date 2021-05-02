using System;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.MongoDb
{
    public class MongoRepositoryClient : IRepositoryClient
    {
        private readonly MongoClient _client;

        public MongoRepositoryClient(MongoConnectionOptions mongoConnectionOptions)
        {
            ArgumentValidation.Validate(nameof(mongoConnectionOptions), mongoConnectionOptions);
            ArgumentValidation.ValidateString(nameof(mongoConnectionOptions.MongoDbHost),
                mongoConnectionOptions.MongoDbHost);


            MongoUrlBuilder urlBuilder = new MongoUrlBuilder();

            if (mongoConnectionOptions.MongoDbHost.Contains(","))
            {
                urlBuilder.Servers =
                    mongoConnectionOptions.MongoDbHost.Split(",").Select(x => new MongoServerAddress(x));
            }
            else
            {
                urlBuilder.Server = new MongoServerAddress(mongoConnectionOptions.MongoDbHost);
            }

            if (!string.IsNullOrWhiteSpace(mongoConnectionOptions.MongoDbUsername)
                && !string.IsNullOrWhiteSpace(mongoConnectionOptions.MongoDbPassword))
            {
                urlBuilder.Username = mongoConnectionOptions.MongoDbUsername;
                urlBuilder.Password = mongoConnectionOptions.MongoDbPassword;
                urlBuilder.DatabaseName = mongoConnectionOptions.DatabaseName;
                urlBuilder.AuthenticationSource = mongoConnectionOptions.AuthenticationSource;
            }

            ConfigureMongoDriver();
            _client = new MongoClient(urlBuilder.ToMongoUrl());
        }

        public async Task<bool> IsRepositoryExistingAsync(string name)
        {
            var databaseNames = await _client.ListDatabaseNamesAsync();

            return databaseNames.ToList().Any(x => string.Compare(x, name,
                StringComparison.InvariantCultureIgnoreCase) == 0);
        }


        public Task CreateRepositoryAsync(string name)
        {
            ArgumentValidation.ValidateString(nameof(name), name);

            // MongoDB creates automatically databases. This method is
            // existing to keep that in mind for other dbms
            return Task.CompletedTask;
        }

        public async Task DropRepositoryAsync(string name)
        {
            ArgumentValidation.ValidateString(nameof(name), name);

            await _client.DropDatabaseAsync(name);
        }

        public IRepository GetRepository(string name)
        {
            ArgumentValidation.ValidateString(nameof(name), name);

            return new MongoRepository(_client.GetDatabase(name));
        }

        private void ConfigureMongoDriver()
        {
            var conventionPack = new ConventionPack {new CamelCaseElementNameConvention()};
            // remove convention first to avoid duplications
            // this call of Remove method makes no errors if occurs before any Register method call
            ConventionRegistry.Remove("camelCase");
            // register convention
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
        }

        public async Task CreateUser(IOspSession session, string authenticationDatabaseName, string databaseName,
            string user,
            string password)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.ValidateString(nameof(authenticationDatabaseName), authenticationDatabaseName);
            ArgumentValidation.ValidateString(nameof(databaseName), databaseName);
            ArgumentValidation.ValidateString(nameof(user), user);
            ArgumentValidation.ValidateString(nameof(password), password);

            var database = _client.GetDatabase(authenticationDatabaseName);
            
            var result = await database.RunCommandAsync<BsonDocument>("{usersInfo: '" + user +  "'}");
            if (result.GetValue("ok").AsDouble > 0 && result.GetValue("users").AsBsonArray.Count > 0) 
            {
                return;
            }

            var createUserCommand = new BsonDocument
            {
                {"createUser", user},
                {"pwd", password},
                {
                    "roles", new BsonArray
                    {
                        new BsonDocument {{"role", "readWrite"}, {"db", databaseName}}
                    }
                }
            };

            await database.RunCommandAsync(new BsonDocumentCommand<BsonDocument>(createUserCommand));
        }
    }
}