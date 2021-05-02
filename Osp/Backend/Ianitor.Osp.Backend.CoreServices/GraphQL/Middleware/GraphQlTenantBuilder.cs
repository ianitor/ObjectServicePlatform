using System;
using GraphQL.Server.Transports.WebSockets;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Middleware
{
    internal static class GraphQlTenantBuilder
    {
       
        /// <summary>
        /// Add the GraphQL web sockets middleware to the HTTP request pipeline
        /// </summary>
        /// <param name="endpoints">Defines a contract for a route builder in an application. A route builder specifies the routes for an application.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> received as parameter</returns>
        public static TenantGraphQlWebSocketsEndpointConventionBuilder MapGraphQlTenantWebSockets(this IEndpointRouteBuilder endpoints, string pattern = "graphql")
        {
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            var requestDelegate = endpoints.CreateApplicationBuilder().UseMiddleware<GraphQlTenantWebSocketsMiddleware>().Build();
            return new TenantGraphQlWebSocketsEndpointConventionBuilder(endpoints.Map(pattern, requestDelegate).WithDisplayName("GraphQL WebSockets"));
        }
        
        internal static PlaygroundTenantEndpointConventionBuilder MapGraphQlTenantPlayground(this IEndpointRouteBuilder endpoints, PlaygroundOptions options, string pattern = "ui/playground")
        {
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            var requestDelegate = endpoints.CreateApplicationBuilder().UseMiddleware<PlaygroundTenantMiddleware>(options ?? new PlaygroundOptions()).Build();
            return new PlaygroundTenantEndpointConventionBuilder(endpoints.Map(pattern, requestDelegate).WithDisplayName("GraphQL Playground"));
        }
        
        internal class PlaygroundTenantEndpointConventionBuilder : IEndpointConventionBuilder
        {
            private readonly IEndpointConventionBuilder _builder;

            internal PlaygroundTenantEndpointConventionBuilder(IEndpointConventionBuilder builder)
            {
                _builder = builder;
            }

            /// <inheritdoc />
            public void Add(Action<EndpointBuilder> convention) => _builder.Add(convention);
        }
        
        /// <summary>
        /// Builds conventions that will be used for customization of Microsoft.AspNetCore.Builder.EndpointBuilder instances.
        /// Special convention builder that allows you to write specific extension methods for ASP.NET Core routing subsystem.
        /// </summary>
        internal class TenantGraphQlWebSocketsEndpointConventionBuilder : IEndpointConventionBuilder
        {
            private readonly IEndpointConventionBuilder _builder;

            internal TenantGraphQlWebSocketsEndpointConventionBuilder(IEndpointConventionBuilder builder)
            {
                _builder = builder;
            }

            /// <inheritdoc />
            public void Add(Action<EndpointBuilder> convention) => _builder.Add(convention);
        }
    }
}