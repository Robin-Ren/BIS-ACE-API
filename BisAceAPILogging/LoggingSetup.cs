using Microsoft.Owin.Logging;
using Owin;
using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;
using BisAceAPILogging.Trace;
using Serilog.Core;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace BisAceAPILogging
{
    /// <summary>
    /// Sets up the logging infrastructure
    /// </summary>
    public static class LoggingSetup
    {
        private static LoggingLevelSwitch _levelSwitch = new LoggingLevelSwitch();
        private static TraceLevel _minimumWebAPITraceLevel = TraceLevel.Warn;

        /// <summary>
        /// Creates logging infrastructure based on a configuration file
        /// </summary>
        /// <remarks>
        /// If there is no configuration file, a default logger will be created.
        /// </remarks>
        /// <param name="configurationFileLocation">The configuration file location.</param>
        public static void Startup(string configurationFileLocation)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(configurationFileLocation) && System.IO.File.Exists(configurationFileLocation))
                {
                    var settings = GetSettings(configurationFileLocation);
                    SetupInternalDiagnostics(settings);
                    CreateLogger(settings);
                }
                else
                {   //no file, logging would be disabled. Lets create a dummy logger to aid in troubleshooting
                    //when attached to process, the data will be written to the VS output window
                    //if in production, trace setup must be performed
                    //see https://msdn.microsoft.com/en-us/library/sk36c28t(v=vs.110).aspx
                    CreateDefaultLogger();

                    //write missing configuration file to log
                    if(string.IsNullOrWhiteSpace(configurationFileLocation))
                    {
                        Log.Logger.Information("Log configuration file was not specified");
                    }
                    else
                    {
                        Log.Logger.Information("Log configuration file {configurationFileLocation} could not be found", configurationFileLocation);
                    }
                }
            }
            catch (Exception error)
            {
                try
                {
                    CreateDefaultLogger();
                    Log.Logger.Error(error, "An error has occurred while creating the logger with configuration file {configurationFileLocation}", configurationFileLocation);
                }
                catch { }
            }
        }

        /// <summary>
        /// Adds logging infrastructure to the appbuilder
        /// </summary>
        /// <remarks>This replaces the OWIN logging factory with ours</remarks>
        /// <param name="app">The application.</param>
        public static void Startup(IAppBuilder app)
        {
            try
            {
                ReplaceOWINLogger(app);
            }
            catch (Exception error)
            {
                Log.Logger.Error(error, "An error has occurred while replacing the OWIN logging middle-ware");
            }
        }

        /// <summary>
        /// Enables trace logging on the HTTPConfiguration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="traceName">Name of the trace.</param>
        public static void Startup(HttpConfiguration configuration, string traceName)
        {
            EnableInputOutputTrace(configuration, _minimumWebAPITraceLevel, traceName);
        }
        /// <summary>
        /// Sets the log level for both our logger and the webAPI trace
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="webAPITraceLevel">The web API trace level.</param>
        /// <returns></returns>
        public static bool SetLogLevel(int logLevel, TraceLevel webAPITraceLevel)
        {
            var returnCode = true;
            if (Enum.IsDefined(typeof(Serilog.Events.LogEventLevel), logLevel))
            {
                _levelSwitch.MinimumLevel = (Serilog.Events.LogEventLevel)logLevel;
                Log.Logger.Information("LogLevel has been set to {logLevel}", (Serilog.Events.LogEventLevel)logLevel);
            }
            else
            {
                Log.Logger.Warning("Failed to set log level to {logLevel}", logLevel);
                returnCode = false;
            }

            SimpleTracer.WebAPITraceLevel = webAPITraceLevel;
            Log.Logger.Information("WebAPITraceLevel has been set to {webAPITraceLevel}", webAPITraceLevel);

            return returnCode;
        }

        /// <summary>
        /// Replaces the owin logger.
        /// </summary>
        /// <param name="app">The application.</param>
        private static void ReplaceOWINLogger(IAppBuilder app)
        {
            //replace OWIN logging factory with ours
            app.SetLoggerFactory(new LibLogLoggerFactory());
        }

        /// <summary>
        /// Creates a default logger
        /// </summary>
        /// <remarks>Trace logger is used</remarks>
        private static void CreateDefaultLogger()
        {
            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.ControlledBy(_levelSwitch)
                  .WriteTo.Trace()
                  .CreateLogger();
        }
        /// <summary>
        /// Gets the logging settings
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> GetSettings(string filePath)
        {
            return Microsoft.Owin.Hosting.Utilities.SettingsLoader.LoadFromSettingsFile(filePath);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private static void CreateLogger(IEnumerable<KeyValuePair<string, string>> settings)
        {
            _levelSwitch.MinimumLevel = GetLevelSwitch(settings);
            GetRollingFile(settings, out string pathFormatInSetting, out string outputTemplateInSetting, out int retainedFileCountLimitInSetting);

            //LibLog is used as an abstraction, so just create the logger here - no need to return it to caller
            Log.Logger = new LoggerConfiguration()
              //.ReadFrom.KeyValuePairs(settings)
              .WriteTo.RollingFile(
                pathFormat: pathFormatInSetting,
                outputTemplate: outputTemplateInSetting,
                retainedFileCountLimit: retainedFileCountLimitInSetting)
              .Enrich.WithMachineName() //ensure that machine name is auto-added to every entry
              .MinimumLevel.ControlledBy(_levelSwitch) //this will override the config file - we pulled the value above
              .CreateLogger();
        }
        /// <summary>
        /// Enables the input output trace for WebAPI
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="minimumWebAPITraceLevel">The minimum web API trace level.</param>
        /// <param name="traceName">Name of the trace.</param>
        private static void EnableInputOutputTrace(HttpConfiguration configuration,
            TraceLevel minimumWebAPITraceLevel, string traceName)
        {
            configuration.EnableSystemDiagnosticsTracing();
            configuration.Services.Replace(typeof(ITraceWriter), new SimpleTracer(traceName, minimumWebAPITraceLevel));
        }
        /// <summary>
        /// Gets the log level for serilog
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        private static Serilog.Events.LogEventLevel GetLevelSwitch(IEnumerable<KeyValuePair<string, string>> settings)
        {
            var dict = settings.ToDictionary(x => x.Key, x => x.Value);
            if (dict.ContainsKey("minimum-level"))
            {
                var value = dict["minimum-level"];
                if (Enum.TryParse(value, out Serilog.Events.LogEventLevel result))
                {
                    return result;
                }
            }
            return Serilog.Events.LogEventLevel.Information;
        }

        /// <summary>
        /// Gets the rolling file info for serilog
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pathFormat">Output pathFormat from settings.</param>
        /// <param name="outputTemplate">Output outputTemplate from settings.</param>
        /// <param name="retainedFileCountLimit">Output retainedFileCountLimit from settings.</param>
        private static void GetRollingFile(IEnumerable<KeyValuePair<string, string>> settings, out string pathFormat, out string outputTemplate, out int retainedFileCountLimit)
        {
            pathFormat = string.Empty;
            outputTemplate = string.Empty;
            retainedFileCountLimit = 0;

            var dict = settings.ToDictionary(x => x.Key, x => x.Value);

            if (dict.ContainsKey("write-to:RollingFile.pathFormat"))
            {
                pathFormat = dict["write-to:RollingFile.pathFormat"];
            }

            if (dict.ContainsKey("write-to:RollingFile.outputTemplate"))
            {
                outputTemplate = dict["write-to:RollingFile.outputTemplate"];
            }

            if (dict.ContainsKey("write-to:RollingFile.retainedFileCountLimit"))
            {
                var countLimit = dict["write-to:RollingFile.retainedFileCountLimit"];
                if(!int.TryParse(countLimit, out retainedFileCountLimit))
                {
                    retainedFileCountLimit = 5;
                }
            }
        }

        /// <summary>
        /// Setup the internal diagnostics.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private static void SetupInternalDiagnostics(IEnumerable<KeyValuePair<string, string>> settings)
        {
            var dict = settings.ToDictionary(x => x.Key, x => x.Value);
            var enabled = true;
            if(dict.ContainsKey("internal-Diagnostics"))
            {
                var value = dict["internal-Diagnostics"];
                bool.TryParse(value, out enabled);
            }
            if(enabled)
            {
                Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Trace.TraceInformation(msg));
                Serilog.Debugging.SelfLog.WriteLine("Internal serilog diagnostics enabled");
            }

            if (dict.ContainsKey("webapi-trace-minimum-level"))
            {
                var value = dict["webapi-trace-minimum-level"];
                if(Enum.TryParse(value, out TraceLevel setting))
                {
                    _minimumWebAPITraceLevel = setting;
                }
            }
        }
    }
}
