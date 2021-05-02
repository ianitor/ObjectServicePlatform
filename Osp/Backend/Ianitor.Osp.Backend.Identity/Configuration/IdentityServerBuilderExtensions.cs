using Ianitor.Osp.Backend.Identity.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Ianitor.Osp.Backend.Identity.Configuration
{
    /// <summary>
    /// Extensions for Identity Server
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Uses OSP singing credential file from file system using the configuration of osp.
        /// </summary>
        /// <param name="builder"></param>
        public static void AddOspSigningCredential(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<ISigningCredentialStore, SigningCredentialService>();
            builder.Services.AddSingleton<IValidationKeysStore, SigningCredentialService>();
        }
    }
}