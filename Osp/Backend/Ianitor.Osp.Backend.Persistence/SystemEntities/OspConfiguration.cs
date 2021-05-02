using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    [CollectionName("OspConfigurations")]
    public class OspConfiguration
    {
        [BsonId(IdGenerator = typeof(NullIdChecker))]
        public string Key { get; set; }
        public object Value { get; set; }
    }
}