using System;
using SimpleInjector;
using BisAceAPIModels.Models;

namespace BisAceDIContainer.DIContainer
{
    public static class DIContainerSetupExtensions
    {
        /// <summary>
        /// Registers types from core code base.
        /// </summary>
        /// <param name="container">The IoC container to register the types into.</param>
        public static void RegisterSharedDIComponents(this Container container)
        {
            RegisterResultTypes(container);
        }

        /// <summary>
        /// Registers the types related to results and errors.
        /// </summary>
        /// <param name="container">The IoC container to register the types into.</param>
        private static void RegisterResultTypes(Container container)
        {
            container.Register<IBisResult, BisResult>();
            container.Register<IBisErrorResponseViewModel, BisErrorResponseViewModel>();
            container.Register<Func<IBisErrorResponseViewModel>>(() => container.GetInstance<IBisErrorResponseViewModel>);
            container.Register<Func<IBisResult>>(() => container.GetInstance<IBisResult>);
        }
    }
}
