using Microsoft.Owin.Logging;
using System;
using System.Diagnostics;
namespace BisAceAPILogging
{
    /// <summary>
    /// A libLog factory for Owin logging replacement
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Logging.ILoggerFactory" />
    public class LibLogLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Creates a new ILogger instance of the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILogger Create(string name)
        {
            return new LibLogLogger(LogProvider.GetLogger(name));
        }

        /// <summary>
        /// A wrapper around LibLog for a OWIN logging interface
        /// </summary>
        /// <seealso cref="Microsoft.Owin.Logging.ILogger" />
        private class LibLogLogger : ILogger
        {
            private readonly ILog _logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="LibLogLogger"/> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            public LibLogLogger(ILog logger)
            {
                _logger = logger;
            }

            /// <summary>
            /// Aggregates most logging patterns to a single method.  This must be compatible with the Func representation in the OWIN environment.
            /// To check IsEnabled call WriteCore with only TraceEventType and check the return value, no event will be written.
            /// </summary>
            /// <param name="eventType"></param>
            /// <param name="eventId"></param>
            /// <param name="state"></param>
            /// <param name="exception"></param>
            /// <param name="formatter"></param>
            /// <returns></returns>
            public bool WriteCore(
                TraceEventType eventType,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                return state == null
                    ? _logger.Log(Map(eventType), null)
                    : _logger.Log(Map(eventType), () => formatter(state, exception), exception);
            }

            /// <summary>
            /// Maps the specified event type.
            /// </summary>
            /// <param name="eventType">Type of the event.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentOutOfRangeException">eventType</exception>
            private LogLevel Map(TraceEventType eventType)
            {
                switch (eventType)
                {
                    case TraceEventType.Critical:
                        return LogLevel.Fatal;
                    case TraceEventType.Error:
                        return LogLevel.Error;
                    case TraceEventType.Warning:
                        return LogLevel.Warn;
                    case TraceEventType.Information:
                        return LogLevel.Info;
                    case TraceEventType.Verbose:
                        return LogLevel.Trace;
                    case TraceEventType.Start:
                        return LogLevel.Info;
                    case TraceEventType.Stop:
                        return LogLevel.Info;
                    case TraceEventType.Suspend:
                        return LogLevel.Info;
                    case TraceEventType.Resume:
                        return LogLevel.Info;
                    case TraceEventType.Transfer:
                        return LogLevel.Info;
                    default:
                        throw new ArgumentOutOfRangeException("eventType");
                }
            }
        }
    }
}
