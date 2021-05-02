using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations.ServiceHooks
{
    internal class CreateServiceHook : ServiceClientOspCommand<ITenantClient>
    {
        private IArgument _isEnabledArg;
        private IArgument _nameArg;
        private IArgument _ckIdArg;
        private IArgument _filterArg;
        private IArgument _serviceHookBaseUriArg;
        private IArgument _serviceHookActionArg;

        public CreateServiceHook(IOptions<OspToolOptions> options, ITenantClient tenantClient, IAuthenticationService authenticationService)
            : base("CreateServiceHook", "Create a new service hook", options, tenantClient, authenticationService)
        {
        }

        protected override void AddArguments()
        {
            _isEnabledArg = CommandArgumentValue.AddArgument("e", "enabled", new[] {"Enabled state of service hook"},
                true, 1);
            _nameArg = CommandArgumentValue.AddArgument("n", "name", new[] {"Display name of service hook"},
                true, 1);
            _ckIdArg = CommandArgumentValue.AddArgument("ck", "ckId",
                new[] {"The construction kit id key the service hook is applied to"},
                true, 1);
            _filterArg = CommandArgumentValue.AddArgument("f", "filter", new[]
                {
                    "Filter arguments in form \"'{AttributeName}' {Operator} '{Value}'\"",
                    "Sample: \"'State' Equals '2'\"",
                    "Attribute name must be a valid argument of the defined entity",
                    $"Possible operators: {string.Join(", ", Enum.GetNames(typeof(FieldFilterOperatorDto)))} ",
                    "Value must be a string, integer, double, boolean, DateTime"
                },
                true, 1, true);
            _serviceHookBaseUriArg = CommandArgumentValue.AddArgument("u", "uri",
                new[] {"The base uri of the service hook"},
                false, 1);

            _serviceHookActionArg = CommandArgumentValue.AddArgument("a", "action",
                new[] {"The action uri of the service hook"},
                false, 1);
        }


        public override async Task Execute()
        {
            var isEnabled = CommandArgumentValue.GetArgumentScalarValue<bool>(_isEnabledArg);
            var name = CommandArgumentValue.GetArgumentScalarValue<string>(_nameArg);
            var serviceHookBaseUri = CommandArgumentValue.GetArgumentScalarValue<string>(_serviceHookBaseUriArg);
            var serviceHookAction = CommandArgumentValue.GetArgumentScalarValue<string>(_serviceHookActionArg);
            var ckId = CommandArgumentValue.GetArgumentScalarValue<string>(_ckIdArg);
            
            var filterArgData = CommandArgumentValue.GetArgumentValue(_filterArg);

            Logger.Info(
                $"Creating service hook for entity '{ckId}' at '{ServiceClient.ServiceUri}'");

            List<FieldFilterDto> fieldFilters = new List<FieldFilterDto>();
            foreach (var filterArg in filterArgData.Values)
            {
                var terms = filterArg.Split(" ");
                if (terms.Length != 3)
                {
                    throw new InvalidOperationException($"Filter term '{filterArg}' is invalid. Three terms needed.");
                }

                var attribute = terms[0].Trim('\'');
                if (!Enum.TryParse(terms[1], true, out FieldFilterOperatorDto operatorDto))
                {
                    throw new InvalidOperationException($"Operator '{terms[1]}' of term '{filterArg}' is invalid.");
                }

                var comparisionValue = terms[2].Trim('\'');

                fieldFilters.Add(new FieldFilterDto
                    {AttributeName = attribute, Operator = operatorDto, ComparisonValue = comparisionValue});
            }

            var createServiceHookDto = new ServiceHookMutationDto
            {
                Enabled = isEnabled,
                Name = name,
                QueryCkId = ckId,
                FieldFilter = JsonConvert.SerializeObject(fieldFilters),
                ServiceHookBaseUri = serviceHookBaseUri,
                ServiceHookAction = serviceHookAction
            };

            var query = new GraphQLRequest
            {
                Query = GraphQl.CreateServiceHook,
                Variables = new {entities = new[] {createServiceHookDto}}
            };

            var result = await ServiceClient.SendMutationAsync<IEnumerable<RtServiceHookDto>>(query);

            Logger.Info($"Service hook '{name}' added (ID '{result.First().RtId}').");
        }
    }
}