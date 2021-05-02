using System;
using System.Threading.Tasks;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public interface IOspSession : IDisposable
    {
        void StartTransaction(); 
        
        Task CommitTransactionAsync();

        Task AbortTransactionAsync();
    }
}