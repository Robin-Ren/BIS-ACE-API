
using System;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIModels.Models;

namespace BisAceAPIBase
{
    /// <summary>
    /// Environment-specific context information for application API calls.
    /// </summary>
    public class BisAceAPICallContext : ABisAceAPICallContext
    {
        /// <summary>
        /// Factory method for creating a default result class.
        /// </summary>
        public Func<IBisResult> ResultFactory;

        /// <summary>
        /// Gets the Cards Business Logic object to use for a call
        /// </summary>
        public ICardsBusinessLogic CardsBL { get; private set; }

        public BisAceAPICallContext(
            Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic
            )
        {
            CardsBL = cardsBusinessLogic;
            ResultFactory = resultFactory;
        }
    }
}
