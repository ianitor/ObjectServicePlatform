using System.Threading;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.Commands;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence
{
    internal class TenantContext : ITenantContextInternal
    {
        public TenantContext(string dataSource, ITenantRepositoryInternal tenantRepository, ICkCache ckCache)
        {
            TenantId = dataSource;
            InternalRepository = tenantRepository;
            CkCache = ckCache;
        }

        public string TenantId { get; }

        public ITenantRepository Repository => InternalRepository;

        public ITenantRepositoryInternal InternalRepository { get; }
        
        public ICkCache CkCache { get; }
        
        public async Task ExportRtModelAsync(IOspSession session, OspObjectId queryId, string filePath,
            CancellationToken? cancellationToken)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(queryId), queryId);
            ArgumentValidation.ValidateExistingFile(nameof(filePath), filePath);
            
            var exporter = new ExportRtModel(this);
            await exporter.Export(session, queryId, filePath, cancellationToken);
        }
        
        public async Task ImportRtModelAsync(IOspSession session, string filePath, CancellationToken? cancellationToken)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.ValidateFilePath(nameof(filePath), filePath);

            var importer = new ImportRtModel(this);
            await importer.Import(session, filePath, cancellationToken);
        }
        
        public async Task ImportRtModelAsTextAsync(IOspSession session, string jsonText)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.ValidateString(nameof(jsonText), jsonText);
            
            var importer = new ImportRtModel(this);
            await importer.ImportText(session, jsonText);
        }
    }
}