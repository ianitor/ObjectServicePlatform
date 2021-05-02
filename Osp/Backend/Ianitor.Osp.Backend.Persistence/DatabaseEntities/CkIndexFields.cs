using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.DatabaseEntities
{
    public class CkIndexFields
    {
        [BsonIgnoreIfNull]
        public int? Weight { get; set; }
        
        public ICollection<string> AttributeNames { get; set; }
    }
}