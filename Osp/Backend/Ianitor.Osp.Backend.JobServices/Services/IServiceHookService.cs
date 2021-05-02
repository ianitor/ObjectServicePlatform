using System.Threading.Tasks;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.JobServices.Services
{
    public interface IServiceHookService
    {
        Task SyncDataSourceAndCreateJobsAsync();
    }
}