using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal class OspSession : IOspSessionInternal
    {
        internal OspSession(IClientSessionHandle clientSessionHandle)
        {
            SessionHandle = clientSessionHandle;
        }
        
        public void Dispose()
        {
            SessionHandle?.Dispose();
        }

        public void StartTransaction()
        {
            SessionHandle.StartTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            await SessionHandle.CommitTransactionAsync();
        }

        public async Task AbortTransactionAsync()
        {
            await SessionHandle.AbortTransactionAsync();
        }

        public IClientSessionHandle SessionHandle { get; }
    }
}