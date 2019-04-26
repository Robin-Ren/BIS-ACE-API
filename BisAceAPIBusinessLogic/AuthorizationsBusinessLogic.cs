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
    public class AuthorizationsBusinessLogic : IAuthorizationsBusinessLogic
    {
        #region Private Members
        private readonly IAuthorizationsDataAccess _dataAccess;
        private readonly IPersonsBusinessLogic _personsBL;
        private readonly IBisApplicationConfig _webApiConfig;
        private readonly Func<IBisResult> _resultFactory;
        private readonly ILog _logger;
        #endregion

        #region Constructors
        public AuthorizationsBusinessLogic(
            IAuthorizationsDataAccess dataAccess,
            IPersonsBusinessLogic personsBL,
            IBisApplicationConfig config,
            Func<IBisResult> resultFactory,
            ILog logger
            )
        {
            _dataAccess = dataAccess;
            _personsBL = personsBL;
            _webApiConfig = config;
            _resultFactory = resultFactory;
            _logger = logger;
        }
        #endregion

        public List<ACEAuthorizations> GetAuthorizationsForPersonId(AccessEngine ace, string personId)
        {
            List<ACEAuthorizations> listAuthorizations = _dataAccess.GetAuthorizationsForViaAuthProfile(ace, personId);

            if (listAuthorizations.Count == 0)
            {
                listAuthorizations = _dataAccess.GetAuthorizationsForViaAuthPerPerson(ace, personId);
            }

            return listAuthorizations;
        }

        public IBisResult GetAllAuthorizationsForActiveCards(AccessEngine ace)
        {
            IBisResult result = _resultFactory();
            List<BisCardAuthorization> cardAuthorizations = new List<BisCardAuthorization>();

            var cardIds = _dataAccess.GetAllCardIds(ace);
            if (cardIds == null && cardIds.Count == 0)
                return result;

            foreach (var cardId in cardIds)
            {
                ACECards aceCard = new ACECards(ace);
                var apiCallResult = aceCard.Get(cardId);
                if (apiCallResult != API_RETURN_CODES_CS.API_SUCCESS_CS)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = BisConstants.RESPONSE_CARD_NOT_FOUND;
                    return result;
                }

                result = _personsBL.GetPerson(ace, aceCard.PERSID);
                if (!result.IsSucceeded)
                {
                    result.ErrorType = BisErrorType.OperationFailed;
                    result.ErrorMessage = BisConstants.RESPONSE_LOAD_OR_SAVE_PERSON_FAILED;
                    _logger.Error(result.ErrorMessage);
                    return result;
                }

                var person = result.GetResource<ACEPersons>();

                BisCardAuthorization cardAuth = new BisCardAuthorization
                {
                    CardNumber = aceCard.CARDNO,
                    CardStartValidDate = person.AUTHFROM.ToString(),
                    CardExpiryDate = person.AUTHUNTIL.ToString()
                };

                // Check auth per person
                var lstAuthorizations = GetAuthorizationsForPersonId(ace, person.GetPersonId());

                if (lstAuthorizations != null && lstAuthorizations.Count > 0)
                    cardAuth.Authorizations = new List<ACEAuthorizations>();
                cardAuth.Authorizations.AddRange(lstAuthorizations);
                cardAuthorizations.Add(cardAuth);
            }

            result.SetResource(cardAuthorizations);

            return result;
        }
    }
}
