using System;
using System.Collections.Generic;
namespace BisAceAPILogging.Trace
{
    /// <summary>
    /// Tracks a request/response call, including the inputs/outputs
    /// </summary>
    public class ApiLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiLogEntry"/> class.
        /// </summary>
        public ApiLogEntry()
        {
            ApiLogEntryId = Guid.NewGuid();
        }
        /// <summary>
        /// Gets or sets the API log entry identifier.
        /// </summary>
        /// <value>
        /// The API log entry identifier.
        /// </value>
        public Guid ApiLogEntryId { get; set; }
        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public string Application { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public string User { get; set; }
        /// <summary>
        /// Gets or sets the machine.
        /// </summary>
        /// <value>
        /// The machine.
        /// </value>
        public string Machine { get; set; }
        /// <summary>
        /// Gets or sets the request ip address.
        /// </summary>
        /// <value>
        /// The request ip address.
        /// </value>
        public string RequestIpAddress { get; set; }
        /// <summary>
        /// Gets or sets the type of the request content.
        /// </summary>
        /// <value>
        /// The type of the request content.
        /// </value>
        public string RequestContentType { get; set; }
        /// <summary>
        /// Gets or sets the request content body.
        /// </summary>
        /// <value>
        /// The request content body.
        /// </value>
        public string RequestContentBody { get; set; }
        /// <summary>
        /// Gets or sets the request URI.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        public string RequestUri { get; set; }
        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        /// <value>
        /// The request method.
        /// </value>
        public string RequestMethod { get; set; }
        /// <summary>
        /// Gets or sets the request route template.
        /// </summary>
        /// <value>
        /// The request route template.
        /// </value>
        public string RequestRouteTemplate { get; set; }
        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        /// <value>
        /// The request headers.
        /// </value>
        public Dictionary<string, string> RequestHeaders { get; set; }
        /// <summary>
        /// Gets or sets the request timestamp.
        /// </summary>
        /// <value>
        /// The request timestamp.
        /// </value>
        public DateTime? RequestTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the type of the response content.
        /// </summary>
        /// <value>
        /// The type of the response content.
        /// </value>
        public string ResponseContentType { get; set; }
        /// <summary>
        /// Gets or sets the response content body.
        /// </summary>
        /// <value>
        /// The response content body.
        /// </value>
        public string ResponseContentBody { get; set; }
        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        /// <value>
        /// The response status code.
        /// </value>
        public int? ResponseStatusCode { get; set; }
        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        /// <value>
        /// The response headers.
        /// </value>
        public Dictionary<string, string> ResponseHeaders { get; set; }
        /// <summary>
        /// Gets or sets the response timestamp.
        /// </summary>
        /// <value>
        /// The response timestamp.
        /// </value>
        public DateTime? ResponseTimestamp { get; set; }  
    }
}
