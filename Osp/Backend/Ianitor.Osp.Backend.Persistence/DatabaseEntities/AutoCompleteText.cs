using MongoDB.Bson.Serialization.Attributes;

namespace Ianitor.Osp.Backend.Persistence.DatabaseEntities
{
    public class AutoCompleteText
    {
        [BsonElement("count")]
        public int OccurrenceCount { get; set; }

        [BsonElement("_id")]
        public string Text { get; set; }
    }
}