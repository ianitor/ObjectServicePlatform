using System.Collections.Generic;
using System.Linq;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class ResultSet<TEntity>
    {
        public ResultSet(IEnumerable<TEntity> result, long totalCount)
        {
            Result = result;
            TotalCount = totalCount;
        }
        
        internal ResultSet(StatementQueryResult<TEntity> statementQueryResult)
        {
            Result = statementQueryResult.Result;
            TotalCount = statementQueryResult.TotalCount.FirstOrDefault()?.Count ?? 0;
        }

        public long TotalCount { get; }
        
        public IEnumerable<TEntity> Result { get; }
    }
}