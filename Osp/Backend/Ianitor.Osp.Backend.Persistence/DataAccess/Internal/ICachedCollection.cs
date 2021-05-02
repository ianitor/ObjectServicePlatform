using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess.Internal
{
    public interface ICachedCollection<TDocument> where TDocument : class, new()
    {
        Task CreateAscendingIndexAsync(string name, IEnumerable<string> fields);
        Task CreateTextIndexAsync(string name, string language, IEnumerable<CkIndexFields> fields);
        Task DropIndexAsync(string name);

        Task<TDocument> FindSingleOrDefaultAsync(IOspSession session, Expression<Func<TDocument, bool>> expression);

        Task<ICollection<TDocument>> FindManyAsync(IOspSession session, FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sort = null, int? skip = null, int? take = null);

        Task<ICollection<TDocument>> FindManyAsync(IOspSession session, Expression<Func<TDocument, bool>> expression,
            int? skip = null, int? limit = null);


        Task InsertMultipleAsync(IOspSession session, IEnumerable<TDocument> documentCollection);
        Task InsertAsync(IOspSession session, TDocument document);

        Task ReplaceByIdAsync<TField>(IOspSession session, TField id, TDocument document);
        Task UpdateOneAsync<TField>(IOspSession session, TField id, UpdateDefinition<TDocument> updateDefinition);
        Task<TDocument> DocumentAsync<TField>(IOspSession session, TField id);

        Task<TDerived> DocumentAsync<TDerived, TField>(IOspSession session, TField id)
            where TDerived : TDocument, new();

        Task<IEnumerable<TDocument>> GetAsync(IOspSession session, int? skip = null, int? take = null);
        Task DeleteOneAsync<TField>(IOspSession session, TField id);
        Task DeleteOneAsync(IOspSession session, Expression<Func<TDocument, bool>> expression);

        Task DeleteManyAsync<TField>(IOspSession session, IEnumerable<TField> ids);
        Task DeleteManyAsync(IOspSession session, Expression<Func<TDocument, bool>> expression);
        Task<BulkImportResult> BulkImportAsync(IOspSession session, IEnumerable<TDocument> documentList);

        IUpdateStream<TDocument> Subscribe(UpdateStreamFilter updateStreamFilter,
            CancellationToken cancellationToken = default);

        IAggregateFluent<TDocument> Aggregate(IOspSession session);

        IAsyncCursor<TOutput> Aggregate<TOutput>(IOspSession session,
            PipelineDefinition<TDocument, TOutput> pipelineDefinition);

        Task<long> GetTotalCountAsync(IOspSession session);
        Task<long> GetTotalCountAsync(IOspSession session, FilterDefinition<TDocument> filterDefinition);
        Task<long> GetTotalCountAsync(IOspSession session, Expression<Func<TDocument, bool>> expression);

        IMongoCollection<TDocument> GetMongoCollection();
    }
}