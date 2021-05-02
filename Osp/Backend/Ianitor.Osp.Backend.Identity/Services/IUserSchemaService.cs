using System.Threading.Tasks;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.Services
{
    public interface IUserSchemaService
    {
        Task SetupAsync();
    }
}