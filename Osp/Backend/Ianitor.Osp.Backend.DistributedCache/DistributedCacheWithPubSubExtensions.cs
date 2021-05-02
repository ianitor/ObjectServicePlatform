using System;
using Ianitor.Common.Shared;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Ianitor.Osp.Backend.DistributedCache
{
    /// <summary>
    ///  DI extension methods for adding distributed cache with pub sub
    /// </summary>
    public static class DistributedCacheWithPubSubExtensions
    {
        /// <summary>
        /// Adds a distributed cache with pub sub mechanisms to the service collection 
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDistributedPubSubCache(this IServiceCollection services)
        {
            ArgumentValidation.Validate(nameof(services), services);
            
            services.AddOptions();
            services.Add(ServiceDescriptor.Singleton<IDistributedWithPubSubCache, DistributedWithPubSubCache>());
            return services;
        }
        
        /// <summary>
        /// Adds a distributed cache with pub sub mechanisms to the service collection 
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="setupAction">An <see cref="T:System.Action`1" /> to configure the provided
        /// <see cref="T:Ianitor.Osp.Backend.Common.DistributedCache.DistributeCacheWithPubSubOptions" />.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDistributedPubSubCache(this IServiceCollection services, Action<DistributeCacheWithPubSubOptions> setupAction)
        {
            ArgumentValidation.Validate(nameof(services), services);
            ArgumentValidation.Validate(nameof(setupAction), setupAction);
            
            services.AddOptions();
            services.Configure(setupAction);
            services.AddDistributedPubSubCache();
            return services;
        }
    }
}