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
    public class DoorAccessGroupsBusinessLogic : IDoorAccessGroupsBusinessLogic
    {
        #region Private Members
        private readonly IDoorAccessGroupsDataAccess _dataAccess;
        private readonly IBisApplicationConfig _webApiConfig;
        private readonly Func<IBisResult> _resultFactory;
        private readonly ILog _logger;
        #endregion

        #region Constructors
        public DoorAccessGroupsBusinessLogic(
            IDoorAccessGroupsDataAccess dataAccess,
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
