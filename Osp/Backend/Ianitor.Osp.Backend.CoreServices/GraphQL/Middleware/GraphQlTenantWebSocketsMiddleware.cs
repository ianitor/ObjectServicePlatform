using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware
{
    public class GraphQlTenantWebSocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOspService _ospService;
        private readonly ILogger<GraphQlTenantWebSocketsMiddleware> _logger;

        public GraphQlTenantWebSocketsMiddleware(
            RequestDelegate next,
            IOspService ospService,
            ILogger<GraphQlTenantWebSocketsMiddleware> logger)
        {
            _next = next;
            _ospService = ospService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
#if !DEBUG
                if (!context.User.Identity.IsAuthenticated)
                {
                    var authorizationResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                    if (authorizationResult.Succeeded)
                    {
                        context.User = authorizationResult.Principal;
                    }
                    else
                    {
                        context.Response.StatusCode = 401; //Unauthorized
                        return;
                    }
                }
#endif
            var tenantId = context.GetTenantId();
            using var systemSession = await _ospService.SystemContext.StartSystemSessionAsync();
            systemSession.StartTransaction();

            if (string.IsNullOrWhiteSpace(tenantId) ||
                !await _ospService.SystemContext.IsTenantExistingAsync(systemSession, tenantId))
            {
                context.Response.StatusCode = 403; //NotFound
                await context.Response.WriteAsync("Invalid tenantId");
                return;
            }

            await systemSession.CommitTransactionAsync();

            await HandleGraphQL(context, tenantId);
        }

        //
        // private async Task HandleGraphQL(HttpContext context, string tenantId)
        // { 
        //     var socket = await context.WebSockets.AcceptWebSocketAsync("graphql-ws");
        //
        //     if (!context.WebSockets.WebSocketRequestedProtocols.Contains(socket.SubProtocol))
        //     {
        //         _logger.LogError(
        //             "Websocket connection does not have correct protocol: graphql-ws. Request protocols: {protocols}",
        //             context.WebSockets.WebSocketRequestedProtocols);
        //
        //         await socket.CloseAsync(
        //             WebSocketCloseStatus.ProtocolError,
        //             "Server only supports graphql-ws protocol",
        //             context.RequestAborted);
        //
        //         return;
        //     }
        //
        //     using (_logger.BeginScope($"GraphQL websocket connection: {context.Connection.Id}"))
        //     {
        //         var connectionLogger =
        //             context.RequestServices.GetService<ILogger<WebSocketConnectionFactory<OspSchema>>>();
        //         var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
        //
        //         var tenantContext = await _ospService.SystemContext.CreateOrGetTenantContext(tenantId);
        //
        //         var schemaContext = context.RequestServices.GetRequiredService<ISchemaContext>();
        //         var messageListeners = context.RequestServices
        //             .GetRequiredService<IEnumerable<IOperationMessageListener>>().Union(new[]
        //             {
        //                 new OspMessageListener(tenantContext)
        //             });
        //         var documentWriter = context.RequestServices.GetRequiredService<IDocumentWriter>();
        //
        //      //   var executor = await schemaContext.CreateExecutor(tenantId, context.RequestServices);
        //
        //         // var connectionFactory = new WebSocketConnectionFactory<OspSchema>(connectionLogger, loggerFactory,
        //         //     executor, messageListeners, documentWriter);
        //         // var connection = connectionFactory.CreateConnection(socket, context.Connection.Id);
        //         //
        //         // await connection.Connect();
        //     }
        // }

        private async Task HandleGraphQL(HttpContext context, string tenantId)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["ConnectionId"] = context.Connection.Id,
                ["Request"] = context.Request
            }))
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    _logger.LogDebug("Request is not a valid websocket request");
                    await _next(context);

                    return;
                }

                _logger.LogDebug("Connection is a valid websocket request");

                var socket = await context.WebSockets.AcceptWebSocketAsync("graphql-ws");

                if (!context.WebSockets.WebSocketRequestedProtocols.Contains(socket.SubProtocol))
                {
                    _logger.LogError(
                        "Websocket connection does not have correct protocol: graphql-ws. Request protocols: {protocols}",
                        context.WebSockets.WebSocketRequestedProtocols);

                    await socket.CloseAsync(
                        WebSocketCloseStatus.ProtocolError,
                        "Server only supports graphql-ws protocol",
                        context.RequestAborted);

                    return;
                }

                using (_logger.BeginScope($"GraphQL websocket connection: {context.Connection.Id}"))
                {
                    var connectionFactory =
                        context.RequestServices.GetRequiredService<ITenantWebSocketConnectionFactory>();
                    var connection = connectionFactory.CreateConnection(socket, tenantId, context.Connection.Id);

                    await connection.Connect();
                }
            }
        }
    }
}