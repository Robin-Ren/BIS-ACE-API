using System.Threading.Tasks;
using System.Collections.Generic;
using BisAceAPIDataAccess;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIDataAccessInterface;
using BisAceAPIBase;

namespace BisAceAPIBusinessLogic
{
    public class CardsBusinessLogic : ICardsBusinessLogic
    {
        private ICardsDataAccess _dataAccess;
        private IBisApplicationConfig _webApiConfig;

        public CardsBusinessLogic(ICardsDataAccess dataAccess, IBisApplicationConfig config)
        {
            _dataAccess = dataAccess;
            _webApiConfig = config;
        }
    }
}
