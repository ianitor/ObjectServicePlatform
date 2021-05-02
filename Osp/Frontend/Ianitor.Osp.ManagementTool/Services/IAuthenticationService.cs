using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Authentication;

namespace Ianitor.Osp.ManagementTool.Services
{
    public interface IAuthenticationService
    {
        Task EnsureAuthenticated(IServiceClientAccessToken serviceClientAccessToken);

        void SaveAuthenticationData(AuthenticationData authenticationData);
    }
}