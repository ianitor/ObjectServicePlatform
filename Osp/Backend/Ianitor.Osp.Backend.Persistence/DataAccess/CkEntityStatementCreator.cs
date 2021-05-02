using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class CkEntityStatementCreator : StatementCreator<CkEntity>
    {
        public CkEntityStatementCreator(IDatabaseContext databaseContext) : base(databaseContext.CkEntities)
        {
        }
        
    }
}