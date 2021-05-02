namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class SearchFilterDto
    {
        public SearchFilterTypesDto? Type { get; set; }
        
        public string Language { get; set; }
        public string SearchTerm { get; set; }
        
        public string[] AttributeNames { get; set; }
    }
}