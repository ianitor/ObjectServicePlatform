using System;

namespace Ianitor.Osp.Backend.JobServices
{
    internal static class JobServiceConstants
    {
        /// <summary>
        /// Name of key of database schema
        /// </summary>
        public const string JobServiceSchemaVersionKey = "JobServices";
        
        /// <summary>
        /// Version of database schema for job service specific data
        /// </summary>
        public const int JobServiceSchemaVersionValue = 1;
        
        /// <summary>
        /// Timespan a cookie is expiring
        /// </summary>
        public static readonly TimeSpan CookieExpireTimeSpan = TimeSpan.FromMinutes(60);
        
        /// <summary>
        /// The name of the cookie of cookie-based auth
        /// </summary>
        public const string CookieName = "Osp-JobServices";
        
        /// <summary>
        /// Policy for authenticated users authorization
        /// </summary>
        public const string AuthenticatedUserPolicy = "AuthenticatedUserPolicy";
        
        public const string JobApiReadOnlyPolicy = "JobApiReadOnlyPolicy";
        public const string JobApiReadWritePolicy = "JobApiReadWritePolicy";
    }
}