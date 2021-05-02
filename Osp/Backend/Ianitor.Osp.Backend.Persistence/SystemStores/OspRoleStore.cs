using AspNetCore.Identity.Mongo.Stores;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
  public class OspRoleStore : RoleStore<OspRole, ObjectId>
  {
    public OspRoleStore(ISystemContext context)
      : base(((IRepositoryInternal)context.OspSystemDatabase).GetCollection<OspRole>().GetMongoCollection())
    {
    }
  }
}
