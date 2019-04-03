using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Tracing;
using BisAceAPILogging.ExceptionFormatters;

namespace BisAceAPILogging.Trace
{
    /// <summary>
    /// A replacement for the MS tracer that writes entries to our logging infrastructure
    /// </summary>
    /// <seealso cref="System.Web.Http.Tracing.ITraceWriter" />
    internal class SimpleTracer : ITraceWriter
    {
        private static ILog _log;
        private static TraceLevel _minimumLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTracer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="minimumLevel">The minimum level.</param>
        public SimpleTracer(string name, TraceLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
            if (_log == null)
            {
                _log = LogProvider.GetLogger(name);
            }
        }
        /// <summary>
        /// Gets or sets the web API trace level.
        /// </summary>
        /// <value>
        /// The web API trace level.
        /// </value>
        public static TraceLevel WebAPITraceLevel
        {
            get { return _minimumLevel; }
            set { _minimumLevel = value; }
        }
        /// <summary>
        /// Invokes the specified traceAction to allow setting values in a new <see cref="T:System.Web.Http.Tracing.TraceRecord" /> if and only if tracing is permitted at the given category and level.
        /// </summary>
        /// <param name="request">The current <see cref="T:System.Net.Http.HttpRequestMessage" />.   It may be null but doing so will prevent subsequent trace analysis  from correlating the trace to a particular request.</param>
        /// <param name="category">The logical category for the trace.  Users can define their own.</param>
        /// <param name="level">The <see cref="T:System.Web.Http.Tracing.TraceLevel" /> at which to write this trace.</param>
        /// <param name="traceAction">The action to invoke if tracing is enabled.  The caller is expected to fill in the fields of the given <see cref="T:System.Web.Http.Tracing.TraceRecord" /> in this action.</param>
        public void Trace(HttpRequestMessage request, string category, TraceLevel level,
        Action<TraceRecord> traceAction)
        {
            TraceRecord rec = new TraceRecord(request, category, level);
            traceAction(rec);
            WriteTrace(rec);
        }

        /// <summary>
        /// Writes the trace.
        /// </summary>
        /// <param name="rec">The record.</param>
        protected void WriteTrace(TraceRecord rec)
        {
            if (_minimumLevel == TraceLevel.Off)
                return;

            if (rec.Exception != null && (_minimumLevel <= TraceLevel.Error || _minimumLevel <= TraceLevel.Fatal))
            { //special handling for exceptions
                if (rec.Exception is ReflectionTypeLoadException)
                { //we need the special property that contains the loader exceptions to be serialized
                    _log.ErrorFormat("An assembly loading error has occurred and was captured by the WebAPI trace module. {Error} WebAPI Trace {@record}", ExceptionFormatter.Format(((ReflectionTypeLoadException)rec.Exception)), new TraceInfo(rec.Operator, rec.Operation, rec.Message, rec.Category, rec.Kind.ToString()));
                }
                else
                {
                    _log.ErrorException("An error has occurred and was captured by the WebAPI trace module. WebAPI Trace {@record}", rec.Exception, new TraceInfo(rec.Operator, rec.Operation, rec.Message, rec.Category, rec.Kind.ToString()));
                }
            }
            else
            {
                switch (rec.Level)
                {
                    case TraceLevel.Debug:
                        if (_minimumLevel <= TraceLevel.Debug && _log.IsDebugEnabled())
                        {
                            WriteMessage(LogLevel.Debug, rec);
                        }
                        break;
                    case TraceLevel.Error:
                    case TraceLevel.Fatal:
                        if (_minimumLevel <= TraceLevel.Error && _log.IsErrorEnabled())
                        {
                            WriteMessage(LogLevel.Error, rec);
                        }
                        break;
                    case TraceLevel.Info:
                        if (_minimumLevel <= TraceLevel.Info && _log.IsInfoEnabled())
                        {
                            WriteMessage(LogLevel.Info, rec);
                        }
                        break;
                    case TraceLevel.Warn:
                        if (_minimumLevel <= TraceLevel.Warn && _log.IsWarnEnabled())
                        {
                            WriteMessage(LogLevel.Warn, rec);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Writes the message to our log
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="rec">The record.</param>
        private void WriteMessage(LogLevel level, TraceRecord rec)
        {
            _log.Log(level, () => "WebAPI Trace {@record}", null,
                   new TraceInfo(rec.Operator, rec.Operation, rec.Message, rec.Category, rec.Kind.ToString()));
        }

        /// <summary>
        /// A limited subset of data from the trace record
        /// </summary>
        /// <remarks>The original trace record cannot be serialized due to it's size</remarks>
        private class TraceInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TraceInfo"/> class.
            /// </summary>
            /// <param name="oper">The operator.</param>
            /// <param name="operation">The operation.</param>
            /// <param name="message">The message.</param>
            /// <param name="category">The category.</param>
            /// <param name="kind">The kind.</param>
            public TraceInfo(string oper, string operation, string message,
                string category, string kind)
            {
                Operator = oper;
                Operation = operation;
                Message = message;
                Category = category;
                Kind = kind;
            }
            /// <summary>
            /// Gets or sets the logical name of the object performing the operation.
            /// </summary>
            /// <value>
            /// The operator.
            /// </value>
            public string Operator { get; private set; }
            /// <summary>
            /// Gets or sets the logical operation name being performed.
            /// </summary>
            /// <value>
            /// The operation.
            /// </value>
            public string Operation { get; private set; }
            /// <summary>
            /// Gets or sets the tracing category.
            /// </summary>
            /// <value>
            /// The category.
            /// </value>
            public string Category { get; private set; }
            /// <summary>
            /// Gets or sets the kind of trace.
            /// </summary>
            /// <value>
            /// The kind.
            /// </value>
            public string Kind { get; private set; }
            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            /// <value>
            /// The message.
            /// </value>
            public string Message { get; private set; }
        }
    }
}
