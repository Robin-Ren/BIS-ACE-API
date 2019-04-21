using System;
using System.Web.Http;
using System.Web.Http.Description;
using BisAceAPIBusinessLogicInterface;
using BisAceAPILogging;
using BisAceAPIModels;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;
using Newtonsoft.Json;

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
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CardsController(
            Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic,
            ILog logger)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
            _logger = logger;
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
            try
            {
                IBisResult result = TryLogin();
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }
                var ace = result.GetResource<AccessEngine>();
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
                _logger.ErrorException(string.Format("GetCard/{0} failed.", cardNumber),
                    ex);
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
            try
            {
                IBisResult result = TryLogin();
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }

                var ace = result.GetResource<AccessEngine>();
                // Save card data
                result = _cardsBL.CreateCard(ace, card);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("AddCard failed. Input request body: {0}", JsonConvert.SerializeObject(card)),
                       ex);
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
            try
            {
                IBisResult result = TryLogin();
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }

                var ace = result.GetResource<AccessEngine>();

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
                _logger.ErrorException(string.Format("UpdateCard failed. Input request body: {0}", JsonConvert.SerializeObject(card)),
                      ex);
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
            try
            {
                IBisResult result = TryLogin();
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }

                var ace = result.GetResource<AccessEngine>();

                if (string.IsNullOrEmpty(cardNumber))
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = BisConstants.RESPONSE_CARD_NUMBER_MUST_BE_PROVIDED;
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
                API_RETURN_CODES_CS apiCallResult = _cardsBL.DeleteCard(aceCard);

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
                _logger.ErrorException(string.Format("DeleteCard/{0}", cardNumber),
                         ex);
                return InternalServerError(ex);
            }
        }

        #region Private Methods

        #endregion
    }
}
