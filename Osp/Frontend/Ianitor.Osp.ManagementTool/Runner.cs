using Ianitor.Common.CommandLineParser;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Authentication;

namespace Ianitor.Osp.ManagementTool
{
    internal class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly IParser _parser;

        public Runner(ILogger<Runner> logger, IParser parser)
        {
            _logger = logger;
            _parser = parser;
        }

        public async Task<int> DoActionAsync()
        {

            try
            {
                _logger.LogInformation("Object Service Platform Management Tool, Version {0}", GetProductVersion());
                _logger.LogInformation(GetCopyright());

                await _parser.ParseAndValidateAsync();

                return 0;
            }
            catch (MandatoryArgumentsMissingException ex)
            {
                _logger.LogError(ex.Message);
                _parser.ShowUsageInformation();
                return -1;
            }
            catch (InvalidProgramException ex)
            {
                _logger.LogError(ex.Message);
                _parser.ShowUsageInformation();
                return -1;
            }
            catch (ServiceConfigurationMissingException ex)
            {
                _logger.LogError(ex.Message + " Please use the 'config' command.");
                return -2;
            }
            catch (ServiceClientResultException ex)
            {
                _logger.LogError(ex.Message);
                return -3;
            }
            catch (ServiceClientException ex)
            {
                _logger.LogError(ex.Message);
                return -3;
            }
            catch (AuthenticationFailedException ex)
            {
                _logger.LogError("Authentication failed.");
                _logger.LogError(ex.Message);
                
                return -4;
            }
            catch (Exception ex)
            {
                Exception tmp = ex;
                while (tmp != null)
                {
                    _logger.LogCritical(tmp, tmp.Message);
                    tmp = tmp.InnerException;
                }

                return -99;
            }
        }

        private static string GetProductVersion()
        {
            var attribute = Assembly
              .GetExecutingAssembly()
              .GetCustomAttributes<AssemblyFileVersionAttribute>()
              .Single();
            return attribute.Version;
        }

        private static string GetCopyright()
        {
            var attribute = Assembly
              .GetExecutingAssembly()
              .GetCustomAttributes<AssemblyCopyrightAttribute>()
              .Single();

            return attribute.Copyright;
        }
    }
}
