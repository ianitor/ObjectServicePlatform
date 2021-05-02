// ReSharper disable once CheckNamespace

using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.CoreServices.Configuration.DependencyInjection
{
    /// <summary>
    /// Osp builder Interface
    /// </summary>
    public interface IOspBuilder
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        IServiceCollection Services { get; }
    }
}