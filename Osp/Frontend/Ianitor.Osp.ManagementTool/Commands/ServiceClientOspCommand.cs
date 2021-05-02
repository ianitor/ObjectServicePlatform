using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands
{
    internal abstract class ServiceClientOspCommand<TServiceClientType> : OspCommand where TServiceClientType : IServiceClient
    {
        private readonly IAuthenticationService _authenticationService;
        protected TServiceClientType ServiceClient { get; }


        protected ServiceClientOspCommand(string commandValue, string commandDescription,  IOptions<OspToolOptions> options, TServiceClientType serviceClient, IAuthenticationService authenticationService)
            : base(commandValue, commandDescription, options)
        {
            _authenticationService = authenticationService;
            ServiceClient = serviceClient;
        }
        
        public override async Task PreValidate()
        {
            await _authenticationService.EnsureAuthenticated(ServiceClient.AccessToken);
            ServiceClient.Initialize();
        }
    }
}