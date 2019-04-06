using System.Threading.Tasks;
using System.Collections.Generic;
using BisAceAPIDataAccess;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIDataAccessInterface;
using BisAceAPIBase;
using BisAceAPIModels.Models;
using System;
using BisAceAPIModels.Models.Enums;
using BisAceAPIModels;
using BisAceAPILogging;

namespace BisAceAPIBusinessLogic
{
    public class LiftAccessGroupsBusinessLogic : ILiftAccessGroupsBusinessLogic
    {
        #region Private Members
        private readonly ILiftAccessGroupsDataAccess _dataAccess;
        private readonly IBisApplicationConfig _webApiConfig;
        private readonly Func<IBisResult> _resultFactory;
        private readonly ILog _logger;
        #endregion

        #region Constructors
        public LiftAccessGroupsBusinessLogic(
            ILiftAccessGroupsDataAccess dataAccess,
            IBisApplicationConfig config,
            Func<IBisResult> resultFactory,
            ILog logger
            )
        {
            _dataAccess = dataAccess;
            _webApiConfig = config;
            _resultFactory = resultFactory;
            _logger = logger;
        }
        #endregion

    }
}
