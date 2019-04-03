using System;
using System.Configuration;

namespace BisAceAPIModels.Models
{
    /// <summary>
    /// WebCenterpoint implementation of the Exception
    /// </summary>
    public class BisException : Exception
    {
        /// <summary>
        /// Default constructor which takes a message parameter
        /// </summary>
        /// <param name="message">The exception message which will be translated to the current culture.</param>
        public BisException(string message)
        {
        }

        /// <summary>
        /// Constructor with message and inner exception parameters
        /// </summary>
        /// <param name="message">The exception message which will be translated to the current culture.</param>
        /// <param name="innerException">The inner exception object</param>
        public BisException(string message, Exception innerException)
        {
        }
    }
}
