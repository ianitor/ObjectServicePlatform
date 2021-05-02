using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool
{
    internal interface IParser
    {
        IOptions<OspToolOptions> Options { get; }
        void ShowUsageInformation();
        Task ParseAndValidateAsync();
        void CreateSamples();
    }
}