using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence
{
    public static class RtEntityIdExtensions
    {
        public static RtEntityId ToRtEntityId(this RtEntity rtEntity)
        {
            return new RtEntityId(rtEntity.CkId, rtEntity.RtId.ToOspObjectId());
        }
    }
}