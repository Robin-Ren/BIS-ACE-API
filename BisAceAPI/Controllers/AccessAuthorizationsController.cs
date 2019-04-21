using System;
using System.Web.Http;
using System.Web.Http.Description;
using BisAceAPIBusinessLogicInterface;
using BisAceAPILogging;
using BisAceAPIModels;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;

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
        private readonly IAuthorizationsBusinessLogic _authsBL;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AccessAuthorizationsController(Func<IBisResult> resultFactory,
            IAuthorizationsBusinessLogic authsBL,
            ILog logger)
        {
            _resultFactory = resultFactory;
            _authsBL =authsBL;
            _logger = logger;
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
                IBisResult result = TryLogin();
                if (!result.IsSucceeded)
                {
                    return CreateResponseFromResult(result);
                }

                var ace = result.GetResource<AccessEngine>();

                result  = _authsBL.GetAllAuthorizationsForActiveCards(ace);

                return CreateResponseFromResult(result);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetAuthorizations failed.",
                         ex);
                return InternalServerError(ex);
            }
        }

        #region Private Methods

        #endregion
    }
}
