using System.Collections.Generic;
using System.Web.Http;
using BisAceAPIBase;
using BisAceAPIBusinessLogic;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIDataAccess;
using BisAceAPIDataAccessInterface;
using BisAceAPILogging;
using BisAceAPIModels.Options;
using BisAceDatabaseContext;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;

namespace BisAceDIContainer.DIContainer
{
    public class BisDIContainer
    {
        private SimpleInjector.Container _container;
        private BisApplicationConfig _currentConfig;
        private readonly Dictionary<int, BisApplicationConfig> _systemConfigs;
        private readonly Dictionary<int, BisConfigurationManager> _bisConfigurationManagers;

        /// <summary>
        /// Initializes a new instance of the DIContainer class.
        /// </summary>
        /// <param name="systemConfigs">The system configs.</param>
        /// <param name="bisConfigurationManagers">The configuration managers.</param>
        /// <param name="globalConfig">The global configuration.</param>
        public BisDIContainer(Dictionary<int, BisApplicationConfig> systemConfigs,
                              Dictionary<int, BisConfigurationManager> bisConfigurationManagers,
                              HttpConfiguration globalConfig)
        {
            _systemConfigs = systemConfigs;
            if (this._systemConfigs.ContainsKey(1))
            {
                // Set the current config file
                _currentConfig = this._systemConfigs[1];
            }

            _bisConfigurationManagers = bisConfigurationManagers;

            ProcessCommonRegister(globalConfig);

            // Register the call context
            _container.Register<BisAceAPICallContext, BisAceAPICallContext>();
        }

        public BisDIContainer(BisApplicationConfig systemConfig, BisConfigurationManager bisConfigurationManager,
                                 HttpConfiguration globalConfig)
        {
            _bisConfigurationManagers = new Dictionary<int, BisConfigurationManager>
            {
                { 0, bisConfigurationManager }
            };
            _currentConfig = systemConfig;

            ProcessCommonRegister(globalConfig);
        }

        /// <summary>
        /// Process all the common register work for various of APIs
        /// </summary>
        /// <param name="globalConfig">The global configuration.</param>
        private void ProcessCommonRegister(HttpConfiguration globalConfig)
        {
            _container = new SimpleInjector.Container();
            _container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();

            #region Unregistered Types
            #region Summary
            /*
            *  This section sets up how to resolve unregistered types.
            *
            *  IDatabase Context
            *     To create one of our database types, we need a connection string which will be different based on what
            *     system the database is being created for. For now, we'll always create a SQLServerDatabase. Eventually
            *     what we want is to create the database type based on a configuration.
            *
            *  IBisApplicationConfig
            *     When needing to resolve an IBisApplicationConfiguration, we'll use the current configuration.
            *
            *  IQuoteHeader
            *     When creating a quote header, we need to use the system's quote header definition file which is specified
            *     in the current configuration object.
            *     
            *  IEmailService
            *     When creating an email service, we need the SMTP user, host, and port settings
            */
            #endregion Summary
            _container.ResolveUnregisteredType += (s, e) =>
            {
                // Set up what to do when we try to register a database context. We need to pass in the connection string in the constructor.
                // Currently, we only support SqlServer databases for the application
                if (e.UnregisteredServiceType == typeof(IDatabaseContext))
                {
                    e.Register(() => new SqlServerDatabase(_currentConfig.BISConnectionString));
                }
                else if (e.UnregisteredServiceType == typeof(IBisApplicationConfig))
                {
                    e.Register(() => _currentConfig);
                }
            };
            #endregion

            //Register shared types
            _container.RegisterSharedDIComponents();

            RegisterDataAccess();
            RegisterBusinessLogic();
            RegisterLogger();

            // Set up the Controllers to use dependency injection
            _container.RegisterWebApiControllers(globalConfig);
            globalConfig.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(_container);
        }

        #region Register Methods
        /// <summary>
        /// Registers business logic classes in the IoC container.
        /// </summary>
        private void RegisterBusinessLogic()
        {
            // ToDo - Use reflection for this so we don't pull in the actual dll references
            _container.Register<ICardsBusinessLogic, CardsBusinessLogic>();
            _container.Register<IPersonsBusinessLogic, PersonsBusinessLogic>();
            _container.Register<IDoorAccessGroupsBusinessLogic, DoorAccessGroupsBusinessLogic>();
            _container.Register<ILiftAccessGroupsBusinessLogic, LiftAccessGroupsBusinessLogic>();
        }

        /// <summary>
        /// Registers data access classes in the IoC container.
        /// </summary>
        private void RegisterDataAccess()
        {
            _container.Register<ICardsDataAccess, CardsDataAccess>();
            _container.Register<IPersonsDataAccess, PersonsDataAccess>();
            _container.Register<IDoorAccessGroupsDataAccess, DoorAccessGroupsDataAccess>();
            _container.Register<ILiftAccessGroupsDataAccess, LiftAccessGroupsDataAccess>();
        }

        /// <summary>
        /// Registers the logger to use in the IoC container.
        /// </summary>
        private void RegisterLogger()
        {
            _container.RegisterConditional(typeof(ILog),
                c => typeof(LogAdapter<>).MakeGenericType(c.Consumer.ImplementationType),
                Lifestyle.Singleton,
                c => true);
        }
        #endregion

        /// <summary>
        /// Gets the BisAce call context for the specified system.
        /// </summary>
        /// <param name="systemId">Id of the system to get the call context for.</param>
        /// <returns>
        /// The call context to use for the API call.
        /// </returns>
        public BisAceAPICallContext GetCallContext(int systemId)
        {
            if (this._systemConfigs.ContainsKey(systemId))
            {
                // Set the current config file
                _currentConfig = this._systemConfigs[systemId];
            }

            try
            {
                return _container.GetInstance<BisAceAPICallContext>();
            }
            finally
            {
                _currentConfig = null;
            }
        }
    }
}