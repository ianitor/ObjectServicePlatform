using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    [CollectionName("Permissions")]
    public class OspPermission
    {
        /// <summary>
        /// Returns the mongodb ID
        /// </summary>
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string PermissionId { get; set; }

        public ICollection<string> PermissionRoleIds { get; set; }
    }
}