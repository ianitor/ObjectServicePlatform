namespace Ianitor.Osp.Backend.Persistence
{
    /// <summary>
    /// Represents an OSP data source
    /// </summary>
    public class OspTenant
    {
        public OspTenant(string tenantId, string databaseName)
        {
            TenantId = tenantId;
            DatabaseName = databaseName;
        }
        
        public string TenantId { get; }
        public string DatabaseName { get; }
    }
}