﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess.Internal
{
    internal sealed class DatabaseContext : IDatabaseContext
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IRepositoryClient _repositoryClient;
        private readonly IRepositoryInternal _repository;

        public DatabaseContext(string dataSourceHost, string databaseName, string databaseUser, string databasePassword, string authenticationDatabaseName)
        {
            ArgumentValidation.ValidateString(dataSourceHost, nameof(dataSourceHost));
            ArgumentValidation.ValidateString(databaseName, nameof(databaseName));
            ArgumentValidation.ValidateString(databaseUser, nameof(databaseUser));
            ArgumentValidation.ValidateString(databasePassword, nameof(databasePassword));

            var sharedSettings = new MongoConnectionOptions
            {
                MongoDbHost = dataSourceHost,
                MongoDbUsername = databaseUser,
                MongoDbPassword = databasePassword,
                DatabaseName = databaseName,
                AuthenticationSource = authenticationDatabaseName
            };

            _repositoryClient = new MongoRepositoryClient(sharedSettings);
            _repository = (IRepositoryInternal) _repositoryClient.GetRepository(databaseName);

            CkEntities = _repository.GetCollection<CkEntity>();
            CkAttributes = _repository.GetCollection<CkAttribute>();
            CkEntityAssociations = _repository.GetCollection<CkEntityAssociation>();
            CkEntityInheritances = _repository.GetCollection<CkEntityInheritance>();
            RtAssociations = _repository.GetCollection<RtAssociation>();
        }
        
        public async Task<IOspSession> StartSessionAsync()
        {
            var session = await _repository.StartSessionAsync();
            return session;
        }

        public ICachedCollection<TEntity> GetRtCollection<TEntity>(string ckId) where TEntity : RtEntity, new()
        {
            var suffix = ckId.Replace(".", "_");
            return _repository.GetCollection<TEntity>(suffix);
        }

        public async Task<ICollection<CkTypeInfo>> GetCkTypeInfoAsync(IOspSession session)
        {
            var aggregate = CkEntities.Aggregate(session);

            return await AggregateCkTypeInfo(aggregate).ToListAsync();
        }

        public async Task<CkTypeInfo> GetCkTypeInfoAsync(IOspSession session, string ckId)
        {
            var ckEntity = await GetCkEntityAsync(session, ckId);
            return await GetCkTypeInfoAsync(session, ckEntity);
        }

        public async Task<CkTypeInfo> GetCkTypeInfoAsync(IOspSession session, CkEntity ckId)
        {
            var filter = Builders<CkEntity>.Filter.BuildIdFilter(ckId.CkId);

            var aggregate = CkEntities.Aggregate(session).Match(filter);

            return await AggregateCkTypeInfo(aggregate).SingleOrDefaultAsync();
        }

        private IAggregateFluent<CkTypeInfo> AggregateCkTypeInfo(IAggregateFluent<CkEntity> aggregate)
        {
            return aggregate.GraphLookup(CkEntityInheritances.GetMongoCollection(),
                    x => x.OriginCkId,
                    x => x.TargetCkId,
                    x => x.CkId,
                    (CkTypeInfo x) => x.BaseTypes, (CkBaseTypeInfo i) => i.BaseTypeDepthIndex)
                .Lookup<CkTypeInfo, CkTypeInfo>(_repository.GetCollectionName<CkEntityAssociation>(),
                    "baseTypes.originCkId",
                    "originCkId",
                    "associations.out.inherited")
                .Lookup<CkTypeInfo, CkTypeInfo>(_repository.GetCollectionName<CkEntityAssociation>(),
                    Constants.IdField,
                    "originCkId",
                    "associations.out.owned")
                .Lookup<CkTypeInfo, CkTypeInfo>(_repository.GetCollectionName<CkEntityAssociation>(),
                    "baseTypes.originCkId",
                    "targetCkId",
                    "associations.in.inherited")
                .Lookup<CkTypeInfo, CkTypeInfo>(_repository.GetCollectionName<CkEntityAssociation>(),
                    Constants.IdField,
                    "targetCkId",
                    "associations.in.owned");
        }

        public ICachedCollection<CkEntity> CkEntities { get; }
        public ICachedCollection<CkAttribute> CkAttributes { get; }
        public ICachedCollection<CkEntityAssociation> CkEntityAssociations { get; }
        public ICachedCollection<CkEntityInheritance> CkEntityInheritances { get; }
        public ICachedCollection<RtAssociation> RtAssociations { get; }

        private async Task<CkEntity> GetCkEntityAsync(IOspSession session, string ckId)
        {
            var ckEntity = await CkEntities.DocumentAsync(session, ckId);
            if (ckEntity == null)
            {
                throw new EntityNotFoundException($"'{ckId}' does not exist in database.");
            }

            return ckEntity;
        }

        public async Task UpdateCollectionsAsync(IOspSession session)
        {
            var ckEntities = (await CkEntities.GetAsync(session)).ToList();
            foreach (var ckEntity in ckEntities)
            {
                var suffix = ckEntity.CkId.Replace(".", "_");
                await _repository.CreateCollectionIfNotExistsAsync<RtEntity>(suffix);
            }
        }

        public async Task UpdateIndexAsync(IOspSession session)
        {
            var ckEntities = (await CkEntities.GetAsync(session)).ToList();

            foreach (var ckEntity in ckEntities)
            {
                string name = ckEntity.CkId.Replace(".", "_");

                var collection = GetRtCollection<RtEntity>(ckEntity.CkId);
                await collection.DropIndexAsync(name);
                await collection.DropIndexAsync("OspSystem");
            }

            foreach (var ckEntity in ckEntities)
            {
                foreach (var index in ckEntity.Indexes)
                {
                    if (index.IndexType == IndexTypes.None)
                    {
                        continue;
                    }

                    var collection = GetRtCollection<RtEntity>(ckEntity.CkId);

                    string newName = ckEntity.CkId.Replace(".", "_") + "_" + ObjectId.GenerateNewId();

                    switch (index.IndexType)
                    {
                        case IndexTypes.Ascending:
                            await collection.CreateAscendingIndexAsync(newName,
                                index.Fields.SelectMany(x => x.AttributeNames));
                            break;
                        case IndexTypes.Text:
                            await collection.CreateTextIndexAsync( newName, index.Language, index.Fields);
                            break;
                        default:
                            throw new NotImplementedException($"Index type {index.IndexType} is not implemented.");
                    }
                }
            }
        }
    }
}