using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Validation;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Utils;
using Ianitor.Osp.Backend.Persistence;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    /// <summary>
    /// Tenant specific implementation for Executer/or
    /// </summary>
    /// <remarks>We need to resolve the correct (tenant-specific) schema before executing.</remarks>
    public class TenantGraphQlExecuter : IGraphQLExecuter<OspSchema>
    {
        private readonly ISchemaContext _schemaContext;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly IOptions<GraphQLOptions> _options;
        private readonly IEnumerable<IDocumentExecutionListener> _listeners;
        private readonly IEnumerable<IValidationRule> _validationRules;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schemaContext"></param>
        /// <param name="documentExecuter"></param>
        /// <param name="options"></param>
        /// <param name="listeners"></param>
        /// <param name="validationRules"></param>
        public TenantGraphQlExecuter(ISchemaContext schemaContext, IDocumentExecuter documentExecuter, IOptions<GraphQLOptions> options, 
            IEnumerable<IDocumentExecutionListener> listeners, IEnumerable<IValidationRule> validationRules)
        {
            _schemaContext = schemaContext;
            _documentExecuter = documentExecuter;
            _options = options;
            _listeners = listeners;
            _validationRules = validationRules;
        }

        /// <inheritdoc />
        public async Task<ExecutionResult> ExecuteAsync(string operationName, string query, Inputs variables, IDictionary<string, object> context,
            IServiceProvider requestServices, CancellationToken cancellationToken = new CancellationToken())
        {
            ITenantContext tenantContext = null;
            if (context is GraphQLUserContext userContext)
            {
                tenantContext = userContext.TenantContext;
            }
            else if (context is MessageHandlingContext messageHandlingContext)
            {
                tenantContext = messageHandlingContext.Get<ITenantContext>(Statics.TenantContext);
            }
            
            // Client tried to use an invalid tenant
            if (tenantContext == null)
            {
                return new ExecutionResult
                {
                    Errors = new ExecutionErrors
                        {new ExecutionError("Invalid request. Please check your client configuration.")}
                };
            }
            

            Schema = await _schemaContext.GetOrCreateAsync(tenantContext);
            
            var executer = new DefaultGraphQLExecuter<OspSchema>(Schema, _documentExecuter, _options, _listeners, _validationRules);
            return await executer.ExecuteAsync(operationName, query, variables, context, requestServices, cancellationToken);
        }

        /// <inheritdoc />
        public OspSchema Schema { get; private set; }
    }
}