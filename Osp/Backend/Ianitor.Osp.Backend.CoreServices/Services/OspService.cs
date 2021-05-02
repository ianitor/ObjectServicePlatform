using Ianitor.Osp.Backend.Persistence;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.Services
{
    public class OspService : IOspService
    {
        public OspService(ISystemContext systemContext)
        {
            SystemContext = systemContext;
        }

        public ISystemContext SystemContext { get; }
    }
}