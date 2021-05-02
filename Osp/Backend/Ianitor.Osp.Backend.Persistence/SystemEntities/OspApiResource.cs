using IdentityServer4.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    [CollectionName("ApiResources")]
    public class OspApiResource : ApiResource
    {
        /// <summary>
        /// Returns the mongodb ID
        /// </summary>
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
    }
}
