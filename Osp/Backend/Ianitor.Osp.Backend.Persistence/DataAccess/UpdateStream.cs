using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    /// <inheritdoc />
    internal class UpdateStream<TDocument> : IUpdateStream<TDocument>
        where TDocument : class, new()
    {
        private readonly ReplaySubject<UpdateInfo<TDocument>> _messageStream = new ReplaySubject<UpdateInfo<TDocument>>(1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Watch(IMongoCollection<TDocument> documentCollection, PipelineDefinition<ChangeStreamDocument<TDocument>, ChangeStreamDocument<TDocument>> pipelineDefinition, CancellationToken requestCancellationToken = default)
        {
            Task.Run(async () =>
            {
                using (var cursor = await documentCollection.WatchAsync(pipelineDefinition,
                    new ChangeStreamOptions {FullDocument = ChangeStreamFullDocumentOption.UpdateLookup}, requestCancellationToken))
                {
                    await cursor.ForEachAsync(change =>
                    {
                        _messageStream.OnNext(new UpdateInfo<TDocument>(change));
                        if (!_messageStream.HasObservers)
                        {
                            _cancellationTokenSource.Cancel();
                        }
                    }, cancellationToken: _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token);
        }
        
        public IObservable<UpdateInfo<TDocument>> GetUpdates()
        {
            return _messageStream;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}