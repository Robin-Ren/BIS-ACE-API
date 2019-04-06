using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http;
using System.Text;
using BisAceAPI.Utils;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIModels;

namespace BisAceAPI.Controllers
{
    //[ClaimsAuthorize]
    /// <summary>
    /// Cards controller.
    /// </summary>
    [RoutePrefix("api")]
    public class CardsController : ABisApiController
    {
        #region Controller Meta Data
        private readonly Func<IBisResult> _resultFactory;
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultFactory"></param>
        /// <param name="cardsBusinessLogic"></param>
        public CardsController(Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
        }
        #endregion

        /// <summary>
        /// Get Card by card number
        /// </summary>
        /// <param name="cardNumber">Specified card number.</param>
        [HttpGet]
        [Route("GetCard/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult Get(string cardNumber)
        {
            IBisResult result = _resultFactory();

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = BisConstants.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                // Validate if the card exists by card No.
                result = _cardsBL.ValidateCardExist(ace, cardNumber);
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }

                ACECards aceCard = result.GetResource<ACECards>();
                // Get card info
                result = _cardsBL.PopulateCardFromACECards(ace, aceCard);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Add card.
        /// </summary>
        /// <param name="card">Contains required information for card.</param>
        [HttpPost]
        [Route("AddCard")]
        [ResponseType(typeof(BisHttpResultBase))]
        public IHttpActionResult PostCard([FromBody]BisCard card)
        {
            IBisResult result = _resultFactory();

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = BisConstants.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                // Save card data
                result = _cardsBL.CreateCard(ace, card);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Update a card.
        /// </summary>
        /// <param name="card">Contains required information for card.</param>
        [HttpPut]
        [Route("UpdateCard")]
        [ResponseType(typeof(BisHttpResultBase))]
        public IHttpActionResult PutCard([FromBody]BisCard card)
        {
            IBisResult result = _resultFactory();

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = BisConstants.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                // Validate if the card exists by card No.
                result = _cardsBL.ValidateCardExist(ace, card.CardNumber);
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);

                }
                ACECards aceCard = result.GetResource<ACECards>();

                // Update card
                result = _cardsBL.UpdateCard(ace, aceCard, card);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Delete Card by card number
        /// </summary>
        /// <param name="cardNumber">Specified card number.</param>
        [HttpDelete]
        [Route("DeleteCard/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult DeleteCard(string cardNumber)
        {
            IBisResult result = _resultFactory();

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = BisConstants.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                if (string.IsNullOrEmpty(cardNumber))
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = BisConstants.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                    return CreateResponseFromResult(result);
                }

                // Validate if the card exists by card No.
                result = _cardsBL.ValidateCardExist(ace, cardNumber);
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }
                ACECards aceCard = result.GetResource<ACECards>();

                // Delete card
                apiCallResult = _cardsBL.DeleteCard(aceCard);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = BisConstants.RESPONSE_CARD_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                result.SetResource(aceCard);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetDoorAccessGroups/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult GetDoorAccessGroups(string cardNumber)
        {
            IBisResult result = _resultFactory();

            if (string.IsNullOrEmpty(cardNumber))
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = BisConstants.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                return CreateResponseFromResult(result);
            }

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = BisConstants.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                // Get card by card number
                var cardId = _cardsBL.GetCardId(cardNumber.PadLeft(12, '0'), ace);

                ACECards aceCard = new ACECards(ace);
                apiCallResult = aceCard.Get(cardId);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = BisConstants.RESPONSE_CARD_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                ACEPersons person = new ACEPersons(ace);
                apiCallResult = person.Get(aceCard.PERSID);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = BisConstants.RESPONSE_PERSON_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                BisCard card = new BisCard
                {
                    CardNumber = aceCard.CARDNO.PadLeft(12, '0')
                };
                person.GetCustomFieldValue("CardName", out string cardName);
                //person.GetCustomFieldValue("CardStartValidDate", out string cardStartValidDate);

                card.CardName = cardName;
                //card.CardStartValidDate = cardStartValidDate;

                result.SetResource(card);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #region Private Methods

        #endregion
    }
}
