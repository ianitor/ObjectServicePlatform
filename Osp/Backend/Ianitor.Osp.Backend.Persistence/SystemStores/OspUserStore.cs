using AspNetCore.Identity.Mongo.Stores;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
  public class OspUserStore : UserStore<OspUser, OspRole, ObjectId>
  {
    public OspUserStore(ISystemContext context, IRoleStore<OspRole> roleStore, ILookupNormalizer normalizer)
      : base(((IRepositoryInternal)context.OspSystemDatabase).GetCollection<OspUser>().GetMongoCollection(), roleStore, normalizer)
    {
    }
  }
}
