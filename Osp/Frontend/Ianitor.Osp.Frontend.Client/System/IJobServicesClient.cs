using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Frontend.Client.System
{
    public interface IJobServicesClient : IServiceClient
    {
        Task<JobDto> GetImportJobStatus(string id);

        /// <summary>
        /// Downloads the job result as binary file
        /// </summary>
        /// <param name="id">Job id</param>
        /// <returns></returns>
        Task<byte[]> DownloadExportRtResultAsync(string id);
    }
}