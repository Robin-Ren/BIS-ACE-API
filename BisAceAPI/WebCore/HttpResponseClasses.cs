using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BisAceAPIModels.Models;

namespace BisAceAPI.WebCore
{
    /// <summary>
    /// Result class which builds a 400 Http response with an object in the body.
    /// </summary>
    /// <seealso cref="System.Web.Http.IHttpActionResult" />
    internal class BadRequestWithModelResult : IHttpActionResult
    {
        private object _model;
        private HttpRequestMessage _request;

        /// <summary>
        /// Creates a new error result object.
        /// </summary>
        /// <param name="model">The object to include in the body of the response.</param>
        /// <param name="request">The request the response is for.</param>
        public BadRequestWithModelResult(object model, HttpRequestMessage request)
        {
            _model = model;
            _request = request;
        }

        /// <summary>
        /// Generate the response.
        /// </summary>
        /// <param name="cancellationToken">Token to use to cancel the action.</param>
        /// <returns>
        /// A HttpResponseMessage to return.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_request.CreateResponse(HttpStatusCode.BadRequest, _model));
        }
    }

    /// <summary>
    /// Result class which builds a 400 Http response with a WcpErrorResponseViewModel in the body.
    /// </summary>
    /// <seealso cref="System.Web.Http.IHttpActionResult" />
    internal class BadRequestWithErrorResponseModelResult : IHttpActionResult
    {
        private readonly BisErrorResponseViewModel _model;
        private HttpRequestMessage _request;

        /// <summary>
        /// Creates a new error result object.
        /// </summary>
        /// <param name="model">The WcpErrorResponseViewModel to include in the body of the response.</param>
        /// <param name="request">The request the response is for.</param>
        public BadRequestWithErrorResponseModelResult(BisErrorResponseViewModel model, HttpRequestMessage request)
        {
            _model = model;
            _request = request;
        }

        /// <summary>
        /// Generate the response.
        /// </summary>
        /// <param name="cancellationToken">Token to use to cancel the action.</param>
        /// <returns>
        /// A HttpResponseMessage to return.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_request.CreateResponse(HttpStatusCode.BadRequest, _model));
        }
    }

    /// <summary>
    /// Class for deserializing a BadRequest response from our old APIs.
    /// </summary>
    internal class BadRequestMessage
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}