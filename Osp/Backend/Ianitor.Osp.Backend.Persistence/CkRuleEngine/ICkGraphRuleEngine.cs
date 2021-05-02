using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine
{
    internal interface ICkGraphRuleEngine
    {
        Task<GraphRuleEngineResult> ValidateAsync(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList);
        Task<GraphRuleEngineResult> ValidateAsync(IOspSession session, IReadOnlyList<EntityUpdateInfo> entityUpdateInfoList, IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList);
        Task<GraphRuleEngineResult> ValidateAsync(IOspSession session, IReadOnlyList<AssociationUpdateInfo> associationUpdateInfoList);
    }
}