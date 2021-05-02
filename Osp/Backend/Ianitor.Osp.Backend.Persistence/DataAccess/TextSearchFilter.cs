
using Ianitor.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class TextSearchFilter
    {
        public TextSearchFilter(object searchTerm)
        {
            ArgumentValidation.Validate(nameof(searchTerm), searchTerm);
            
            SearchTerm = searchTerm;
        }

        public object SearchTerm { get; }
    }
}