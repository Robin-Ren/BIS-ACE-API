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
    [RoutePrefix("api")]
    public class DoorAccessGroupsController : ABisApiController
    {
        #region Controller Meta Data
        private readonly Func<IBisResult> _resultFactory;
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        public DoorAccessGroupsController(Func<IBisResult> resultFactory,
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
