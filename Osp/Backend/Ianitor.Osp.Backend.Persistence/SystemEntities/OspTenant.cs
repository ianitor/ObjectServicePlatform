using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

// ReSharper disable UnusedMember.Global

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    [CollectionName("OspTenant")]
    public class OspTenant 
    {
        [BsonId(IdGenerator = typeof(NullIdChecker))]
        public string TenantId { get; set; }

        public string DatabaseName { get; set; }
    }
}
