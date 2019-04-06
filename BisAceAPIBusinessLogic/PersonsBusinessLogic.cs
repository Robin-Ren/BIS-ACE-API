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
                return result;
            }

            ACEPersons acePerson = new ACEPersons(ace);
            API_RETURN_CODES_CS apiCallResult = acePerson.Get(personId);

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.NotFound;
                result.ErrorMessage = BisConstants.RESPONSE_PERSON_NOT_FOUND;
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

            // API_PERS_INVALID_CUSTOM_FIELD_NAME is returned if fieldname is not found
            API_RETURN_CODES_CS apiCallResult = person.SetCustomFieldValue(("CardName"), card.CardName);
            //apiCallResult = person.SetCustomFieldValue("CardStartValidDate", card.CardStartValidDate);

            apiCallResult = person.Update();
            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_BIS_API_CALL_FAILED;
                return result;
            }

            return result;
        }
    }
}
