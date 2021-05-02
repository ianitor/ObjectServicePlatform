using Ianitor.Osp.Backend.Authentication.DynamicAuth;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace Ianitor.Osp.Backend.Authentication.Microsoft
{
  internal class MicrosoftAuthSchemeCreator : IAuthSchemeCreator<MicrosoftIdentityProvider>
  {
    private readonly IDynamicAuthOptionsBuilder<MicrosoftAccountOptions> _micAuthOptions;

    /// <summary>
    /// c'tor
    /// </summary>
    /// <param name="micAuthOptions">Authentication builder for Microsoft</param>
    public MicrosoftAuthSchemeCreator(IDynamicAuthOptionsBuilder<MicrosoftAccountOptions> micAuthOptions)
    {
      _micAuthOptions = micAuthOptions;
    }

    public AuthenticationScheme Create(MicrosoftIdentityProvider identityProvider)
    {
      var options = _micAuthOptions.CreateOptions(identityProvider.Alias);
      options.ClientId = identityProvider.ClientId;
      options.ClientSecret = identityProvider.ClientSecret;

      return new AuthenticationScheme(identityProvider.Alias, identityProvider.Alias, typeof(MicrosoftAccountHandler));
    }
  }
}
