using System.Collections.Generic;
using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Frontend.Client.System
{
    public interface IIdentityServicesClient : IServiceClient
    {
        Task<IEnumerable<IdentityProviderDto>> GetIdentityProviders();
        Task<IdentityProviderDto> GetIdentityProvider(string id);
        Task CreateIdentityProvider(IdentityProviderDto identityProvider);
        Task UpdateIdentityProvider(string id, IdentityProviderDto identityProvider);
        Task DeleteIdentityProvider(string id);
        Task<IEnumerable<ClientDto>> GetClients();
        Task<ClientDto> GetClient(string clientId);
        Task CreateClient(ClientDto client);
        Task UpdateClient(string clientId, ClientDto client);
        Task DeleteClient(string clientId);
        Task<IEnumerable<UserDto>> GetUsers();
        Task CreateUser(UserDto userDto);
        Task UpdateUser(string userName, UserDto userDto);
        Task DeleteUser(string userName);
        Task ResetPassword(string userName, string password);
        Task<RoleDto> GetRoleByName(string roleName);
    }
}