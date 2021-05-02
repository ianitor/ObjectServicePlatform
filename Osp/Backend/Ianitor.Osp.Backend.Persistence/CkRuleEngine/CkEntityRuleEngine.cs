using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess;

namespace Ianitor.Osp.Backend.Persistence.CkRuleEngine
{
    internal class CkEntityRuleEngine : ICkEntityRuleEngine
    {
        private readonly ICkCache _ckCache;
        private readonly ITenantRepositoryInternal _tenantRepository;

        public CkEntityRuleEngine(ICkCache ckCache, ITenantRepositoryInternal tenantRepository)
        {
            _ckCache = ckCache;
            _tenantRepository = tenantRepository;
        }

        public Task<CkEntityRuleEngineResult> ValidateAsync(IReadOnlyList<EntityUpdateInfo> entityUpdateInfos)
        {
            var entityValidatorResult = new CkEntityRuleEngineResult();
            
            entityValidatorResult.RtEntitiesToCreate.AddRange(entityUpdateInfos.Where(e=> e.ModOption == EntityModOptions.Create).Select(e=> e.RtEntity));
            entityValidatorResult.RtEntitiesToUpdate.AddRange(entityUpdateInfos.Where(e=> e.ModOption == EntityModOptions.Update).Select(e=> e.RtEntity));
            entityValidatorResult.RtEntitiesToDelete.AddRange(entityUpdateInfos.Where(e=> e.ModOption == EntityModOptions.Delete).Select(e=> e.RtEntity));
            
            // Currently, no rules are defined.

            return Task.FromResult(entityValidatorResult);
        }
    }
}
