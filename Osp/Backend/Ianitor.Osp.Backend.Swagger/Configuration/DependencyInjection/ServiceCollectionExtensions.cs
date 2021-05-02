using Microsoft.AspNetCore.Identity;
using System;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Swagger;
using Ianitor.Osp.Backend.Swagger.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServiceCollectionExtensions = Microsoft.AspNetCore.Identity.IdentityBuilderExtensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddOspApiVersioningAndDocumentation(
      this IServiceCollection services,
      Action<OspSwaggerOptions> setupOspSwaggerOptionsAction = null,
      Action<IdentityOptions> setupAction = null)
    {
      ArgumentValidation.Validate(nameof(services), services);

      if (setupOspSwaggerOptionsAction != null)
      {
        services.Configure(setupOspSwaggerOptionsAction);
      }

      services.AddApiVersioning(options => options.ReportApiVersions = true);
      services.AddVersionedApiExplorer();
      
      services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
      services.AddSwaggerGen();


      return services;
    }
  }
}
