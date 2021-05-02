using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Ianitor.Osp.Backend.Common.Authorization
{
    /// <summary>
    /// Allows to transform a jwt token to a cookie based auth
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CookieBasedAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        public CookieBasedAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
            {
                try
                {
                    if (context.Request.Query.TryGetValue("jwt_token", out StringValues tokenStringValues))
                    {
                        if (tokenStringValues.Count > 0)
                        {
                            context.Response.Cookies.Append("OspIdentityAccessToken", tokenStringValues[0]);
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {tokenStringValues[0]}" });
                        }
                    }
                    if (context.Request.Cookies.TryGetValue("OspIdentityAccessToken", out string token))
                    {
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                }
                catch
                {
                    // if multiple headers it may throw an error.  Ignore both.
                }
            }
            await _next(context);
        }
    }
}
