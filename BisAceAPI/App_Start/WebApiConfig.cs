using System.Web.Http;
using System.Configuration;
using System.IO;
using BisAceAPIModels;
using BisAceAPILogging;

namespace BisAceAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            ConfigureLogging(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        /// <summary>
        /// Configures the logging.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void ConfigureLogging(HttpConfiguration config)
        {
            var configDirectory = ConfigurationManager.AppSettings[BisConstants.CONFIG_SYS_FOLDER_CONFIG_SETTING];
            string loggingFile = string.Empty;
            if (!string.IsNullOrWhiteSpace(configDirectory))
            {
                loggingFile = Path.Combine(configDirectory, BisConstants.LOGGING_CONFIGURATION_FILENAME);
            }
            LoggingSetup.Startup(loggingFile);
            LoggingSetup.Startup(config, "BisAceAPI");
        }
    }
}
