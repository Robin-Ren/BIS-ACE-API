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
    public class BISController : BisApiController
    {
        #region Controller Meta Data
        private readonly Func<IBisResult> _resultFactory;
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        public BISController(Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
        }
        #endregion

        [HttpGet]
        [Route("GetCard/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult Get(string cardNumber)
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

        [HttpPost]
        [Route("AddCard")]
        [ResponseType(typeof(BisHttpResultBase))]
        public IHttpActionResult PostCard([FromBody]BisCard card)
        {
            IBisResult result = _resultFactory();

            if (card == null)
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

                // Save card data
                var personId = "0013475D736CCE66";
                ACECards aceCard = new ACECards(ace)
                {
                    CARDNO = card.CardNumber,
                    PERSID = personId,
                    CODEDATA = HexadecimalEncodingHelper.ToHexString(card.CardNumber.PadLeft(12, '0'))
                };
                apiCallResult = aceCard.Add();

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = ConstantStrings.RESPONSE_BIS_API_CALL_FAILED;
                    return CreateResponseFromResult(result);
                }

                // SAVE PERSON DATA
                ACEPersons person = new ACEPersons(ace);
                apiCallResult = person.Get(personId);

                // API_PERS_INVALID_CUSTOM_FIELD_NAME is returned if fieldname is not found
                apiCallResult = person.SetCustomFieldValue(("CardName"), card.CardName);
                //apiCallResult = person.SetCustomFieldValue("CardStartValidDate", card.CardStartValidDate);

                apiCallResult = person.Update();
                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = ConstantStrings.RESPONSE_BIS_API_CALL_FAILED;
                    return CreateResponseFromResult(result);
                }

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("UpdateCard")]
        [ResponseType(typeof(BisHttpResultBase))]
        public IHttpActionResult PutCard([FromBody]BisCard card)
        {
            IBisResult result = _resultFactory();

            if (card == null ||
                string.IsNullOrEmpty(card.CardNumber))
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
                var cardId = GetCardId(card.CardNumber.PadLeft(12, '0'), ace);

                if (string.IsNullOrEmpty(cardId))
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = ConstantStrings.RESPONSE_CARD_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                ACECards aceCard = new ACECards(ace);
                apiCallResult = aceCard.Get(cardId);

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = ConstantStrings.RESPONSE_CARD_NOT_FOUND;
                    return CreateResponseFromResult(result);
                }

                // Save card data
                var personId = "0013475D736CCE66";

                //aceCard.CARDNO = card.CardNumber;
                //aceCard.PERSID = personId;
                aceCard.CODEDATA = HexadecimalEncodingHelper.ToHexString(card.CardNumber.PadLeft(12, '0'));
                //aceCard.CODEDATA2 = HexadecimalEncodingHelper.ToHexString("2");
                //aceCard.CODEDATA3 = HexadecimalEncodingHelper.ToHexString("2");
                //aceCard.CODEDATA4 = HexadecimalEncodingHelper.ToHexString("2");

                apiCallResult = aceCard.Update();

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = ConstantStrings.RESPONSE_BIS_API_CALL_FAILED;
                    return CreateResponseFromResult(result);
                }

                // SAVE PERSON DATA
                ACEPersons person = new ACEPersons(ace);
                apiCallResult = person.Get(personId);

                // API_PERS_INVALID_CUSTOM_FIELD_NAME is returned if fieldname is not found
                apiCallResult = person.SetCustomFieldValue(("CardName"), card.CardName);
                //apiCallResult = person.SetCustomFieldValue("CardStartValidDate", card.CardStartValidDate);

                apiCallResult = person.Update();
                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.InvalidInput;
                    result.ErrorMessage = ConstantStrings.RESPONSE_BIS_API_CALL_FAILED;
                    return CreateResponseFromResult(result);
                }

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("DeleteCard/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult DeleteCard(string cardNumber)
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

                apiCallResult = aceCard.Delete();

                if (API_RETURN_CODES_CS.API_SUCCESS_CS != apiCallResult)
                {
                    result.ErrorType = BisErrorType.NotFound;
                    result.ErrorMessage = ConstantStrings.RESPONSE_CARD_NOT_FOUND;
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

        private string GetCardId(string strCardNo, AccessEngine ace)
        {
            // Create query
            var ace_Query = new ACEQuery(ace);
            string strColumn = "cardid";
            string strTable = "cards";
            string strWhere = String.Format("cardno='{0}' and status>0", strCardNo);

            // Fetch card(s) for person

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

        #endregion
    }
}
