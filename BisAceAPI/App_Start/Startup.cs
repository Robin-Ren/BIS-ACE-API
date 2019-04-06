using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using BisAceAPIBase;
using BisAceAPILogging;
using BisAceAPIModels;
using BisAceAPIModels.Options;
using BisAceDIContainer.DIContainer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BisAceAPI.Startup))]

namespace BisAceAPI
{
    internal partial class Startup
    {
        private BisDIContainer _diContainer;

        private Dictionary<int, BisApplicationConfig> _systemConfigs;

        private Dictionary<int, BisConfigurationManager> _bisOptionManagers = new Dictionary<int, BisConfigurationManager>();

        /// <summary>
        /// Change this to true to see first chance exceptions in the log.
        /// </summary>
        /// <remarks>Even handled exceptions will be logged, *generally* speaking. However, this
        /// will result in many, many duplicates in the log file due to the way that WebAPI
        /// handles exceptions.</remarks>
        private const bool _LogFirstChanceExceptionsInDebugMode = false;

        public void Configuration(IAppBuilder app)
        {
            try
            {
                ConfigureLogging(app);
                LoadSettings();
                SetupBisConfigurationManager();
                LoadDIContainer();
            }
            catch
            {
                // do nothing
            }
        }

        /// <summary>
        /// This method reads the Option files and builds them into an instance of <see cref="BisConfigurationManager"/>.
        /// The instance of <see cref="BisConfigurationManager"/> is then added to <see cref="_bisOptionManagers"/> dictionary.
        /// Option files are Xml files maintained on the server.
        /// Their location / names are configured in environment's settings file's  OptionFiles configuration setting.
        /// </summary>
        private void SetupBisConfigurationManager()
        {
            // Loop through all the system configs
            // In a multi-tenant setup, single API instance may cater to multiple environments
            // Each environment should have a its own system key and *.settings file.
            foreach (var systemConfig in _systemConfigs)
            {
                //Create an instance of configuration manager per system config
                var BisConfigManager = new BisConfigurationManager();

                BisApplicationConfig bisApplicationConfig = systemConfig.Value;
                int key = systemConfig.Key;

                //Add option files to configuration manager in case they are available
                int numberOfFilesAdded = AddOptionFilesToConfigurationManager(BisConfigManager, bisApplicationConfig);

                //If files were indeed added to the configuration manager, then load the xmls into the configuration manager
                if (numberOfFilesAdded > 0)
                {
                    BisConfigManager.Build();
                    _bisOptionManagers.Add(key, BisConfigManager);
                }
            }
        }

        /// <summary>
        /// This method verifies that the option files are indeed present on the file system.
        /// If yes, the files are added to <paramref name="BisConfigManager"/>.
        /// </summary>
        /// <param name="BisConfigManager"></param>
        /// <param name="workingConfiguration"></param>
        /// <returns>Number of Option files that were added</returns>
        private int AddOptionFilesToConfigurationManager(BisConfigurationManager BisConfigManager, BisApplicationConfig workingConfiguration)
        {
            int numberOfFilesAdded = 0;
            const char optionFileSeperator = ';';

            //If 'OptionFiles' setting is not configured, then this environment is not using them.
            //So, no need to proceed any further
            if (string.IsNullOrWhiteSpace(workingConfiguration.OptionFiles))
            {
                return numberOfFilesAdded;
            }

            foreach (var optionFileName in workingConfiguration.OptionFiles.Split(optionFileSeperator))
            {
                //Only if File exists, then add it to configuration manager
                if (File.Exists(optionFileName))
                {
                    BisConfigManager.AddFile(optionFileName, optional: true);
                    numberOfFilesAdded++;
                }
            }

            return numberOfFilesAdded;
        }

        private void LoadSettings()
        {
            _systemConfigs = new Dictionary<int, BisApplicationConfig>();

            string configDirectory = ConfigurationManager.AppSettings[BisConstants.CONFIG_SYS_FOLDER_CONFIG_SETTING];

            if (!string.IsNullOrWhiteSpace(configDirectory))
            {
                // Get all app settings with the format of Config:{id}. Pull off the "Config:" so we are
                //   left with just the system id
                var configSettingKeys = ConfigurationManager.AppSettings.AllKeys
                                        .Where(key => key.StartsWith(BisConstants.CONFIG_SYS_CONFIG_PREFIX));

                foreach (string key in configSettingKeys)
                {
                    if (int.TryParse(key.Remove(0, BisConstants.CONFIG_SYS_CONFIG_PREFIX.Length), out int sysId) && !_systemConfigs.ContainsKey(sysId))
                    {
                        // Get the file name and path to the specific settings file
                        string fileName = Path.Combine(configDirectory, ConfigurationManager.AppSettings[key] + ".settings");

                        // Load the config file
                        if (File.Exists(fileName))
                        {
                            var settings = Microsoft.Owin.Hosting.Utilities.SettingsLoader.LoadFromSettingsFile(fileName);

                            // If settings were actually loaded
                            if (settings.Count > 0)
                            {
                                // Create the new call context and add it in to the available contexts
                                _systemConfigs.Add(sysId, new BisApplicationConfig(settings));
                            }
                        }
                    }
                }
            }
        }

        private void LoadDIContainer()
        {
            _diContainer = new BisDIContainer(_systemConfigs, _bisOptionManagers, GlobalConfiguration.Configuration);
        }

        /// <summary>
        /// Setup the logging infrastructure
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureLogging(IAppBuilder app)
        {
            LoggingSetup.Startup(app);
        }
    }
}