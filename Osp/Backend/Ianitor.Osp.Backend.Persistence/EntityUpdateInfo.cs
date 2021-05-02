using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence
{
    public class EntityUpdateInfo
    {
        public EntityUpdateInfo(RtEntity rtEntity, EntityModOptions modOption)
        {
            RtEntity = rtEntity;
            ModOption = modOption;
        }
        
        public RtEntity RtEntity { get;  }
        
        public EntityModOptions ModOption { get;  }
    }
}