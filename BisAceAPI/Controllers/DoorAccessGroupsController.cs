using System;
using System.Web.Http;
using System.Web.Http.Description;
using BisAceAPIBusinessLogicInterface;
using BisAceAPIModels.Models;

namespace BisAceAPI.Controllers
{
    /// <summary>
    /// Controller for door access groups.
    /// </summary>
    //[ClaimsAuthorize]
    [RoutePrefix("api")]
    public class DoorAccessGroupsController : ABisApiController
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
        public DoorAccessGroupsController(Func<IBisResult> resultFactory,
            ICardsBusinessLogic cardsBusinessLogic)
        {
            _resultFactory = resultFactory;
            _cardsBL = cardsBusinessLogic;
        }
        #endregion

        /// <summary>
        /// Get door access groups
        /// </summary>
        /// <param name="cardNumber"></param>
        [HttpGet]
        [Route("GetDoorAccessGroups/{cardNumber}")]
        [ResponseType(typeof(BisCard))]
        public IHttpActionResult GetDoorAccessGroups(string cardNumber)
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
