using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class UpdateStreamFilter
    {
        public UpdateTypes UpdateTypes { get; set; }
        
        public OspObjectId? RtId { get; set; }
    }
}