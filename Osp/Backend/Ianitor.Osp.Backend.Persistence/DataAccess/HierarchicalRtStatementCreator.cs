using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal class HierarchicalRtStatementCreator : RtStatementCreator
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly ObjectId _rtId;
        private readonly GraphDirections _graphDirections;
        private readonly string _targetCkId;

        internal HierarchicalRtStatementCreator(EntityCacheItem entityCacheItem, IDatabaseContext databaseContext,
            string language, ObjectId rtId, GraphDirections graphDirections, string targetCkId) : base(entityCacheItem, databaseContext, language)
        {
            _databaseContext = databaseContext;
            _rtId = rtId;
            _graphDirections = graphDirections;
            _targetCkId = targetCkId;
        }
        
        protected override IAggregateFluent<RtEntity> CreateAggregate(IOspSession ospSession)
        {
            var targetRtIdAttribute = nameof(RtAssociation.TargetRtId);

            var matchFilter = Builders<RtAssociation>.Filter.And(
                Builders<RtAssociation>.Filter.Eq(x => x.OriginRtId, _rtId),
                Builders<RtAssociation>.Filter.Eq(x=> x.TargetCkId, _targetCkId));
            
            var targetCollection = _databaseContext.GetRtCollection<RtEntity>(_targetCkId);

            if (_graphDirections.HasFlag(GraphDirections.Inbound))
            {
                matchFilter =  Builders<RtAssociation>.Filter.And(
                    Builders<RtAssociation>.Filter.Eq(x => x.TargetRtId, _rtId),
                    Builders<RtAssociation>.Filter.Eq(x=> x.OriginCkId, _targetCkId));

                targetRtIdAttribute = nameof(RtAssociation.OriginRtId);
            }
            
            var aggregate = _databaseContext.RtAssociations.Aggregate(ospSession)
                .Match(matchFilter).Lookup(targetCollection.GetMongoCollection().CollectionNamespace.CollectionName,
                    targetRtIdAttribute.ToCamelCase(), "_id", "children")
                .ReplaceRoot<RtEntity>(new BsonDocument("$arrayElemAt", new BsonArray().Add("$children").Add(0)));

            return aggregate;
        }
    }
}