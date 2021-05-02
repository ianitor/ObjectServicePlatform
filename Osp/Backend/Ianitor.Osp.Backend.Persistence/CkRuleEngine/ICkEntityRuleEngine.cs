using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine
{
    public interface ICkEntityRuleEngine
    {
        Task<CkEntityRuleEngineResult> ValidateAsync(IReadOnlyList<EntityUpdateInfo> entityUpdateInfos);
    }
}