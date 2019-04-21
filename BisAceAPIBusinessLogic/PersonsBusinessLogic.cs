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
    public class PersonsBusinessLogic : IPersonsBusinessLogic
    {
        #region Private Members
        private readonly IPersonsDataAccess _dataAccess;
        private readonly IBisApplicationConfig _webApiConfig;
        private readonly Func<IBisResult> _resultFactory;
        private readonly ILog _logger;
        #endregion

        #region Constructors
        public PersonsBusinessLogic(
            IPersonsDataAccess dataAccess,
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

        /// <summary>
        /// Get person information by given person id.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="personId">Person ID.</param>
        /// <returns>Instance of IBisResult.</returns>
        public IBisResult GetPerson(AccessEngine ace, string personId)
        {
            IBisResult result = _resultFactory();

            if (string.IsNullOrEmpty(personId))
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                _logger.Error(result.ErrorMessage + " [PersonId]");
                return result;
            }

            ACEPersons acePerson = new ACEPersons(ace);
            API_RETURN_CODES_CS apiCallResult = acePerson.Get(personId);

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.NotFound;
                result.ErrorMessage = BisConstants.RESPONSE_PERSON_NOT_FOUND;
                _logger.Error(result.ErrorMessage);
                return result;
            }

            result.SetResource(acePerson);
            return result;
        }

        /// <summary>
        /// Update person data by card information.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="card">Input card information.</param>
        /// <param name="person">Person instance to update.</param>
        public IBisResult UpdatePersonFromCardInfo(AccessEngine ace, BisCard card, ACEPersons person)
        {
            IBisResult result = _resultFactory();

            person.FIRSTNAME = card.PersonFirstName;
            person.LASTNAME = card.PersonLastName;

            API_RETURN_CODES_CS apiCallResult = person.Update();
            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.OperationFailed;
                result.ErrorMessage = BisConstants.RESPONSE_LOAD_OR_SAVE_PERSON_FAILED;
                _logger.Error(result.ErrorMessage);
                return result;
            }

            #region Authorizations
            if (card.AuthorizationIds != null && card.AuthorizationIds.Count > 0)
            {
                ACEDateT dateFrom = null;
                ACEDateT dateUtil = null;

                if (!string.IsNullOrEmpty(card.CardStartValidDate) &&
                    DateTime.TryParse(card.CardStartValidDate, out DateTime startValidDate))
                {
                    dateFrom = new ACEDateT((uint)startValidDate.Day, (uint)startValidDate.Month, (uint)startValidDate.Year);
                }

                if (!string.IsNullOrEmpty(card.CardExpiryDate) &&
                DateTime.TryParse(card.CardExpiryDate, out DateTime expiryDate))
                {
                    dateUtil = new ACEDateT((uint)expiryDate.Day, (uint)expiryDate.Month, (uint)expiryDate.Year);
                }

                apiCallResult = person.SetAuthorizations(card.AuthorizationIds.ToArray(),
                    new ACEDateT[] { dateFrom, dateFrom },
                    new ACEDateT[] { dateUtil, dateUtil });
                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.OperationFailed;
                    result.ErrorMessage = BisConstants.RESPONSE_SET_PERSON_AUTHORIZATIONS_FAILED;
                    _logger.Error(result.ErrorMessage);
                    return result;
                }
            }
            #endregion

            return result;
        }
    }
}
