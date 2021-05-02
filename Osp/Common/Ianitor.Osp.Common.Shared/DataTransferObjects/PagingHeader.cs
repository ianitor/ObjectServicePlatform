using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class PagingHeader  
    {  
        public PagingHeader(  
            long totalCount, int skip, int take)  
        {  
            TotalCount = totalCount;  
            Skip = skip;  
            Take = take;  
        }  
  
        public long TotalCount { get; }  
        public int Skip { get; }  
        public int Take { get; }  
  
        public string ToJson() => JsonConvert.SerializeObject(this,   
            new JsonSerializerSettings {  
                ContractResolver = new   
                    CamelCasePropertyNamesContractResolver() });  
  
    }  
}