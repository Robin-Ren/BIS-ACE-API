using System;
using System.Collections.Generic;

namespace BisAceAPIModels.Options
{
    /// <summary>
    /// Extensions methods class for the Simple Injector container
    /// </summary>
    public static class BisDependencyInjectionContainerOptionsExtensions
    {
        /// <summary>
        /// Given a dictionary of configurations for systems and a section name, registers a collection of IWcpOption objects into the container
        /// </summary>
        /// <typeparam name="T">The type of options being created.</typeparam>
        /// <param name="container">The dependency injection container to register the resulting collection with.</param>
        /// <param name="systemConfigs">A dictionary of system configuration managers keyed by system id.</param>
        /// <param name="sectionName">The name of the section to find in each configuration to build out the options.</param>
        /// <param name="afterCreate">Optional. Method taking in the new options to perform post-processing.</param>
        public static void RegisterOptionsForSystems<T>(this SimpleInjector.Container container, Dictionary<int, BisConfigurationManager> systemConfigs, string sectionName, Action<T> afterCreate = null)
            where T : class, new()
        {
            // Create the list to hold the options
            var systemOptions = new List<IBisOptions<T>>();

            // Add in an options instance for each system
            foreach (var sysConfig in systemConfigs)
            {
                systemOptions.Add(BisOptions<T>.ConfigureFromConfiguration(sysConfig.Value, sectionName, sysConfig.Key, afterCreate));
            }

            // Register the collection
            container.RegisterCollection<IBisOptions<T>>(systemOptions);
        }

        /// <summary>
        /// Registers a set of options to the IoC container. These options will not be tied to any given system so only one instance can be registered per options type.
        /// </summary>
        /// <typeparam name="T">The type of options class to register.</typeparam>
        /// <param name="container">The container to register the options with.</param>
        /// <param name="configManager">The configuration manager with the loaded options files to pull the option values from.</param>
        /// <param name="sectionName">The name of the section to use to get the sections.</param>
        /// <param name="afterCreate">Optional. Action which runs after the options have been created which can be used to set data.</param>
        public static IBisOptions<T> RegisterOptions<T>(this SimpleInjector.Container container, BisConfigurationManager configManager, string sectionName, Action<T> afterCreate = null)
            where T : class, new()
        {
            IBisOptions<T> options = BisOptions<T>.ConfigureFromConfiguration(configManager, sectionName, 0, afterCreate);
            container.RegisterSingleton(options);

            return options;
        }
    }
}
