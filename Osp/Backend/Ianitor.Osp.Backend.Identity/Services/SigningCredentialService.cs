using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ianitor.Common.Shared;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Ianitor.Osp.Backend.Identity.Services
{
    internal class SigningCredentialService: IValidationKeysStore, ISigningCredentialStore
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEnumerable<SecurityKeyInfo> _keys;
        private readonly SigningCredentials _credential;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="ospIdentityOptions">The osp identity options.</param>
        /// <exception cref="System.ArgumentNullException">keys</exception>
        public SigningCredentialService(IOptions<OspIdentityOptions> ospIdentityOptions)
        {
            ArgumentValidation.Validate(nameof(ospIdentityOptions), ospIdentityOptions);
            
            ArgumentValidation.ValidateString(nameof(ospIdentityOptions.Value.KeyFilePath), ospIdentityOptions.Value.KeyFilePath);
            ArgumentValidation.ValidateString(nameof(ospIdentityOptions.Value.KeyFilePassword), ospIdentityOptions.Value.KeyFilePassword);
            
            if (File.Exists(ospIdentityOptions.Value.KeyFilePath))
            {
                Logger.Debug($"SigninCredentialExtension adding key from file {ospIdentityOptions.Value.KeyFilePath}");
        
                var certificate = new X509Certificate2(ospIdentityOptions.Value.KeyFilePath, ospIdentityOptions.Value.KeyFilePassword);
                _credential = new SigningCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaSha256);

                var keyInfo = new SecurityKeyInfo
                {
                    Key = _credential.Key,
                    SigningAlgorithm = _credential.Algorithm
                };
                _keys = new[] {keyInfo};
            }
            else
            {
                Logger.Error($"SigninCredentialExtension cannot find key file {ospIdentityOptions.Value.KeyFilePath}");
            }
        }

        /// <summary>
        /// Gets all validation keys.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            return Task.FromResult(_keys);
        }

        /// <summary>
        /// Gets the signing credentials.
        /// </summary>
        /// <returns></returns>
        public Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            return Task.FromResult(_credential);
        }
    }
}