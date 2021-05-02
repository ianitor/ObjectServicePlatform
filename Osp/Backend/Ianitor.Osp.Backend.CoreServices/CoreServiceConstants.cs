using System;

namespace Ianitor.Osp.Backend.CoreServices
{
    /// <summary>
    /// Common constants for core services
    /// </summary>
    public static class CoreServiceConstants
    {
        internal const string SystemApiReadOnlyPolicy = "SystemApiReadOnlyPolicy";
        internal const string SystemApiReadWritePolicy = "SystemApiReadWritePolicy";
        internal const string AuthenticatedUserPolicy = "AuthenticatedUserPolicy";
        internal const string TenantApiReadWritePolicy = "TenantApiReadWritePolicy";
        
        /// <summary>
        /// Name of key of database schema
        /// </summary>
        public const string CoreServiceSchemaVersionKey = "CoreServices";

        /// <summary>
        /// Version of database schema for core service specific data
        /// </summary>
        public const int CoreServiceSchemaVersionValue = 1;

        /// <summary>
        /// The name of the cookie of cookie-based auth
        /// </summary>
        public const string CookieName = "Osp-CoreServices";
        
        /// <summary>
        /// Timespan a cookie is expiring
        /// </summary>
        public static readonly TimeSpan CookieExpireTimeSpan = TimeSpan.FromMinutes(60);
    }
}