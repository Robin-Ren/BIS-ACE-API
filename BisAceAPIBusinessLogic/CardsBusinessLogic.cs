using System;
using System.Collections.Generic;
using BisAceAPIBase;
using BisAceAPIBusinessLogicInterface;
using BisAceAPILogging;
using BisAceAPIModels;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;
using BisAceAPIModels.Utils;

namespace BisAceAPIBusinessLogic
{
    public class CardsBusinessLogic : ICardsBusinessLogic
    {
        #region Private Members
        private readonly IPersonsBusinessLogic _personsBL;
        private readonly IAuthorizationsBusinessLogic _authsBL;
        private readonly IBisApplicationConfig _webApiConfig;
        private readonly Func<IBisResult> _resultFactory;
        private readonly ILog _logger;
        #endregion

        #region Constructors
        public CardsBusinessLogic(
            IPersonsBusinessLogic personsBL,
            IAuthorizationsBusinessLogic authsBL,
            IBisApplicationConfig config,
            Func<IBisResult> resultFactory,
            ILog logger
            )
        {
            _webApiConfig = config;
            _resultFactory = resultFactory;
            _personsBL = personsBL;
            _authsBL = authsBL;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Validate if card exists by given card number.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <returns>Instance of IBisResult.</returns>
        public IBisResult ValidateCardExist(AccessEngine ace, string cardNumber)
        {
            IBisResult result = _resultFactory();

            if (string.IsNullOrEmpty(cardNumber))
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_CARD_NUMBER_MUST_BE_PROVIDED;
                return result;
            }

            // Get card by card number
            var cardId = GetCardId(cardNumber.PadLeft(12, '0'), ace);

            ACECards aceCard = new ACECards(ace);
            API_RETURN_CODES_CS apiCallResult = aceCard.Get(cardId);

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.NotFound;
                result.ErrorMessage = BisConstants.RESPONSE_CARD_NOT_FOUND;
            }
            else
            {
                result.SetResource(aceCard);
            }

            return result;
        }

        /// <summary>
        /// Get card information by given ace card.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <returns>Instance of IBisResult.</returns>
        public IBisResult PopulateCardFromACECards(AccessEngine ace, ACECards aceCard)
        {
            IBisResult result = _resultFactory();
            if (aceCard == null)
            {
                result.ErrorType = BisErrorType.NotFound;
                result.ErrorMessage = BisConstants.RESPONSE_CARD_NOT_FOUND;
                return result;
            }

            result = _personsBL.GetPerson(ace, aceCard.PERSID);

            if (!result.IsSucceeded)
            {
                return result;
            }

            var person = result.GetResource<ACEPersons>();
            // Check auth per person
            var lstAuthorization = _authsBL.GetAuthorizationsForPersonId(ace, person.GetPersonId());

            BisCard card = new BisCard
            {
                CardNumber = aceCard.CARDNO.PadLeft(12, '0')
            };

            card.CardStartValidDate = person.AUTHFROM.ToString();
            card.CardExpiryDate = person.AUTHUNTIL.ToString();

            result.SetResource(card);

            return result;
        }

        /// <summary>
        /// Save card by input card information
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="card">Contains required information of card.</param>
        public IBisResult CreateCard(AccessEngine ace, BisCard card)
        {
            IBisResult result = _resultFactory();

            if (card == null)
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                return result;
            }

            ACECards aceCard = new ACECards(ace)
            {
                CARDNO = card.CardNumber.PadLeft(12, '0'),
                PERSID = card.PersonId.PadLeft(16, '0'),
                CODEDATA = HexadecimalEncodingHelper.ToHexString(card.CardNumber.PadLeft(12, '0'))
            };
            API_RETURN_CODES_CS apiCallResult = aceCard.Add();

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.OperationFailed;
                result.ErrorMessage = BisConstants.RESPONSE_UNABLE_TO_CREATE_OR_UPDATE_CARD;
                return result;
            }

            // SAVE PERSON DATA
            ACEPersons person = new ACEPersons(ace);
            apiCallResult = person.Get(card.PersonId);
            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                apiCallResult = person.Add();
            }

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.OperationFailed;
                result.ErrorMessage = BisConstants.RESPONSE_LOAD_OR_SAVE_PERSON_FAILED;
                return result;
            }

            // Save Person data
            return _personsBL.UpdatePersonFromCardInfo(ace, card, person);
        }

        /// <summary>
        /// Update card by input card information
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="aceCard">Existing ACECards instance.</param>
        /// <param name="card">Contains required information of card.</param>
        public IBisResult UpdateCard(AccessEngine ace, ACECards aceCard, BisCard card)
        {
            IBisResult result = _resultFactory();

            if (card == null)
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                return result;
            }

            // Save card data
            aceCard.PERSID = card.PersonId;

            API_RETURN_CODES_CS apiCallResult = aceCard.Update();

            if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
            {
                result.ErrorType = BisErrorType.OperationFailed;
                result.ErrorMessage = BisConstants.RESPONSE_UNABLE_TO_CREATE_OR_UPDATE_CARD;
                return result;
            }

            // Get PERSON
            result = _personsBL.GetPerson(ace, aceCard.PERSID);
            if (!result.IsSucceeded)
            {
                return result;
            }
            var person = result.GetResource<ACEPersons>();

            // Save Person data
            return _personsBL.UpdatePersonFromCardInfo(ace, card, person);
        }

        /// <summary>
        /// Delete card.
        /// </summary>
        /// <param name="aceCard">The ACECards instance to delete.</param>
        /// <returns>API_RETURN_CODES_CS Code of delete operation.</returns>
        public API_RETURN_CODES_CS DeleteCard(ACECards aceCard)
        {
            if (aceCard == null)
            {
                return API_RETURN_CODES_CS.API_CARD_INVALID_CARDNO_CS;
            }

            return aceCard.Delete();
        }

        /// <summary>
        /// Get card identifier by given card number
        /// </summary>
        /// <param name="cardNumber">Specified card number.</param>
        /// <param name="ace">BIS Access engine.</param>
        /// <returns>Card ID if found.</returns>
        public string GetCardId(string cardNumber, AccessEngine ace)
        {
            // Create query
            var ace_Query = new ACEQuery(ace);
            string strColumn = "cardid";
            string strTable = "bsuser.cards";
            string strWhere = String.Format("cardno='{0}' and status > 0", cardNumber);

            API_RETURN_CODES_CS result = ace_Query.Select(strColumn, strTable, strWhere);

            if (result == API_RETURN_CODES_CS.API_SUCCESS_CS)
            {
                var valueId = new ACEColumnValue();

                if (ace_Query.FetchNextRow())
                {
                    result = ace_Query.GetRowData("cardid", valueId);
                    return valueId.value;
                }
            }
            return string.Empty;
        }
    }
}
