using System;
using Ianitor.Osp.Backend.Infrastructure.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ianitor.Osp.Backend.Authentication.DynamicAuth
{
  /// <summary>
  /// Builder for the Dynamic authentication.
  /// </summary>
  public interface IDynamicAuthBuilder
  {
    public IServiceCollection Services { get; }
  }

  /// <inheritdoc />
  public class DynamicAuthBuilder : IDynamicAuthBuilder
  {
    public IServiceCollection Services { get; }

    internal DynamicAuthBuilder(IServiceCollection serviceCollection)
    {
      Services = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
      AddCommonRequirements();
    }
    private void AddCommonRequirements()
    {
      Services.TryAddScoped<IDynamicAuthSchemeService, DynamicAuthSchemeService>();
      Services.TryAddScoped<IAuthSchemeCreatorFactory, AuthSchemeCreatorFactory>();
      Services.AddInitializationService<DynamicAuthSchemeServiceInitializer>();
    }
  }
}
