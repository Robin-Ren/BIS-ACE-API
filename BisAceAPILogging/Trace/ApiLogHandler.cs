using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BisAceAPILogging.Trace
{
    /// <summary>
    /// Logs input/output calls for WebAPI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class ApiLogHandler<T> : DelegatingHandler
        where T: class
    {
        private static readonly ILog _log = LogProvider.For<ApiLogHandler<T>>();
        private readonly string _applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiLogHandler{T}"/> class.
        /// </summary>
        public ApiLogHandler()
        {
            _applicationName = typeof(T).FullName;
        }
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_log.IsTraceEnabled())
            {
                var apiLogEntry = CreateApiLogEntryWithRequestData(request);
                if (request.Content != null)
                {
                    //*MAS+1 01/08/18 WEB-1561 Check to see if the Task holds any Exceptions with the Request Data, if yes, do not populate the field. Else, it returns System.Aggregate exceptions.
                    await request.Content.ReadAsStringAsync()
                        .ContinueWith(task =>
                        {
                           apiLogEntry.RequestContentBody = (task.Exception != null) ? string.Empty : task.Result;
                        }, cancellationToken);
                }

                return await base.SendAsync(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        var response = task.Result;

                    // Update the API log entry with response info
                    apiLogEntry.ResponseStatusCode = (int)response.StatusCode;
                        apiLogEntry.ResponseTimestamp = DateTime.Now;

                        if (response.Content != null)
                        {
                            apiLogEntry.ResponseContentBody = response.Content.ReadAsStringAsync().Result;
                            apiLogEntry.ResponseContentType = response.Content.Headers.ContentType?.MediaType; //CSR 12/17 WEB-1214 don't crash when content type is null
                            apiLogEntry.ResponseHeaders = SerializeHeaders(response.Content.Headers);
                        }

                        _log.TraceFormat("{@apiLogEntry}", apiLogEntry);

                        return response;
                    }, cancellationToken);
            }
            else
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }

        /// <summary>
        /// Creates the API log entry with request data.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private ApiLogEntry CreateApiLogEntryWithRequestData(HttpRequestMessage request)
        {
            var context = ((HttpContextBase)request.Properties["MS_HttpContext"]);
            var routeData = request.GetRouteData();

            return new ApiLogEntry
            {
                Application = _applicationName,
                User = context.User.Identity.Name,
                Machine = Environment.MachineName,
                RequestContentType = context.Request.ContentType,
                RequestRouteTemplate = routeData.Route.RouteTemplate,
                RequestIpAddress = context.Request.UserHostAddress,
                RequestMethod = request.Method.Method,
                RequestHeaders = SerializeHeaders(request.Headers),
                RequestTimestamp = DateTime.UtcNow,
                RequestUri = request.RequestUri.ToString()
            };
        }

        /// <summary>
        /// Formats the headers so that we can actually serialize them into the log
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        private Dictionary<string, string> SerializeHeaders(HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = string.Empty;
                    foreach (var value in item.Value)
                    {
                        header += value + " ";
                    }

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }
            return dict;
        }
    }
}
