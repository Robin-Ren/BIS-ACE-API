using System.Collections.Generic;
using BisAceAPIBase;
using BisAceAPILogging;

namespace BisAceDIContainer.DIContainer
{
    public class BisCallContextFactory : ABisCallContextFactory<BisAceAPICallContext>
    {
        public BisDIContainer DIContainer { get; set; }
        public Dictionary<int, BisApplicationConfig> SystemConfigs { get; set; }

        public override IDictionary<string, BisAceAPICallContext> BuildCallContexts(int systemId)
        {

            IDictionary<string, BisAceAPICallContext> availableContexts = null;
            BisAceAPICallContext callContext = null;

            try
            {
                availableContexts = new Dictionary<string, BisAceAPICallContext>();

                callContext = DIContainer.GetCallContext(systemId);
                callContext.BisConfig = SystemConfigs[systemId];

                availableContexts.Add("1", callContext);
            }
            catch (System.Exception ex)
            {
                LogProvider.For<BisCallContextFactory>().ErrorException("Error Building BIS ACE API Context", ex);
            }

            return availableContexts;
        }
    }
}
