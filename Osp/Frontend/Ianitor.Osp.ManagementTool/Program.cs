using System;
using System.IO;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Common.Configuration;
using Ianitor.Common.Shared.Services;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.Services;
using Ianitor.Osp.Frontend.Client.Authentication;
using Ianitor.Osp.Frontend.Client.System;
using Ianitor.Osp.Frontend.Client.Tenants;
using Ianitor.Osp.ManagementTool.Commands;
using Ianitor.Osp.ManagementTool.Commands.Implementations;
using Ianitor.Osp.ManagementTool.Commands.Implementations.Authentication;
using Ianitor.Osp.ManagementTool.Commands.Implementations.Clients;
using Ianitor.Osp.ManagementTool.Commands.Implementations.IdentityProviders;
using Ianitor.Osp.ManagementTool.Commands.Implementations.NotificationMessages;
using Ianitor.Osp.ManagementTool.Commands.Implementations.ServiceHooks;
using Ianitor.Osp.ManagementTool.Commands.Implementations.Tenants;
using Ianitor.Osp.ManagementTool.Commands.Implementations.Users;
using Ianitor.Osp.ManagementTool.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

namespace Ianitor.Osp.ManagementTool
{
    internal static class Program
    {
        private static async Task<int> Main()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var servicesProvider = BuildDi();
                using (servicesProvider as IDisposable)
                {
                    var runner = servicesProvider.GetRequiredService<Runner>();
                    return await runner.DoActionAsync();
                }
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                return -100;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            // Runner is the custom class
            services.AddTransient<Runner>();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        $".{Constants.OspToolUserFolderName}{Path.DirectorySeparatorChar}settings.json"),
                    optional: true, reloadOnChange: true)
                .Build();

            services.Configure<OspToolOptions>(options =>
                config.GetSection(Constants.OspToolOptionsRootNode).Bind(options));

            services.Configure<OspToolAuthenticationOptions>(options =>
                config.GetSection(Constants.AuthenticationRootNode).Bind(options));

            // configure Logging with NLog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });

            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IParserService, ParserService>();
            services.AddSingleton<IParser, Parser>();
            services.AddSingleton<IConfigWriter, ConfigWriter>(provider =>
            {
                var configWriter = new ConfigWriter();
                configWriter.AddOptions(Constants.OspToolOptionsRootNode,
                    provider.GetService<IOptions<OspToolOptions>>());
                configWriter.AddOptions(Constants.AuthenticationRootNode,
                    provider.GetService<IOptions<OspToolAuthenticationOptions>>());
                return configWriter;
            });
            
            services.AddOptions<AuthenticatorOptions>()
                .Configure<IOptions<OspToolOptions>>(
                    (options, ospToolOptions) =>
                    {
                        options.IssuerUri = ospToolOptions.Value.IdentityServiceUrl;
                        options.ClientId = CommonConstants.OspToolClientId;
                        options.ClientSecret = CommonConstants.OspToolClientSecret;
                    });
            
            services.AddOptions<TenantClientOptions>()
                .Configure<IOptions<OspToolOptions>>(
                    (options, ospToolOptions) =>
                    {
                        options.TenantId = ospToolOptions.Value.TenantId;
                        options.EndpointUri = ospToolOptions.Value.CoreServiceUrl;
                    });
            
            services.AddOptions<CoreServiceClientOptions>()
                .Configure<IOptions<OspToolOptions>>(
                    (options, ospToolOptions) =>
                    {
                        options.EndpointUri = ospToolOptions.Value.CoreServiceUrl;
                    });
            
            services.AddOptions<JobServiceClientOptions>()
                .Configure<IOptions<OspToolOptions>>(
                    (options, ospToolOptions) =>
                    {
                        options.EndpointUri = ospToolOptions.Value.JobServiceUrl;
                    });

            services.AddOptions<IdentityServiceClientOptions>()
                .Configure<IOptions<OspToolOptions>>(
                    (options, ospToolOptions) =>
                    {
                        options.EndpointUri = ospToolOptions.Value.IdentityServiceUrl;
                    });

            services.AddSingleton<ITenantClientAccessToken, ServiceClientAccessToken>();
            services.AddSingleton<IJobServiceClientAccessToken, ServiceClientAccessToken>();
            services.AddSingleton<IIdentityServiceClientAccessToken, ServiceClientAccessToken>();
            services.AddSingleton<ICoreServiceClientAccessToken, ServiceClientAccessToken>();

            services.AddSingleton<ITenantClient, TenantClient>();
            services.AddSingleton<ICoreServicesClient, CoreServicesClient>();
            services.AddSingleton<IIdentityServicesClient, IdentityServicesClient>();
            services.AddSingleton<IIdentityServicesSetupClient, IdentityServicesSetupClient>();
            services.AddSingleton<IJobServicesClient, JobServicesClient>();
            services.AddSingleton<IAuthenticatorClient, AuthenticatorClient>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<INotificationRepository, WsNotificationRepository>();
            services.AddTransient<IOspCommand, ConfigOspCommand>();
            services.AddTransient<IOspCommand, SetupCommand>();
            services.AddTransient<IOspCommand, TestCommand>();

            services.AddTransient<IOspCommand, LogInCommand>();
            services.AddTransient<IOspCommand, AuthStatusCommand>();

            services.AddTransient<IOspCommand, ImportConstructionKitModel>();
            services.AddTransient<IOspCommand, ImportRuntimeModel>();
            services.AddTransient<IOspCommand, ExportRuntimeModel>();

            services.AddTransient<IOspCommand, GetClients>();
            services.AddTransient<IOspCommand, AddAuthorizationCodeClient>();
            services.AddTransient<IOspCommand, AddClientCredentialsClient>();
            services.AddTransient<IOspCommand, UpdateClient>();
            services.AddTransient<IOspCommand, DeleteClient>();

            services.AddTransient<IOspCommand, GetIdentityProviders>();
            services.AddTransient<IOspCommand, AddIdentityProvider>();
            services.AddTransient<IOspCommand, UpdateIdentityProvider>();
            services.AddTransient<IOspCommand, DeleteIdentityProvider>();

            services.AddTransient<IOspCommand, CreateTenant>();
            services.AddTransient<IOspCommand, CleanTenant>();
            services.AddTransient<IOspCommand, AttachTenant>();
            services.AddTransient<IOspCommand, DeleteTenant>();
            services.AddTransient<IOspCommand, ClearTenantCache>();

            services.AddTransient<IOspCommand, GetUsers>();
            services.AddTransient<IOspCommand, CreateUser>();
            services.AddTransient<IOspCommand, UpdateUser>();
            services.AddTransient<IOspCommand, DeleteUser>();
            services.AddTransient<IOspCommand, ResetPassword>();

            services.AddTransient<IOspCommand, GetServiceHooks>();
            services.AddTransient<IOspCommand, CreateServiceHook>();
            services.AddTransient<IOspCommand, UpdateServiceHook>();
            services.AddTransient<IOspCommand, DeleteServiceHook>();

            services.AddTransient<IOspCommand, GetNotificationMessages>();
            services.AddTransient<IOspCommand, CreateNotification>();
            services.AddTransient<IOspCommand, CompletePendingNotifications>();
            services.AddTransient<IOspCommand, ResetNotificationsInError>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}