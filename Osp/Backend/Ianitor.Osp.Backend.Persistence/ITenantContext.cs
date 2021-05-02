using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence
{
    public interface ITenantContext 
    {
        string TenantId { get; }

        ITenantRepository Repository { get; }
        ICkCache CkCache { get; }

        Task ExportRtModelAsync(IOspSession session, OspObjectId queryId, string filePath,
            CancellationToken? cancellationToken);

        Task ImportRtModelAsync(IOspSession session, string filePath,
            CancellationToken? cancellationToken);

        Task ImportRtModelAsTextAsync(IOspSession session, string jsonText);
    }
}