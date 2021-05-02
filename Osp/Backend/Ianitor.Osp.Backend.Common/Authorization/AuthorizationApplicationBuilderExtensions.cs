using Ianitor.Common.Shared;
using Microsoft.AspNetCore.Builder;

namespace Ianitor.Osp.Backend.Common.Authorization
{
    /// <summary>
    /// Extensions of Application Builder for non-mvc based authorization
    /// </summary>
    public static class AuthorizationApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds authorization for the current application builder
        /// </summary>
        /// <param name="app"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app, string policyName)
        {
            // Null checks removed for brevity
            ArgumentValidation.ValidateString(nameof(policyName), policyName);

            return app.UseMiddleware<AuthorizationMiddleware>(policyName);
        }
    }
}