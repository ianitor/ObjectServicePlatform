using System;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.Authentication.DynamicAuth
{
  /// <inheritdoc />>
  internal class AuthSchemeCreatorFactory : IAuthSchemeCreatorFactory
  {
    private readonly IServiceProvider _provider;

    public AuthSchemeCreatorFactory(IServiceProvider provider)
    {
      _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }


    /// <inheritdoc />>
    public IAuthSchemeCreator<TAuthProvider> GetCreator<TAuthProvider>()
    where TAuthProvider : OspIdentityProvider
    {
      var requestedType = typeof(IAuthSchemeCreator<>).MakeGenericType(typeof(TAuthProvider));

      return (IAuthSchemeCreator<TAuthProvider>)_provider.GetRequiredService(requestedType);
    } 
  }
}
