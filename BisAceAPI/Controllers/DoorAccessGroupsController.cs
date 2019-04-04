using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http;
using BisAceAPI.Models;
using System.Text;
using BisAceAPI.Utils;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;
using BisAceAPIBusinessLogicInterface;

namespace BisAceAPI.Controllers
{
    //[ClaimsAuthorize]
    [RoutePrefix("api")]
    public class DoorAccessGroupsController : ABisApiController
    {
        #region Controller Meta Data
        private readonly Func<IBisResult> _resultFactory;
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        public CardsController(Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
        }
        #endregion


        [HttpGet]
        [Route("GetDoorAccessGroups/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult GetDoorAccessGroups(string cardNumber)
        {
            IBisResult result = _resultFactory();

            if (string.IsNullOrEmpty(cardNumber))
            {
                result.ErrorType = BisErrorType.InvalidInput;
                result.ErrorMessage = ConstantStrings.RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED;
                return CreateResponseFromResult(result);
            }

            try
            {
                API_RETURN_CODES_CS apiCallResult = TryLogin(out AccessEngine ace);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.Unauthorised;
                    result.ErrorMessage = ConstantStrings.RESPONSE_LOGIN_ERROR;
                    return CreateResponseFromResult(result);
                }

                // Get card by card number
                var cardId = GetCardId(cardNumber.PadLeft(12, '0'), ace);

                ACECards aceCard = new ACECards(ace);
                apiCallResult = aceCard.Get(cardId);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = ConstantStrings.RESPONSE_CARD_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                ACEPersons person = new ACEPersons(ace);
                apiCallResult = person.Get(aceCard.PERSID);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = ConstantStrings.RESPONSE_PERSON_NOT_FOUND;
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
