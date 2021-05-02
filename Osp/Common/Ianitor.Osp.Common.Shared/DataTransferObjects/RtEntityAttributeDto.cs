using System.Collections.Generic;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class RtEntityAttributeDto : GraphQLDto
    {
        public string AttributeName { get; set; }

        public object Value { get; set; }
        public ICollection<object> Values { get; set; }
    }
}