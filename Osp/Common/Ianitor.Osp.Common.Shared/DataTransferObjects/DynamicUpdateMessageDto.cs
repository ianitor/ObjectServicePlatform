using System.Collections.Generic;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class DynamicUpdateMessageDto<TItem> where TItem : GraphQLDto
    {
        public ICollection<TItem> Items { get; set; }
    }
}