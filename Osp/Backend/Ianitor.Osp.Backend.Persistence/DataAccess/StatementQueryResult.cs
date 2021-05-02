using System.Collections.Generic;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once MemberCanBePrivate.Global
    internal class StatementQueryResult<TEntity>
    {
        public IEnumerable<StatementQueryTotalCount> TotalCount { get; set; }
        public IEnumerable<TEntity> Result { get; set; }
    }
}