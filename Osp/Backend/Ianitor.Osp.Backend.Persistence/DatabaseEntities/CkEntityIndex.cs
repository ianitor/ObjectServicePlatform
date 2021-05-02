using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.DatabaseEntities
{
    public class CkEntityIndex
    {
        public IndexTypes IndexType { get; set; }
        
        [BsonIgnoreIfNull]
        public string Language { get; set; }
        
        public ICollection<CkIndexFields> Fields { get; set; }
    }
}