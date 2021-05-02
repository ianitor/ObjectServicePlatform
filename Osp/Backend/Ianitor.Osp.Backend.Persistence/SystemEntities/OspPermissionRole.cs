using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
  [CollectionName("PermissionRoles")]
  public class OspPermissionRole
  {
    /// <summary>
    /// Returns the mongodb ID
    /// </summary>
    [BsonId]
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }

    public string RoleId { get; set; }

    public string Name { get; set; }

    public ICollection<string> SubjectIds { get; set; }
    public ICollection<string> IdentityRoleIds { get; set; }
  }
}
