using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal interface IOspSessionInternal : IOspSession
    {
        IClientSessionHandle SessionHandle { get; }
    }
}