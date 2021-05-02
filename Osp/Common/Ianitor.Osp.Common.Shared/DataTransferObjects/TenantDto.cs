namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    /// <summary>
    /// Data transfer object for OSP tenants
    /// </summary>
    public class TenantDto
    {
        /// <summary>
        /// Gets or sets the database name
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets Id of tenant
        /// </summary>
        public string TenantId { get; set; }
    }
}
