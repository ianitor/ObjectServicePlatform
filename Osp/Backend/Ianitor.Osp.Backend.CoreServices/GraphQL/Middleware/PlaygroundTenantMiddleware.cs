using System;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Server.Ui.Playground;
using Ianitor.Osp.Backend.CoreServices.Services;
using Microsoft.AspNetCore.Http;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware
{
    /// <summary>
    /// A middleware for Playground
    /// </summary>
    public class PlaygroundTenantMiddleware
    {
        private readonly PlaygroundOptions _options;


        private readonly IOspService _ospService;

        /// <summary>
        /// The page model used to render Playground
        /// </summary>
        private PlaygroundPageModel _pageModel;

        private string _lastTenantId;

        /// <summary>
        /// Create a new <see cref="PlaygroundMiddleware"/>
        /// </summary>
        /// <param name="next">The next request delegate; not used, this is a terminal middleware.</param>
        /// <param name="ospService">OSP service instance to check the tenantId</param>
        /// <param name="options">Options to customize middleware</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter",
            Justification = "ASP.NET Core conventions")]
        // ReSharper disable once UnusedParameter.Local
        public PlaygroundTenantMiddleware(RequestDelegate next, IOspService ospService, PlaygroundOptions options)
        {
            _ospService = ospService;
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        /// <summary>
        /// Try to execute the logic of the middleware
        /// </summary>
        /// <param name="httpContext">The HttpContext</param>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var tenantId = httpContext.GetTenantId();
            using var systemSession = await _ospService.SystemContext.StartSystemSessionAsync();
            systemSession.StartTransaction();

            if (string.IsNullOrWhiteSpace(tenantId) ||
                !await _ospService.SystemContext.IsTenantExistingAsync(systemSession, tenantId))
            {
                httpContext.Response.StatusCode = 403; //NotFound
                await httpContext.Response.WriteAsync("Invalid tenant");
                return;
            }

            await systemSession.CommitTransactionAsync();

            await InvokePlayground(httpContext.Response, tenantId);
        }

        private async Task InvokePlayground(HttpResponse httpResponse, string tenantId)
        {
            httpResponse.ContentType = "text/html";
            httpResponse.StatusCode = 200;

            if (String.Compare(tenantId, _lastTenantId, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _lastTenantId = tenantId;
                _pageModel =
                    new PlaygroundPageModel(_options.GraphQLEndPoint.Value.Replace("{tenantId}", tenantId),
                        _options);
            }

            var data = Encoding.UTF8.GetBytes(_pageModel.Render());
            await httpResponse.Body.WriteAsync(data, 0, data.Length);
        }
    }
}