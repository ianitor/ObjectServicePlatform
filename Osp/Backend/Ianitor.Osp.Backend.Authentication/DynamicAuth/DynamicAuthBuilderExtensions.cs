using Ianitor.Osp.Backend.Authentication.Azure;
using Ianitor.Osp.Backend.Authentication.Google;
using Ianitor.Osp.Backend.Authentication.Microsoft;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.Authentication.DynamicAuth
{
  public static class DynamicAuthBuilderExtensions
  {
    public static IDynamicAuthBuilder AddOpenIdConnect(this IDynamicAuthBuilder builder)
    {
      builder.Services.AddTransient<IDynamicAuthOptionsBuilder<OpenIdConnectOptions>,
        OpenIdDynamicAuthOptionsBuilder>();
      builder.Services.AddTransient<IAuthSchemeCreator<AzureAdIdentityProvider>, AzureAuthSchemeCreator>();

      return builder;
    }

    public static IDynamicAuthBuilder AddGoogle(this IDynamicAuthBuilder builder)
    {
      builder.Services.AddTransient<IDynamicAuthOptionsBuilder<GoogleOptions>,
        OAuthDynamicAuthOptionsBuilder<GoogleHandler, GoogleOptions>>();
      builder.Services.AddTransient<IAuthSchemeCreator<GoogleIdentityProvider>, GoogleAuthSchemeCreator>();

      return builder;
    }

    public static IDynamicAuthBuilder AddMicrosoft(this IDynamicAuthBuilder builder)
    {
      builder.Services.AddTransient<IDynamicAuthOptionsBuilder<MicrosoftAccountOptions>,
        OAuthDynamicAuthOptionsBuilder<MicrosoftAccountHandler, MicrosoftAccountOptions>>();
      builder.Services.AddTransient<IAuthSchemeCreator<MicrosoftIdentityProvider>, MicrosoftAuthSchemeCreator>();
      return builder;
    }
  }
}
