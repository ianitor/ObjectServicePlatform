﻿using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using NLog;

namespace Ianitor.Osp.Backend.Common.Cors
{
    /// <summary>
    /// Implements a CORS policy provider that allows all known clients stored in OSP database
    /// </summary>
    public class CorsPolicyProvider : ICorsPolicyProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IOspClientStore _clientStore;
        private CorsPolicy _corsPolicy;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IChannel<string> _channel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientStore">Client store object to access all available clients.</param>
        /// <param name="distributedCache">Instance of distributed cache</param>
        public CorsPolicyProvider(IOspClientStore clientStore, IDistributedWithPubSubCache distributedCache)
        {
            _clientStore = clientStore;
            _channel = distributedCache.Subscribe<string>(CacheCommon.KeyCorsClients);
            _channel.OnMessage(OnInvalidateData);
        }

        private Task OnInvalidateData(ChannelMessage<string> arg)
        {
            _corsPolicy = null;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            if (_corsPolicy == null)
            {
                var clients = await _clientStore.GetClients();
                var origins = clients.Where(x=> x.AllowedCorsOrigins != null).SelectMany(x => x.AllowedCorsOrigins).ToArray();
                
                Logger.Info($"Creating CORS policy from cache: {string.Join(", ", origins)}");

                var policyBuilder = new CorsPolicyBuilder()
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                _corsPolicy = policyBuilder.Build();
            }

            return _corsPolicy;
        }
    }
}
