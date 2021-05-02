using IdentityServer4.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    /// <summary>
    /// Represents a client application
    /// </summary>
    [CollectionName("Clients")]
    public class OspClient : Client
    {
        /// <summary>
        /// Returns the mongodb ID
        /// </summary>
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
    }
}
