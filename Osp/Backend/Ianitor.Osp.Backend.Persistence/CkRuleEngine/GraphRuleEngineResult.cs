using System.Collections.Generic;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine
{
    public class GraphRuleEngineResult
    {
        public GraphRuleEngineResult()
        {
            RtAssociationsToCreate = new List<RtAssociation>();
            RtAssociationsToDelete = new List<RtAssociation>();
        }
        
        public List<RtAssociation> RtAssociationsToCreate { get; }
        public List<RtAssociation> RtAssociationsToDelete { get; }
    }
}