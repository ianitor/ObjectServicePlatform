using System.Collections.Generic;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache
{
    public class TextSearchLanguageCacheItem
    {
        public TextSearchLanguageCacheItem(string language)
        {
            Language = language;
            Fields = new List<CkIndexFields>();
        }
        
        public string Language { get; }
        
        public IList<CkIndexFields> Fields { get; }
    }
}