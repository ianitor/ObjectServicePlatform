using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;

namespace Ianitor.Osp.Backend.Persistence.DataAccess.Internal
{
    internal class CachedCollection<TDocument> : ICachedCollection<TDocument>
        where TDocument : class, new()

    {
        private readonly IMongoCollection<TDocument> _documentCollection;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal CachedCollection(IMongoCollection<TDocument> documentCollection)
        {
            _documentCollection = documentCollection;
        }

        public IMongoCollection<TDocument> GetMongoCollection()
        {
            return _documentCollection;
        }

        public IAggregateFluent<TDocument> Aggregate(IOspSession session)
        {
            ArgumentValidation.Validate(nameof(session), session);

            return _documentCollection.Aggregate(((IOspSessionInternal) session).SessionHandle,
                new AggregateOptions {AllowDiskUse = true});
        }

        public IAsyncCursor<TOutput> Aggregate<TOutput>(IOspSession session,
            PipelineDefinition<TDocument, TOutput> pipelineDefinition)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(pipelineDefinition), pipelineDefinition);

            return _documentCollection.Aggregate(((IOspSessionInternal) session).SessionHandle, pipelineDefinition);
        }

        public async Task CreateAscendingIndexAsync(string name, IEnumerable<string> fields)
        {
            ArgumentValidation.ValidateString(nameof(name), name);
            ArgumentValidation.Validate(nameof(fields), fields);

            var indexKeys =
                fields.Select(f =>
                    Builders<TDocument>.IndexKeys.Ascending(Constants.AttributesName + "." + f.ToCamelCase()));


            await _documentCollection.Indexes.CreateOneAsync(new CreateIndexModel<TDocument>(
                Builders<TDocument>.IndexKeys.Combine(indexKeys), new CreateIndexOptions
                {
                    Name = name
                }
            ));
        }

        public async Task CreateTextIndexAsync(string name, string language,
            IEnumerable<CkIndexFields> fields)
        {
            ArgumentValidation.ValidateString(nameof(name), name);
            ArgumentValidation.ValidateString(nameof(language), language);
            ArgumentValidation.Validate(nameof(fields), fields);

            Dictionary<string, int> weights = new Dictionary<string, int>();

            var indexKeys =
                fields.SelectMany(f => f.AttributeNames).Select(f =>
                    Builders<TDocument>.IndexKeys.Text(Constants.AttributesName + "." + f.ToCamelCase()));

            foreach (var field in fields)
            {
                foreach (var attributeName in field.AttributeNames)
                {
                    weights.Add(Constants.AttributesName + "." + attributeName.ToCamelCase(),
                        field.Weight.GetValueOrDefault(1));
                }
            }

            await _documentCollection.Indexes.CreateOneAsync(new CreateIndexModel<TDocument>(
                Builders<TDocument>.IndexKeys.Combine(
                    indexKeys), new CreateIndexOptions
                {
                    Name = name,
                    Weights = new BsonDocument(weights),
                    DefaultLanguage = language
                }
            ));
        }

        public async Task DropIndexAsync(string name)
        {
            ArgumentValidation.ValidateString(nameof(name), name);

            var r = await _documentCollection.Indexes.ListAsync();
            foreach (var i in await r.ToListAsync())
            {
                if (i["name"].ToString().StartsWith(name))
                {
                    await _documentCollection.Indexes.DropOneAsync(i["name"].ToString());
                }
            }
        }

        /// <summary>
        /// Finds all documents matching a given expression
        /// </summary>
        /// <param name="session">Session object to manage the transaction</param>
        /// <param name="filterDefinition">The filter definition</param>
        /// <param name="sort">Sort definition</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="take">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <returns>Returns a cursor</returns>
        public async Task<ICollection<TDocument>> FindManyAsync(IOspSession session,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sort = null, int? skip = null, int? take = null)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(filterDefinition), filterDefinition);

            try
            {
                var cursor = await _documentCollection.FindAsync(((IOspSessionInternal) session).SessionHandle,
                    filterDefinition, new FindOptions<TDocument>
                    {
                        Sort = sort,
                        Skip = skip,
                        Limit = take
                    });
                return await cursor.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ICollection<TDocument>> FindManyAsync(IOspSession session,
            Expression<Func<TDocument, bool>> expression,
            int? skip = null,
            int? limit = null)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(expression), expression);

            try
            {
                var cursor = await _documentCollection.FindAsync(((IOspSessionInternal) session).SessionHandle,
                    expression,
                    new FindOptions<TDocument> {Skip = skip, Limit = limit});
                return await cursor.ToListAsync();
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task<TDocument> FindSingleOrDefaultAsync(IOspSession session,
            Expression<Func<TDocument, bool>> expression)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(expression), expression);

            try
            {
                return await (await _documentCollection.FindAsync(((IOspSessionInternal) session).SessionHandle,
                    expression)).SingleOrDefaultAsync();
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task<long> GetTotalCountAsync(IOspSession session)
        {
            return await GetTotalCountAsync(session, document => true);
        }

        public async Task<long> GetTotalCountAsync(IOspSession session, FilterDefinition<TDocument> filterDefinition)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(filterDefinition), filterDefinition);

            return await _documentCollection.CountDocumentsAsync(((IOspSessionInternal) session).SessionHandle,
                filterDefinition);
        }


        public async Task<long> GetTotalCountAsync(IOspSession session, Expression<Func<TDocument, bool>> expression)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(expression), expression);

            return await _documentCollection.CountDocumentsAsync(((IOspSessionInternal) session).SessionHandle,
                expression);
        }

        public async Task ReplaceByIdAsync<TField>(IOspSession session, TField id, TDocument document)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(id), id);
            ArgumentValidation.Validate(nameof(document), document);

            try
            {
                var filter = Builders<TDocument>.Filter.BuildIdFilter(id);
                var result = await _documentCollection.ReplaceOneAsync(((IOspSessionInternal) session).SessionHandle,
                    filter, document);
                ThrowIfNotAcknowledged(result.IsAcknowledged);
                ThrowIfMatchedCountZero<TField, TDocument>(result.MatchedCount, id);
            }
            catch (MongoWriteException ex)
            {
                Logger.Error(ex);
                HandleWriteException<TDocument>(ex);
            }
        }

        public async Task UpdateOneAsync<TField>(IOspSession session, TField id,
            UpdateDefinition<TDocument> updateDefinition)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(id), id);
            ArgumentValidation.Validate(nameof(updateDefinition), updateDefinition);

            try
            {
                var filter = Builders<TDocument>.Filter.BuildIdFilter(id);
                var result = await _documentCollection.UpdateOneAsync(((IOspSessionInternal) session).SessionHandle,
                    filter, updateDefinition);
                ThrowIfNotAcknowledged(result.IsAcknowledged);
                ThrowIfMatchedCountZero<TField, TDocument>(result.MatchedCount, id);
            }
            catch (MongoWriteException ex)
            {
                Logger.Error(ex);
                HandleWriteException<TDocument>(ex);
            }
        }

        public async Task<TDerived> DocumentAsync<TDerived, TField>(IOspSession session, TField id)
            where TDerived : TDocument, new()
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(id), id);

            try
            {
                var filter = Builders<TDocument>.Filter.BuildIdFilter(id);
                var s = (TDerived) await (await _documentCollection.FindAsync(
                        ((IOspSessionInternal) session).SessionHandle, filter))
                    .SingleOrDefaultAsync();
                return s;
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task<TDocument> DocumentAsync<TField>(IOspSession session, TField id)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(id), id);

            try
            {
                return await DocumentAsync<TDocument, TField>(session, id);
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task<IEnumerable<TDocument>> GetAsync(IOspSession session, int? skip = null, int? take = null)
        {
            ArgumentValidation.Validate(nameof(session), session);

            try
            {
                var options = new FindOptions<TDocument> {Limit = take, Skip = skip};
                return await (await _documentCollection.FindAsync(((IOspSessionInternal) session).SessionHandle,
                    _ => true, options)).ToListAsync();
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task DeleteOneAsync<TField>(IOspSession session, TField id)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(id), id);

            try
            {
                var filter = Builders<TDocument>.Filter.BuildIdFilter(id);
                await _documentCollection.DeleteOneAsync(((IOspSessionInternal) session).SessionHandle, filter);
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task DeleteOneAsync(IOspSession session, Expression<Func<TDocument, bool>> expression)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(expression), expression);

            try
            {
                var deleteResult =
                    await _documentCollection.DeleteOneAsync(((IOspSessionInternal) session).SessionHandle, expression);
                ThrowIfNotAcknowledged(deleteResult.IsAcknowledged);
                ThrowIfMatchedCountZero<TDocument>(deleteResult.DeletedCount, expression);
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task DeleteManyAsync<TField>(IOspSession session, IEnumerable<TField> ids)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(ids), ids);

            try
            {
                var filter = Builders<TDocument>.Filter.In(Constants.IdField, ids);

                var deleteResult =
                    await _documentCollection.DeleteManyAsync(((IOspSessionInternal) session).SessionHandle, filter);
                ThrowIfNotAcknowledged(deleteResult.IsAcknowledged);
                ThrowIfMatchedCountZero(deleteResult.DeletedCount);
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task DeleteManyAsync(IOspSession session, Expression<Func<TDocument, bool>> expression)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(expression), expression);

            try
            {
                var deleteResult =
                    await _documentCollection.DeleteManyAsync(((IOspSessionInternal) session).SessionHandle,
                        expression);
                ThrowIfNotAcknowledged(deleteResult.IsAcknowledged);
                ThrowIfMatchedCountZero<TDocument>(deleteResult.DeletedCount, expression);
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        public async Task<BulkImportResult> BulkImportAsync(IOspSession session, IEnumerable<TDocument> documentList)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(documentList), documentList);

            try
            {
                var listWrites = new List<WriteModel<TDocument>>();
                foreach (var v in documentList)
                {
                    listWrites.Add(new InsertOneModel<TDocument>(v));
                }

                var result =
                    await _documentCollection.BulkWriteAsync(((IOspSessionInternal) session).SessionHandle, listWrites);
                return new BulkImportResult(result);
            }
            catch (MongoBulkWriteException e)
            {
                throw new OperationFailedException($"Bulk import failed: {e.Message}", e);
            }
        }


        public IUpdateStream<TDocument> Subscribe(UpdateStreamFilter updateStreamFilter,
            CancellationToken cancellationToken = default)
        {
            var updateStream = new UpdateStream<TDocument>();

            PipelineDefinition<ChangeStreamDocument<TDocument>, ChangeStreamDocument<TDocument>> pipeline =
                new EmptyPipelineDefinition<ChangeStreamDocument<TDocument>>();


            pipeline = pipeline.Match(x =>
                x.OperationType == ChangeStreamOperationType.Update &&
                updateStreamFilter.UpdateTypes.HasFlag(UpdateTypes.Update) ||
                x.OperationType == ChangeStreamOperationType.Insert &&
                updateStreamFilter.UpdateTypes.HasFlag(UpdateTypes.Insert) ||
                x.OperationType == ChangeStreamOperationType.Delete &&
                updateStreamFilter.UpdateTypes.HasFlag(UpdateTypes.Delete) ||
                x.OperationType == ChangeStreamOperationType.Replace &&
                updateStreamFilter.UpdateTypes.HasFlag(UpdateTypes.Replace) ||
                updateStreamFilter.UpdateTypes == UpdateTypes.Undefined
            );


            if (updateStreamFilter.RtId.HasValue)
            {
                var filter = Builders<ChangeStreamDocument<TDocument>>.Filter.Eq("fullDocument." + Constants.IdField, updateStreamFilter.RtId.Value.ToObjectId());
                pipeline = pipeline.Match(filter);
            }

            updateStream.Watch(_documentCollection, pipeline, cancellationToken);
            return updateStream;
        }

        #region Insert operations

        public async Task InsertMultipleAsync(IOspSession session, IEnumerable<TDocument> documentCollection)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(documentCollection), documentCollection);

            try
            {
                await _documentCollection.InsertManyAsync(((IOspSessionInternal) session).SessionHandle,
                    documentCollection);
            }
            catch (MongoWriteException ex)
            {
                HandleWriteException<TDocument>(ex);
                throw;
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }


        public async Task InsertAsync(IOspSession session, TDocument document)
        {
            ArgumentValidation.Validate(nameof(session), session);
            ArgumentValidation.Validate(nameof(document), document);

            try
            {
                await _documentCollection.InsertOneAsync(((IOspSessionInternal) session).SessionHandle, document);
            }
            catch (MongoWriteException ex)
            {
                HandleWriteException<TDocument>(ex);
                throw;
            }
            catch (MongoException e)
            {
                Logger.Error(e);
                throw new OperationFailedException(e.Message, e);
            }
        }

        #endregion Insert operations


        #region Exception helpers

        private void HandleWriteException<T>(MongoWriteException ex)
        {
            if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new DuplicateKeyException($"Error adding item of type {nameof(T)}", typeof(T), ex);
            }

            throw new OperationFailedException("Operation was not completed.", ex);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ThrowIfNotAcknowledged(bool acknowledged)
        {
            if (!acknowledged)
            {
                throw new MongoException("The action was not acknowledged.");
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ThrowIfMatchedCountZero<TField, T>(long matchedCount, TField id)
        {
            if (matchedCount == 0)
            {
                var message = $"Operation failed because ID '{id}' is not existing for document type {nameof(T)}.";
                throw new EntityNotFoundException(message);
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ThrowIfMatchedCountZero<T>(long matchedCount, Expression<Func<TDocument, bool>> expression)
        {
            if (matchedCount == 0)
            {
                var message =
                    $"Operation failed because filter '{expression}' did not match any documents for type {nameof(T)}.";
                throw new EntityNotFoundException(message);
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ThrowIfMatchedCountZero(long matchedCount)
        {
            if (matchedCount == 0)
            {
                var message = $"Operation may failed because no data matched.";
                throw new EntityNotFoundException(message);
            }
        }

        #endregion Exception helpers
    }
}