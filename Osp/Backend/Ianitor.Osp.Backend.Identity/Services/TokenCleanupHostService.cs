using System;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NLog;

namespace Ianitor.Osp.Backend.Identity.Services
{
    internal class TokenCleanupHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<OspIdentityOptions> _identityOptions;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private TimeSpan CleanupInterval => TimeSpan.FromSeconds(_identityOptions.Value.TokenCleanupInterval);
        
        private CancellationTokenSource _source;
        
        public TokenCleanupHostService(IServiceProvider serviceProvider, IOptions<OspIdentityOptions> identityOptions)
        {
            _serviceProvider = serviceProvider;
            _identityOptions = identityOptions;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_identityOptions.Value.EnableTokenCleanup)
            {
                if (_source != null)
                {
                    throw new InvalidOperationException("Already started. Call Stop first.");
                }

                Logger.Debug("Starting grant removal");

                _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                Task.Factory.StartNew(() => StartInternalAsync(_source.Token), cancellationToken);
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_identityOptions.Value.EnableTokenCleanup)
            {
                if (_source == null)
                {
                    throw new InvalidOperationException("Not started. Call Start first.");
                }

                Logger.Debug("Stopping grant removal");

                _source.Cancel();
                _source = null;
            }

            return Task.CompletedTask;
        }
        
        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Debug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Logger.Debug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error("Task.Delay exception: {0}. Exiting.", ex.Message);
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Debug("CancellationRequested. Exiting.");
                    break;
                }

                await RemoveExpiredGrantsAsync();
            }
        }

        async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var persistentGrantStore = serviceScope.ServiceProvider.GetRequiredService<IOspPersistentGrantStore>();
                    await persistentGrantStore.RemoveExpiredGrantsAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception removing expired grants: {exception}", ex.Message);
            }
        }
    }
}