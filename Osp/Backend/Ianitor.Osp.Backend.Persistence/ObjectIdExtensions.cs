using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Persistence
{
    public static class ObjectIdExtensions
    {
        public static ObjectId ToObjectId(this OspObjectId objectId)
        {
            return new ObjectId(objectId.ToString());
        }
        
        public static OspObjectId ToOspObjectId(this ObjectId objectId)
        {
            return new OspObjectId(objectId.ToByteArray());
        }
    }
}