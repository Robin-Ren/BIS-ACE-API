using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BisAceAPI.WebCore;
using BisAceAPIModels.Models;
using BisAceAPIModels.Models.Enums;

namespace BisAceAPI.Controllers
{
    public abstract class BisApiController : ApiController
    {
        protected API_RETURN_CODES_CS TryLogin(out AccessEngine aceEngine)
        {
            IEnumerable<string> userValues;
            IEnumerable<string> passwordValues;
            var userId = string.Empty;
            var password = string.Empty;

            if (Request.Headers.TryGetValues("user", out userValues))
            {
                var usersEnumerator = userValues.GetEnumerator();
                if (usersEnumerator.MoveNext())
                {
                    userId = usersEnumerator.Current;
                }
            }
            if (Request.Headers.TryGetValues("password", out passwordValues))
            {
                var passwordEnumerator = passwordValues.GetEnumerator();
                if (passwordEnumerator.MoveNext())
                {
                    password = passwordEnumerator.Current;
                }
            }

            // Do the login process here
            aceEngine = new AccessEngine();
            API_RETURN_CODES_CS result = aceEngine.Login(userId, password, ConfigurationHelper.SERVER_NAME);

            return result;
        }

        /// <summary>
        /// Create a 4xx-level response based on the provided <see cref="IBisResult"/> object.
        /// </summary>
        /// <param name="errorResult">The result with the error details.</param>
        /// <returns>A 400-level response based on the data in the provided error result.</returns>
        protected IHttpActionResult Create4xxErrorResponse(IBisResult errorResult)
        {
            // If the error is not found, return a 404
            if (errorResult.ErrorType == BisErrorType.NotFound)
            {
                return NotFound();
            }

            // Otherwise, create a new response body and return a 400
            return new BadRequestWithErrorResponseModelResult(errorResult.CreateErrorResponseModel(), Request);
        }

        /// <summary>
        /// Create a 200 or 4xx-level response based on the provided <see cref="IBisResult" /> object.
        /// </summary>
        /// <param name="bisResult">The result.</param>
        /// <returns>
        /// A 200 or 400-level response based on the data in the provided result.
        /// </returns>
        protected IHttpActionResult CreateResponseFromResult(IBisResult bisResult)
        {
            if (bisResult.IsSucceeded)
                return Ok(bisResult);
            else
                // The result is not valid so respond with the appropriate error.
                return Create4xxErrorResponse(bisResult);
        }

        /// <summary>
        /// Create a 200 or 4xx-level response based on the provided <see cref="IBisResult" /> object.
        /// The resource specified will be returned within a 200 result.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="BisResult">The result.</param>
        /// <returns>
        /// A 200 or 400-level response based on the data in the provided result.
        /// </returns>
        protected IHttpActionResult CreateResponseFromResult<TResource>(IBisResult result) where TResource : class
        {
            if (result.IsSucceeded)
                return Ok(result.GetResource<TResource>());
            else
                // The result is not valid so respond with the appropriate error.
                return Create4xxErrorResponse(result);
        }

        /// <summary>
        /// exposes the OK result
        /// </summary>
        /// <returns>OK result</returns>
        public IHttpActionResult ObtainOK()
        {
            return Ok();
        }

        /// <summary>
        /// returns the specified response message
        /// </summary>
        /// <param name="message">response message to return</param>
        /// <returns>result</returns>
        public IHttpActionResult ObtainResponseMessage(HttpResponseMessage message)
        {
            return ResponseMessage(message);
        }

        /// <summary>
        /// exposes a bad request result for the specified message
        /// </summary>
        /// <param name="msg">message text</param>
        /// <returns>bad request result with the specified text</returns>
        public IHttpActionResult GetBadRequest(string msg)
        {
            return BadRequest(msg);
        }
    }
}