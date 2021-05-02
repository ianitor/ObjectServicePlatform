using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.ServiceHooks
{
    internal class DeleteServiceHook : ServiceClientOspCommand<ITenantClient>
    {
        private readonly ITenantClient _tenantClient;
        private IArgument _serviceHookIdArg;

        public DeleteServiceHook(IOptions<OspToolOptions> options, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("DeleteServiceHook", "Deletes a service hook", options, tenantClient, authenticationService)
        {
            _tenantClient = tenantClient;
        }

        protected override void AddArguments()
        {
            _serviceHookIdArg = CommandArgumentValue.AddArgument("id", "serviceHookId", new[] { "ID of the service hook" },
                true, 1);
        }

        public override async Task Execute()
        {
            var serviceHookId = CommandArgumentValue.GetArgumentScalarValue<string>(_serviceHookIdArg);

            Logger.Info($"Deleting service hook '{serviceHookId}' at '{_tenantClient.ServiceUri}'");
            
            var getQuery = new GraphQLRequest
            {
                Query = GraphQl.GetServiceHookDetails,
                Variables = new
                {
                    rtId = serviceHookId
                }
            };
            
            var getResult = await _tenantClient.SendQueryAsync<RtServiceHookDto>(getQuery);
            if (!getResult.Items.Any())
            {
                throw new InvalidOperationException(
                    $"Service Hook with ID '{serviceHookId}' does not exist.");
            }

            var serviceHookDto = getResult.Items.First();
            
            var deleteMutation = new GraphQLRequest
            {
                Query = GraphQl.DeleteServiceHook,
                Variables = new
                {
                    entities = new[]
                    {
                        new MutationDto
                        {
                            RtId = serviceHookDto.RtId
                        }
                    }
                }
            };

            var result = await _tenantClient.SendMutationAsync<bool>(deleteMutation);

            if (result)
            {
                Logger.Info($"Service hook '{serviceHookId}' deleted.");
                
            }
            else
            {
                Logger.Error($"Service hook '{serviceHookId}' delete failed.");
            }            
        }
    }
}