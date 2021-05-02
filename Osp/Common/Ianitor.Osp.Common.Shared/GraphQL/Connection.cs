using System.Collections.Generic;

namespace Ianitor.Osp.Common.Shared.GraphQL
{
    public class Connection<TDto>
    {
        public ICollection<TDto> Edges { get; set; }

        public ICollection<TDto> Items { get; set; }

        public PageInfo PageInfo { get; set; }
        public int TotalCount { get; set; }
    }
}