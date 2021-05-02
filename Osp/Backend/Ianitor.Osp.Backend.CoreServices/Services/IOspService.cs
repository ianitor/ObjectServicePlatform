using Ianitor.Osp.Backend.Persistence;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.Services
{
    public interface IOspService
    {
        ISystemContext SystemContext { get; }
    }
}