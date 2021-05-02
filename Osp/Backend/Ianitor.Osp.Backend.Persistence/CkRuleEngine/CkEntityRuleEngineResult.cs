using System.Collections.Generic;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine
{
    public class CkEntityRuleEngineResult
    {
        public CkEntityRuleEngineResult()
        {
            RtEntitiesToCreate = new List<RtEntity>();
            RtEntitiesToUpdate = new List<RtEntity>();
            RtEntitiesToDelete = new List<RtEntity>();
        }

        public List<RtEntity> RtEntitiesToCreate { get; }
        public List<RtEntity> RtEntitiesToUpdate { get; }
        public List<RtEntity> RtEntitiesToDelete { get; }
    }
}