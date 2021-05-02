using Ianitor.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class SortOrderItem
    {
        public SortOrderItem(string attributeName, SortOrders sortOrder)
        {
            ArgumentValidation.ValidateString(nameof(attributeName), attributeName);
            
            AttributeName = attributeName;
            SortOrder = sortOrder;
        }
        
        public string AttributeName { get; }
        public SortOrders SortOrder { get; }
    }
}