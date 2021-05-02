using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands
{
    internal abstract class JobOspCommand : OspCommand
    {
        protected IJobServicesClient ServicesClient { get; }
        private readonly IAuthenticationService _authenticationService;

        protected JobOspCommand(string commandValue, string commandDescription, IOptions<OspToolOptions> options,
            IJobServicesClient jobServicesClient, IAuthenticationService authenticationService)
            : base(commandValue, commandDescription, options)
        {
            ServicesClient = jobServicesClient;
            _authenticationService = authenticationService;
        }

        public override async Task PreValidate()
        {
            await _authenticationService.EnsureAuthenticated(ServicesClient.AccessToken);
            ServicesClient.Initialize();
        }

        protected async Task DownloadJobResultAsync(string id, string filePath)
        {
            Logger.Info($"Dowloading file of job '{id}'.");

            var responseContent = await ServicesClient.DownloadExportRtResultAsync(id);

            await File.WriteAllBytesAsync(filePath, responseContent);
            Logger.Info($"File downloaded at '{filePath}'.");
        }

        protected virtual async Task WaitForJob(string id)
        {
            Logger.Info($"Waiting for job '{id}' to finish.");

            JobDto lastJobDto = null;
            while (true)
            {
                var jobDto = await ServicesClient.GetImportJobStatus(id);
                if (jobDto.Status == "Succeeded")
                {
                    Logger.Info($"Job id '{id}' has completed at '{jobDto.StateChangedAt?.ToLocalTime()}'.");
                    break;
                }

                if (jobDto.Status == "Failed")
                {
                    Logger.Info(
                        $"Job id '{id}' has failed at '{jobDto.StateChangedAt?.ToLocalTime()}'. See server logs for more details.");
                    break;
                }

                if (jobDto.Status == "Deleted")
                {
                    Logger.Info(
                        $"Job id '{id}' has failed at '{jobDto.StateChangedAt?.ToLocalTime()}'. See server logs for more details.");
                    break;
                }

                if (lastJobDto == null || (lastJobDto.StateChangedAt != jobDto.StateChangedAt ||
                                           lastJobDto.Status != jobDto.Status))
                {
                    Logger.Info(
                        $"Job '{id}' has status '{jobDto.Status}', changed at '{jobDto.StateChangedAt?.ToLocalTime()}'.");
                }

                lastJobDto = jobDto;
                Thread.Sleep(2000);
            }
        }
    }
}