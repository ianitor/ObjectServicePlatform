
using System.Collections.Generic;
using Ianitor.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class AttributeSearchFilter
    {
        public AttributeSearchFilter(IEnumerable<string> attributeNames, object searchTerm)
        {
            ArgumentValidation.Validate(nameof(attributeNames), attributeNames);
            ArgumentValidation.Validate(nameof(searchTerm), searchTerm);

            AttributeNames = attributeNames;
            SearchTerm = searchTerm;
        }
        
        public object SearchTerm { get; }
        
        public IEnumerable<string> AttributeNames { get; }
    }
}