using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection
{
    /// <summary>
    /// IdentityServer helper class for DI configuration
    /// </summary>
    public class OspBuilder : IOspBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OspBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public OspBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public IServiceCollection Services { get; }
    }
}