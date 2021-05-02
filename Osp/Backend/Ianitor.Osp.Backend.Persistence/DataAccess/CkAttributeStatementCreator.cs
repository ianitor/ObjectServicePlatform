using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class CkAttributeStatementCreator : StatementCreator<CkAttribute>
    {
        public CkAttributeStatementCreator(IDatabaseContext databaseContext) : base(databaseContext.CkAttributes)
        {
        }
    }
}