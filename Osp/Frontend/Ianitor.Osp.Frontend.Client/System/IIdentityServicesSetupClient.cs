using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Frontend.Client.System
{
    public interface IIdentityServicesSetupClient : IServiceClient
    {
        Task AddAdminUser(AdminUserDto adminUserDto);
    }
}