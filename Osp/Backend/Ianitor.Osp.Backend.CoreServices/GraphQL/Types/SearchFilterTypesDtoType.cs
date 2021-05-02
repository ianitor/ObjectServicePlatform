using GraphQL.Types;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    public class SearchFilterTypesDtoType : EnumerationGraphType<SearchFilterTypesDto>
    {
        public SearchFilterTypesDtoType()
        {
            Name = "SearchFilterTypes";
            Description = "The type of search that is used (a text based search using text analysis (high performance, scoring, maybe more false positives) or filtering of attributes (lower performance, more exact results)";
        }
    }
}