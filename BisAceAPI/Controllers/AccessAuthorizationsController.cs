using System;
using System.Web.Http;
using System.Web.Http.Description;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIModels.Models;

namespace BisAceAPI.Controllers
{
    /// <summary>
    /// Controller for access authorizations.
    /// </summary>
    //[ClaimsAuthorize]
    [RoutePrefix("api")]
    public class AccessAuthorizationsController : ABisApiController
    {
        #region Controller Meta Data
        private readonly ICardsBusinessLogic _cardsBL;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultFactory"></param>
        /// <param name="cardsBusinessLogic"></param>
        public AccessAuthorizationsController(Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
        }
        #endregion

        /// <summary>
        /// Get access authorizations
        /// </summary>
        [HttpGet]
        [Route("GetAuthorizations")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult GetAllAuthorizations()
        {
            try
            {
                IBisResult result = TryLogin(out AccessEngine ace);



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
