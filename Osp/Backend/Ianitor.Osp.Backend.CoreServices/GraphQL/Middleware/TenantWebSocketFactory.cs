using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using Ianitor.Osp.Backend.CoreServices.Services;
using Microsoft.Extensions.Logging;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware
{
    internal interface ITenantWebSocketConnectionFactory
    {
        WebSocketConnection CreateConnection(WebSocket socket, string tenantId, string connectionId);
    }
    
    internal class TenantWebSocketFactory : ITenantWebSocketConnectionFactory
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IGraphQLExecuter<OspSchema> _executer;
        private readonly IEnumerable<IOperationMessageListener> _messageListeners;
        private readonly IOspService _ospService;
        private readonly IDocumentWriter _documentWriter;

        public TenantWebSocketFactory(ILogger<TenantWebSocketFactory> logger,
            ILoggerFactory loggerFactory,
            IGraphQLExecuter<OspSchema> executer,
            IEnumerable<IOperationMessageListener> messageListeners,
            IOspService ospService,
            IDocumentWriter documentWriter)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _executer = executer;
            _messageListeners = messageListeners;
            _ospService = ospService;
            _documentWriter = documentWriter;
        }

        public WebSocketConnection CreateConnection(WebSocket socket, string tenantId, string connectionId)
        {
            _logger.LogDebug("Creating server for connection {connectionId}", connectionId);

            var transport = new WebSocketTransport(socket, _documentWriter);
            var manager = new SubscriptionManager(_executer, _loggerFactory);
            var server = new SubscriptionServer(
                transport,
                manager,
                _messageListeners.Union(new []{new OspMessageListener(_ospService, tenantId)}),
                _loggerFactory.CreateLogger<SubscriptionServer>()
            );

            return new WebSocketConnection(transport, server);
        }
    }
}